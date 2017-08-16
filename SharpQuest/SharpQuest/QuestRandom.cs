using System;
using System.Collections.Generic;

namespace SharpQuest
{
    public class QuestRandom
    {
        static Random rand = new Random();
        
        class QuestRandomSeq
        {
            private int[] values = null;
            private int indx = 0;

            public int Get(int max)
            {
                if (values == null)
                    return rand.Next(max);
                return values[indx++];
            }
        }
        
        static Dictionary<string, QuestRandomSeq> dict = new Dictionary<string, QuestRandomSeq>();

        public static bool useArrayBased = false;
        
        public static int Get(string name, int max)
        {
            if (useArrayBased)
                return dict[name].Get(max);

            if (name == "TCPDiapazone.GetRandom1") return max / 2;
            if (name == "TCPDiapazone.GetRandom2") return max / 2;
            if (name == "GetPathByProbability1") return max / 2;
            if (name == "GetPathByProbability2") return max / 2;
            
            return rand.Next(max);
        }
    }
}