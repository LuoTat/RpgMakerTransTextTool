using System.Collections.Concurrent;

namespace RpgMakerTransTextTool.FileOperations
{
    public class TextFileReader
    {
        //存储根文件夹的绝对路径
        private string _absoluteFolderPath;

        public TextFileReader(string folderPath)
        {
            _absoluteFolderPath = folderPath;
        }
        //public void ReadTextFile(string filePath, string relativeFilePath, ConcurrentBag<TextFile> textFileList)
        //{
        //    string txtString;
        //    try
        //    {
        //        using (StreamReader reader = new StreamReader(filePath))
        //        {
        //            txtString = reader.ReadToEnd();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error reading file: {ex.Message}");
        //        // 这里可以根据需要进行异常处理
        //        txtString = string.Empty; // 或者返回 null，表示读取失败
        //    }
        //    TextFile textFile = new TextFile(filePath, relativeFilePath, txtString);
        //    textFileList.Add(textFile);
        //}
        //public void ReadTextFilesInDirectory(string folderPath, ConcurrentBag<TextFile> textFileList, bool processScripts)
        //{
        //    //获取根文件夹下的所有TXT文件
        //    var txtFiles = Directory.EnumerateFiles(folderPath, "*.txt");

        //    //处理当前文件夹下的所有TXT文件
        //    foreach (var txtFile in txtFiles)
        //    {
        //        //获取txtFile的相对路径
        //        string relativeFilePath = Path.GetRelativePath(_absoluteFolderPath, txtFile);
        //        //判断文件是否位于Scripts文件夹下
        //        bool isUnderScriptsFolder = relativeFilePath.StartsWith("Scripts");

        //        //特殊处理Scripts文件夹下的TXT文件
        //        if (isUnderScriptsFolder && !processScripts) { }
        //        else
        //        {
        //            //在每个线程中实例化一个TextFile并读取TXT文件
        //            ReadTextFile(txtFile, relativeFilePath, textFileList);

        //        }
        //    }
        //    //处理子文件夹
        //    var subFolders = Directory.EnumerateDirectories(folderPath);
        //    foreach (var subFolder in subFolders)
        //    {
        //        //递归处理子文件夹
        //        ReadTextFilesInDirectory(subFolder, textFileList, processScripts);
        //    }

        //}


        public void ReadTextFile(string absoluteFilePath, string relativeFilePath, ConcurrentBag<TextFile> textFileList)
        {
            string txtString;
            try
            {
                //using (StreamReader reader = new StreamReader(filePath))
                //{
                //    txtString = reader.ReadToEnd();
                //}
                txtString = File.ReadAllText(absoluteFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
                // 这里可以根据需要进行异常处理
                txtString = string.Empty; // 或者返回 null，表示读取失败
            }
            TextFile textFile = new TextFile(absoluteFilePath, relativeFilePath, txtString);
            textFileList.Add(textFile);
        }
        public void ReadTextFilesInDirectory(string folderPath, ConcurrentBag<TextFile> textFileList, bool processScripts)
        {
            Parallel.ForEach(Directory.EnumerateFiles(folderPath, "*.txt"), absoluteFilePath =>
            {
                //获取filePath的相对路径
                string relativeFilePath = Path.GetRelativePath(_absoluteFolderPath, absoluteFilePath);
                //判断文件是否位于Scripts文件夹下
                bool isUnderScriptsFolder = relativeFilePath.StartsWith("Scripts");

                //特殊处理Scripts文件夹下的TXT文件
                if (isUnderScriptsFolder && !processScripts) { }
                else
                {
                    //在每个线程中实例化一个TextFile并读取TXT文件
                    ReadTextFile(absoluteFilePath, relativeFilePath, textFileList);
                }
            });
            Parallel.ForEach(Directory.EnumerateDirectories(folderPath), subFolderPath =>
            {
                //递归处理子文件夹
                ReadTextFilesInDirectory(subFolderPath, textFileList, processScripts);
            });
        }
    }
}