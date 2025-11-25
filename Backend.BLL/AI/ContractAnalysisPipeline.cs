using System.Text;

namespace Backend.BLL.AI;

public class ContractAnalysisPipeline
    {
        private readonly AiInitializer _aiInitializerModel1;
        private readonly AiInitializer _aiInitializerModel2;
        private readonly ContractChunker _chunker;

        public ContractAnalysisPipeline(AiInitializer model1, AiInitializer model2)
        {
            _aiInitializerModel1 = model1 ?? throw new ArgumentNullException(nameof(model1));
            _aiInitializerModel2 = model2 ?? throw new ArgumentNullException(nameof(model2));
            _chunker = new ContractChunker();
        }

        /// <summary>
        /// Главный метод анализа контракта
        /// </summary>
        public async Task<string> AnalyzeContractAsync(string contractCode)
        {
            if (string.IsNullOrWhiteSpace(contractCode))
                throw new ArgumentException("Contract code is empty", nameof(contractCode));

            // 1. Разделяем на чанки
            var chunks = _chunker.SplitIntoChunks(contractCode);

            // 2. Анализ каждого чанка через модель 1
            var chunkAnalyses = new List<string>();
            for (int i = 0; i < chunks.Count; i++)
            {
                string chunkPrompt = $"Analyze this Solidity code for vulnerabilities:\n{chunks[i]}";
                string analysis = await _aiInitializerModel1.AskAsync(chunkPrompt);

                chunkAnalyses.Add($"Chunk {i} Analysis:\n{analysis}");
            }

            // 3. Формируем общий промт для модели 2
            var combinedPrompt = new StringBuilder();
            combinedPrompt.AppendLine("Based on the following chunk analyses, provide a final vulnerability assessment for the smart contract:\n");
            foreach (var analysis in chunkAnalyses)
            {
                combinedPrompt.AppendLine(analysis);
                combinedPrompt.AppendLine();
            }

            // 4. Получаем финальный ответ от модели 2
            string finalReport = await _aiInitializerModel2.AskAsync(combinedPrompt.ToString());

            return finalReport;
        }
    }