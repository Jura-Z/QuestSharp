using System.IO;

namespace SharpQuest
{
//    min:integer;
//    max:integer;
//    value:integer;
//    Name:TTextField;
//    GameName:TTextField;
//    CriticalMessage:TTextField;
//    ParType:integer;
//    Hidden:boolean;  //not active now
//    ShowIfZero:boolean;
//    LoLimit:boolean;
//    Enabled:boolean;
//    Money:boolean;
//
//    ViewFormatStrings: array [1..10] of TParWiewString;
//    ValueOfViewStrings:integer;
//
//    AltStartValues:TValuesList;
//	AltDiapStartValues:TCPDiapazone;
//
//    constructor Create(parameternumber:integer);
//    function GetDefaultMinGate:integer;
//    function GetDefaultMaxGate:integer;
//
//    function GetVFStringByValue(value:integer):WideString;
//
//    procedure Clear(parameternumber:integer);
//
//    procedure CopyDataFrom(var source:TParameter);
//
    //TParameter
    public struct QuestParameter
    {
        public struct ParameterViewString
        {
            public int min;
            public int max;
            public string str;

            public void Load(Stream fs)
            {
                min = QuestStreamReader.ReadInt(fs);
                max = QuestStreamReader.ReadInt(fs);
                str = QuestStreamReader.ReadTextField(fs);
            }

            public override string ToString()
            {
                return string.Format("[ParameterViewString] {0} [{1}/{2}]", str, min, max);
            }
        }


        public int min;
        public int max;
        public int value;

        public string Name;
        public string GameName;
        public string CriticalMessage;

        public int ParType;
        public bool Hidden;
        public bool ShowIfZero;
        public bool LoLimit;
        public bool Enabled;
        public bool Money;

        public ParameterViewString[] ViewFormatStrings;
        public int ValueOfViewStrings;


        public QuestValuesList AltStartValues;
        public QuestTCPRange AltDiapStartValues;

        public override string ToString()
        {
            if (IsEmpty())
                return "[EmptyQuestParameter]";

            if (string.IsNullOrEmpty(GameName))
                return string.Format("[QuestParameter] Name = {1} | {2} in [{3}..{4}]", GameName, Name, value, min,
                    max);
            return string.Format("[QuestParameter] GameName = {0} | Name = {1} | {2} in [{3}..{4}]", GameName, Name,
                value, min, max);
        }

        private bool IsEmpty()
        {
            return string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(GameName) && value == 0 && min == 0 && max == 0;
        }

//    AltStartValues:TValuesList;
//	AltDiapStartValues:TCPDiapazone;

        public const int FailParType = 1;
        public const int SuccessParType = 2;
        public const int DeathParType = 3;
        public const int NoCriticalParType = 0;

        public string GetVFStringByValue(int value)
        {
            var answer = "Can not GetVFString - Out of range";

            for (var i = 0; i < ValueOfViewStrings; ++i)
            {
                if ((value >= ViewFormatStrings[i].min) && (value <= ViewFormatStrings[i].max))
                    answer = ViewFormatStrings[i].str.Trim();
            }
            return answer;
        }

        public int GetDefaultMinGate()
        {
            var answer = min;

            if ((ParType != NoCriticalParType) && (ParType != SuccessParType) && LoLimit) 
                ++answer;

            return answer;
        }

        public int GetDefaultMaxGate()
        {
            var answer = max;

            if ((ParType != NoCriticalParType) && (ParType != SuccessParType) && LoLimit == false)
                --answer;

            return answer;
        }


        public void Load_4_0_1(FileStream fs)
        {
            Money = false;

            min = QuestStreamReader.ReadInt(fs);
            max = QuestStreamReader.ReadInt(fs);
            value = QuestStreamReader.ReadInt(fs);
            ParType = QuestStreamReader.ReadInt(fs);

            Hidden = QuestStreamReader.ReadBool(fs);
            ShowIfZero = QuestStreamReader.ReadBool(fs);
            LoLimit = QuestStreamReader.ReadBool(fs);
            Enabled = QuestStreamReader.ReadBool(fs);

            ValueOfViewStrings = QuestStreamReader.ReadInt(fs);
            Money = QuestStreamReader.ReadBool(fs);

            Name = QuestStreamReader.ReadTextField(fs);

            ViewFormatStrings = new ParameterViewString[10];
            for (var i = 0; i < ValueOfViewStrings; ++i)
                ViewFormatStrings[i].Load(fs);

            CriticalMessage = QuestStreamReader.ReadTextField(fs);

            var AltDiapStartValuesStr = QuestStreamReader.ReadTextField(fs);
            AltDiapStartValues.Assign(AltDiapStartValuesStr);
        }

        public void Load_3_9_6(FileStream fs)
        {
            Money = false;

            min = QuestStreamReader.ReadInt(fs);
            max = QuestStreamReader.ReadInt(fs);
            value = QuestStreamReader.ReadInt(fs);
            ParType = QuestStreamReader.ReadInt(fs);

            Hidden = QuestStreamReader.ReadBool(fs);
            ShowIfZero = QuestStreamReader.ReadBool(fs);
            LoLimit = QuestStreamReader.ReadBool(fs);
            Enabled = QuestStreamReader.ReadBool(fs);

            ValueOfViewStrings = QuestStreamReader.ReadInt(fs);
            Money = QuestStreamReader.ReadBool(fs);

            Name = QuestStreamReader.ReadTextField(fs);

            ViewFormatStrings = new ParameterViewString[10];
            for (var i = 0; i < ValueOfViewStrings; ++i)
                ViewFormatStrings[i].Load(fs);

            CriticalMessage = QuestStreamReader.ReadTextField(fs);
            
            AltStartValues.Load(fs);
            AltDiapStartValues.CopyDataFrom(AltStartValues);
            AltStartValues.Clear();
        }

        public void Load_3_9_5(FileStream fs)
        {
            Money = false;

            min = QuestStreamReader.ReadInt(fs);
            max = QuestStreamReader.ReadInt(fs);
            value = QuestStreamReader.ReadInt(fs);
            ParType = QuestStreamReader.ReadInt(fs);

            Hidden = QuestStreamReader.ReadBool(fs);
            ShowIfZero = QuestStreamReader.ReadBool(fs);
            LoLimit = QuestStreamReader.ReadBool(fs);
            Enabled = QuestStreamReader.ReadBool(fs);

            ValueOfViewStrings = QuestStreamReader.ReadInt(fs);
            Money = QuestStreamReader.ReadBool(fs);

            Name = QuestStreamReader.ReadTextField(fs);

            ViewFormatStrings = new ParameterViewString[10];
            for (var i = 0; i < ValueOfViewStrings; ++i)
                ViewFormatStrings[i].Load(fs);

            CriticalMessage = QuestStreamReader.ReadTextField(fs);
            AltDiapStartValues.Clear();
            AltDiapStartValues.Add(value,value);
        }

        public void Load_3_9_0(FileStream fs)
        {
            Money = false;

            min = QuestStreamReader.ReadInt(fs);
            max = QuestStreamReader.ReadInt(fs);
            value = QuestStreamReader.ReadInt(fs);
            ParType = QuestStreamReader.ReadInt(fs);

            Hidden = QuestStreamReader.ReadBool(fs);
            ShowIfZero = QuestStreamReader.ReadBool(fs);
            LoLimit = QuestStreamReader.ReadBool(fs);
            Enabled = QuestStreamReader.ReadBool(fs);

            ValueOfViewStrings = QuestStreamReader.ReadInt(fs);

            Name = QuestStreamReader.ReadTextField(fs);

            ViewFormatStrings = new ParameterViewString[10];
            for (var i = 0; i < ValueOfViewStrings; ++i)
                ViewFormatStrings[i].Load(fs);

            CriticalMessage = QuestStreamReader.ReadTextField(fs);
            AltDiapStartValues.Clear();
            AltDiapStartValues.Add(value,value);
        }

        public void Load(FileStream fs)
        {
            Money = false;

            min = QuestStreamReader.ReadInt(fs);
            max = QuestStreamReader.ReadInt(fs);
            value = QuestStreamReader.ReadInt(fs);
            ParType = QuestStreamReader.ReadInt(fs);

            Hidden = QuestStreamReader.ReadBool(fs);
            ShowIfZero = QuestStreamReader.ReadBool(fs);
            LoLimit = QuestStreamReader.ReadBool(fs);
            Enabled = QuestStreamReader.ReadBool(fs);

            Name = QuestStreamReader.ReadTextField(fs);
            GameName = QuestStreamReader.ReadTextField(fs);
            CriticalMessage = QuestStreamReader.ReadTextField(fs);

            ViewFormatStrings = new ParameterViewString[10];
            ValueOfViewStrings = 1;

            ViewFormatStrings[0].max = max;
            ViewFormatStrings[0].min = min;
            ViewFormatStrings[0].str = GameName.Trim();

            AltDiapStartValues.Clear();
            AltDiapStartValues.Add(value,value);
        }
    }
}