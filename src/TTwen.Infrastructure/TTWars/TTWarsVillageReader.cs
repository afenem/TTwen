using Microsoft.Playwright;
using TTwen.Application.Models;
using TTwen.Application.Services;
using TTwen.Domain.Snapshots;

namespace TTwen.Infrastructure.TTWars;

/// <summary>
/// TTWars HTML sayfalarından köy durumlarını çıkaran Playwright okuyucusudur.
/// </summary>
/// <remarks>
/// Selector ayrıntıları yalnızca bu sınıfta tutulur. Okuyucu bir köyde sorun olduğunda
/// diğer köylerin sonuçlarını korumaya çalışır ve başarısızlığı tanısal ayrıntıyla raporlar.
/// </remarks>
public sealed class TTWarsVillageReader
{
    private readonly IPage _page;

    /// <summary>
    /// Reader'ı belirli bir Playwright sayfasına bağlar.
    /// </summary>
    public TTWarsVillageReader(IPage page)
    {
        _page = page ?? throw new ArgumentNullException(nameof(page));
    }

    /// <summary>
    /// Hesabın erişebildiği köyleri okur.
    /// </summary>
    public async Task<TTWarsVillageReadResult> ReadAllAsync(
        CancellationToken cancellationToken)
    {
        var capturedAtUtc = DateTimeOffset.UtcNow;
        var originalUrl = _page.Url;
        var snapshots = new List<VillageSnapshot>();
        var failures = new List<string>();

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (await IsLoginPageAsync(cancellationToken))
            {
                return Failure(
                    "TTWars oturumu giriş sayfasında.",
                    "Köyleri okumadan önce TTWars hesabıyla giriş yapılmalıdır.",
                    capturedAtUtc);
            }

            var links = await ReadVillageLinksFromCurrentPageAsync(cancellationToken);

            if (links.Count == 0)
            {
                await NavigateRelativeAsync("dorf3.php", cancellationToken);

                if (await IsLoginPageAsync(cancellationToken))
                {
                    return Failure(
                        "TTWars oturumu giriş sayfasında.",
                        "dorf3.php açıldıktan sonra giriş ekranı görüldü.",
                        capturedAtUtc);
                }

                links = await ReadVillageLinksFromCurrentPageAsync(cancellationToken);
            }

            if (links.Count == 0)
            {
                var diagnostics = await ReadVillageListDiagnosticsAsync(cancellationToken);

                return Failure(
                    "TTWars köy listesi bulunamadı.",
                    $"URL: {_page.Url}{Environment.NewLine}{diagnostics}",
                    capturedAtUtc);
            }

            foreach (var link in links)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    await NavigateToVillageAsync(link.Id, cancellationToken);

                    var pageData = await ReadVillagePageDataAsync(
                        link.Id,
                        link.Name,
                        link.IsCapital,
                        cancellationToken);

                    snapshots.Add(
                        TTWarsVillageSnapshotMapper.Map(
                            pageData,
                            capturedAtUtc));
                }
                catch (Exception exception) when (
                    exception is not OperationCanceledException)
                {
                    failures.Add(
                        $"{link.Name} ({link.Id}): {exception.Message}");
                }
            }

            var message = failures.Count == 0
                ? $"{snapshots.Count} köy başarıyla okundu."
                : $"{snapshots.Count} köy okundu, {failures.Count} köy okunamadı.";

            return new TTWarsVillageReadResult(
                Success: failures.Count == 0 && snapshots.Count > 0,
                Villages: snapshots,
                RequestedCount: links.Count,
                FailedCount: failures.Count,
                Message: message,
                Details: failures.Count == 0
                    ? null
                    : string.Join(Environment.NewLine, failures),
                CapturedAtUtc: capturedAtUtc);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            return Failure(
                "TTWars köy okuması sırasında hata oluştu.",
                exception.Message,
                capturedAtUtc);
        }
        finally
        {
            await RestoreOriginalPageAsync(originalUrl, cancellationToken);
        }
    }

    /// <summary>
    /// Mevcut sayfanın giriş sayfası olup olmadığını kontrol eder.
    /// </summary>
    private async Task<bool> IsLoginPageAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!Uri.TryCreate(_page.Url, UriKind.Absolute, out var currentUri))
            return false;

        if (currentUri.AbsolutePath.Contains(
                "login",
                StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return await _page.Locator(
                "form[action*='login'], input[type='password'], input[name='password']")
            .CountAsync() > 0;
    }

    /// <summary>
    /// Köy değiştirme bağlantılarını mevcut HTML'den çıkarır.
    /// </summary>
    private async Task<IReadOnlyList<VillageLink>> ReadVillageLinksFromCurrentPageAsync(
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = await _page.EvaluateAsync<VillageLink[]>(
            """
            () => {
              const clean = value => (value || '').replace(/\s+/g, ' ').trim();

              const getId = href => {
                const text = clean(href);
                const newdid = text.match(/[?&]newdid=(\d+)/i);
                if (newdid) return newdid[1];

                const did = text.match(/[?&](?:did|wid)=(\d+)/i);
                return did ? did[1] : null;
              };

              const readName = node => {
                if (!node) return '';
                const nameNode = node.querySelector('.name');
                if (nameNode) return clean(nameNode.textContent);

                const anchor = node.matches('a') ? node : node.querySelector('a');
                if (anchor) {
                  return clean(
                    anchor.textContent || anchor.getAttribute('title') || ''
                  );
                }

                return clean(node.textContent);
              };

              const rows = [];
              const seen = new Set();

              const selectors = [
                '#overview tbody tr td.vil a[href*="newdid="]',
                '#vlist tbody tr td.link a[href*="newdid="]',
                '#vlist tbody tr a[href*="newdid="]',
                '#sidebarBoxVillagelist a[href*="newdid="]',
                '#villageList a[href*="newdid="]',
                '.villageList a[href*="newdid="]',
                'a.village-name[href*="newdid="]'
              ];

              for (const selector of selectors) {
                for (const anchor of document.querySelectorAll(selector)) {
                  const href = anchor.getAttribute('href') || '';
                  const id = getId(href);
                  if (!id || seen.has(id)) continue;

                  const container =
                    anchor.closest('tr, li, .listEntry, .village, .active')
                    || anchor.parentElement
                    || anchor;

                  const text = clean(
                    (container.className || '')
                    + ' '
                    + (container.getAttribute('title') || '')
                    + ' '
                    + (container.textContent || '')
                  ).toLowerCase();

                  const name = readName(container) || clean(anchor.textContent);
                  if (!name) continue;

                  seen.add(id);
                  rows.push({
                    Id: id,
                    Name: name,
                    IsCapital: /capital|başkent/.test(text) ? true : null
                  });
                }
              }

              for (const entry of document.querySelectorAll(
                '#sidebarBoxVillageList .listEntry.village[data-did], '
                + '.villageList .listEntry.village[data-did], '
                + '.listEntry.village[data-did]'
              )) {
                const id = clean(entry.getAttribute('data-did'));
                const name = clean(
                  entry.querySelector('.name')?.textContent || ''
                );

                if (!id || !name || seen.has(id)) continue;

                const text = clean(
                  (entry.className || '')
                  + ' '
                  + (entry.getAttribute('title') || '')
                ).toLowerCase();

                seen.add(id);
                rows.push({
                  Id: id,
                  Name: name,
                  IsCapital: /capital|başkent/.test(text) ? true : null
                });
              }

              return rows;
            }
            """);

        return result ?? [];
    }

    /// <summary>
    /// Belirli köye geçer.
    /// </summary>
    private async Task NavigateToVillageAsync(
        string villageId,
        CancellationToken cancellationToken)
    {
        var target = new Uri(
            new Uri(_page.Url),
            $"dorf1.php?newdid={Uri.EscapeDataString(villageId)}");

        await _page.GotoAsync(
            target.AbsoluteUri,
            new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded
            });

        cancellationToken.ThrowIfCancellationRequested();
    }

    /// <summary>
    /// Göreli TTWars adresine gider.
    /// </summary>
    private async Task NavigateRelativeAsync(
        string relativePath,
        CancellationToken cancellationToken)
    {
        var target = new Uri(new Uri(_page.Url), relativePath);

        await _page.GotoAsync(
            target.AbsoluteUri,
            new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded
            });

        cancellationToken.ThrowIfCancellationRequested();
    }

    /// <summary>
    /// Aktif köy sayfasındaki alanları tek JavaScript değerlendirmesinde çıkarır.
    /// </summary>
    private async Task<TTWarsVillagePageData> ReadVillagePageDataAsync(
        string villageId,
        string fallbackName,
        bool? fallbackCapital,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await _page.EvaluateAsync<TTWarsVillagePageData>(
            """
            (context) => {
              const clean = value => (value || '').replace(/\s+/g, ' ').trim();

              const parseNumber = value => {
                const normalized = clean(value)
                  .replace(/[^\d-]/g, '');

                if (!normalized) return null;

                const parsed = Number.parseInt(normalized, 10);
                return Number.isFinite(parsed) ? parsed : null;
              };

              const parseStock = selector => {
                const node = document.querySelector(selector);
                if (!node) {
                  return {
                    current: null,
                    capacity: null,
                    found: false
                  };
                }

                const text = clean(node.textContent);
                const match = text.match(
                  /(-?[\d\s.,]+)\s*\/\s*(-?[\d\s.,]+)/
                );

                if (!match) {
                  return {
                    current: parseNumber(text),
                    capacity: null,
                    found: true
                  };
                }

                return {
                  current: parseNumber(match[1]),
                  capacity: parseNumber(match[2]),
                  found: true
                };
              };

              const readDirectH1Name = () => {
                const h1 = document.querySelector(
                  '#content.village1 h1, #content h1, h1'
                );

                if (!h1) return '';

                const textNode = Array.from(h1.childNodes)
                  .find(node => node.nodeType === Node.TEXT_NODE);

                return clean(
                  textNode?.textContent || h1.textContent || ''
                );
              };

              const parseSignedCoordinate = selector => {
                const node = document.querySelector(selector);
                if (!node) return null;

                const text = clean(node.textContent)
                  .replace(/[‪-‮‎‏]/g, '')
                  .replace(/−/g, '-');

                const match = text.match(/-?\d+/);
                if (!match) return null;

                const parsed = Number.parseInt(match[0], 10);
                return Number.isFinite(parsed) ? parsed : null;
              };

              const mapCoordinateFromHref = () => {
                for (const link of document.querySelectorAll(
                  'a[href*="karte"], area[href*="karte"]'
                )) {
                  const href = link.getAttribute('href') || '';
                  const xMatch = href.match(/[?&]x=(-?\d+)/i);
                  const yMatch = href.match(/[?&]y=(-?\d+)/i);

                  if (xMatch && yMatch) {
                    return {
                      x: Number.parseInt(xMatch[1], 10),
                      y: Number.parseInt(yMatch[1], 10)
                    };
                  }
                }

                return { x: null, y: null };
              };

              const readPopulation = () => {
                const selectors = [
                  '#sidebarBoxVillagelist .active .population span',
                  '#sidebarBoxActiveVillage .population span',
                  '.villageInfobox .population span',
                  '.villageInfobox .population',
                  '.population span'
                ];

                for (const selector of selectors) {
                  const node = document.querySelector(selector);
                  const value = node ? parseNumber(node.textContent) : null;

                  if (value !== null) return value;
                }

                return null;
              };

              const readProduction = resourceClass => {
                for (const row of document.querySelectorAll(
                  '#production tbody tr, table#production tr'
                )) {
                  const hasResourceMarker =
                    row.querySelector(
                      'img.' + resourceClass + ', .' + resourceClass
                    );

                  if (!hasResourceMarker) continue;

                  const number = row.querySelector('td.num, .num');
                  const value = number
                    ? parseNumber(number.textContent)
                    : null;

                  if (value !== null) return value;
                }

                return null;
              };

              const woodStock = readStock('#l4');
              const clayStock = readStock('#l3');
              const ironStock = readStock('#l2');
              const cropStock = readStock('#l1');

              const coordinateFromHref = mapCoordinateFromHref();

              const x =
                parseSignedCoordinate('.coordinateX')
                ?? parseSignedCoordinate('.cox')
                ?? coordinateFromHref.x;

              const y =
                parseSignedCoordinate('.coordinateY')
                ?? parseSignedCoordinate('.coy')
                ?? coordinateFromHref.y;

              const capitalText = clean(
                (document.querySelector('#cap')?.textContent || '')
                + ' '
                + (document.body?.className || '')
              ).toLowerCase();

              const woodPerHour = readProduction('r1');
              const clayPerHour = readProduction('r2');
              const ironPerHour = readProduction('r3');
              const cropPerHour = readProduction('r4');

              return {
                Id: context.id,
                Name: readDirectH1Name() || context.fallbackName,
                X: x,
                Y: y,
                Population: readPopulation(),
                IsCapital:
                  /capital|başkent/.test(capitalText)
                    ? true
                    : context.fallbackCapital,
                Wood: woodStock.current,
                Clay: clayStock.current,
                Iron: ironStock.current,
                Crop: cropStock.current,
                WarehouseCapacity:
                  woodStock.capacity
                  ?? clayStock.capacity
                  ?? ironStock.capacity,
                GranaryCapacity: cropStock.capacity,
                WoodPerHour: woodPerHour,
                ClayPerHour: clayPerHour,
                IronPerHour: ironPerHour,
                CropPerHour: cropPerHour,
                HasResourceData:
                  woodStock.found
                  || clayStock.found
                  || ironStock.found
                  || cropStock.found,
                HasProductionData:
                  woodPerHour !== null
                  || clayPerHour !== null
                  || ironPerHour !== null
                  || cropPerHour !== null,
                SourceUrl: location.href,
                Diagnostics: JSON.stringify({
                  l1: !!document.querySelector('#l1'),
                  l2: !!document.querySelector('#l2'),
                  l3: !!document.querySelector('#l3'),
                  l4: !!document.querySelector('#l4'),
                  productionRows:
                    document.querySelectorAll('#production tr').length,
                  populationElements:
                    document.querySelectorAll('.population').length,
                  coordinateX:
                    !!document.querySelector('.coordinateX, .cox'),
                  coordinateY:
                    !!document.querySelector('.coordinateY, .coy')
                })
              };
            }
            """,
            new
            {
                id = villageId,
                fallbackName,
                fallbackCapital
            });
    }

    /// <summary>
    /// Köy listesi bulunamadığında selector durumunu tanılar.
    /// </summary>
    private async Task<string> ReadVillageListDiagnosticsAsync(
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await _page.EvaluateAsync<string>(
            """
            () => JSON.stringify({
              url: location.href,
              title: document.title,
              overviewRows:
                document.querySelectorAll('#overview tbody tr').length,
              vlistRows:
                document.querySelectorAll('#vlist tbody tr').length,
              sidebarNewdidLinks:
                document.querySelectorAll(
                  '#sidebarBoxVillagelist a[href*="newdid="], '
                  + '#villageList a[href*="newdid="]'
                ).length,
              dataDidEntries:
                document.querySelectorAll('.listEntry.village[data-did]').length,
              loginForms:
                document.querySelectorAll(
                  'form[action*="login"], input[type="password"]'
                ).length
            })
            """);
    }

    /// <summary>
    /// Okuma sonrasında kullanıcıyı ilk sayfaya geri getirir.
    /// </summary>
    private async Task RestoreOriginalPageAsync(
        string originalUrl,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(originalUrl)
            || string.Equals(
                originalUrl,
                _page.Url,
                StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _page.GotoAsync(
                originalUrl,
                new PageGotoOptions
                {
                    WaitUntil = WaitUntilState.DOMContentLoaded
                });
        }
        catch
        {
            // Ana okuma sonucu korunur; sayfa geri dönüşü yardımcı işlemdir.
        }
    }

    /// <summary>
    /// Başarısız bir köy okuma sonucunu oluşturur.
    /// </summary>
    private static TTWarsVillageReadResult Failure(
        string message,
        string? details,
        DateTimeOffset capturedAtUtc)
    {
        return new TTWarsVillageReadResult(
            Success: false,
            Villages: Array.Empty<VillageSnapshot>(),
            RequestedCount: 0,
            FailedCount: 0,
            Message: message,
            Details: details,
            CapturedAtUtc: capturedAtUtc);
    }

    /// <summary>
    /// Köy listesi içindeki tek bir köyün temel navigasyon bilgisidir.
    /// </summary>
    private sealed record VillageLink(
        string Id,
        string Name,
        bool? IsCapital);
}