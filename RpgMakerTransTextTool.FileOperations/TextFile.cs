using RpgMakerTransTextTool.TextOperations;

namespace RpgMakerTransTextTool.FileOperations;

public class TextFile
{
    //存储TXT下的已提取字符串extractedStrings(extractedString, filePath)

    public TextFile(string absoluteTextFilePath, string relativeTextFilePath, string txtString)
    {
        AbsoluteTextFilePath = absoluteTextFilePath;

        // // 单独提取Scripts文件夹下的TXT文件
        // if (relativeTextFilePath.StartsWith("Scripts")) StringExtractor.ExtractScriptsStrings(ExtractedStrings, txtString, absoluteTextFilePath);
        //
        // // 其他的TXT文件
        // else StringExtractor.ExtractOtherStrings(ExtractedStrings, txtString, relativeTextFilePath);


        StringExtractor.ExtractOtherStrings(ExtractedStrings, txtString, relativeTextFilePath);
    }

    // 记录TXT文件的绝对路径
    public string AbsoluteTextFilePath { get; }

    // 存储提取的字符串
    public List<string> ExtractedStrings { get; } = [];
}