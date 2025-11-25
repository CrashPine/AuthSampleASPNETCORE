namespace Backend.API.Settings;

public class AiModelSettings
{
    public string BaseUri { get; set; } = "";
    public string Model { get; set; } = "";
    public string? ApiKey { get; set; }
}