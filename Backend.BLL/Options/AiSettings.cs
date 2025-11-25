namespace Backend.API.Settings;

public class AiSettings
{
    public AiModelSettings Local { get; set; } = new();
    public AiModelSettings Cloud { get; set; } = new();
}
