using System.Collections.Concurrent;
using System.Diagnostics;
using RpgMakerTransTextTool.FileOperations;
using RpgMakerTransTextTool.TextOperations;

namespace RpgMakerTransTextTool.App;

internal abstract class Program
{
    // 记录当前程序的根目录
    private static readonly string AppRootFolderPath = AppDomain.CurrentDomain.BaseDirectory;

    private static void Main()
    {
        Console.WriteLine("欢迎使用RPGVXACE字符串处理工具！");
        Stopwatch stopWatch = new();
        while (true)
        {
            Console.WriteLine("请选择要执行的操作：");
            Console.WriteLine("1. 读取文件夹并提取字符串");
            Console.WriteLine("2. 写入提取的字符串");
            Console.WriteLine("0. 退出程序");

            string? input             = Console.ReadLine();
            string? scriptsFolderPath = string.Empty; // 记录Scripts文件夹的根目录

            if (input == "1")
            {
                // 根文件目录
                Console.WriteLine("请输入Scripts文件夹的根目录");
                while (true)
                {
                    scriptsFolderPath = Console.ReadLine();
                    if (string.IsNullOrEmpty(scriptsFolderPath)) Console.WriteLine("输入不能为空，请重新输入。");
                    else if (!Directory.Exists(scriptsFolderPath)) Console.WriteLine("输入的文件夹地址无效，请重新输入。");
                    else break;
                }

                // 询问是否需要提取Scripts文件夹下的TXT文件
                Console.WriteLine("是否想要提取Scripts文件夹下的TXT文件？(Y/N)");
                bool processScripts;
                while (true)
                {
                    string? userInput = Console.ReadLine();
                    if (!string.IsNullOrEmpty(userInput))
                    {
                        if (userInput.Trim().Equals("Y", StringComparison.CurrentCultureIgnoreCase))
                        {
                            processScripts = true;
                            break;
                        }

                        if (userInput.Trim().Equals("N", StringComparison.CurrentCultureIgnoreCase))
                        {
                            processScripts = false;
                            break;
                        }
                    }

                    Console.WriteLine("请输入Y/N");
                }

                // 用ConcurrentBag来保存所有的TextFile实例
                // ConcurrentBag是线程安全的
                ConcurrentBag<TextFile> textFileList = [];

                // 用textFileReader来读取所有的TXT文件
                // 将所有的TextFile实例保存到textFilesList里
                TextFileReader textFileReader = new(scriptsFolderPath);

                stopWatch.Start();
                //#################################################################################
                // 读取文件夹下的所有TXT文件
                textFileReader.ReadTextFilesInDirectory(textFileList, processScripts);
                //#################################################################################
                stopWatch.Stop();
                Console.WriteLine("TXT读取并提取结束");
                Console.WriteLine($"读取TXT共用时：{stopWatch.Elapsed}");

                // 实例化一个TextFileWriter并将textFileList传入
                TextFileWriter textFileWriter = new(textFileList);

                stopWatch.Restart();
                //#################################################
                // 输出DictionaryData.bin文件
                textFileWriter.SaveDictionaryToBinaryFile();
                //#################################################
                stopWatch.Stop();
                Console.WriteLine("生成字典结束");
                Console.WriteLine($"生成字典共用时：{stopWatch.Elapsed}");

                if (File.Exists(Path.Combine(AppRootFolderPath, "Data", "ManualTransFile.json")))
                {
                    Console.WriteLine("ManualTransFile.json文件已存在，是否要覆盖？ (Y/N)");
                    string? answer = Console.ReadLine();

                    // 如果用户输入N，则不覆盖
                    if (answer == null || answer.Trim().Equals("N", StringComparison.CurrentCultureIgnoreCase)) continue;
                    Console.WriteLine("覆盖ManualTransFile.json文件。");
                }

                stopWatch.Restart();
                //#################################################
                // 输出ManualTransFile.json文件
                textFileWriter.OutPutManualTransFileJson();
                //#################################################
                stopWatch.Stop();
                Console.WriteLine("生成ManualTransFile.json结束");
                Console.WriteLine($"生成ManualTransFile.json共用时：{stopWatch.Elapsed}");
            }
            else if (input == "2")
            {
                if (File.Exists(Path.Combine(AppRootFolderPath, "Data", "DictionaryData.bin")) && File.Exists(Path.Combine(AppRootFolderPath, "Data", "ManualTransFile.json")))
                {
                    // 实例化一个StringInjector
                    StringInjector stringInjector = new(scriptsFolderPath);

                    stopWatch.Restart();
                    //#####################################
                    // 替换所有的TXT文件中的字符串
                    stringInjector.ReplaceStringsInFiles();
                    //#####################################
                    stopWatch.Stop();
                    Console.WriteLine("字符串替换结束");
                    Console.WriteLine($"字符串替换共用时：{stopWatch.Elapsed}");
                }
                else Console.WriteLine("请先提取字符串。");
            }
            else if (input == "0")
            {
                // 退出程序
                Console.WriteLine("程序已退出。");
                break;
            }
            else Console.WriteLine("无效的选项，请重新选择！");
        }
    }
}