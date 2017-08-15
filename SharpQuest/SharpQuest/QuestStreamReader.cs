using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SharpQuest
{
    public class QuestStreamReader
    {
        public static int ReadInt(Stream fs)
        {
            var bint = new byte[4];
            fs.Read(bint, 0, 4);
            return BitConverter.ToInt32(bint, 0);
        }

        public static bool ReadBool(Stream fs)
        {
            var bint = new byte[1];
            fs.Read(bint, 0, 1);
            return BitConverter.ToBoolean(bint, 0);
        }
        
        public static int ReadSetInt(Stream fs) // ONE BYTE
        {
            return fs.ReadByte();
        }

        static readonly StringBuilder sb = new StringBuilder();
        public static string ReadTextField(Stream fs)
        {
//            Прочитать из файла 4 байта, записать в m (это количество строк)
//            Нижеследующий текст повторить m раз
//                Прочитать из файла 4 байта, записать в t (это количество символов в следующей строке)
//            Установить у временного буфера tempstr длину t
//                Нижеследующий текст повторить t раз
//            Прочитать из файла 2 байта, интерпретируя их как символ в двухбайтового UNICODE, записать в буфер tempstr.
//                Добавить в выходной буфер text полученную в tempstr строку и символ перевода каретки.

            sb.Length = 0;
            var m = ReadInt(fs);
            for (var i = 0; i < m; ++i)
            {
                if (i != 0)
                    sb.AppendLine();
                        
                var t = ReadInt(fs);
                var buffer = new byte[2 * t];
                fs.Read(buffer, 0, 2 * t);

                var bufferString = Encoding.Unicode.GetString(buffer);    // 16 bit
                sb.Append(bufferString);
            }
            var s = sb.ToString();
            s = s.Replace("\r", "");
            return s;
        }

        public static double ReadDouble(FileStream fs)
        {
            var bint = new byte[8];
            fs.Read(bint, 0, 8);
            return BitConverter.ToDouble(bint, 0);
        }
    }
}