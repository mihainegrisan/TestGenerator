using System.Reflection;

namespace TestGenerator.Web.Services;

public class SecretsManager
{
    private readonly (string Section, string Key) ApiConnectionInfo = ("ChatGPTSecrets:Api", "Key");
    private readonly IConfiguration _configuration;

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