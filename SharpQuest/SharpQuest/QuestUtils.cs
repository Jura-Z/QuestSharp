using System.Collections.Generic;
using System.Text;

namespace SharpQuest
{
    public class QuestUtils
    {
        static readonly StringBuilder sb = new StringBuilder();
        public static string ArrayToString<T>(T[] arr)
        {
            sb.Length = 0;
            sb.AppendFormat("[{0}]", string.Join(", ", arr));
            return sb.ToString();
        }
        
        public static string ArrayToString<T>(List<T> arr)
        {
            sb.Length = 0;
            sb.AppendFormat("[{0}]", string.Join(", ", arr));
            return sb.ToString();
        }
    }
}