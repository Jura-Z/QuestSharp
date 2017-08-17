using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace SharpQuest
{
    public class QuestRandom
    {
        static Random rand = new Random();
        
        class QuestRandomSeq
        {
            private int[] values = null;
            public int indx = 0;

            public QuestRandomSeq(ref int[] s)
            {
                indx = 0;
                values = s;
            }

            public int Get(int max)
            {
                if (values == null)
                    return rand.Next(max);
                
                var res = values[indx++];
                Assert.IsTrue(res < max);
                return res;
            }
        }
        
        //static Dictionary<string, QuestRandomSeq> dict = new Dictionary<string, QuestRandomSeq>();

        private static QuestRandomSeq seq;
        
        public static int Index()
        {
            return seq.indx;
        }
        public static bool useArrayBased = false;

        public static void SetSeq(ref int[] s)
        {
            seq = new QuestRandomSeq(ref s);
        }
        
        
        public static int Get(string name, int max)
        {
            if (useArrayBased)
                return seq.Get(max);
                //return dict[name].Get(max);

            if (name == "A") return max / 2;
            if (name == "B") return max / 2;
            if (name == "C") return max / 2;
            if (name == "D") return max / 2;
            
            return rand.Next(max);
        }
    }
}