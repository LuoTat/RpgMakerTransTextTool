using System.Collections.Concurrent;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RpgMakerTransTextTool.FileOperations;

public class TextFileWriter
{
    // 记录当前程序的根目录
    private static readonly string AppRootFolderPath = AppDomain.CurrentDomain.BaseDirectory;

    // 记录Script文件夹的根目录
    private readonly string _scriptsFolderPath;

    // 用Dictionary存储所有提取的字符串及其位置信息
    // 字典的键为提取的字符串，值为包含该字符串的文件的绝对路径列表
    private readonly Dictionary<string, List<string>> _allExtractedStringsDictionary = new(StringComparer.Ordinal);


    // 实现把所有TextFile存储为字典
    public TextFileWriter(ConcurrentBag<TextFile> textFileList, string scriptsFolderPath)

    {
        // 创建Data文件夹
        Directory.CreateDirectory(Path.Combine(AppRootFolderPath, "Data"));
        _scriptsFolderPath = scriptsFolderPath;

        // 遍历当前textFileList中提取的字符串信息
        foreach (TextFile textFile in textFileList)
        {
            // 获取当前文件的绝对路径
            string absoluteFilePath = textFile.AbsoluteTextFilePath;

            // 遍历当前文件中提取的字符串信息
            foreach (string extractedString in textFile.ExtractedStrings)
            {
                // 如果字典中已经包含该字符串，则将当前文件的位置信息添加到对应的列表中
                if (_allExtractedStringsDictionary.TryGetValue(extractedString, out List<string>? extractedStringsLocationsList)) extractedStringsLocationsList.Add(absoluteFilePath);
                else _allExtractedStringsDictionary.Add(extractedString, [absoluteFilePath]); // 否则，在字典中新增该字符串及其位置信息列表
            }
        }
    }

    // 输出ManualTransFile.json文件
    public void OutPutManualTransFileJson()
    {
        // 创建一个新的 JObject
        JObject jObject = new();

        // 遍历 _allExtractedStringsDictionary 的键，将每个键作为属性和值添加到 JObject 中
        foreach (string key in _allExtractedStringsDictionary.Keys)
        {
            jObject.Add(key, JToken.FromObject(key));
        }

        // 将 JObject 对象转换为 JSON 字符串
        string json = jObject.ToString(Formatting.Indented);

        // 将 JSON 字符串写入文件
        string outputFilePath = Path.Combine(AppRootFolderPath, "Data", "ManualTransFile.json");
        File.WriteAllText(outputFilePath, json);
    }

    // 将字典输出为JSON文件
    public void SaveDictionaryToBinaryFile()
    {
        string outPutFilePath = Path.Combine(AppRootFolderPath, "Data", "DictionaryData.bin");

        try
        {
            // 创建一个新的 JObject
            JObject jsonObject = new()
            {
                // 将 _scriptsFolderPath 的值添加到 JObject 中
                { "_scriptsFolderPath", JToken.FromObject(_scriptsFolderPath) },
                // 将 _allExtractedStringsDictionary 添加到 JObject 中
                { "_allExtractedStringsDictionary", JToken.FromObject(_allExtractedStringsDictionary) }
            };

            // 将 JObject 对象转换为 JSON 字符串
            string json = jsonObject.ToString(Formatting.Indented);

            // 将 JSON 字符串写入文件
            File.WriteAllText(outPutFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("保存字典到DictionaryData.bin时出现异常");
            Console.WriteLine(ex.ToString());
            Environment.Exit(1);
        }
    }
}