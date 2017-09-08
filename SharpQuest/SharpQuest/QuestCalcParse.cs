using System;
using System.Linq;

namespace SharpQuest
{
    public class QuestCalcParse
    {
        
        bool num_error = true;
        bool diapazone_error = true;
        bool parameters_error = false;
        bool sym_warning = false;
        public bool default_expression = false;
        public bool calc_error = false;
        //private string _str;
        public bool error;
        public int answer;

        public QuestCalcParse()
        {
            Clear();
            //Parse(str, pars);
        }
        public void Clear()
        {
            error = false;
            answer = 0;;
            diapazone_error = true;
            parameters_error = false;
            sym_warning = false;
            default_expression = false;
            calc_error = false;
            num_error = true;
        }

        public string AssignAndPreprocess(string str, int currparnum)
        {
            Clear();
            int i;
            string tempstr;
            string orig_str = str;
            str = ConvertStirng(str);
            str = FixSeparate(str);
            str = FixFinal(str);
            str = FixNum(str);
            bool parentheses_error = !CheckParentheses(str, 0, str.Length-1);
            if (parentheses_error) error = true;
            tempstr = str;
            if (!error)
            { 
                i = str.Length-1;
                if ((i >= 1) && (str[0] == '(') && (str[i] == ')') && CheckParentheses(str, 2, i - 1))
                {
                    tempstr = "";
                    for (int c = 1; c < i; c++)
                        tempstr = tempstr + str[c];
                }
            }
            if (orig_str != ConvertStirng(tempstr))
                sym_warning = true;

            if ((str == "") || (str == "[p" + currparnum + "]"))
            {
                default_expression = true;
                str = "[p" + currparnum + "]";
            }

            return str;
        }

        public void Parse(string str, int[] pars)
        {
            str = ConvertStirng(str);

            string tmp = InsertParValues(str, pars, false);
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
        public static string InsertParValues(string str, int[] pars, bool asItIs = true)
        {
            string result = str;
            for (int i = 0; i < pars.Length; i++)
            {
                int val = pars[i];
                string s = string.Format("{0}", val);
                if (val < 0)
                    s = asItIs ? s : string.Format("(0{0})", val);
                
                result = StringReplace(result, string.Format("[p{0}]", i+1), s);
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
                        string left = str.Substring(0, c);
                        string right = str.Substring(c+1, lenexpr-c-1);

                        QuestTCPVariant rf = Calc(right);
                        if (calc_error) return result;

                        QuestTCPVariant lf = Calc(left);
                        if (calc_error) return result;

                        try
                        {
                            char ch = str[c];
                            if (ch == '+') OpAdd(lf, rf, ref result);
                            if (ch == '-') OpSub(lf, rf, ref result);
                            if (ch == '*') OpMul(lf, rf, ref result);
                            if (ch == '/') OpDiv(lf, rf, ref result);
                            if (ch == 'f') OpDivTrunc(lf, rf, ref result);
                            if (ch == 'g') OpMod(lf, rf, ref result);
                            if (ch == '$') OpTo(lf, rf, ref result);
                            if (ch == '#') OpIn(lf, rf, ref result);
                            if (ch == '>') OpHi(lf, rf, ref result);
                            if (ch == '<') OpLo(lf, rf, ref result);
                            if (ch == 'c') OpHiEq(lf, rf, ref result);
                            if (ch == 'b') OpLoEq(lf, rf, ref result);
                            if (ch == 'e') OpNotEq(lf, rf, ref result);
                            if (ch == '=') OpEq(lf, rf, ref result);
                            if (ch == '&') OpAnd(lf, rf, ref result);
                            if (ch == '|') OpOr(lf, rf, ref result);

                        }
                        catch
                        {
                            calc_error = true;
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
            result.Clear();
            if ((lf.vtype == QuestTCPVType.vtExt) && (rf.vtype == QuestTCPVType.vtExt))
                result.vf = (lf.vf != 0) || (rf.vf != 0) ? 1 : 0;
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
            for (int i = 0; i < len; i++)
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
        private static string StringReplace(string str, string oStr, string nStr)
        {
            return str.Replace(oStr, nStr);
        }
        private string ConvertStirng(string str)
        {
            string result = str.ToLower();
            result = StringReplace(result,"div", "f");
            result = StringReplace(result,"mod", "g");
            result = StringReplace(result,"in", "#");
            result = StringReplace(result,"to", "$");
            result = StringReplace(result,"or", "|");
            result = StringReplace(result,"and", "&");
            result = StringReplace(result,"<>", "e");
            result = StringReplace(result,">=", "c");
            result = StringReplace(result,"<=", "b");
            result = StringReplace(result,"..", "h");
            result = StringReplace(result,".", ",");

            result = StringReplace(result," ", "");
            result = StringReplace(result,"d", "");
            result = StringReplace(result,"m", "");
            result = StringReplace(result,"o", "");
            result = StringReplace(result,"t", "");
            result = StringReplace(result,"i", "");
            result = StringReplace(result,"a", "");
            result = StringReplace(result,"n", "");
            result = StringReplace(result,"d", "");
            result = "("+result+")";

            return result;

        }

        // Вся строка делится на строки в [ ] и вне них
        // для строк без [] вызывается FixLitNorm, а в [] FixLitDP
        private string FixSeparate(string _str)
        {
            string result = "";
            bool normalscan = true; ;
            int c = 0;
            int l = _str.Length;
            string tstr = "";
            while (c < l)
            {
                if (normalscan)
                {
                    if (_str[c] == '[')
                    {
                        result += FixLitNorm(tstr);
                        tstr = "[";
                        normalscan = false;
                        c++;
                    }
                    else
                    {
                        tstr += _str[c];
                        c++;
                        if (c >= l)
                            result += FixLitNorm(tstr);
                    }
                }
                else
                {
                    if ((c >= l) || (_str[c] == ']')) {
                        result += FixLitDP(tstr + ']');
                        tstr = "";
                        normalscan = true;
                    }
                    else
                        tstr += _str[c];
                    c++;
                }
            }
            return result;
        }

        // упрощаем конструкции, стоящие за скобками []
        private string FixLitNorm(string _str)
        {
            string result = "";
            // Убираем заведомо ненужные символы
            char[] chrs = new char[] { '+', '-', '*', '/', '#', '$', 'c', 'b', 'e', 'f', 'g', '=', '>', '<', '&', '|', ',', '(', ')' };

            string gstr = "";
            foreach (char c in _str)
            {
                if (((c >= '0') && (c <= '9')) || chrs.Contains(c))
                {
                    gstr += c;
                }
            }
            _str = gstr;
            // Начинаем преобразование конструкций
            while (true)
            {
                gstr = _str;
                string tstr = _str;
                while (true) // Скобки поглощают некоторые рядом стоящие символы
                {
                    _str = tstr;
                    tstr = StringReplace(tstr, ")(", ")*(");
                    tstr = StringReplace(tstr, "()", "");
                    tstr = StringReplace(tstr, ".", ",");
                    tstr = StringReplace(tstr, ",,", ",");
                    tstr = StringReplace(tstr, "(,", "(0,");
                    tstr = StringReplace(tstr, "),", ")*0,");
                    tstr = StringReplace(tstr, ")0", ")*0");
                    tstr = StringReplace(tstr, ")1", ")*1");
                    tstr = StringReplace(tstr, ")2", ")*2");
                    tstr = StringReplace(tstr, ")3", ")*3");
                    tstr = StringReplace(tstr, ")4", ")*4");
                    tstr = StringReplace(tstr, ")5", ")*5");
                    tstr = StringReplace(tstr, ")6", ")*6");
                    tstr = StringReplace(tstr, ")7", ")*7");
                    tstr = StringReplace(tstr, ")8", ")*8");
                    tstr = StringReplace(tstr, ")9", ")*9");
                    tstr = StringReplace(tstr, ",(", ",*(");
                    tstr = StringReplace(tstr, "0(", "0*(");
                    tstr = StringReplace(tstr, "1(", "1*(");
                    tstr = StringReplace(tstr, "2(", "2*(");
                    tstr = StringReplace(tstr, "3(", "3*(");
                    tstr = StringReplace(tstr, "4(", "4*(");
                    tstr = StringReplace(tstr, "5(", "5*(");
                    tstr = StringReplace(tstr, "6(", "6*(");
                    tstr = StringReplace(tstr, "7(", "7*(");
                    tstr = StringReplace(tstr, "8(", "8*(");
                    tstr = StringReplace(tstr, "9(", "9*(");
                    if (_str == tstr)
                        break;
                }


                // Упрощаем конструкции из рядом стоящих операций
                int l = _str.Length;
                tstr = "";
                result = "";
                int i = 0;
                while (i < l)
                {

                    if ((i < l) && (GetOperationOrder(_str[i]) > 0))
                    {
                        tstr = "";
                        while ((i < l) && (GetOperationOrder(_str[i]) > 0))
                        {
                            tstr += _str[i];
                            i++;
                        }

                        result += FixOp(tstr);
                    }
                    if ((i < l) && GetOperationOrder(_str[i]) < 0)
                    {
                        tstr = "";
                        while ((i < l) && (GetOperationOrder(_str[i]) < 0))
                        {
                            tstr += _str[i];
                            i++;
                        }

                        result += tstr;
                    }
                }

                _str = result;
                tstr = _str;
                while (true) // Cнова скобки вносят преобразования
                {
                    _str = tstr;
                    tstr = StringReplace(tstr, "(+", "(");
                    tstr = StringReplace(tstr, "(*", "(");
                    tstr = StringReplace(tstr, "(/", "(");
                    tstr = StringReplace(tstr, "(&", "(");
                    tstr = StringReplace(tstr, "(|", "(");
                    tstr = StringReplace(tstr, "(#", "(");
                    tstr = StringReplace(tstr, "($", "(");
                    tstr = StringReplace(tstr, "(c", "(");
                    tstr = StringReplace(tstr, "(b", "(");
                    tstr = StringReplace(tstr, "(e", "(");
                    tstr = StringReplace(tstr, "(f", "(");
                    tstr = StringReplace(tstr, "(g", "(");
                    tstr = StringReplace(tstr, "(<", "(");
                    tstr = StringReplace(tstr, "(>", "(");
                    tstr = StringReplace(tstr, "(=", "(");
                    tstr = StringReplace(tstr, "-)", ")");
                    tstr = StringReplace(tstr, "+)", ")");
                    tstr = StringReplace(tstr, "*)", ")");
                    tstr = StringReplace(tstr, "/)", ")");
                    tstr = StringReplace(tstr, "&)", ")");
                    tstr = StringReplace(tstr, "|)", ")");
                    tstr = StringReplace(tstr, "$)", ")");
                    tstr = StringReplace(tstr, "#)", ")");
                    tstr = StringReplace(tstr, "c)", ")");
                    tstr = StringReplace(tstr, "b)", ")");
                    tstr = StringReplace(tstr, "e)", ")");
                    tstr = StringReplace(tstr, "f)", ")");
                    tstr = StringReplace(tstr, "g)", ")");
                    tstr = StringReplace(tstr, ">)", ")");
                    tstr = StringReplace(tstr, "<)", ")");
                    tstr = StringReplace(tstr, "=)", ")");
                    tstr = StringReplace(tstr, "()", "");
                    tstr = StringReplace(tstr, ")(", ")*(");
                    if (_str == tstr)
                        break;
                }
                if (gstr == _str) // если поменять больше нечего - получаем ответ
                    break;
            }
            
            return result;
        }

        // упрощает и распознает диапазон или параметр
        private string FixLitDP(string str)
        {
            if (StringReplace(str, "p", "") != str)
                return FixLitP(str);
            else
                return FixLitD(str);
        }

        // упрощает и распознает параметр
        private string FixLitP(string str)
        {
            string tstr = "";
            foreach (char c in str)
            {
                if (tstr.Length > 2)
                    break;
                if ((c >= '0') && (c <= '9'))
                    tstr += c;
            }
            int i = int.Parse('0' + tstr);
            if ((i > 0) && (i < 25))
            {
                return "[p" + i + "]";
            }
            else
            {
                parameters_error = true;
                error = true;
                return  "[err]";
            }
        }

        // упрощает и распознает диапазон
        private string FixLitD(string str)
        {

            string tstr = "";
            int l = str.Length;
            foreach (char c in str)
            {
                if (((c >= '0') && (c <= '9')) || (c == '-') || (c == ';') || (c == 'h'))
                    tstr += c;
            }
            str = ';' + tstr + ';';
            tstr = str;
            while (true)
            {
                str = tstr;
                tstr = StringReplace(tstr, "--", "");
                tstr = StringReplace(tstr, ";;", ";");
                tstr = StringReplace(tstr, "h;", ";");
                tstr = StringReplace(tstr, ";h", ";");
                tstr = StringReplace(tstr, "-;", ";");
                tstr = StringReplace(tstr, "-h", "h");
                tstr = StringReplace(tstr, "hh", "h");
                if (str == tstr)
                    break;
            }
            if ((tstr != ";") && (str.Length > 0))
            {
                str = "[" + str.Substring(1, str.Length - 2) + "]";
                QuestTCPRange d = new QuestTCPRange();
                d.Assign(str);
                str = d.GetString();
                return str;
            }
            else
            {
                diapazone_error = true;
                error = true;
                return "[err]";

            }
        }

        private string FixFinal(string str)
        {
            string tstr = str;
            while (true)
            {
                str = tstr;
                tstr = StringReplace(tstr, "-,", "-0,");

                tstr = StringReplace(tstr, ")[", ")*[");
                tstr = StringReplace(tstr, "](", "]*(");
                tstr = StringReplace(tstr, ")(", ")*(");
                tstr = StringReplace(tstr, "][", "]*[");

                tstr = StringReplace(tstr, "],", "]*0,");
                tstr = StringReplace(tstr, "]0", "]*0");
                tstr = StringReplace(tstr, "]1", "]*1");
                tstr = StringReplace(tstr, "]2", "]*2");
                tstr = StringReplace(tstr, "]3", "]*3");
                tstr = StringReplace(tstr, "]4", "]*4");
                tstr = StringReplace(tstr, "]5", "]*5");
                tstr = StringReplace(tstr, "]6", "]*6");
                tstr = StringReplace(tstr, "]7", "]*7");
                tstr = StringReplace(tstr, "]8", "]*8");
                tstr = StringReplace(tstr, "]9", "]*9");
                tstr = StringReplace(tstr, ",[", ",*[");
                tstr = StringReplace(tstr, "0[", "0*[");
                tstr = StringReplace(tstr, "1[", "1*[");
                tstr = StringReplace(tstr, "2[", "2*[");
                tstr = StringReplace(tstr, "3[", "3*[");
                tstr = StringReplace(tstr, "4[", "4*[");
                tstr = StringReplace(tstr, "5[", "5*[");
                tstr = StringReplace(tstr, "6[", "6*[");
                tstr = StringReplace(tstr, "7[", "7*[");
                tstr = StringReplace(tstr, "8[", "8*[");
                tstr = StringReplace(tstr, "9[", "9*[");
                if (str == tstr)
                    break;
            }
            return str;
        }
        private string FixNum(string str)
        {
            int i = 0;
            int c = str.Length;
            string s = "";
            string s1 = "";
            double f = 0;
            while (i < c)
            {
                if (((str[i] >= '0') && (str[i] <= '9')) || (str[i] == ','))
                {
                    s += str[i];
                }
                else
                {
                    if (s != "")
                    {
                        if (!double.TryParse(s.Replace(',','.'), out f))
                        {
                            error = true;
                            num_error = true;
                            return "";
                        }

                        if (f > 999999999) f = 999999999;
                        if ((f < 0.0001) && (f != 0)) f = 0.0001;

                        s1 += f.ToString() + str[i];
                        s = "";
                    }
                    else
                        s1 += str[i];
                }
                i++;
            }
            return s1;
        }
        private char FixOp(string str)
        {
            int cc = 0;
            int qq = 0;
            string result = "";

            foreach (char c in str)
            {
                if (c == '-') cc++;
                if (c == '+') qq++;
            }
            result = StringReplace(str, "-", "");
            result = StringReplace(result, "+", "");
            if ((cc % 2) == 1)
                result += "-";
            else if ((qq > 0) || (cc > 0))
                result += "+";

            int l = result.Length;
            cc = 0;
            qq = 0;
            for (int i = l - 1; i >= 0; i--)
            {
                if (cc <= GetOperationOrder(result[i]))
                {
                    qq = i;
                    cc = GetOperationOrder(result[i]);
                }
            }
            return result[qq];
        }

    }
}