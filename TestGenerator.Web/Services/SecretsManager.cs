using System.Reflection;

namespace TestGenerator.Web.Services;

public class SecretsManager
{
    private readonly IConfiguration _configuration;
    private readonly (string Section, string Key) ApiConnectionInfo = ("ChatGPTSecrets:Api", "Key");

    public SecretsManager()
    {
        var builder = new ConfigurationBuilder().AddUserSecrets(Assembly.GetExecutingAssembly(), true);

        _configuration = builder.Build();
    }

    public string GetApiKey()
    {
        return _configuration.GetSection(ApiConnectionInfo.Section)[ApiConnectionInfo.Key];
    }
}