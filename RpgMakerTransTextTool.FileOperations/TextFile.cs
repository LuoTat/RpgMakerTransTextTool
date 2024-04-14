using RpgMakerTransTextTool.TextOperations;

namespace RpgMakerTransTextTool.FileOperations;

public class TextFile
{
    //存储TXT下的已提取字符串extractedStrings(extractedString, filePath)

    public TextFile(string absoluteTextFilePath, string txtString)
    {
        AbsoluteTextFilePath = absoluteTextFilePath;
        StringExtractor.ExtractStrings(ExtractedStrings, txtString);
    }

    // 记录TXT文件的绝对路径
    public string AbsoluteTextFilePath { get; }

    // 存储提取的字符串
    public List<string> ExtractedStrings { get; } = [];
}