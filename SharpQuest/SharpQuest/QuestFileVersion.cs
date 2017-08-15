namespace SharpQuest
{
    public class QuestFileVersion
    {
        public const int FileVersion_Older_than_3_8_5=1111111111;
        public const int FileVersion_3_8_5=1111111112;
        public const int FileVersion_3_9_0=1111111113;
        public const int FileVersion_3_9_1=1111111114;
        public const int FileVersion_3_9_2=1111111115;
        public const int FileVersion_3_9_3=1111111116;
        public const int FileVersion_3_9_4=1111111117;
        public const int FileVersion_3_9_5=1111111118;
        public const int FileVersion_3_9_6=1111111119;   //в названии редактора - 40alpha
        public const int FileVersion_4_0_0=1111111120;
        public const int FileVersion_4_0_1=1111111121;
        public const int FileVersion_4_0_2=1111111122;

        private int _value = FileVersion_Older_than_3_8_5;
        
        public QuestFileVersion(int intVersion)
        {
            _value = intVersion;
        }

        public static implicit operator QuestFileVersion(int intVersion)
        {
            return new QuestFileVersion(intVersion);
        }
        
        public static implicit operator int(QuestFileVersion version)
        {
            return version._value;
        }
        
        public override string ToString()
        {
            switch (_value)
            {
                case FileVersion_Older_than_3_8_5:
                    return "< 3.8.5";
                case FileVersion_3_8_5:
                    return "3.8.5";
                case FileVersion_3_9_0:
                    return "3.9.0";
                case FileVersion_3_9_1:
                    return "3.9.1";
                case FileVersion_3_9_2:
                    return "3.9.2";
                case FileVersion_3_9_3:
                    return "3.9.3";
                case FileVersion_3_9_4:
                    return "3.9.4";
                case FileVersion_3_9_5:
                    return "3.9.5";
                case FileVersion_3_9_6:
                    return "3.9.6";
                case FileVersion_4_0_0:
                    return "4.0.0";
                case FileVersion_4_0_1:
                    return "4.0.1";
                case FileVersion_4_0_2:
                    return "4.0.2";
            }
            return "Unknown";
        }

        public bool isOlderThan3_8_5()
        {
            return _value <= FileVersion_Older_than_3_8_5;
        }

        public bool isSince3_8_5()
        {
            return _value >= FileVersion_3_8_5;
        }

        public bool isSince3_9_0()
        {
            return _value >= FileVersion_3_9_0;
        }

        public bool isSince3_9_1()
        {
            return _value >= FileVersion_3_9_1;
        }

        public bool isSince3_9_2()
        {
            return _value >= FileVersion_3_9_2;
        }
        
        public bool isSince3_9_3()
        {
            return _value >= FileVersion_3_9_3;
        }
        
        public bool isSince3_9_4()
        {
            return _value >= FileVersion_3_9_4;
        }
        
        public bool isSince3_9_5()
        {
            return _value >= FileVersion_3_9_5;
        }
        
        public bool isSince3_9_6()
        {
            return _value >= FileVersion_3_9_6;
        }

        public bool isSince4_0_0()
        {
            return _value >= FileVersion_4_0_0;
        }
        
        public bool isSince4_0_1()
        {
            return _value >= FileVersion_4_0_1;
        }

        public bool isSince4_0_2()
        {
            return _value >= FileVersion_4_0_2;
        }
        
    }
}