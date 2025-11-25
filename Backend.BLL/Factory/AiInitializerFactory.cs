using Backend.API.Settings;
using Backend.BLL.AI;
using Microsoft.Extensions.Options;

namespace Backend.BLL.Factory;

public class AiInitializerFactory : IAiInitializerFactory
{
    private readonly AiSettings _settings;

    public AiInitializerFactory(IOptions<AiSettings> options)
    {
        _settings = options.Value;
    }

    public AiInitializer CreateLocal()
        => new AiInitializer(_settings.Local.BaseUri, _settings.Local.Model, _settings.Local.ApiKey);

    public AiInitializer CreateCloud()
        => new AiInitializer(_settings.Cloud.BaseUri, _settings.Cloud.Model, _settings.Cloud.ApiKey);
}
