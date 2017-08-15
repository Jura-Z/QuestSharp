using System.IO;

namespace SharpQuest
{
    public class QuestParameterDelta
    {
        public const int StayAsIs = 0;
        public const int HideParameter = 2;
        public const int ShowParameter = 1;

        //        maxparameters=24;

        public QuestValuesList ValuesGate;
        public QuestValuesList ModZeroesGate;

        public int bitmask;
        public int min;
        public int max;
        public int delta;
        public bool DeltaPercentFlag;
        public bool DeltaApprFlag;
        public bool DeltaExprFlag;
        public string Expression;
        public string CriticalMessage;
        public int ParameterViewAction;
        public bool CriticalMessageVisible;


        public void Load(Stream fs)
        {
            Clear();

            min = QuestStreamReader.ReadInt(fs);
            max = QuestStreamReader.ReadInt(fs);
            delta = QuestStreamReader.ReadInt(fs);
            ParameterViewAction = QuestStreamReader.ReadInt(fs);
            CriticalMessageVisible = QuestStreamReader.ReadBool(fs);
            DeltaPercentFlag = QuestStreamReader.ReadBool(fs);
            CriticalMessage = QuestStreamReader.ReadTextField(fs);
        }

        public void Load_3_9_3(Stream fs)
        {
            Clear();

            bitmask = QuestStreamReader.ReadInt(fs);

            min = QuestStreamReader.ReadInt(fs);
            max = QuestStreamReader.ReadInt(fs);
            delta = QuestStreamReader.ReadInt(fs);
            ParameterViewAction = QuestStreamReader.ReadInt(fs);
            CriticalMessageVisible = QuestStreamReader.ReadBool(fs);
            DeltaPercentFlag = QuestStreamReader.ReadBool(fs);

            ValuesGate.Load(fs);
            ModZeroesGate.Load(fs);

            CriticalMessage = QuestStreamReader.ReadTextField(fs);
        }


        public void Load_3_9_4(Stream fs)
        {
            Clear();

            bitmask = QuestStreamReader.ReadInt(fs);

            min = QuestStreamReader.ReadInt(fs);
            max = QuestStreamReader.ReadInt(fs);
            delta = QuestStreamReader.ReadInt(fs);
            ParameterViewAction = QuestStreamReader.ReadInt(fs);
            CriticalMessageVisible = QuestStreamReader.ReadBool(fs);
            DeltaPercentFlag = QuestStreamReader.ReadBool(fs);

            DeltaApprFlag = QuestStreamReader.ReadBool(fs);

            ValuesGate.Load(fs);
            ModZeroesGate.Load(fs);

            CriticalMessage = QuestStreamReader.ReadTextField(fs);
        }

        public void Load_3_9_6(Stream fs)
        {
            Clear();

            bitmask = QuestStreamReader.ReadInt(fs);

            min = QuestStreamReader.ReadInt(fs);
            max = QuestStreamReader.ReadInt(fs);
            delta = QuestStreamReader.ReadInt(fs);
            ParameterViewAction = QuestStreamReader.ReadInt(fs);
            
            CriticalMessageVisible = QuestStreamReader.ReadBool(fs);
            DeltaPercentFlag = QuestStreamReader.ReadBool(fs);

            DeltaApprFlag = QuestStreamReader.ReadBool(fs);
            DeltaExprFlag = QuestStreamReader.ReadBool(fs);

            Expression = QuestStreamReader.ReadTextField(fs);

            ValuesGate.Load(fs);
            ModZeroesGate.Load(fs);

            CriticalMessage = QuestStreamReader.ReadTextField(fs);
        }


        public void Clear()
        {
            bitmask = 0;
            min = 0;
            max = 1;
            delta = 0;
            ParameterViewAction = StayAsIs;
            CriticalMessage = ""; //;'Сообщение достижения критического значения параметром ';
            CriticalMessageVisible = false;
            DeltaPercentFlag = false;
            DeltaApprFlag = false;
            DeltaExprFlag = false;
            
            ValuesGate = new QuestValuesList();
            ModZeroesGate = new QuestValuesList();
            
            ValuesGate.Clear();
            ModZeroesGate.Clear();
            
            Expression = "";
        }
    }
}