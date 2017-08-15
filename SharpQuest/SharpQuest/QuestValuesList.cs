using System.IO;

namespace SharpQuest
{
    public struct QuestValuesList
    {
        public bool Negation;
        public int[] Values;
        public int Count;
        
        public void Clear()
        {
            Values = new int[1];
            Count = 0;
            Negation = true;
        }

        public void Load(Stream fs)
        {
            Count = QuestStreamReader.ReadInt(fs);
            Negation = QuestStreamReader.ReadBool(fs);
            
            Values = new int[Count + 2];

            for (var i = 0; i < Count; ++i)
                Values[i] = QuestStreamReader.ReadInt(fs);
        }
    }
}