using System.Text.RegularExpressions;

namespace RpgMakerTransTextTool.TextOperations;

public static partial class StringExtractor
{
    // private static readonly string[] AllOtherFileCategories =
    // [
    //     "CommonEvents",
    //     "Maps",
    //     "Actors",
    //     "Classes",
    //     "Enemies",
    //     "Items",
    //     "MapInfos",
    //     "Skills",
    //     "States",
    //     "System",
    //     "Troops"
    // ];
    //
    // private static readonly string[] AllOtherStringsPatterns =
    // [
    //     //((?:\\.|[^""])+)?
    //     @"^\s*(?:Name\b|ShowMessage\b|ShowChoices\b)[^""]*(?:""((?:\\.|[^""])+)?""[^""\n]*)+",        //CommonEvents
    //     @"^\s*(?:DisplayName\b|ShowMessage\b|ShowChoices\b)[^""]*(?:""((?:\\.|[^""])+)?""[^""\n]*)+", //Maps
    //     @"^\s*(?:Name\b|Nickname\b)[^""]*""((?:\\.|[^""])+)",                                         //Actors
    //     @"^\s*(?:Name\b)[^""]*""((?:\\.|[^""])+)",                                                    //Classes
    //     @"^\s*(?:Name\b)[^""]*""((?:\\.|[^""])+)",                                                    //Enemies
    //     @"^\s*(?:Name\b|Description\b)[^""]*""((?:\\.|[^""])+)",                                      //Items
    //     @"^\s*(?:MapName\b)[^""]*""((?:\\.|[^""])+)",                                                 //MapInfos
    //     @"^\s*(?:Name\b|Description\b|ActionMessage\b)[^""]*""((?:\\.|[^""])+)",                      //Skills
    //     @"^\s*(?:Name\b|Message\b)[^""]*""((?:\\.|[^""])+)",                                          //States
    //     @"^[^""]*""((?:\\.|[^""])+)",                                                                 //System
    //     @"^\s*(?:Name\b|ShowMessage\b)[^""]*""((?:\\.|[^""])+)"                                       //Troops
    // ];
    //
    // private static readonly string[] AllScriptsStringsPatterns =
    // [
    //     //标准读一行^[^""]*""([^""]+)
    //     //标准有多个^[^""\n]*(?:""([^""]+)?""[^""\n]*)+
    //     @"^[^""\n]*""((?:\\.|[^""])+)",                                                                                                                                    //Vocab.txt
    //     @"(Teleport to %s\?)",                                                                                                                                             //画面%002fパーティ編成.txt
    //     @"^(?!\s*(?:\bmethod\b|\btext\b|\bresult\b|\blv_s\b))[^""\n]*(?:""((?:\\.|[^""])+)?""[^""\n]*)+",                                                                  //画面%002f転職編集.txt
    //     @"^(?=\s*(?:\bROOT_COMMAND\b|\bBASIC_COMMAND\b|\bCLASS_HISTORY_COMMAND\b|\bBASIC_PARAM_VOCAB\b|\bSPECIAL_PARAM_VOCAB\b))[^""]*(?:""((?:\\.|[^""])+)?""[^""\]]*)+", //改造%002fステータス画面.txt
    //     @"^(?=\s*(?::lib_|GET_ITEM_|-?\d|draw_text\((?:lr|rect)|return\s\[|txt|text))[^""\n]*""((?:\\.|[^""])+)",                                                          //図鑑%002f本体.txt
    //     @"^(?=\s*(?:\$game_message\.add|Word\.new))[^""\n]+""((?:\\.|[^""])+)",                                                                                            //仲間化システム.txt
    //     @"^\s*RESULT[^""\n]+""((?:\\.|[^""])+)",                                                                                                                           //贈呈%002f好感度管理.txt
    //     @"^\s*(?:AREA|PLACE|CONFIRM)[^""\n]+""((?:\\.|[^""])+)",                                                                                                           //画面%002fワープ選択.txt
    //     @"^\s*(?:\$game_message\.add|text)[^""\n]+""((?:\\.|[^""])+)",                                                                                                     //公開コマンド.txt
    //     @"^.*(?::name\s=>|:help\s=>)[^""\n]+""((?:\\.|[^""])+)",                                                                                                           //コンフィグ＋.txt
    //     @"^\s*:desc\s{2}=>[^""\n]+""((?:\\.|[^""])+)",                                                                                                                     //娯楽%002fスロット.txt
    //     @"^\s*(?::dead_word|:orgasm_word|:predation_word|:incontinence_word)[^""\n]+""((?:\\.|[^""])+)",                                                                   //DownWords(Actor).txt
    //     @"^\s*(?::dead_word|:orgasm_word|:predation_word|:incontinence_word)[^""\n]+""((?:\\.|[^""])+)",                                                                   //DownWords(Enemy).txt
    //     @"^\s*:(?:question|yes|no)[^""\n]+""((?:\\.|[^""])+)",                                                                                                             //Follower.txt
    //     @"(?:^\s*PLACE\b[^""]*(?:""((?:\\.|[^""])+)?""[^""\]]*)+|^\s*{:name\s=>\s""((?:\\.|[^""])+))",                                                                     //IDReserve.txt
    //     @"^\s*""((?:\\.|[^""])+)",                                                                                                                                         //JobChange.txt
    //     @"^\s*ACTOR_FIX_ABILITY[^""]*(?:""((?:\\.|[^""])+)?""[^""\}]*)+",                                                                                                  //Library(Actor).txt
    //     @"^\s*(?:ENCOUNTER_ENEMY_PLACE|ENEMY_DESCRIPTION)[^""]*(?:""((?:\\.|[^""])+)?""[^""\}]*)+",                                                                        //Library(Enemy).txt
    //     @"^[^""#S\n]*""((?:\\.|[^""])+)""[^""\n]*",                                                                                                                        //Library(Medal).txt
    //     @"^\s*:word_\d\s=>\s\[""((?:\\.|[^""])+)",                                                                                                                         //Present.txt
    //     @"^\s*:word_\d\s=>\s\[""((?:\\.|[^""])+)",                                                                                                                         //SkillWords(Actor).txt
    //     @"^\s*:word_\d\s=>\s\[""((?:\\.|[^""])+)",                                                                                                                         //SkillWords(Enemy).txt
    //     @"^\s*:name\s=>\s""((?:\\.|[^""])+)",                                                                                                                              //Library(H).txt
    //     @"^\s*(?:ENEMY_SPECIAL_NAME|RACE_SPECIAL_NAME|SKILL_SPECIAL_NAME|ILLUSTRATOR_NAME|SPECIAL_SALE)[^\]]*(?:\[[^""]*""[^""]*"",\s""((?:\\.|[^""])+)?""\][^\[\]]+)+",   //▽ パッチスクリプト読込 ver5.txt
    //     @"^\s*def\slibrary[^""]*(?:""((?:\\.|[^""])+)?""[^""\}]*)+",                                                                                                       //トリス情報表示関連 ver8.txt
    //     @"^[^""\n]*(?:""((?:\\.|[^""#])+)?""[^""\n]*)+",                                                                                                                   //トリス戦闘回想 ver10.txt
    //     @"^[^""\n]*(?:""(?!Iconset)((?:\\.|[^""#])+)?""[^""\n]*)+"                                                                                                         //12%002fdamage_display_refine.txt
    // ];

    public static void ExtractStrings(List<string> extractedStrings, string txtString)
    {
        // 使用正则表达式查找所有匹配项
        // 遍历所有匹配项
        foreach (Match match in MyRegex().Matches(txtString))
        {
            // 获取匹配的字符串
            string extractedString = match.Groups[1].Value;

            // 将提取的字符串添加到列表中
            if (extractedString != string.Empty) extractedStrings.Add(extractedString);
            //Console.WriteLine($"{relativeFilePath}: {extractedString}"); //打印当前提取的字符串
        }
    }

    // 正则表达式，用于匹配被双引号包围的字符串
    [GeneratedRegex(@"""((?:\\.|[^""])+)?""", RegexOptions.Multiline)]
    private static partial Regex MyRegex();
}