using System.Text;
using System.Text.RegularExpressions;

namespace RpgMakerTransTextTool.TextOperations;

public static partial class StringExtractor
{
    private const RegexOptions Options = RegexOptions.Multiline;

    private static readonly string[] AllOtherFileCategories =
    [
        "CommonEvents",
        "Maps",
        "Actors",
        "Classes",
        "Enemies",
        "Items",
        "MapInfos",
        "Skills",
        "States",
        "System",
        "Troops"
    ];

    private static readonly string[] AllOtherStringsPatterns =
    [
        //((?:\\.|[^""])+)?
        @"^\s*(?:Name\b|ShowMessage\b|ShowChoices\b)[^""]*(?:""((?:\\.|[^""])+)?""[^""\n]*)+",        //CommonEvents
        @"^\s*(?:DisplayName\b|ShowMessage\b|ShowChoices\b)[^""]*(?:""((?:\\.|[^""])+)?""[^""\n]*)+", //Maps
        @"^\s*(?:Name\b|Nickname\b)[^""]*""((?:\\.|[^""])+)",                                         //Actors
        @"^\s*(?:Name\b)[^""]*""((?:\\.|[^""])+)",                                                    //Classes
        @"^\s*(?:Name\b)[^""]*""((?:\\.|[^""])+)",                                                    //Enemies
        @"^\s*(?:Name\b|Description\b)[^""]*""((?:\\.|[^""])+)",                                      //Items
        @"^\s*(?:MapName\b)[^""]*""((?:\\.|[^""])+)",                                                 //MapInfos
        @"^\s*(?:Name\b|Description\b|ActionMessage\b)[^""]*""((?:\\.|[^""])+)",                      //Skills
        @"^\s*(?:Name\b|Message\b)[^""]*""((?:\\.|[^""])+)",                                          //States
        @"^[^""]*""((?:\\.|[^""])+)",                                                                 //System
        @"^\s*(?:Name\b|ShowMessage\b)[^""]*""((?:\\.|[^""])+)"                                       //Troops
    ];

    private static readonly string[] AllScriptsStringsPatterns =
    [
        //标准读一行^[^""]*""([^""]+)
        //标准有多个^[^""\n]*(?:""([^""]+)?""[^""\n]*)+
        @"^[^""\n]*""((?:\\.|[^""])+)",                                                                                                                                    //Vocab.txt
        @"(Teleport to %s\?)",                                                                                                                                             //画面%002fパーティ編成.txt
        @"^(?!\s*(?:\bmethod\b|\btext\b|\bresult\b|\blv_s\b))[^""\n]*(?:""((?:\\.|[^""])+)?""[^""\n]*)+",                                                                  //画面%002f転職編集.txt
        @"^(?=\s*(?:\bROOT_COMMAND\b|\bBASIC_COMMAND\b|\bCLASS_HISTORY_COMMAND\b|\bBASIC_PARAM_VOCAB\b|\bSPECIAL_PARAM_VOCAB\b))[^""]*(?:""((?:\\.|[^""])+)?""[^""\]]*)+", //改造%002fステータス画面.txt
        @"^(?=\s*(?::lib_|GET_ITEM_|-?\d|draw_text\((?:lr|rect)|return\s\[|txt|text))[^""\n]*""((?:\\.|[^""])+)",                                                          //図鑑%002f本体.txt
        @"^(?=\s*(?:\$game_message\.add|Word\.new))[^""\n]+""((?:\\.|[^""])+)",                                                                                            //仲間化システム.txt
        @"^\s*RESULT[^""\n]+""((?:\\.|[^""])+)",                                                                                                                           //贈呈%002f好感度管理.txt
        @"^\s*(?:AREA|PLACE|CONFIRM)[^""\n]+""((?:\\.|[^""])+)",                                                                                                           //画面%002fワープ選択.txt
        @"^\s*(?:\$game_message\.add|text)[^""\n]+""((?:\\.|[^""])+)",                                                                                                     //公開コマンド.txt
        @"^.*(?::name\s=>|:help\s=>)[^""\n]+""((?:\\.|[^""])+)",                                                                                                           //コンフィグ＋.txt
        @"^\s*:desc\s{2}=>[^""\n]+""((?:\\.|[^""])+)",                                                                                                                     //娯楽%002fスロット.txt
        @"^\s*(?::dead_word|:orgasm_word|:predation_word|:incontinence_word)[^""\n]+""((?:\\.|[^""])+)",                                                                   //DownWords(Actor).txt
        @"^\s*(?::dead_word|:orgasm_word|:predation_word|:incontinence_word)[^""\n]+""((?:\\.|[^""])+)",                                                                   //DownWords(Enemy).txt
        @"^\s*:(?:question|yes|no)[^""\n]+""((?:\\.|[^""])+)",                                                                                                             //Follower.txt
        @"(?:^\s*PLACE\b[^""]*(?:""((?:\\.|[^""])+)?""[^""\]]*)+|^\s*{:name\s=>\s""((?:\\.|[^""])+))",                                                                     //IDReserve.txt
        @"^\s*""((?:\\.|[^""])+)",                                                                                                                                         //JobChange.txt
        @"^\s*ACTOR_FIX_ABILITY[^""]*(?:""((?:\\.|[^""])+)?""[^""\}]*)+",                                                                                                  //Library(Actor).txt
        @"^\s*(?:ENCOUNTER_ENEMY_PLACE|ENEMY_DESCRIPTION)[^""]*(?:""((?:\\.|[^""])+)?""[^""\}]*)+",                                                                        //Library(Enemy).txt
        @"^[^""#S\n]*""((?:\\.|[^""])+)""[^""\n]*",                                                                                                                        //Library(Medal).txt
        @"^\s*:word_\d\s=>\s\[""((?:\\.|[^""])+)",                                                                                                                         //Present.txt
        @"^\s*:word_\d\s=>\s\[""((?:\\.|[^""])+)",                                                                                                                         //SkillWords(Actor).txt
        @"^\s*:word_\d\s=>\s\[""((?:\\.|[^""])+)",                                                                                                                         //SkillWords(Enemy).txt
        @"^\s*:name\s=>\s""((?:\\.|[^""])+)",                                                                                                                              //Library(H).txt
        @"^\s*(?:ENEMY_SPECIAL_NAME|RACE_SPECIAL_NAME|SKILL_SPECIAL_NAME|ILLUSTRATOR_NAME|SPECIAL_SALE)[^\]]*(?:\[[^""]*""[^""]*"",\s""((?:\\.|[^""])+)?""\][^\[\]]+)+",   //▽ パッチスクリプト読込 ver5.txt
        @"^\s*def\slibrary[^""]*(?:""((?:\\.|[^""])+)?""[^""\}]*)+",                                                                                                       //トリス情報表示関連 ver8.txt
        @"^[^""\n]*(?:""((?:\\.|[^""#])+)?""[^""\n]*)+",                                                                                                                   //トリス戦闘回想 ver10.txt
        @"^[^""\n]*(?:""(?!Iconset)((?:\\.|[^""#])+)?""[^""\n]*)+"                                                                                                         //12%002fdamage_display_refine.txt
    ];

    public static void ExtractOtherStrings(List<string> extractedStrings, string txtString, string relativeFilePath)
    {
        // 正则表达式，用于匹配被双引号包围的字符串
        const string pattern = @"""((?:\\.|[^""])+)?""";

        // 使用正则表达式查找所有匹配项
        // 遍历所有匹配项
        foreach (Match match in Regex.Matches(txtString, pattern, Options))
        {
            // 获取匹配的字符串
            string extractedString = match.Groups[1].Value;

            // 将提取的字符串添加到列表中
            if (extractedString != string.Empty) extractedStrings.Add(extractedString);
            //Console.WriteLine($"{relativeFilePath}: {extractedString}"); //打印当前提取的字符串
        }
    }

    // public static void ExtractScriptsStrings(List<string> extractedStrings, string txtString, string absoluteFilePath)
    // {
    //     // 获取文件名
    //     string fileName = Path.GetFileName(absoluteFilePath);
    //
    //     // 使用正则表达式进行匹配
    //     const string ignorePart          = "Script_\\d{8}_";
    //     Regex        regex               = new(ignorePart);
    //     string       scriptsFileCategory = regex.Replace(fileName, string.Empty);
    //
    //     string? pattern = scriptsFileCategory switch
    //     {
    //         "Vocab.txt"                        => AllScriptsStringsPatterns[0],
    //         "画面%002fパーティ編成.txt"                => AllScriptsStringsPatterns[1],
    //         "画面%002f転職編集.txt"                  => AllScriptsStringsPatterns[2],
    //         "改造%002fステータス画面.txt"               => AllScriptsStringsPatterns[3],
    //         "図鑑%002f本体.txt"                    => AllScriptsStringsPatterns[4],
    //         "仲間化システム.txt"                      => AllScriptsStringsPatterns[5],
    //         "贈呈%002f好感度管理.txt"                 => AllScriptsStringsPatterns[6],
    //         "画面%002fワープ選択.txt"                 => AllScriptsStringsPatterns[7],
    //         "公開コマンド.txt"                       => AllScriptsStringsPatterns[8],
    //         "コンフィグ＋.txt"                       => AllScriptsStringsPatterns[9],
    //         "娯楽%002fスロット.txt"                  => AllScriptsStringsPatterns[10],
    //         "DownWords(Actor).txt"             => AllScriptsStringsPatterns[11],
    //         "DownWords(Enemy).txt"             => AllScriptsStringsPatterns[12],
    //         "Follower.txt"                     => AllScriptsStringsPatterns[13],
    //         "IDReserve.txt"                    => AllScriptsStringsPatterns[14],
    //         "JobChange.txt"                    => AllScriptsStringsPatterns[15],
    //         "Library(Actor).txt"               => AllScriptsStringsPatterns[16],
    //         "Library(Enemy).txt"               => AllScriptsStringsPatterns[17],
    //         "Library(Medal).txt"               => AllScriptsStringsPatterns[18],
    //         "Present.txt"                      => AllScriptsStringsPatterns[19],
    //         "SkillWords(Actor).txt"            => AllScriptsStringsPatterns[20],
    //         "SkillWords(Enemy).txt"            => AllScriptsStringsPatterns[21],
    //         "Library(H).txt"                   => AllScriptsStringsPatterns[22],
    //         "▽ パッチスクリプト読込 ver5.txt"            => AllScriptsStringsPatterns[23],
    //         "トリス情報表示関連 ver8.txt"               => AllScriptsStringsPatterns[24],
    //         "トリス戦闘回想 ver10.txt"                => AllScriptsStringsPatterns[25],
    //         "12%002fdamage_display_refine.txt" => AllScriptsStringsPatterns[26],
    //         _                                  => null
    //     };
    //
    //     // 如果没有匹配到文件类别，则直接返回
    //     if (pattern == null) return;
    //
    //     MatchCollection matches = Regex.Matches(txtString, pattern, Options);
    //     foreach (Match? match in matches.Cast<Match>())
    //         foreach (Group? group in match.Groups.Cast<Group>().Skip(1))
    //         {
    //             CaptureCollection captures = group.Captures;
    //             foreach (Capture? capture in captures.Cast<Capture>()) extractedStrings.Add(capture.Value);
    //
    //             //Console.WriteLine($"{absoluteFilePath}: {extractedString}"); //打印当前提取的字符串
    //         }
    // }
}