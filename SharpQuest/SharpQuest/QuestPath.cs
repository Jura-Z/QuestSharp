using System.IO;

namespace SharpQuest
{
    public class QuestPath
    {
        private const int MaxPathCoords = 20;
        
        public double Probability;

        public bool VoidPathFlag; // Флаг пустого пути

        public bool AlwaysShowWhenPlaying; // Показывать путь при игре, даже если он не подходит по диапазонам

        // NoMeanFlag; //Флаг, говорящий о том, что путь является незначимым. !!!!!

        public int dayscost;

        public int ShowOrder;//Порядок показа

        public int PathNumber; // Номер перехода

        public int PathIndx;    // indx in array

        public int PassTimesValue; // Сколько раз при игре можно пройти данный переход;

        public int FromLocationNumber; // Номер локации из которой совершается переход
        public int ToLocationNumber; // Номер локации в которую совершается переход

        public int FromLocation; // Номер локации из которой совершается переход
        public int ToLocation; // Номер локации в которую совершается переход

        public string StartPathMessage; // Сообщение вопроса перехода
        public string EndPathMessage; // Cooбщение в конце перехода

        public string LogicExpression; //Логическое условие возможности совершения перехода

        public QuestParameterDelta[] DPars;//: array [1..maxparameters] of TParameterDelta;

        //Изображение пути в редакторе

        public override string ToString()
        {
            return string.Format("<Path #{0} {1} -> {2}. {3} | {4}>", PathNumber, FromLocation, ToLocation, StartPathMessage, EndPathMessage);
        }

        public void Clear()
        {
            dayscost = 0;
            ShowOrder = 5;
            Probability = 1;
            PassTimesValue = 1;
            PathNumber = 0;
            FromLocation = 0;
            ToLocation = 0;
            StartPathMessage = "M.Par_Get('PathClassStartPathMessage')";

            EndPathMessage = "";
            LogicExpression = "";
            
            DPars = new QuestParameterDelta[Quest.maxparameters];
            
            for (var i = 0; i < Quest.maxparameters; ++i)
            {
                DPars[i] = new QuestParameterDelta();
                DPars[i].Clear();
            }

            VoidPathFlag = true;

            AlwaysShowWhenPlaying = false;
        }
        
        public void Load_4_0_2(FileStream fs, int xRes, int yRes)
        {
            Clear();
            
            
            Probability = QuestStreamReader.ReadDouble(fs);
            dayscost = QuestStreamReader.ReadInt(fs);
            
            PathNumber = QuestStreamReader.ReadInt(fs);
            FromLocation = QuestStreamReader.ReadInt(fs);
            ToLocation = QuestStreamReader.ReadInt(fs);
            VoidPathFlag = QuestStreamReader.ReadBool(fs);
            
            AlwaysShowWhenPlaying = QuestStreamReader.ReadBool(fs);
            PassTimesValue = QuestStreamReader.ReadInt(fs);
            ShowOrder = QuestStreamReader.ReadInt(fs);

            for (var i = 0; i < Quest.maxparameters; ++i) 
                DPars[i].Load_3_9_6(fs);
            
            LogicExpression = QuestStreamReader.ReadTextField(fs);
            
            StartPathMessage = QuestStreamReader.ReadTextField(fs);
            EndPathMessage = QuestStreamReader.ReadTextField(fs);
        }

        public void Load_3_9_6(FileStream fs, int xRes, int yRes)
        {
            Clear();
            
            Probability = QuestStreamReader.ReadDouble(fs);
            dayscost = QuestStreamReader.ReadInt(fs);
            
            PathNumber = QuestStreamReader.ReadInt(fs);
            FromLocation = QuestStreamReader.ReadInt(fs);
            ToLocation = QuestStreamReader.ReadInt(fs);
            VoidPathFlag = QuestStreamReader.ReadBool(fs);
            
            AlwaysShowWhenPlaying = QuestStreamReader.ReadBool(fs);
            PassTimesValue = QuestStreamReader.ReadInt(fs);

            for (var i = 0; i < Quest.maxparameters; ++i) 
                DPars[i].Load_3_9_6(fs);
            
            LogicExpression = QuestStreamReader.ReadTextField(fs);
            
            StartPathMessage = QuestStreamReader.ReadTextField(fs);
            EndPathMessage = QuestStreamReader.ReadTextField(fs);
        }

        public void Load_3_9_4(FileStream fs, int xRes, int yRes)
        {
            Clear();
            
            Probability = QuestStreamReader.ReadDouble(fs);
            dayscost = QuestStreamReader.ReadInt(fs);
            
            PathNumber = QuestStreamReader.ReadInt(fs);
            FromLocation = QuestStreamReader.ReadInt(fs);
            ToLocation = QuestStreamReader.ReadInt(fs);
            VoidPathFlag = QuestStreamReader.ReadBool(fs);
            
            AlwaysShowWhenPlaying = QuestStreamReader.ReadBool(fs);
            PassTimesValue = QuestStreamReader.ReadInt(fs);

            for (var i = 0; i < 12; ++i) 
                DPars[i].Load_3_9_4(fs);

            StartPathMessage = QuestStreamReader.ReadTextField(fs);
            EndPathMessage = QuestStreamReader.ReadTextField(fs);
        }

        public void Load_3_9_3(FileStream fs)
        {
            Clear();
            
            Probability = QuestStreamReader.ReadDouble(fs);
            dayscost = QuestStreamReader.ReadInt(fs);
            
            PathNumber = QuestStreamReader.ReadInt(fs);
            FromLocation = QuestStreamReader.ReadInt(fs);
            ToLocation = QuestStreamReader.ReadInt(fs);
            VoidPathFlag = QuestStreamReader.ReadBool(fs);
            
            AlwaysShowWhenPlaying = QuestStreamReader.ReadBool(fs);
            PassTimesValue = QuestStreamReader.ReadInt(fs);

            for (var i = 0; i < 12; ++i) 
                DPars[i].Load_3_9_3(fs);
            
            StartPathMessage = QuestStreamReader.ReadTextField(fs);
            EndPathMessage = QuestStreamReader.ReadTextField(fs);
        }

        public void Load_3_9_2(FileStream fs)
        {
            Clear();
            
            PathNumber = QuestStreamReader.ReadInt(fs);
            FromLocation = QuestStreamReader.ReadInt(fs);
            ToLocation = QuestStreamReader.ReadInt(fs);
            VoidPathFlag = QuestStreamReader.ReadBool(fs);
            
            AlwaysShowWhenPlaying = QuestStreamReader.ReadBool(fs);
            PassTimesValue = QuestStreamReader.ReadInt(fs);

            for (var i = 0; i < 12; ++i) 
                DPars[i].Load(fs);

            StartPathMessage = QuestStreamReader.ReadTextField(fs);
            EndPathMessage = QuestStreamReader.ReadTextField(fs);
        }

        public void Load_3_9_1(FileStream fs)
        {
            Clear();
            
            PathNumber = QuestStreamReader.ReadInt(fs);
            FromLocation = QuestStreamReader.ReadInt(fs);
            ToLocation = QuestStreamReader.ReadInt(fs);
            VoidPathFlag = QuestStreamReader.ReadBool(fs);
            
            AlwaysShowWhenPlaying = QuestStreamReader.ReadBool(fs);
            PassTimesValue = QuestStreamReader.ReadInt(fs);

            for (var i = 0; i < 9; ++i) 
                DPars[i].Load(fs);

            StartPathMessage = QuestStreamReader.ReadTextField(fs);
            EndPathMessage = QuestStreamReader.ReadTextField(fs);
        }

        public void Load_3_8_5(FileStream fs)
        {
            Clear();
            
            PathNumber = QuestStreamReader.ReadInt(fs);
            FromLocation = QuestStreamReader.ReadInt(fs);
            ToLocation = QuestStreamReader.ReadInt(fs);
            VoidPathFlag = QuestStreamReader.ReadBool(fs);

            AlwaysShowWhenPlaying = QuestStreamReader.ReadBool(fs);

            for (var i = 0; i < 9; ++i) 
                DPars[i].Load(fs);

            StartPathMessage = QuestStreamReader.ReadTextField(fs);
            EndPathMessage = QuestStreamReader.ReadTextField(fs);
        }

        public void Load(FileStream fs)
        {
            Clear();
            PathNumber = QuestStreamReader.ReadInt(fs);
            FromLocation = QuestStreamReader.ReadInt(fs);
            ToLocation = QuestStreamReader.ReadInt(fs);
            VoidPathFlag = QuestStreamReader.ReadBool(fs);
            
            for (var i = 0; i < 9; ++i) 
                DPars[i].Load(fs);

            StartPathMessage = QuestStreamReader.ReadTextField(fs);
            EndPathMessage = QuestStreamReader.ReadTextField(fs);
        }
    }
}