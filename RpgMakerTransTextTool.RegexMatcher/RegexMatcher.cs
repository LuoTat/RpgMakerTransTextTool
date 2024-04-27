using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace RpgMakerTransTextTool.RegexMatcher;

public class RegexMatcher
{
    // 记录当前程序的根目录
    private static readonly string AppRootFolderPath = AppDomain.CurrentDomain.BaseDirectory;

    // 存储正则表达式的 JSON 文件路径
    private static readonly string JsonFilePath = Path.Combine(AppRootFolderPath, "RegexMatcher.json");

    // 存储文件名和正则表达式的键值对
    private readonly List<KeyValuePair<string, string>>? _fileRegexPairs;

    public RegexMatcher()
    {
        string json = File.ReadAllText(JsonFilePath);
        _fileRegexPairs = JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(json);
    }

    public Regex? GetRegexForFile(string fileName)
    {
        // 如果没有找到或者正则表达式为空，则返回 null
        return _fileRegexPairs == null ? null : (from pair in _fileRegexPairs where Regex.IsMatch(fileName, pair.Key) select new Regex(pair.Value)).FirstOrDefault();
    }
}