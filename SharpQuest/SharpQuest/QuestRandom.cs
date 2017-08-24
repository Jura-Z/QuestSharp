using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace SharpQuest
{
    public class QuestRandom
    {
        public struct Element
        {
            public string name;
            public int val;
            public int max;

            public override string ToString() { return string.Format("<{0}|{1} {2}>", name, val, max); }
        }
        
        class QuestRandomSeq
        {
            public List<Element> values = new List<Element>();
            public int indx = 0;
            Random rand = new Random();

            public QuestRandomSeq()
            {
            }

            public void Add(string n, int v, int m)
            {
                values.Add(new Element() {name =  n, val = v, max = m});
            }

            public int Get(string name, int max)
            {
                if (values == null || values.Count == 0)
                    return rand.Next(max);
                
                var res = values[indx++];
                
                if (res.name != name) throw new Exception("name is different");
                if (res.max != max) throw new Exception(string.Format("max is different. '{3}' expected {0} but received {1}. randomCallCount = {2}", max, res.max, randomCallCount, name));
                if (res.max != 0 && res.val >= res.max) throw new Exception(string.Format("val {0} >= max {1}", res.val, res.max));
                
                return res.val;
            }
        }
        
        private static QuestRandomSeq seq = new QuestRandomSeq();
        private static int randomCallCount = 0;
        
        
        
        public static int Get(string name, int max)
        {
            ++randomCallCount;
            
            return seq.Get(name, max);
            //return max / 2;
        }

        public static void AddSeq(string name, int value, int max)
        {
            seq.Add(name, value, max);
        }

        public static void FinishSeq()
        {
            
        }

        public static int RamdomCallCount()
        {
            return randomCallCount;
        }

        public static void DebugPrint(int indx)
        {
            Console.Write(seq.values[indx].ToString() + ", ");
        }
    }
}