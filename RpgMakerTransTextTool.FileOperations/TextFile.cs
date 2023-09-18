using RpgMakerTransTextTool.TextOperations;

namespace RpgMakerTransTextTool.FileOperations
{
    public class TextFile
    {
        private readonly string _absoluteFilePath;
        private readonly bool _isUnderScriptsFolder;
        //存储TXT下的字符串为一个长字符串
        private readonly string _txtString;
        //存储TXT下的已提取字符串extractedStrings(extractedString, filePath)
        private readonly List<string> _extractedStrings = new();

        public string AbsoluteFilePath => _absoluteFilePath;
        public List<string> ExtractedStrings => _extractedStrings;

        public TextFile(string absoluteFilePath, string relativeFilePath, string txtString)
        {
            _absoluteFilePath = absoluteFilePath;
            _isUnderScriptsFolder = relativeFilePath.StartsWith("Scripts");
            _txtString = txtString;
            if (_isUnderScriptsFolder)
            {
                //单独提取Scripts文件夹下的TXT文件
                StringExtractor.ExtractScriptsStrings(_txtString, absoluteFilePath, _extractedStrings);
            }
            else
            {
                //其他的TXT文件
                StringExtractor.ExtractOtherStrings(_txtString, absoluteFilePath, relativeFilePath, _extractedStrings);
            }
        }
    }
}
