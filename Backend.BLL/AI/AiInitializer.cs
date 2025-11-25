using System.Text;
using OllamaSharp;
using OllamaSharp.Models;


namespace Backend.BLL.AI;

public class AiInitializer : IDisposable
{
    private readonly OllamaApiClient _ollamaClient;
    private readonly string _model;

    public AiInitializer(
        string baseUri = "http://192.168.140.86:55555",
        string model = "qwen3-coder:480b-cloud",
        string apiKey = "ca289e87c7e74920a6aac6b76048b174.lV5A1Sr-3LOX0VOKPTPLSdeO"
    )
    {
        _model = model;

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(baseUri)
        };

        if (!string.IsNullOrEmpty(apiKey))
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        }

        _ollamaClient = new OllamaApiClient(httpClient);
        _ollamaClient.SelectedModel = model; // установить модель, если нужно
    }

    public async Task<string> AskAsync(string prompt)
    {
        var request = new GenerateRequest
        {
            Model = _model,
            Prompt = prompt,
            Stream = true // если вы хотите стрим
        };

        var sb = new StringBuilder();

        await foreach (var chunk in _ollamaClient.GenerateAsync(request))
        {
            if (chunk?.Response != null)
            {
                sb.Append(chunk.Response);
            }
        }

        return sb.ToString();
    }

    public async Task<string[]> AskChunksAsync(string[] chunks)
    {
        var results = new string[chunks.Length];
        for (int i = 0; i < chunks.Length; i++)
        {
            results[i] = await AskAsync(chunks[i]);
        }
        return results;
    }

    public void Dispose()
    {
        _ollamaClient?.Dispose();
    }
}

