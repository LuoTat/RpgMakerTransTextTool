using System.Text;

namespace RpgMakerTransTextTool.StringOperations;

public static class StringEscaper
{
    // 转义字符串
    public static string EscapeString(string str)
    {
        StringBuilder sb   = new();
        foreach (char c in str)
        {
            switch (c)
            {
                case '\r':
                    sb.Append(@"\r");
                    break;
                case '\n':
                    sb.Append(@"\n");
                    break;
                case '\\':
                    sb.Append(@"\\");
                    break;
                case '\"':
                    sb.Append(@"\""");
                    break;
                default:
                    sb.Append(c);
                    break;
            }
        }
        return sb.ToString();
    }

    // 反转义字符串
    public static string UnescapeString(string str)
    {
        StringBuilder sb = new();
        bool escapeMode = false;
        foreach (char c in str)
        {
            if (escapeMode)
            {
                switch (c)
                {
                    case 'r':
                        sb.Append('\r');
                        break;
                    case 'n':
                        sb.Append('\n');
                        break;
                    case '\\':
                        sb.Append('\\');
                        break;
                    case '\"':
                        sb.Append('\"');
                        break;
                    default:
                        throw new ArgumentException($"Invalid escape character: \\{c}");
                }
                escapeMode = false;
            }
            else
            {
                switch (c)
                {
                    case '\\':
                        escapeMode = true;
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
        }
        return sb.ToString();
    }
}