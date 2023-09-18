using RpgMakerTransTextTool.FileOperations;
using RpgMakerTransTextTool.TextOperations;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace RpgMakerTransTextTool
{
    internal class Program
    {
        private static readonly string _rootFolderPath = AppDomain.CurrentDomain.BaseDirectory;

        static void Main(string[] args)
        {
            Console.WriteLine("欢迎使用RPGVXACE字符串处理工具！\n");
            Stopwatch stopWatch = new();
            TimeSpan elapsedTime;
            while (true)
            {
                Console.WriteLine("请选择要执行的操作：");
                Console.WriteLine("1. 读取文件夹并提取字符串");
                Console.WriteLine("2. 写入提取的字符串");
                Console.WriteLine("0. 退出程序");
                string? input = Console.ReadLine();
                if (input == "1")
                {
                    //根文件目录
                    Console.WriteLine("请输入Scripts文件夹的根目录");
                    string folderPath;
                    while (true)
                    {
                        folderPath = Console.ReadLine()!;
                        if (string.IsNullOrEmpty(folderPath)) { Console.WriteLine("输入不能为空，请重新输入。\n"); }
                        else if (!System.IO.Directory.Exists(folderPath)) { Console.WriteLine("输入的文件夹地址无效，请重新输入。\n"); }
                        else { break; }
                    }
                    //询问是否需要提取Scripts文件夹下的TXT文件
                    Console.WriteLine("\n是否想要提取Scripts文件夹下的TXT文件？(Y/N)");
                    bool processScripts;
                    while (true)
                    {
                        string userInput = Console.ReadLine()!;
                        if (userInput.Trim().ToUpper() == "Y") { processScripts = true; break; }
                        else if (userInput.Trim().ToUpper() == "N") { processScripts = false; break; }
                        else { Console.WriteLine("请输入Y/N"); }
                    }
                    //用于保存所有的TextFile实例
                    ConcurrentBag<TextFile> textFileList = new();
                    //实现所有的TextFile实例并且保存到textFiles里
                    TextFileReader textFileReader = new(folderPath);
                    stopWatch.Start();
                    //#################################################################################
                    textFileReader.ReadTextFilesInDirectory(folderPath, textFileList, processScripts);
                    //#################################################################################
                    stopWatch.Stop();
                    elapsedTime = stopWatch.Elapsed;
                    Console.WriteLine("\nTXT读取并提取结束");
                    Console.WriteLine($"读取TXT共用时：{elapsedTime}");
                    stopWatch.Restart();
                    //#################################################
                    TextFileWriter textFileWriter = new(textFileList);
                    //输出DictionaryData.bin文件
                    textFileWriter.SaveDictionaryToBinaryFile(folderPath);
                    //#################################################
                    stopWatch.Stop();
                    elapsedTime = stopWatch.Elapsed;
                    Console.WriteLine($"生成字典与JSON文件共用时：{elapsedTime}");
                    if (File.Exists(Path.Combine(_rootFolderPath, "Data", "ManualTransFile.json")))
                    {
                        Console.WriteLine("\nManualTransFile.json文件已存在，是否要覆盖？ (Y/N)");
                        string? answer = Console.ReadLine();
                        if (answer != null && (answer.ToUpper() == "Y" || answer.ToUpper() == "YES"))
                        {
                            //覆盖ManualTransFile.json文件
                            textFileWriter.OutPutManualTransFileJson();
                        }
                    }
                    else
                    {
                        //输出ManualTransFile.json文件
                        textFileWriter.OutPutManualTransFileJson();
                    }
                }
                else if (input == "2")
                {
                    if (File.Exists(Path.Combine(_rootFolderPath, "Data", "DictionaryData.bin")) && File.Exists(Path.Combine(_rootFolderPath, "Data", "ManualTransFile.json")))
                    {
                        StringInjector stringInjector = new();
                        stopWatch.Restart();
                        //#####################################
                        stringInjector.ReplaceStringsInFiles();
                        //#####################################
                        stopWatch.Stop();
                        elapsedTime = stopWatch.Elapsed;
                        Console.WriteLine("字符串替换结束");
                        Console.WriteLine($"字符串替换共用时：{elapsedTime}");
                    }
                    else
                    {
                        Console.WriteLine("请先提取字符串。");
                    }
                }
                else if (input == "0")
                {
                    // 退出程序
                    Console.WriteLine("程序已退出。");
                    break;
                }
                else
                {
                    Console.WriteLine("无效的选项，请重新选择！");
                }
            }
        }

    }
}