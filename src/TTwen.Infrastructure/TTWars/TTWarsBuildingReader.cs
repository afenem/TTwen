using System.Text.Json;
using Microsoft.Playwright;
using TTwen.Application.Models;
using TTwen.Domain.Snapshots;

namespace TTwen.Infrastructure.TTWars;

/// <summary>
/// TTWars dorf2.php sayfasından köy merkezi binalarını okuyan Playwright reader'ıdır.
/// </summary>
/// <remarks>
/// Öncelik modern buildingSlot DOM'una, ardından klasik Travian/T3.6 village_map
/// sınıflarına verilir. Tüm slotlar tek JavaScript değerlendirmesinde çıkarılır;
/// her bina için ayrı build.php navigasyonu yapılmaz.
/// </remarks>
public sealed class TTWarsBuildingReader
{
    private readonly IPage _page;

    /// <summary>
    /// Reader'ı belirli bir Playwright sayfasına bağlar.
    /// </summary>
    public TTWarsBuildingReader(IPage page)
    {
        _page = page ?? throw new ArgumentNullException(nameof(page));
    }

    /// <summary>
    /// Aktif köydeki bina slotlarını okur.
    /// </summary>
    public async Task<TTWarsBuildingReadResult> ReadAsync(
        CancellationToken cancellationToken)
    {
        var capturedAtUtc = DateTimeOffset.UtcNow;

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!Uri.TryCreate(_page.Url, UriKind.Absolute, out var currentUri))
                throw new InvalidOperationException("Aktif Playwright URL'si geçersiz.");

            var target = new Uri(currentUri, "dorf2.php");

            await _page.GotoAsync(
                target.AbsoluteUri,
                new PageGotoOptions
                {
                    WaitUntil = WaitUntilState.DOMContentLoaded
                });

            cancellationToken.ThrowIfCancellationRequested();

            if (await IsLoginPageAsync(cancellationToken))
            {
                return Failure(
                    "TTWars oturumu giriş sayfasında.",
                    "Bina bilgileri okunmadan önce hesabın giriş yapmış olması gerekir.",
                    capturedAtUtc,
                    _page.Url);
            }

            var raw = await ReadSlotRowsAsync(cancellationToken);

            if (raw.Count == 0)
            {
                var diagnostics = await ReadDiagnosticsAsync(cancellationToken);
                return Failure(
                    "Bina slotları bulunamadı.",
                    diagnostics,
                    capturedAtUtc,
                    _page.Url);
            }

            var buildings = raw
                .Where(row => row.SlotId > 0)
                .GroupBy(row => row.SlotId)
                .Select(group => group.First())
                .OrderBy(row => row.SlotId)
                .Select(row => new BuildingSnapshot(
                    SlotId: row.SlotId,
                    Name: row.Name,
                    Level: row.Level,
                    Gid: row.Gid,
                    IsOccupied: row.IsOccupied,
                    IsUnderConstruction: row.IsUnderConstruction,
                    SourceUrl: _page.Url))
                .ToList();

            var occupied = buildings.Count(item => item.IsOccupied);

            return new TTWarsBuildingReadResult(
                Success: true,
                Buildings: buildings,
                Message: $"{buildings.Count} bina slotu okundu ({occupied} dolu).",
                Details: null,
                CapturedAtUtc: capturedAtUtc,
                SourceUrl: _page.Url);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (PlaywrightException exception)
        {
            return Failure(
                "Bina bilgileri Playwright üzerinden okunamadı.",
                exception.Message,
                capturedAtUtc,
                _page.Url);
        }
        catch (Exception exception)
        {
            return Failure(
                "Bina bilgileri okunurken beklenmeyen bir hata oluştu.",
                exception.Message,
                capturedAtUtc,
                _page.Url);
        }
    }

    /// <summary>
    /// Aktif sayfanın giriş sayfası olup olmadığını kontrol eder.
    /// </summary>
    private async Task<bool> IsLoginPageAsync(
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (Uri.TryCreate(_page.Url, UriKind.Absolute, out var uri)
            && uri.AbsolutePath.Contains("login", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return await _page.Locator(
                "form[action*='login'], input[type='password'], input[name='password']")
            .CountAsync() > 0;
    }

    /// <summary>
    /// Modern ve klasik bina işaretlerini tek DOM taramasında çıkarır.
    /// </summary>
    private async Task<IReadOnlyList<BuildingRow>> ReadSlotRowsAsync(
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var rawJson = await _page.EvaluateAsync<string>(
            """
            () => {
              const clean = value => (value || '').replace(/\s+/g, ' ').trim();

              const parseNumber = value => {
                const text = clean(value)
                  .replace(/[‪-‮‎‏]/g, '')
                  .replace(/−/g, '-')
                  .replace(/[^\d-]/g, '');

                if (!text) return null;

                const number = Number.parseInt(text, 10);
                return Number.isFinite(number) ? number : null;
              };

              const firstMatch = (text, patterns) => {
                for (const pattern of patterns) {
                  const match = text.match(pattern);
                  if (match) return match;
                }
                return null;
              };

              const parseGid = text => {
                const match = firstMatch(text, [
                  /(?:^|\s)gid[_-]?(\d{1,2})(?:\s|$)/i,
                  /(?:^|\s)g(\d{1,2})(?:\s|$)/i,
                  /\bgid[_-]?(\d{1,2})\b/i,
                  /\bg(\d{1,2})\b/i
                ]);

                return match ? Number(match[1]) : null;
              };

              const parseLevel = text => {
                const match = firstMatch(text, [
                  /(?:level|seviye|niveau|stufe)\s*(\d{1,2})/i,
                  /(?:^|\s)level[_-]?(\d{1,2})(?:\s|$)/i,
                  /(?:^|\s)lvl[_-]?(\d{1,2})(?:\s|$)/i
                ]);

                if (match) return Number(match[1]);

                const bare = text.match(/(?:^|\s)(\d{1,2})(?:\s|$)/);
                return bare ? Number(bare[1]) : null;
              };

              const cleanName = text => {
                let value = clean(text);

                value = value
                  .replace(/(?:^|\s)(?:gid|g|aid|slot)[_-]?\d{1,2}(?=\s|$)/gi, ' ')
                  .replace(/(?:^|\s)(?:level|seviye|niveau|stufe)[\s:_-]*\d{1,2}(?=\s|$)/gi, ' ')
                  .replace(/\s+/g, ' ')
                  .trim();

                return value || 'Bina';
              };

              const parseSlotFromText = text => {
                const match = firstMatch(text, [
                  /(?:^|\s)aid[_-]?(\d{1,2})(?:\s|$)/i,
                  /(?:^|\s)slot[_-]?(\d{1,2})(?:\s|$)/i,
                  /[?&]id=(\d{1,2})(?:[^0-9]|$)/i
                ]);

                return match ? Number(match[1]) : null;
              };

              const result = [];
              const seen = new Set();

              const add = row => {
                if (!row || !Number.isFinite(row.SlotId) || row.SlotId <= 0) return;
                if (seen.has(row.SlotId)) return;
                seen.add(row.SlotId);
                result.push(row);
              };

              // Modern skins / T4-style markup.
              for (const slot of document.querySelectorAll('div.buildingSlot')) {
                const anchor = slot.querySelector('a[href], area[href]');
                const image = slot.querySelector('img.building, img[alt]');
                const levelNode = slot.querySelector('.labelLayer, .level, .label, [data-level]');

                const classText = clean(
                  String(slot.className || '')
                  + ' '
                  + String(image?.className || '')
                );

                const attributes = [
                  slot.getAttribute('data-aid') || '',
                  slot.getAttribute('data-slot') || '',
                  slot.getAttribute('data-id') || '',
                  slot.getAttribute('data-gid') || '',
                  anchor?.getAttribute('href') || ''
                ].join(' ');

                const slotId =
                  parseSlotFromText(attributes)
                  ?? (Number(slot.getAttribute('data-aid')) || null)
                  ?? (Number(slot.getAttribute('data-slot')) || null);

                if (slotId === null) continue;

                const displayText = clean(
                  [
                    slot.getAttribute('data-name') || '',
                    image?.getAttribute('alt') || '',
                    anchor?.getAttribute('title') || '',
                    levelNode?.getAttribute('data-level') || '',
                    levelNode?.textContent || '',
                    slot.textContent || ''
                  ].join(' ')
                );

                const gid = parseGid(attributes + ' ' + classText + ' ' + displayText);
                const level = parseLevel(
                  [
                    slot.getAttribute('data-level') || '',
                    levelNode?.getAttribute('data-level') || '',
                    levelNode?.textContent || '',
                    displayText
                  ].join(' ')
                );

                const emptyText = /building\s+site|bina\s+arsas[ıi]|empty|boş/i.test(displayText);

                add({
                  SlotId: slotId,
                  Name: cleanName(
                    slot.getAttribute('data-name')
                    || image?.getAttribute('alt')
                    || anchor?.getAttribute('title')
                    || displayText
                  ),
                  Level: level,
                  Gid: gid,
                  IsOccupied: !emptyText && (gid !== null || !!anchor || level !== null),
                  IsUnderConstruction:
                    /underconstruction|under-?construction|inşaat|yükseltme/i.test(classText + ' ' + displayText)
                    || /(?:^|\s)g\d+b(?:\s|$)/i.test(classText)
                });
              }

              // Classic Travian/T3.6 map images: d1..d20 -> slots 19..38.
              for (const image of document.querySelectorAll(
                '#village_map img.building, #village_map img[class*=" building "]'
              )) {
                const classText = clean(String(image.className || ''));
                const slotMatch = classText.match(/(?:^|\s)d(\d{1,2})(?:\s|$)/i);
                if (!slotMatch) continue;

                const visualIndex = Number(slotMatch[1]);
                if (visualIndex < 1 || visualIndex > 20) continue;

                const slotId = visualIndex + 18;
                const alt = clean(image.getAttribute('alt') || '');
                const gid = parseGid(classText + ' ' + alt);
                const level = parseLevel(alt);
                const empty = /building\s+site|bina\s+arsas[ıi]|empty|boş/i.test(alt);

                add({
                  SlotId: slotId,
                  Name: cleanName(alt),
                  Level: level,
                  Gid: gid,
                  IsOccupied: !empty && (gid !== null || level !== null),
                  IsUnderConstruction:
                    /underconstruction|under-?construction|inşaat|yükseltme/i.test(classText + ' ' + alt)
                    || /(?:^|\s)g\d+b(?:\s|$)/i.test(classText)
                });
              }

              // Classic level overlay provides reliable level values even when the
              // building image itself has no textual level.
              for (const levelNode of document.querySelectorAll('#levels > div')) {
                const classText = clean(String(levelNode.className || ''));
                const text = clean(levelNode.textContent || '');
                const level = parseNumber(text);

                const dMatch = classText.match(/(?:^|\s)d(\d{1,2})(?:\s|$)/i);
                if (dMatch) {
                  add({
                    SlotId: Number(dMatch[1]) + 18,
                    Name: 'Bina',
                    Level: level,
                    Gid: null,
                    IsOccupied: level !== null,
                    IsUnderConstruction: false
                  });
                  continue;
                }

                const specialMatch = classText.match(/(?:^|\s)l(39|40)(?:\s|$)/i);
                if (specialMatch) {
                  add({
                    SlotId: Number(specialMatch[1]),
                    Name: Number(specialMatch[1]) === 39 ? 'Rally Point' : 'Surlar',
                    Level: level,
                    Gid: Number(specialMatch[1]) === 39 ? 16 : null,
                    IsOccupied: level !== null,
                    IsUnderConstruction: false
                  });
                }
              }

              // Final compatibility fallback: image-map build.php links can still
              // provide slot + title when visual classes are incomplete.
              for (const area of document.querySelectorAll(
                'map#map2 area[href*="build.php?id="], map#map1 area[href*="build.php?id="]'
              )) {
                const href = area.getAttribute('href') || '';
                const idMatch = href.match(/[?&]id=(\d{1,2})(?:[^0-9]|$)/i);
                if (!idMatch) continue;

                const slotId = Number(idMatch[1]);
                if (slotId < 19 || slotId > 40) continue;

                const title = clean(
                  area.getAttribute('title')
                  || area.getAttribute('alt')
                  || ''
                );

                const gid = parseGid(title);
                const level = parseLevel(title);
                const empty = /building\s+site|bina\s+arsas[ıi]|empty|boş/i.test(title);

                add({
                  SlotId: slotId,
                  Name: cleanName(title),
                  Level: level,
                  Gid: gid,
                  IsOccupied: !empty && (gid !== null || level !== null || title.length > 0),
                  IsUnderConstruction:
                    /underconstruction|under-?construction|inşaat|yükseltme/i.test(title)
                });
              }

              return JSON.stringify(result);
            }
            """);

        return JsonSerializer.Deserialize<List<BuildingRow>>(
            rawJson ?? "[]") ?? [];
    }

    /// <summary>
    /// Selector tanılaması üretir.
    /// </summary>
    private async Task<string> ReadDiagnosticsAsync(
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await _page.EvaluateAsync<string>(
            """
            () => JSON.stringify({
              url: location.href,
              title: document.title,
              buildingSlots:
                document.querySelectorAll('div.buildingSlot').length,
              villageBuildingImages:
                document.querySelectorAll('#village_map img.building, #village_map img[class*=" building "]').length,
              levelNodes:
                document.querySelectorAll('#levels > div').length,
              buildLinks:
                document.querySelectorAll('a[href*="build.php?id="], area[href*="build.php?id="]').length
            })
            """);
    }

    /// <summary>
    /// Standart bina okuma hatası sonucu oluşturur.
    /// </summary>
    private static TTWarsBuildingReadResult Failure(
        string message,
        string? details,
        DateTimeOffset capturedAtUtc,
        string sourceUrl)
    {
        return new TTWarsBuildingReadResult(
            Success: false,
            Buildings: Array.Empty<BuildingSnapshot>(),
            Message: message,
            Details: details,
            CapturedAtUtc: capturedAtUtc,
            SourceUrl: sourceUrl);
    }

    /// <summary>
    /// JavaScript sonucunun C# taşıma modelidir.
    /// </summary>
    private sealed record BuildingRow(
        int SlotId,
        string Name,
        int? Level,
        int? Gid,
        bool IsOccupied,
        bool IsUnderConstruction);
}