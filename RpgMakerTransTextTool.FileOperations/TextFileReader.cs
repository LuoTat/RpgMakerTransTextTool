using System.Collections.Concurrent;

namespace RpgMakerTransTextTool.FileOperations;

public class TextFileReader(string absoluteFolderPath)
{
    private static void ReadTextFile(string absoluteTextFilePath, string relativeTextFilePath, ConcurrentBag<TextFile> textFileList)
    {
        string txtString = string.Empty;
        try
        {
            txtString = File.ReadAllText(absoluteTextFilePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error when reading file: {relativeTextFilePath}");
            Console.WriteLine(ex.ToString());
            Environment.Exit(1);
        }

        // 实例化一个TextFile并添加到textFileList
        TextFile textFile = new(absoluteTextFilePath, relativeTextFilePath, txtString);
        textFileList.Add(textFile);
    }

    public void ReadTextFilesInDirectory(ConcurrentBag<TextFile> textFileList, bool processScripts)
    {
        string[] fileExtensions = { ".txt", ".rb" };

        foreach (string extension in fileExtensions)
        {
            Parallel.ForEach(Directory.EnumerateFiles(absoluteFolderPath, $"*{extension}", SearchOption.AllDirectories), absoluteTextFilePath =>
            {
                // 获取filePath的相对路径
                string relativeTextFilePath = Path.GetRelativePath(absoluteFolderPath, absoluteTextFilePath);

                // 判断文件是否位于Scripts文件夹下
                bool isUnderScriptsFolder = relativeTextFilePath.StartsWith("Scripts");

                // 特殊处理Scripts文件夹下的TXT文件
                // 在每个线程中实例化一个TextFile并读取TXT文件
                if (!isUnderScriptsFolder || processScripts) ReadTextFile(absoluteTextFilePath, relativeTextFilePath, textFileList);
            });
        }
    }
}