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
            private List<int> values = new List<int>();
            public int indx = 0;

            public QuestRandomSeq()
            {
            }

            public void Add(int v)
            {
                values.Add(v);
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
        
        static Dictionary<string, QuestRandomSeq> dict = new Dictionary<string, QuestRandomSeq>();

        private static QuestRandomSeq seq;
        private static int randomCallCount = 0;
        
        public static bool useArrayBased = true;
        
        
        public static int Get(string name, int max)
        {
            ++randomCallCount;
            
            if (useArrayBased)
                //return seq.Get(max);
                return dict[name].Get(max);

            if (name == "A") return max / 2;
            if (name == "B") return max / 2;
            if (name == "C") return max / 2;
            if (name == "D") return max / 2;
            
            return rand.Next(max);
        }

        public static void AddSeq(string name, int value)
        {
            if (dict.ContainsKey(name) == false)
                dict[name] = new QuestRandomSeq();
            dict[name].Add(value);
        }

        public static void FinishSeq()
        {
            
        }

        public static int RamdomCallCount()
        {
            return randomCallCount;
        }
    }
}