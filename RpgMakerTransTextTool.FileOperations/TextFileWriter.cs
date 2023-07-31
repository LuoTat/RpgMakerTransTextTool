using System.Collections.Concurrent;

namespace RpgMakerTransTextTool.FileOperations
{
    public class TextFileWriter
    {
        private Dictionary<string, List<string>> _allExtractedStringsDictionary = new(StringComparer.Ordinal);
        private static readonly string _rootFolderPath = AppDomain.CurrentDomain.BaseDirectory;


        //实现把所有TXT的已提取字符串元组存储为字典
        public TextFileWriter(ConcurrentBag<TextFile> textFileList)
        {
            Directory.CreateDirectory(Path.Combine(_rootFolderPath, "Data"));
            //遍历当前文件中提取的字符串信息
            foreach (var textFile in textFileList)
            {
                //获取当前文件的绝对路径
                string absoluteFilePath = textFile.AbsoluteFilePath;
                //遍历当前文件中提取的字符串信息
                foreach (var extractedStringInfo in textFile.ExtractedStrings)
                {
                    string extractedString = extractedStringInfo;
                    //如果字典中已经包含该字符串，则将当前文件的位置信息添加到对应的列表中
                    if (_allExtractedStringsDictionary.TryGetValue(extractedString, out var extractedStringLocations))
                    {
                        extractedStringLocations.Add(absoluteFilePath);
                    }
                    else //否则，在字典中新增该字符串及其位置信息列表
                    {
                        _allExtractedStringsDictionary.Add(extractedString, new List<string> { absoluteFilePath });
                    }
                }
            }
        }
        //输出ManualTransFile.json文件
        public void OutPutManualTransFileJson()
        {
            //使用StringBuilder来拼接ManualTransFile.json字符串
            var jsonStringBuilder = new System.Text.StringBuilder();
            jsonStringBuilder.AppendLine("{");
            foreach (var extractedString in _allExtractedStringsDictionary.Keys)
            {
                jsonStringBuilder.AppendLine($"    \"{extractedString}\": \"{extractedString}\",");
            }
            //移除最后一个多余的逗号
            if (_allExtractedStringsDictionary.Count > 0)
            {
                jsonStringBuilder.Remove(jsonStringBuilder.Length - 3, 1);
            }
            jsonStringBuilder.AppendLine("}");
            //将JSON字符串写入文件
            string outputFilePath = Path.Combine(_rootFolderPath, "Data", "ManualTransFile.json");
            using (StreamWriter writer = new(outputFilePath))
            {
                writer.Write(jsonStringBuilder.ToString());
            }
            Console.WriteLine("\n提取的ManualTransFile.json结果已保存到文件中。");
        }
        // 将字典输出为二进制文件
        public void SaveDictionaryToBinaryFile()
        {
            string outputFilePath = Path.Combine(_rootFolderPath, "Data", "DictionaryData.bin");
            try
            {
                using (BinaryWriter binaryWriter = new(File.Open(outputFilePath, FileMode.Create)))
                {
                    //写入字典的键值对数量
                    binaryWriter.Write(_allExtractedStringsDictionary.Count);
                    foreach (var kvp in _allExtractedStringsDictionary)
                    {
                        //写入键的内容
                        binaryWriter.Write(kvp.Key);
                        //写入每个键对应的值的数量和信息
                        binaryWriter.Write(kvp.Value.Count);
                        foreach (var location in kvp.Value)
                        {
                            binaryWriter.Write(location);//字符串所在文件的绝对路径
                        }
                    }
                }
                Console.WriteLine("提取的DictionaryData.bin结果已保存到文件中。");
            }
            catch (Exception ex)
            {
                Console.WriteLine("保存字典到二进制文件时出现异常：" + ex.Message);
            }
        }
    }
}
