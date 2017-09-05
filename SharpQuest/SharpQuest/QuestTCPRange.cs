using System;namespace SharpQuest{    public struct QuestTCPRange    {        public long[] low;        public long[] hi;        public int count;        public void Clear()        {            count = 0;            low = hi = new long[0];        }        public void Assign(string str)        {            Clear();            if (str == ";") return;            var l = str.Length;            var i = 0;            var tstr = "";            var slow = 200000000;            var shi = -200000000;            while (i < l)            {                if (((str[i] >= '0') && (str[i] <= '9')) || (str[i] == '-'))                {                    tstr = tstr + str[i];                    ++i;                }                else                {                    if ((str[i] == 'h') || (str[i] == ';') || (str[i] == ']'))                    {                        var c = 0;                        var ecjump = !int.TryParse(tstr, out c);                        if (!ecjump)                        {                            if (slow > c) slow = c;                            if (shi < c) shi = c;                        }                        ecjump = false;                        tstr = "";                        if (((str[i] == ';') || (str[i] == ']')))                        {                            Add(slow, shi);                            slow = 200000000;                            shi = -200000000;                            tstr = "";                        }                        ++i;                    }                    else                    {                        ++i;                    }                }            }        }        public double GetRandom()        {            if (count <= 0) return 0;            var mh = new long[count];            var ml = new long[count];            long c = 0;            for (var i = 0; i < count; ++i)            {                ml[i] = c;                mh[i] = c + (hi[i] - low[i]);                c += (hi[i] - low[i]) + 1;            }            c = QuestRandom.Get("A", (int)c);            long f = 0;            for (var i = 0; i < count; ++i)            {                if ((c < ml[i]) || (c > mh[i])) continue;                f = QuestRandom.Get("B", (int)(hi[i]-low[i]+1)) + low[i];            }            return f;        }        public void Add(long l, long h)        {            var min = Math.Min(l, h);            var max = Math.Max(l, h);            ++count;            Array.Resize(ref low, count);            Array.Resize(ref hi, count);            low[count - 1] = min;            hi[count - 1] = max;        }        public void Add(double d)
        {
            long tmp = (long)Math.Truncate(d);
            Add(tmp, tmp);
        }        public void Add(QuestTCPRange other)
        {
            if (other.count == 0)
                return;
            Array.Resize(ref low, count + other.count);            Array.Resize(ref hi, count + other.count);            for (int i = 0; i < other.count; i++)
            {
                low[count + i] = other.low[i];
                hi[count + i] = other.hi[i];
            }            count = count + other.count;
        }        public void CopyDataFrom(QuestValuesList altStartValues)        {            count = altStartValues.Count;            low = new long[count];            hi = new long[count];            for (var i = 0; i < count; ++i)            {                hi[i] = low[i] = altStartValues.Values[i];            }        }        public void CopyDataFrom(QuestTCPRange range)        {            count = range.count;            low = new long[count];            hi = new long[count];            for (var i = 0; i < count; ++i)            {                hi[i] = range.hi[i];                low[i] = range.low[i];
            }        }        public long GetMinimun()
        {
            long result = low[0];
            for (int i = 1; i < count; i++)
            {
                if (result > low[i])
                    result = low[i];
            }
            return result;
        }
        public long GetMaximun()
        {
            long result = hi[0];
            for (int i = 1; i < count; i++)
            {
                if (result < hi[i])
                    result = hi[i];
            }
            return result;
        }        public bool Have(double value)
        {
            long tmp = (long)Math.Round(value);
            for (int i = 0; i < count; i++)
            {
                if ((low[i] <= tmp) && (hi[i] >= tmp))
                    return true;
            }
            return false;
        }
        public string GetString()
        {
            string result = "[";
            for (int i = 0; i < count; i++)
            {
                if (low[i] == hi[i])
                    result += low[i].ToString();
                else
                    result += string.Format("{0}h{1}", low[i], hi[i]);

                if (i < count - 1)
                    result += ';';
                else
                    result += ']';
            }
            return result;
        }
    }}