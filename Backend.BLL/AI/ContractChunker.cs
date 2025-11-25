using System.Text.RegularExpressions;

namespace Backend.BLL.AI;

public class ContractChunker
{
    private static readonly string[] TopLevelKeywords = new[]
    {
        "function",
        "constructor",
        "modifier",
        "event",
        "error",
        "fallback",
        "receive",
        "struct",
        "enum"
    };

    public List<string> SplitIntoChunks(string code)
    {
        // Убираем комментарии, чтобы не ловить ключевые слова внутри них
        code = RemoveComments(code);

        var chunks = new List<string>();

        // Находим основной контракт
        var contractMatch = Regex.Match(code, @"contract\s+\w+\s*{", RegexOptions.Singleline);
        if (!contractMatch.Success)
        {
            // Если контракт не найден, возвращаем весь код как один чанк
            chunks.Add(code.Trim());
            return chunks;
        }

        int contractStart = contractMatch.Index + contractMatch.Length;
        int contractEnd = FindMatchingBrace(code, contractStart - 1);
        if (contractEnd == -1) contractEnd = code.Length;

        string contractBody = code.Substring(contractStart, contractEnd - contractStart).Trim();

        // Сохраняем контекст: все поля, структуры, события, модификаторы
        var contextBuilder = new List<string>();
        var memberRegex = new Regex(@"\b(?:struct|enum|event|modifier)\b\s+\w+\s*{[^}]*}", RegexOptions.Singleline);
        foreach (Match m in memberRegex.Matches(contractBody))
        {
            contextBuilder.Add(m.Value.Trim());
            contractBody = contractBody.Replace(m.Value, ""); // удаляем из тела, чтобы не дублировать
        }

        // Добавляем поля и переменные состояния
        var fieldRegex = new Regex(@"(?:\w+\s+)+\w+\s*(?:=\s*[^;]+)?;", RegexOptions.Singleline);
        foreach (Match m in fieldRegex.Matches(contractBody))
        {
            contextBuilder.Add(m.Value.Trim());
            contractBody = contractBody.Replace(m.Value, "");
        }

        // Теперь ищем функции и конструкторы
        var funcRegex = new Regex(@"\b(?:function|constructor|fallback|receive)\b\s*[\w\s\(\),]*{", RegexOptions.Singleline);
        int index = 0;
        while (index < contractBody.Length)
        {
            var match = funcRegex.Match(contractBody, index);
            if (!match.Success) break;

            int start = match.Index;
            int end = FindMatchingBrace(contractBody, start + match.Length - 1);
            if (end == -1) end = contractBody.Length;

            // Формируем чанк: контекст + функция
            var chunkBuilder = new List<string>(contextBuilder);
            chunkBuilder.Add(contractBody.Substring(start, end - start).Trim());
            chunks.Add(string.Join("\n", chunkBuilder));

            index = end;
        }

        return chunks;
    }

    private int FindMatchingBrace(string code, int startBraceIndex)
    {
        int depth = 0;
        for (int i = startBraceIndex; i < code.Length; i++)
        {
            if (code[i] == '{') depth++;
            else if (code[i] == '}') depth--;

            if (depth == 0) return i + 1; // возвращаем индекс после закрывающей скобки
        }
        return -1;
    }

    private string RemoveComments(string code)
    {
        // Убираем /* */ и // комментарии
        code = Regex.Replace(code, @"/\*.*?\*/", "", RegexOptions.Singleline);
        code = Regex.Replace(code, @"//.*", "");
        return code;
    }
}