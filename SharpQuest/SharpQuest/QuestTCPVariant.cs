using System;using System.Globalization;
using System.Linq;

namespace SharpQuest{    public enum QuestTCPVType
    {
        vtRange, vtExt
    };    public class QuestTCPVariant    {        public QuestTCPRange vd;
        public double vf;
        public QuestTCPVType vtype;        public QuestTCPVariant()
        {
            vd = new QuestTCPRange();
            Clear();
        }        public void Clear()        {            vf = 0;            vd.Clear();            vtype = QuestTCPVType.vtExt;        }        public void CopyDataFrom(QuestTCPVariant source)
        {
            vd.CopyDataFrom(source.vd);
            vf = source.vf;
            vtype = source.vtype;
        }        private bool strIsValue(string str, int l)
        {
            for (int i = 0; i < l; i++)
            {
                char c = str[i];
                if (((c >= '0') && (c <= '9')) || (c == ',') || (c == 'E'))
                    continue;
                return false;
            }
            return true;
        }        public bool Assign(string str)
        {
            int l = str.Length;
            if (l == 0) str = "0";
            if (strIsValue(str, l))
            {
                vtype = QuestTCPVType.vtExt;
                vd.Clear();
                str = str.Replace(",", ".");
                if (!double.TryParse(str, out vf))
                    vf = 0;
                return true;
            }
            if ((l > 1) && (str[0] == '[') && (str[l - 1] == ']'))
            {
                char[] chs = { 'h', ';', '[', ']', '-' };
                for (int i = 0; i < l; i++)
                {
                    char c = str[i];
                    
                    if (((c >= '0') && (c <= '9')) || chs.Contains(c))
                        continue;
                    return false;
                }
                vtype = QuestTCPVType.vtRange;
                vd.Assign(str);
                vf = 0;
                return true;
            }
            return false;
        }
        public double GetValue()
        {
            if (vtype == QuestTCPVType.vtRange)
                return vd.GetRandom();
            else if (vtype == QuestTCPVType.vtExt)
                return vf;
            else
                return 0;
        }
    }}