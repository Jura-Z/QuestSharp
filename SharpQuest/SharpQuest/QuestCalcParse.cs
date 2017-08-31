using System;

namespace SharpQuest
{
    public class QuestCalcParse
    {
        bool calc_error = false;
        //private string _str;
        public bool error;
        public int answer;

        public QuestCalcParse(string str, int currParNum, int[] pars)
        {
            error = false;
            answer = 0;
            str = ConvertStirng(str);
            Parse(str, pars);
        }
        private void Parse(string str, int[] pars)
        {
            string tmp = InsertParValues(str, pars);
            tmp = "(" + tmp + ")";
            QuestTCPVariant range = Calc(tmp);
            double value = range.GetValue();
            value = (value < -2000000000) ? -2000000000 : value;
            value = (value > 2000000000) ? 2000000000 : value;

            try
            {
                answer = (int)Math.Round(value + +0.00000000001f);
            }
            catch
            {
                calc_error = true;
                error = true;
                answer = 0;
            }
            if ((answer < 0) && (value > 0))
            {
                calc_error = true;
                error = true;
                answer = 0;
            }
            if (calc_error) error = true;
        }
        private string InsertParValues(string str, int[] pars)
        {
            string result = str;
            for (int i = 0; i < pars.Length; i++)
            {
                int val = pars[i];
                string s = (val < 0) ? string.Format("(0{0})", val) : string.Format("{0}", val);
                result = StirngReplace(result, string.Format("[p{0}]", i+1), s);
            }
            return result;
        }
        private QuestTCPVariant Calc(string str)
        {
            QuestTCPVariant result = new QuestTCPVariant();
            if (!calc_error)
            {
                if (!result.Assign(str))
                {
                    int lenexpr = str.Length;
                    if ((str[0] == '(') && (str[lenexpr - 1] == ')') && CheckParentheses(str, 1, lenexpr-2))
                    {
                        string tmp = str.Substring(1, lenexpr - 2);
                        QuestTCPVariant var = Calc(tmp);
                        result.CopyDataFrom(var);
                    }
                    else
                    {
                        int c = FindOperation(str, lenexpr - 1);
                        if (c == -1)
                        {
                            calc_error = true;
                            return result;
                        }
                        string left = str.Substring(0, c-1);
                        string right = str.Substring(c + 1, lenexpr-c);

                        QuestTCPVariant rf = Calc(right);
                        if (calc_error) return result;

                        QuestTCPVariant lf = Calc(left);
                        if (calc_error) return result;

                        try
                        {
                            char ch = str[c];
                            if (c == '+') OpAdd(lf, rf, ref result);
                            if (c == '-') OpSub(lf, rf, ref result);
                            if (c == '*') OpMul(lf, rf, ref result);
                            if (c == '/') OpDiv(lf, rf, ref result);
                            if (c == 'f') OpDivTrunc(lf, rf, ref result);
                            if (c == 'g') OpMod(lf, rf, ref result);
                            if (c == '$') OpTo(lf, rf, ref result);
                            if (c == '#') OpIn(lf, rf, ref result);
                            if (c == '>') OpHi(lf, rf, ref result);
                            if (c == '<') OpLo(lf, rf, ref result);
                            if (c == 'c') OpHiEq(lf, rf, ref result);
                            if (c == 'b') OpLoEq(lf, rf, ref result);
                            if (c == 'e') OpNotEq(lf, rf, ref result);
                            if (c == '=') OpEq(lf, rf, ref result);
                            if (c == '&') OpAnd(lf, rf, ref result);
                            if (c == '|') OpOr(lf, rf, ref result);

                        }
                        catch
                        {
                        }
                    }
                }
            }
            return result;
        }
        private void OpAdd(QuestTCPVariant lf, QuestTCPVariant rf, ref QuestTCPVariant result)
        {
            result.Clear();
            result.vf = lf.GetValue() + rf.GetValue();
        }
        private void OpSub(QuestTCPVariant lf, QuestTCPVariant rf, ref QuestTCPVariant result)
        {
            result.Clear();
            result.vf = lf.GetValue() - rf.GetValue();
        }
        private void OpMul(QuestTCPVariant lf, QuestTCPVariant rf, ref QuestTCPVariant result)
        {
            result.Clear();
            result.vf = lf.GetValue() * rf.GetValue();
        }
        private void OpDiv(QuestTCPVariant lf, QuestTCPVariant rf, ref QuestTCPVariant result)
        {
            result.Clear();
            double l = lf.GetValue();
            double r = rf.GetValue();
            if (r == 0)
                result.vf = (l < 0) ? -2000000000 : 2000000000;
            else
                result.vf = l / r;
        }
        private void OpDivTrunc(QuestTCPVariant lf, QuestTCPVariant rf, ref QuestTCPVariant result)
        {
            OpDiv(lf, rf, ref result);
            result.vf = Math.Truncate(result.vf);
        }
        private void OpMod(QuestTCPVariant lf, QuestTCPVariant rf, ref QuestTCPVariant result)
        {
            result.Clear();
            double l = lf.GetValue();
            double r = rf.GetValue();
            r = Math.Truncate(r);
            if (r == 0)
                result.vf = (l < 0) ? -2000000000 : 2000000000;
            else
            {
                r = (r < 0) ? -r: r;
                bool ldz = l < 0;
                l = ldz ? -l : l;
                result.vf = Math.Truncate(l - r * Math.Truncate(l / r));
                result.vf = ldz ? -result.vf : result.vf;
            }
                
        }
        private void OpTo(QuestTCPVariant lf, QuestTCPVariant rf, ref QuestTCPVariant result)
        {
            result.Clear();
            long min = (lf.vtype == QuestTCPVType.vtExt) ? (long)Math.Round(lf.vf) : lf.vd.GetMinimun();
            long max = (rf.vtype == QuestTCPVType.vtExt) ? (long)Math.Round(rf.vf) : rf.vd.GetMaximun();
            result.vtype = QuestTCPVType.vtRange;
            result.vd.Add(min, max);
        }
        private void OpIn(QuestTCPVariant lf, QuestTCPVariant rf, ref QuestTCPVariant result)
        {
            result.Clear();
            if ((lf.vtype == QuestTCPVType.vtExt) && (rf.vtype == QuestTCPVType.vtExt))
                result.vf = lf.vf == rf.vf ? 1 : 0;
            else if ((lf.vtype == QuestTCPVType.vtRange) && (rf.vtype == QuestTCPVType.vtExt))
                result.vf = lf.vd.Have(rf.vf) ? 1 : 0;
            else if ((lf.vtype == QuestTCPVType.vtExt) && (rf.vtype == QuestTCPVType.vtRange))
                result.vf = rf.vd.Have(lf.vf) ? 1 : 0;
            else if ((lf.vtype == QuestTCPVType.vtRange) && (rf.vtype == QuestTCPVType.vtRange))
                result.vf = rf.vd.Have(lf.GetValue()) ? 1 : 0;
        }
        private void OpLo(QuestTCPVariant lf, QuestTCPVariant rf, ref QuestTCPVariant result)
        {
            result.Clear();
            result.vf = lf.GetValue() < rf.GetValue() ? 1 : 0;
        }
        private void OpHi(QuestTCPVariant lf, QuestTCPVariant rf, ref QuestTCPVariant result)
        {
            result.Clear();
            result.vf = lf.GetValue() > rf.GetValue() ? 1 : 0;
        }
        private void OpLoEq(QuestTCPVariant lf, QuestTCPVariant rf, ref QuestTCPVariant result)
        {
            result.Clear();
            result.vf = lf.GetValue() <= rf.GetValue() ? 1 : 0;
        }
        private void OpHiEq(QuestTCPVariant lf, QuestTCPVariant rf, ref QuestTCPVariant result)
        {
            result.Clear();
            result.vf = lf.GetValue() >= rf.GetValue() ? 1 : 0;
        }
        private void OpEq(QuestTCPVariant lf, QuestTCPVariant rf, ref QuestTCPVariant result)
        {
            result.Clear();
            result.vf = lf.GetValue() == rf.GetValue() ? 1 : 0;
        }
        private void OpNotEq(QuestTCPVariant lf, QuestTCPVariant rf, ref QuestTCPVariant result)
        {
            result.Clear();
            result.vf = lf.GetValue() != rf.GetValue() ? 1 : 0;
        }
        private void OpAnd(QuestTCPVariant lf, QuestTCPVariant rf, ref QuestTCPVariant result)
        {
            result.Clear();
            if ((lf.vtype == QuestTCPVType.vtExt) && (rf.vtype == QuestTCPVType.vtExt))
                result.vf = (lf.vf != 0) && (rf.vf != 0) ? 1 : 0;
            else if ((lf.vtype == QuestTCPVType.vtRange) && (rf.vtype == QuestTCPVType.vtRange))
            {
                result.CopyDataFrom(lf);
                result.vd.Add(rf.vd);
            }
            else if ((lf.vtype == QuestTCPVType.vtExt) && (rf.vtype == QuestTCPVType.vtRange))
            {
                result.CopyDataFrom(rf);
                result.vd.Add(lf.vf);
            }
            else if ((lf.vtype == QuestTCPVType.vtRange) && (rf.vtype == QuestTCPVType.vtExt))
            {
                result.CopyDataFrom(lf);
                result.vd.Add(rf.vf);
            }
        }
        private void OpOr(QuestTCPVariant lf, QuestTCPVariant rf, ref QuestTCPVariant result)
        {
            OpAnd(lf, rf, ref result); // ?? The same in source code
        }

        private int GetOperationOrder(char c)
        {
            if ((c == '/') || (c == 'f') || (c == 'g'))
                return 1;
            else if (c == '*')
                return 2;
            else if (c == '-')
                return 3;
            else if (c == '+')
                return 4;
            else if (c == '$')
                return 5;
            else if (c == '#')
                return 6;
            else if ((c == 'c') || (c == 'b') || (c == 'e') || (c == '>') || (c == '<') || (c == '='))
                return 7;
            else if (c == '&')
                return 8;
            else if (c == '|')
                return 9;
            return -1;
        }
        private int FindOperation(string str, int len)
        {
            int oporder = 0;
            int pos = 0;
            int spcount = 0;
            int pcount = 0;
            for (int i = 0; i <= len; i++)
            {
                char c = str[i];
                if (c == '(') pcount++;
                if (c == ')') pcount--;
                if (c == '[') spcount++;
                if (c == ']') spcount--;
                if ((pcount == 0) && (spcount == 0))
                {
                    int newoporder = GetOperationOrder(c);
                    if (oporder <= newoporder)
                    {
                        oporder = newoporder;
                        pos = i;
                    }
                }
            }
            return pos;
        }
        private bool CheckParentheses(string s, int start, int end)
        {
            int cnt = 0;
            for (int i = start; i <= end; i++)
            {
                char c = s[i];
                if (c == '(') cnt++;
                if (c == ')') cnt--;
                if (cnt < 0)
                    return false;
            }
            return cnt == 0;
        }
        private string StirngReplace(string str, string oStr, string nStr)
        {
            return str.Replace(oStr, nStr);
        }
        private string ConvertStirng(string str)
        {
            string result = str.ToLower();
            result = StirngReplace(result,"div", "f");
            result = StirngReplace(result,"mod", "g");
            result = StirngReplace(result,"in", "#");
            result = StirngReplace(result,"to", "$");
            result = StirngReplace(result,"or", "|");
            result = StirngReplace(result,"and", "&");
            result = StirngReplace(result,"<>", "e");
            result = StirngReplace(result,">=", "c");
            result = StirngReplace(result,"<=", "b");
            result = StirngReplace(result,"..", "h");
            result = StirngReplace(result,".", ",");

            result = StirngReplace(result," ", "");
            result = StirngReplace(result,"d", "");
            result = StirngReplace(result,"m", "");
            result = StirngReplace(result,"o", "");
            result = StirngReplace(result,"t", "");
            result = StirngReplace(result,"i", "");
            result = StirngReplace(result,"a", "");
            result = StirngReplace(result,"n", "");
            result = StirngReplace(result,"d", "");
            result = "("+result+")";

            return result;

        }
        
    }
}