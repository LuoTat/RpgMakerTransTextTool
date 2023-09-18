using System.Text;
using System.Text.RegularExpressions;

namespace RpgMakerTransTextTool.TextOperations
{
    public class StringInjector
    {
        private string _absoluteFolderPath;
        private readonly static string _rootFolderPath = AppDomain.CurrentDomain.BaseDirectory;
        private readonly static string _dictionaryDataBinFilePath = Path.Combine(_rootFolderPath, "Data", "DictionaryData.bin");
        private readonly static string _manualTransFileJsonFilePath = Path.Combine(_rootFolderPath, "Data", "ManualTransFile.json");
        private readonly static string _scriptsFolderPath = Path.Combine(_rootFolderPath, "Scripts");


        private readonly Dictionary<string, List<string>> _allExtractedStringsDictionary = new();
        private readonly List<string> _manualTransFileJsonUntranslatedStrings = new();
        private readonly List<string> _manualTransFileJsonTranslatedStrings = new();



        public StringInjector()
        {
            // _absoluteFolderPath = absoluteFolderPath;
            Directory.CreateDirectory(_scriptsFolderPath);
            Directory.CreateDirectory(Path.Combine(_scriptsFolderPath, "CommonEvents"));
            Directory.CreateDirectory(Path.Combine(_scriptsFolderPath, "Maps"));
            Directory.CreateDirectory(Path.Combine(_scriptsFolderPath, "Scripts"));

            ReadDictionaryFromBinaryFile();
            ExtractStringsFromJson();
        }
        public void ReadDictionaryFromBinaryFile()
        {
            try
            {
                using (FileStream fileStream = new(_dictionaryDataBinFilePath, FileMode.Open))
                {
                    using BinaryReader binaryReader = new(fileStream);
                    // 读取字典的Scripts文件夹的根目录
                    _absoluteFolderPath = binaryReader.ReadString();
                    // 读取字典的键值对数量
                    int count = binaryReader.ReadInt32();
                    // 循环读取每个键值对
                    for (int i = 1; i <= count; i++)
                    {
                        string key = new(binaryReader.ReadString());
                        // 读取值的数量
                        int valueCount = binaryReader.ReadInt32();
                        // 读取值并添加到列表
                        List<string> valueList = new();
                        for (int j = 1; j <= valueCount; j++)
                        {
                            //int absoluteFilePathLength = binaryReader.ReadInt32();
                            string absoluteFilePath = new(binaryReader.ReadString());
                            valueList.Add(absoluteFilePath);
                        }
                        // 将键值对添加到字典
                        _allExtractedStringsDictionary.Add(key, valueList);
                    }
                }
                Console.WriteLine("成功读取字典数据。");
            }
            catch (Exception ex)
            {
                Console.WriteLine("读取字典数据时出现异常：" + ex.Message);
            }
        }
        public void ExtractStringsFromJson()
        {
            string jsonContent = File.ReadAllText(_manualTransFileJsonFilePath);
            // 匹配字符串的正则表达式
            string pattern = @"^\s*""((?:\\.|[^""])*)""\s*:\s*""((?:\\.|[^""])*)""";
            MatchCollection matches = Regex.Matches(jsonContent, pattern, RegexOptions.Multiline);

            foreach (Match match in matches.Cast<Match>())
            {
                // 第一个捕获组
                string untranslatedString = match.Groups[1].Value;
                // 第二个捕获组
                string translatedString = match.Groups[2].Value;
                _manualTransFileJsonUntranslatedStrings.Add(untranslatedString);
                _manualTransFileJsonTranslatedStrings.Add(translatedString);
            }
        }
        public void ReplaceStringsInFiles()
        {
            //使用Dictionary缓存文件内容
            Dictionary<string, string> txtFileCache = new();
            //遍历所有键值对，每对键值对包含一个原始字符串和其在各个文件中的位置信息列表
            for (int i = 0; i < _manualTransFileJsonUntranslatedStrings.Count; i++)
            {
                string untranslatedString = _manualTransFileJsonUntranslatedStrings[i];
                string translatedString = _manualTransFileJsonTranslatedStrings[i];
                if (!string.IsNullOrEmpty(translatedString))
                {
                    if (_allExtractedStringsDictionary.TryGetValue(untranslatedString, out var extractedStringLocations))
                    {
                        foreach (var extractedStringLocation in extractedStringLocations)
                        {
                            string absoluteFilePath = extractedStringLocation;
                            //先判断文件内容是否已经在缓存中，如果不在则读取并缓存
                            if (!txtFileCache.TryGetValue(absoluteFilePath, out string? fileContent))
                            {
                                fileContent = File.ReadAllText(absoluteFilePath);
                                txtFileCache[absoluteFilePath] = fileContent;
                            }
                            //使用正则表达式替换所有未翻译的字符串为翻译后的字符串
                            string escapedUntranslatedString = Regex.Escape(untranslatedString);
                            //string escapedTranslatedString = Regex.Escape(translatedString);
                            string pattern = escapedUntranslatedString;
                            fileContent = Regex.Replace(fileContent, pattern, translatedString);
                            //将修改后的内容放回缓存中
                            txtFileCache[absoluteFilePath] = fileContent;
                        }
                    }
                }
                //Console.WriteLine($"正在替换第{i + 1}/{_manualTransFileJsonUntranslatedStrings.Count}个字符串");
            }
            //一次性将所有文件内容写回文件
            foreach (var kvp in txtFileCache)
            {
                string absoluteFilePath = kvp.Key;
                string relativeFilePath = Path.GetRelativePath(_absoluteFolderPath, absoluteFilePath);
                //bool isUnderScriptsFolder = relativeFilePath.StartsWith("Scripts");
                string filePath = Path.Combine(_scriptsFolderPath, relativeFilePath);
                string fileContent = kvp.Value;
                // 决定使用哪种编码
                //var fileEncoding = isUnderScriptsFolder ? Encoding.UTF8 : Encoding.UTF8;
                //File.WriteAllText(filePath, fileContent, fileEncoding);
                File.WriteAllText(filePath, fileContent);
            }
        }
    }
}
