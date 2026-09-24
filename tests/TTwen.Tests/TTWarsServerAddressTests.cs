using Xunit;
using TTwen.Domain.ValueObjects;

namespace TTwen.Tests;

/// <summary>
/// TTWars sunucu adresinin doğrulama ve normalize kurallarını test eder.
/// </summary>
public sealed class TTWarsServerAddressTests
{
    [Fact]
    public void ParseAddsHttpsWhenSchemeIsMissing()
    {
        var address = TTWarsServerAddress.Parse("nor7.ttwars.com");

        Assert.Equal("https://nor7.ttwars.com", address.Value);
    }

    [Fact]
    public void ParsePreservesTheServerPath()
    {
        var address = TTWarsServerAddress.Parse(
            "https://nor7.ttwars.com/dorf1.php/");

        Assert.Equal(
            "https://nor7.ttwars.com/dorf1.php",
            address.Value);
    }

    [Fact]
    public void ParseAcceptsHttpAndHttps()
    {
        var http = TTWarsServerAddress.Parse(
            "http://nor7.ttwars.com");

        var https = TTWarsServerAddress.Parse(
            "https://nor7.ttwars.com");

        Assert.Equal("http://nor7.ttwars.com", http.Value);
        Assert.Equal("https://nor7.ttwars.com", https.Value);
    }

    [Fact]
    public void ParseRejectsUnsupportedScheme()
    {
        Assert.Throws<ArgumentException>(
            () => TTWarsServerAddress.Parse("ftp://nor7.ttwars.com"));
    }

    [Fact]
    public void ParseRejectsEmbeddedCredentials()
    {
        Assert.Throws<ArgumentException>(
            () => TTWarsServerAddress.Parse(
                "https://user:password@nor7.ttwars.com"));
    }
}