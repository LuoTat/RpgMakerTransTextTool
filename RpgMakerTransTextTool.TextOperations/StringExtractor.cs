using System.Text;
using System.Text.RegularExpressions;

namespace RpgMakerTransTextTool.TextOperations
{
    public static class StringExtractor
    {
        private static readonly string[] _allOtherFileCategories =
        {
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
         };
        private static readonly string[] _allOtherStringsPatterns =
        {
            //((?:\\.|[^""])+)?
            @"^\s*(?:Name\b|ShowMessage\b|ShowChoices\b)[^""]*(?:""((?:\\.|[^""])+)?""[^""\n]*)+",                                                                              //CommonEvents
            @"^\s*(?:DisplayName\b|ShowMessage\b|ShowChoices\b)[^""]*(?:""((?:\\.|[^""])+)?""[^""\n]*)+",                                                                       //Maps
            @"^\s*(?:Name\b|Nickname\b)[^""]*""((?:\\.|[^""])+)",                                                                                                               //Actors
            @"^\s*(?:Name\b)[^""]*""((?:\\.|[^""])+)",                                                                                                                          //Classes
            @"^\s*(?:Name\b)[^""]*""((?:\\.|[^""])+)",                                                                                                                          //Enemies
            @"^\s*(?:Name\b|Description\b)[^""]*""((?:\\.|[^""])+)",                                                                                                            //Items
            @"^\s*(?:MapName\b)[^""]*""((?:\\.|[^""])+)",                                                                                                                       //MapInfos
            @"^\s*(?:Name\b|Description\b|ActionMessage\b)[^""]*""((?:\\.|[^""])+)",                                                                                            //Skills
            @"^\s*(?:Name\b|Message\b)[^""]*""((?:\\.|[^""])+)",                                                                                                                //States
            @"^[^""]*""((?:\\.|[^""])+)",                                                                                                                                       //System
            @"^\s*(?:Name\b|ShowMessage\b)[^""]*""((?:\\.|[^""])+)",                                                                                                            //Troops
        };
        private static readonly string[] _allScriptsStringsPatterns =
        {
            //标准读一行^[^""]*""([^""]+)
            //标准有多个^[^""\n]*(?:""([^""]+)?""[^""\n]*)+
            @"^[^""\n]*""((?:\\.|[^""])+)",                                                                                                                                     //Vocab.txt
            @"(Teleport to %s\?)",                                                                                                                                              //画面%002fパーティ編成.txt
            @"^(?!\s*(?:\bmethod\b|\btext\b|\bresult\b|\blv_s\b))[^""\n]*(?:""((?:\\.|[^""])+)?""[^""\n]*)+",                                                                   //画面%002f転職編集.txt
            @"^(?=\s*(?:\bROOT_COMMAND\b|\bBASIC_COMMAND\b|\bCLASS_HISTORY_COMMAND\b|\bBASIC_PARAM_VOCAB\b|\bSPECIAL_PARAM_VOCAB\b))[^""]*(?:""((?:\\.|[^""])+)?""[^""\]]*)+",  //改造%002fステータス画面.txt
            @"^(?=\s*(?::lib_|GET_ITEM_|-?\d|draw_text\((?:lr|rect)|return\s\[|txt|text))[^""\n]*""((?:\\.|[^""])+)",                                                           //図鑑%002f本体.txt    
            @"^(?=\s*(?:\$game_message\.add|Word\.new))[^""\n]+""((?:\\.|[^""])+)",                                                                                             //仲間化システム.txt
            @"^\s*RESULT[^""\n]+""((?:\\.|[^""])+)",                                                                                                                            //贈呈%002f好感度管理.txt
            @"^\s*(?:AREA|PLACE|CONFIRM)[^""\n]+""((?:\\.|[^""])+)",                                                                                                            //画面%002fワープ選択.txt
            @"^\s*(?:\$game_message\.add|text)[^""\n]+""((?:\\.|[^""])+)",                                                                                                      //公開コマンド.txt
            @"^.*(?::name\s=>|:help\s=>)[^""\n]+""((?:\\.|[^""])+)",                                                                                                            //コンフィグ＋.txt
            @"^\s*:desc\s{2}=>[^""\n]+""((?:\\.|[^""])+)",                                                                                                                      //娯楽%002fスロット.txt
            @"^\s*(?::dead_word|:orgasm_word|:predation_word|:incontinence_word)[^""\n]+""((?:\\.|[^""])+)",                                                                    //DownWords(Actor).txt
            @"^\s*(?::dead_word|:orgasm_word|:predation_word|:incontinence_word)[^""\n]+""((?:\\.|[^""])+)",                                                                    //DownWords(Enemy).txt
            @"^\s*:(?:question|yes|no)[^""\n]+""((?:\\.|[^""])+)",                                                                                                              //Follower.txt
            @"(?:^\s*PLACE\b[^""]*(?:""((?:\\.|[^""])+)?""[^""\]]*)+|^\s*{:name\s=>\s""((?:\\.|[^""])+))",                                                                      //IDReserve.txt
            @"^\s*""((?:\\.|[^""])+)",                                                                                                                                          //JobChange.txt
            @"^\s*ACTOR_FIX_ABILITY[^""]*(?:""((?:\\.|[^""])+)?""[^""\}]*)+",                                                                                                   //Library(Actor).txt
            @"^\s*(?:ENCOUNTER_ENEMY_PLACE|ENEMY_DESCRIPTION)[^""]*(?:""((?:\\.|[^""])+)?""[^""\}]*)+",                                                                         //Library(Enemy).txt
            @"^[^""#S\n]*""((?:\\.|[^""])+)""[^""\n]*",                                                                                                                         //Library(Medal).txt
            @"^\s*:word_\d\s=>\s\[""((?:\\.|[^""])+)",                                                                                                                          //Present.txt
            @"^\s*:word_\d\s=>\s\[""((?:\\.|[^""])+)",                                                                                                                          //SkillWords(Actor).txt
            @"^\s*:word_\d\s=>\s\[""((?:\\.|[^""])+)",                                                                                                                          //SkillWords(Enemy).txt
            @"^\s*:name\s=>\s""((?:\\.|[^""])+)",                                                                                                                               //Library(H).txt
            @"^\s*(?:ENEMY_SPECIAL_NAME|RACE_SPECIAL_NAME|SKILL_SPECIAL_NAME|ILLUSTRATOR_NAME|SPECIAL_SALE)[^\]]*(?:\[[^""]*""[^""]*"",\s""((?:\\.|[^""])+)?""\][^\[\]]+)+",    //▽ パッチスクリプト読込 ver5.txt
            @"^\s*def\slibrary[^""]*(?:""((?:\\.|[^""])+)?""[^""\}]*)+",                                                                                                        //トリス情報表示関連 ver8.txt
            @"^[^""\n]*(?:""((?:\\.|[^""#])+)?""[^""\n]*)+",                                                                                                                    //トリス戦闘回想 ver10.txt
            @"^[^""\n]*(?:""(?!Iconset)((?:\\.|[^""#])+)?""[^""\n]*)+",                                                                                                         //12%002fdamage_display_refine.txt
        };
        private static readonly RegexOptions _options = RegexOptions.Multiline;

        public static void ExtractOtherStrings(string txtString, string absoluteFilePath, string relativeFilePath, List<string> extractedStrings)
        {
            string? pattern = _allOtherFileCategories.FirstOrDefault(relativeFilePath.StartsWith) switch
            {
                "CommonEvents" => _allOtherStringsPatterns[0],
                "Maps" => _allOtherStringsPatterns[1],
                "Actors" => _allOtherStringsPatterns[2],
                "Classes" => _allOtherStringsPatterns[3],
                "Enemies" => _allOtherStringsPatterns[4],
                "Items" => _allOtherStringsPatterns[5],
                "MapInfos" => _allOtherStringsPatterns[6],
                "Skills" => _allOtherStringsPatterns[7],
                "States" => _allOtherStringsPatterns[8],
                "System" => _allOtherStringsPatterns[9],
                "Troops" => _allOtherStringsPatterns[10],
                _ => null
            };
            if (pattern != null)
            {
                foreach (Match match in Regex.Matches(txtString, pattern, _options).Cast<Match>())
                {
                    foreach (Group group in match.Groups.Cast<Group>().Skip(1))
                    {
                        CaptureCollection captures = group.Captures;
                        foreach (Capture capture in captures.Cast<Capture>())
                        {
                            extractedStrings.Add(capture.Value);
                            //Console.WriteLine($"{absoluteFilePath}: {extractedString}"); //打印当前提取的字符串
                        }
                    }
                }
                if (pattern == _allOtherStringsPatterns[0] || pattern == _allOtherStringsPatterns[1] || pattern == _allOtherStringsPatterns[2])
                {
                    //对355开头的代码行进行特殊处理
                    StringBuilder stringBuilder = new();
                    foreach (Match match in Regex.Matches(txtString, @"(?<=\d55\("").*(?="")", _options).Cast<Match>())
                    {
                        stringBuilder.Append(match.Value);
                    }
                    foreach (Match match in Regex.Matches(stringBuilder.ToString(), @"^.*?(?=\\"")(?:\\""(\\\\\\""|.)+?\\"".*?(?=\\""|$))+", _options).Cast<Match>())
                    {
                        foreach (Group group in match.Groups.Cast<Group>().Skip(1))
                        {
                            CaptureCollection captures = group.Captures;
                            foreach (Capture capture in captures.Cast<Capture>())
                            {
                                extractedStrings.Add(capture.Value);
                                //Console.WriteLine($"{absoluteFilePath}: {extractedString}"); //打印当前提取的字符串
                            }
                        }
                    }
                }
            }
            else { }
        }
        public static void ExtractScriptsStrings(string txtString, string absoluteFilePath, List<string> extractedStrings)
        {
            string fileName = Path.GetFileName(absoluteFilePath);

            // 使用正则表达式进行匹配
            string ignorePart = "Script_\\d{8}_";
            Regex regex = new(ignorePart);
            string scriptsFileCategory = regex.Replace(fileName, string.Empty);

            string? pattern = scriptsFileCategory switch
            {
                "Vocab.txt" => _allScriptsStringsPatterns[0],
                "画面%002fパーティ編成.txt" => _allScriptsStringsPatterns[1],
                "画面%002f転職編集.txt" => _allScriptsStringsPatterns[2],
                "改造%002fステータス画面.txt" => _allScriptsStringsPatterns[3],
                "図鑑%002f本体.txt" => _allScriptsStringsPatterns[4],
                "仲間化システム.txt" => _allScriptsStringsPatterns[5],
                "贈呈%002f好感度管理.txt" => _allScriptsStringsPatterns[6],
                "画面%002fワープ選択.txt" => _allScriptsStringsPatterns[7],
                "公開コマンド.txt" => _allScriptsStringsPatterns[8],
                "コンフィグ＋.txt" => _allScriptsStringsPatterns[9],
                "娯楽%002fスロット.txt" => _allScriptsStringsPatterns[10],
                "DownWords(Actor).txt" => _allScriptsStringsPatterns[11],
                "DownWords(Enemy).txt" => _allScriptsStringsPatterns[12],
                "Follower.txt" => _allScriptsStringsPatterns[13],
                "IDReserve.txt" => _allScriptsStringsPatterns[14],
                "JobChange.txt" => _allScriptsStringsPatterns[15],
                "Library(Actor).txt" => _allScriptsStringsPatterns[16],
                "Library(Enemy).txt" => _allScriptsStringsPatterns[17],
                "Library(Medal).txt" => _allScriptsStringsPatterns[18],
                "Present.txt" => _allScriptsStringsPatterns[19],
                "SkillWords(Actor).txt" => _allScriptsStringsPatterns[20],
                "SkillWords(Enemy).txt" => _allScriptsStringsPatterns[21],
                "Library(H).txt" => _allScriptsStringsPatterns[22],
                "▽ パッチスクリプト読込 ver5.txt" => _allScriptsStringsPatterns[23],
                "トリス情報表示関連 ver8.txt" => _allScriptsStringsPatterns[24],
                "トリス戦闘回想 ver10.txt" => _allScriptsStringsPatterns[25],
                "12%002fdamage_display_refine.txt" => _allScriptsStringsPatterns[26],
                _ => null
            };
            if (pattern != null)
            {
                MatchCollection matches = Regex.Matches(txtString, pattern, _options);
                foreach (Match match in matches.Cast<Match>())
                {
                    foreach (Group group in match.Groups.Cast<Group>().Skip(1))
                    {
                        CaptureCollection captures = group.Captures;
                        foreach (Capture capture in captures.Cast<Capture>())
                        {
                            extractedStrings.Add(capture.Value);
                            //Console.WriteLine($"{absoluteFilePath}: {extractedString}"); //打印当前提取的字符串
                        }
                    }
                }
            }
            else { }
        }
    }
}