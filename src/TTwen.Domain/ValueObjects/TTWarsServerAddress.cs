namespace TTwen.Domain.ValueObjects;

/// <summary>
/// TTWars sunucu adresini doğrulanmış ve normalize edilmiş biçimde temsil eder.
/// </summary>
/// <remarks>
/// Adres doğrulamasını tek bir value object içinde tutmak, URL kurallarının
/// bağlantı kodu ve arayüz arasında tekrar tekrar yazılmasını önler.
/// </remarks>
public readonly record struct TTWarsServerAddress
{
    /// <summary>
    /// Normalize edilmiş HTTP(S) adresidir.
    /// </summary>
    public string Value { get; }

    private TTWarsServerAddress(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Kullanıcı tarafından verilen sunucu adresini doğrular ve normalize eder.
    /// </summary>
    /// <param name="input">Tam adres veya şema içermeyen host adresi.</param>
    /// <returns>Doğrulanmış TTWars adresi.</returns>
    /// <exception cref="ArgumentException">
    /// Adres boşsa, geçersizse veya HTTP/HTTPS dışında bir şema kullanıyorsa fırlatılır.
    /// </exception>
    public static TTWarsServerAddress Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentException(
                "TTWars sunucu adresi boş olamaz.",
                nameof(input));
        }

        var value = input.Trim();

        // Şema açıkça verilmişse onu korur; şema yoksa TTWars için HTTPS varsayılır.
        if (!Uri.TryCreate(value, UriKind.Absolute, out var parsedInput)
            || string.IsNullOrWhiteSpace(parsedInput.Scheme))
        {
            value = "https://" + value;
        }

        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri)
            || string.IsNullOrWhiteSpace(uri.Host))
        {
            throw new ArgumentException(
                "TTWars sunucu adresi geçerli bir URL olmalıdır.",
                nameof(input));
        }

        if (!string.Equals(uri.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase)
            && !string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "TTWars sunucu adresi yalnızca HTTP veya HTTPS kullanabilir.",
                nameof(input));
        }

        if (!string.IsNullOrEmpty(uri.UserInfo))
        {
            throw new ArgumentException(
                "Sunucu adresinde kullanıcı adı veya parola bulunmamalıdır.",
                nameof(input));
        }

        var normalizedPath = uri.AbsolutePath.TrimEnd('/');
        var normalized = $"{uri.Scheme}://{uri.Authority}{normalizedPath}{uri.Query}";

        return new TTWarsServerAddress(normalized);
    }

    /// <summary>
    /// Adresin metinsel karşılığını döndürür.
    /// </summary>
    public override string ToString() => Value;
}
