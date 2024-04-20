using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using RpgMakerTransTextTool.StringOperations;

namespace RpgMakerTransTextTool.TextOperations;

public class StringInjector
{
    // 记录当前程序的根目录
    private static readonly string AppRootFolderPath = AppDomain.CurrentDomain.BaseDirectory;

    // 记录DictionaryData.bin路径
    private static readonly string DictionaryDataBinFilePath = Path.Combine(AppRootFolderPath, "Data", "DictionaryData.bin");

    // 记录ManualTransFile.json路径
    private static readonly string ManualTransFileJsonFilePath = Path.Combine(AppRootFolderPath, "Data", "ManualTransFile.json");

    // 设置输出的Scripts文件夹的根目录
    private static readonly string OutPutScriptsFolderPath = Path.Combine(AppRootFolderPath, "Scripts");

    // 设置原本的Scripts文件夹的根目录
    private readonly string _scriptsFolderPath;

    // 反序列化后的字典数据
    private readonly Dictionary<string, List<string>> _allExtractedStringsDictionary;

    // 记录ManualTransFile.json中的已翻译字符串
    private readonly List<string> _manualTransFileJsonTranslatedStrings = [];

    // 记录ManualTransFile.json中的未翻译字符串
    private readonly List<string> _manualTransFileJsonUntranslatedStrings = [];

    public StringInjector()
    {
        (_scriptsFolderPath, _allExtractedStringsDictionary) = ReadDictionaryFromBinaryFile();
        ExtractStringsFromJson();
    }

    private static (string, Dictionary<string, List<string>>) ReadDictionaryFromBinaryFile()
    {
        Dictionary<string, List<string>>? allExtractedStringsDictionary = null;
        string?                           scriptsFolderPath             = null;

        try
        {
            string  json       = File.ReadAllText(DictionaryDataBinFilePath);
            JObject jsonObject = JObject.Parse(json);

            // Extract _scriptsFolderPath from the JSON object
            scriptsFolderPath = jsonObject["_scriptsFolderPath"]?.ToString();

            // Extract allExtractedStringsDictionary from the JSON object
            allExtractedStringsDictionary = jsonObject["_allExtractedStringsDictionary"]?.ToObject<Dictionary<string, List<string>>>();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading dictionary from JSON file");
            Console.WriteLine(ex.ToString());
            Environment.Exit(1);
        }

        if (allExtractedStringsDictionary != null && scriptsFolderPath != null)
        {
            return (scriptsFolderPath, allExtractedStringsDictionary);
        }

        Console.WriteLine("Error loading dictionary from JSON file");
        Environment.Exit(1);
        return (scriptsFolderPath, allExtractedStringsDictionary);
    }

    private void ExtractStringsFromJson()
    {
        string jsonContent = File.ReadAllText(ManualTransFileJsonFilePath);

        // 解析JSON字符串
        JObject jsonObject = JObject.Parse(jsonContent);

        // 遍历JSON对象的属性
        foreach (JProperty property in jsonObject.Properties())
        {
            // 将属性名添加到未翻译字符串列表
            _manualTransFileJsonUntranslatedStrings.Add(property.Name);

            // 将属性值添加到已翻译字符串列表
            _manualTransFileJsonTranslatedStrings.Add(property.Value.ToString());
        }
    }

    public void ReplaceStringsInFiles()
    {
        // 使用Dictionary缓存文件内容
        // 其中键为文件路径，值为文件内容
        Dictionary<string, string> txtFileCache = new();

        // 遍历ManualTransFile.json中的所有未翻译字符串
        for (int i = 0; i < _manualTransFileJsonUntranslatedStrings.Count; i++)
        {
            // 获取未翻译字符串和已翻译字符串
            string untranslatedString = _manualTransFileJsonUntranslatedStrings[i];
            string translatedString   = _manualTransFileJsonTranslatedStrings[i];
            // 如果已翻译字符串为空，则跳过
            if (string.IsNullOrEmpty(translatedString)) continue;

            // 如果未翻译字符串与已翻译字符串相同，则跳过
            if (untranslatedString.Equals(translatedString)) continue;

            // 如果未翻译字符串不在字典中，则跳过
            if (!_allExtractedStringsDictionary.TryGetValue(untranslatedString, out List<string>? extractedStringLocations)) continue;

            // 遍历所有包含未翻译字符串的文件
            foreach (string extractedStringLocation in extractedStringLocations)
            {
                // 先判断文件内容是否已经在缓存中，如果不在则读取并缓存
                if (!txtFileCache.TryGetValue(extractedStringLocation, out string? fileContent))
                {
                    fileContent                           = File.ReadAllText(extractedStringLocation);
                    txtFileCache[extractedStringLocation] = fileContent;
                }

                // 使用正则表达式替换所有未翻译的字符串为翻译后的字符串
                fileContent = Regex.Replace(fileContent, Regex.Escape(StringEscaper.EscapeString(untranslatedString)), StringEscaper.EscapeString(translatedString));

                //将修改后的内容放回缓存中
                txtFileCache[extractedStringLocation] = fileContent;
            }

            Console.WriteLine($"正在替换第{i + 1}/{_manualTransFileJsonUntranslatedStrings.Count}个字符串");
        }

        // 一次性将所有文件内容输出到根目录下的Scripts文件夹
        foreach ((string? absoluteFilePath, string? fileContent) in txtFileCache)
        {
            // 获取绝对路径和相对路径
            string relativeFilePath = Path.GetRelativePath(_scriptsFolderPath, absoluteFilePath);

            // 获得根目录下的Scripts文件夹的绝对路径
            string filePath = Path.Combine(OutPutScriptsFolderPath, relativeFilePath);

            // 输出文件
            // 确保目录存在，如果不存在则创建
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            File.WriteAllText(filePath, fileContent);
        }
    }
}