namespace TestGenerator.Web.Services;

public class SecretsManager
{
    private const string ApiKey = "apiKey";
    private readonly IConfiguration _configuration;

    public SecretsManager()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("secrets.json", optional: true, reloadOnChange: true);

        _configuration = builder.Build();
    }

    public string GetApiKey()
    {
        return _configuration[ApiKey];
    }
}