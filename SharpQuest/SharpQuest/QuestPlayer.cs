using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Newtonsoft.Json;

namespace SharpQuest
{
    public class QuestPlayer
    {
        public readonly Quest quest;
        private int currentLocationIndx;

        private int lastpathindex = 0;
        public int daysPassed = 0;
        public int[] Pars = new int[Quest.maxparameters];
        public bool[] ParVisState = new bool[Quest.maxparameters];
        public int[,] PathesWeCanGo = new int[Quest.maxlocations, Quest.maxpathes];

        public string CustomCriticalMessage = "";
        public int CurrentCriticalParameter = 0;
            
        public string ShowParameters(int i)
        {
            var qp = this;
            if (qp.ParVisState[i] == false)
                return "";
                
            if ((!qp.quest.Pars[i].Enabled) || (qp.quest.Pars[i].Hidden)) 
                return "";

            if ((qp.Pars[i] == 0) && (!qp.quest.Pars[i].ShowIfZero))
                return "";

            var str = qp.quest.Pars[i].GetVFStringByValue(qp.Pars[i]);
            str = str.Replace("<>", qp.Pars[i].ToString());// StringReplaceEC(str, "<>", qp.Pars[i]);
            return str;
        }

        public void ShowParameters()
        {
            var sb = new StringBuilder();
            sb.AppendLine("----------------------------------");
            for (var i = 0; i < Quest.maxparameters; ++i)
            {
                var str = ShowParameters(i);

                sb.AppendLine(str);
            }
            sb.AppendLine("----------------------------------");
            
            Console.WriteLine(sb.ToString());//FixStringValueParameters(sb.ToString(),false) );
        }

        [Serializable]
        struct PathState
        {
            public int Loc;
            public int Path;
            public int Value;
        }
        
        [Serializable]
        struct State
        {
            public string Description;
            public string[] StrPars;
            public int dayspassed;
            public string CustomCriticalMessage;
            public int CurrentCriticalParameter;
            public int[] Pars;
            public bool[] ParVisState;
            public List<PathState> PathesWeCanGo;
        }
        
        public static string ToJSON<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.None,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
        }
        
        public QuestPlayer(Quest q, string steps)
        {
            Console.WriteLine("-----------------START------------------");
            quest = q;
            StartGame();

            for (var stepi = 0; stepi < steps.Length; ++stepi)
            {
                var variant = steps[stepi];
                var variantInt = int.Parse(variant.ToString()) - 1;
                    
                ShowParameters();
                
                var loc = CurrentLocation();
                Console.WriteLine(loc);
            
                Console.WriteLine("----------------------------------");
                
                //
                State s = new State();
                s.Description = loc.LocationDescription;
                s.StrPars = new string[Quest.maxparameters];
                s.Pars = new int[Quest.maxparameters];
                s.ParVisState = new bool[Quest.maxparameters];
                for (var i = 0; i < Quest.maxparameters; ++i)
                {
                    s.StrPars[i] = ShowParameters(i);
                    s.Pars[i] = Pars[i];
                    s.ParVisState[i] = ParVisState[i];
                }
                s.dayspassed = daysPassed;
                s.CustomCriticalMessage = CustomCriticalMessage;
                s.CurrentCriticalParameter = CurrentCriticalParameter;
                
                s.PathesWeCanGo = new List<PathState>();
                for (int i = 0; i < q.LocationsValue; ++i)
                {
                    for (int j = 0; j < q.PathesValue; ++j)
                    {
                        if (PathesWeCanGo[i, j] != 0)
                        {
                            s.PathesWeCanGo.Add(new PathState() {Loc = i + 1, Path = j + 1, Value = PathesWeCanGo[i, j]});
                        }
                    }
                }
                

                Console.WriteLine(ToJSON(s));

                Console.WriteLine("----------------------------------");
                
                var trans = PossibleTransitions();
                foreach (var v in trans)
                    Console.WriteLine("- " + v.StartPathMessage);

                var p = trans[variantInt];
                Console.WriteLine("===================================");
                Console.WriteLine("- " + p.StartPathMessage);
                Console.WriteLine("===================================");
                
                if (string.IsNullOrEmpty(p.EndPathMessage) == false)
                {
                    Console.WriteLine(p.EndPathMessage);
                    Console.WriteLine("===================================");
                    Console.WriteLine("- Далее");
                    Console.WriteLine("===================================");
                    ++stepi;
                    //Assert.IsTrue(steps[stepi] == '1');
                }
                
                DoTransition(p);
            }
            
            Console.WriteLine("-----------------Finish------------------");
        }
        
        public QuestPlayer(Quest q)
        {
            quest = q;
            StartGame();
        }

        public void StartGame()
        {
            var q = quest;
            lastpathindex = 0;
 
            daysPassed = 0;
            currentLocationIndx = q.startLocationIndx;

            for (var i = 0; i < Quest.maxparameters; ++i)
            {
                ParVisState[i] = q.Pars[i].Enabled && q.Pars[i].Hidden == false;
            }
            
            for (var i = 0; i < Quest.maxlocations; ++i)
                for (var c = 0; c < Quest.maxpathes; ++c)
                    PathesWeCanGo[i, c] = 0;

            for (var i = 0; i < q.PathesValue; ++i)
            {
                if (q.Paths[i].PassTimesValue == 0)
                    PathesWeCanGo[q.Paths[i].FromLocation, i] = 2000000000;
                else 
                    PathesWeCanGo[q.Paths[i].FromLocation, i] = q.Paths[i].PassTimesValue;
                
                //Console.WriteLine("{0} {1}", q.Paths[i].FromLocation, i);
            }
            
            for (var i = 0; i < Quest.maxparameters; ++i)
            {
                if (q.Pars[i].AltDiapStartValues.count > 0)
                {
                    Pars[i] = (int)Math.Floor(q.Pars[i].AltDiapStartValues.GetRandom());
                } 
                else
                {
                    Pars[i] = q.Pars[i].value;
                }
            }

            WeAreInTheLocation();
        }

        bool IsGamePathParameterFail(int pathIndx, int[] pars, int[] oldPars)
        {
            if (pathIndx <= 0) return false;

            for (var i = 0; i < Quest.maxparameters; ++i)
            {
                if (quest.Pars[i].Enabled == false) continue;
                
                if (oldPars[i] == pars[i]) continue;
                
                if ( quest.Pars[i].ParType != QuestParameter.FailParType && quest.Pars[i].ParType != QuestParameter.DeathParType )
                    continue;

                if ((quest.Pars[i].LoLimit) && (pars[i] <= quest.Pars[i].min) ||
                    (!quest.Pars[i].LoLimit) && (pars[i] >= quest.Pars[i].max))
                {
                    CustomCriticalMessage = quest.Paths[pathIndx].DPars[i].CriticalMessage.Trim();
                    CurrentCriticalParameter = i;
                    
                    Console.WriteLine( i + " -> " + quest.Paths[pathIndx].DPars[i].CriticalMessage.Trim() );
                    return true;
                }
                
            }
            return false;
        }
        
        bool IsGamePathParameterSuccess(int pathIndx, int[] pars, int[] oldPars)
        {
            if (pathIndx <= 0) return false;

            for (var i = 0; i < Quest.maxparameters; ++i)
            {
                if (quest.Pars[i].Enabled == false) continue;
                
                if (oldPars[i] == pars[i]) continue;
                
                if ( quest.Pars[i].ParType != QuestParameter.SuccessParType )
                    continue;

                if ((quest.Pars[i].LoLimit) && (pars[i] <= quest.Pars[i].min) ||
                    (!quest.Pars[i].LoLimit) && (pars[i] >= quest.Pars[i].max))
                {
                    CustomCriticalMessage = quest.Paths[pathIndx].DPars[i].CriticalMessage.Trim();
                    CurrentCriticalParameter = i;
                    
                    Console.WriteLine( i + " -> " + quest.Paths[pathIndx].DPars[i].CriticalMessage.Trim() );
                    return true;
                }
                
            }
            return false;
        }

        void WeAreInTheLocation()
        {
            var successFlag = false;

//            var failFlag = IsGamePathParameterFail(lastpathindex, Pars, UndoPar[UndoIndex]);
//            if (!failFlag)
//                successFlag = IsGamePathParameterSuccess(lastpathindex, Pars, UndoPar[UndoIndex]);
//
//            if (successFlag)
//            {
//                System.Console.WriteLine("ShowSuccess");
//                return;
//            }
//
//            if (failFlag) 
//            {
//                System.Console.WriteLine("ShowFail");
//                return;
//            }

            QuestParameterDelta[] delta = new QuestParameterDelta[Quest.maxparameters];
            int[] tpars = new int[Quest.maxparameters];
            for (var i = 0; i < Quest.maxparameters; ++i)
            {
                delta[i] = quest.Locations[currentLocationIndx].DPars[i];
                tpars[i] = Pars[i];
            }

            ProcessParametersWithDelta(delta);
            ProcessParVisualOpions(delta);
            
            //Печатаем текст локации,если все нормально
            if (quest.Locations[currentLocationIndx].VoidLocation == false)
            {
                quest.Locations[currentLocationIndx].FindLocationDescription(Pars);
//                SDTcall(
//                    FixStringValueParameters(trim(PlayGame.Locations[locationindex].LocationDescription.text), true),
//                    false);
            }

            daysPassed += quest.Locations[currentLocationIndx].DaysCost;

            successFlag = false;
            var failFlag = IsGamePathParameterFail(currentLocationIndx, Pars, tpars);
            if (!failFlag)
                successFlag = IsGamePathParameterSuccess(currentLocationIndx, Pars, tpars);

            if (successFlag)
            {
                System.Console.WriteLine("ShowSuccess");
                return;
            }

            if (failFlag) 
            {
                System.Console.WriteLine("ShowFail");
                return;
            }
            

//            for (var i = 0; i < Quest.maxparameters; ++i)
//            {
//                Console.WriteLine(string.Join(",", Pars));
//            }

//            if (GameState != GameStateGoPath) 
//                return;
//
//            //
        }


        void ProcessParametersWithDelta(QuestParameterDelta[] delta) // delta.size == maxparameters
        {
            int[] tpars = new int[Quest.maxparameters];
            
            for (var i = 0; i < Quest.maxparameters; ++i)
            {
                if (quest.Pars[i].Enabled)
                {
                    var tstr = delta[i].Expression.Trim();
                    
                    if (delta[i].DeltaExprFlag)
                    {
                        if (tstr != null)
                        {
                            var parse = new QuestCalcParse(tstr, i, Pars);
                            if (parse.error == false)
                                tpars[i] = parse.answer;
                        } else {
                            tpars[i] = Pars[i];
                        }
                    }
                    else
                    {
                        if (delta[i].DeltaApprFlag) 
                        {
                            tpars[i] = delta[i].delta;
                        }
                        else
                        {
                            if (delta[i].DeltaPercentFlag)
                                tpars[i] = Pars[i] + (int)Math.Round((Pars[i] / 100.0f) * (delta[i].delta));
                            else
                                tpars[i] = Pars[i] + delta[i].delta;
                        }
                    }
                    if (tpars[i] > quest.Pars[i].max) tpars[i] = quest.Pars[i].max;
                    if (tpars[i] < quest.Pars[i].min) tpars[i] = quest.Pars[i].min;
                }
            }
            
            for (var i = 0; i < Quest.maxparameters; ++i)
            {
                if (quest.Pars[i].Enabled)
                {
                    Pars[i] = tpars[i];
                }
            }
        }
        
        void ProcessParVisualOpions(QuestParameterDelta[] delta)    // delta.size == maxparameters
        {
            for (var i = 0; i < Quest.maxparameters; ++i)
            {
                if (delta[i].ParameterViewAction == QuestParameterDelta.HideParameter)
                {
                    ParVisState[i] = false;
                }

                if (delta[i].ParameterViewAction == QuestParameterDelta.ShowParameter)
                {
                    ParVisState[i] = true;
                }
            }
        }

        bool NeverHadPathes(int loc)
        {
            return quest.Locations[loc].transitions.Count == 0;
        }

        bool ArePathesStillFromHere(int loc)
        {
            if (quest.Locations[loc].EndLocationFlag || quest.Locations[loc].FailLocationFlag)
                return true;

            if (NeverHadPathes(loc)) 
                return true;

            for (var i = 0; i < quest.PathesValue; ++i)
                if (PathesWeCanGo[loc, i] >= 1)
                    return true;
            
            return false;
        }

        bool IsParametersGatesOk(int[] Pars, int pathIndx)
        {
            var tstr = quest.Paths[pathIndx].LogicExpression.Trim();
            if (tstr != "") 
            {
//                    parse:=TCalcParse.Create;
////          parse.AssignAndPreprocess(LogicExpression.Text,1); // 1 - не имееет значения
//                    parse.internal_str:=parse.ConvertToInternal(tstr);
//                    if not parse.default_expression then 
//                    {
////              if not parse.error then
//                        parse.Parse(CalcParseClass.TParValues(pars));
//                        if (not parse.calc_error)and(parse.answer=0) then 
//                        {
//                            return false;
//                        }
//                    }
//                    parse.Destroy;
            }

            for (var i = 0; i < Quest.maxparameters; ++i)
            {
                if (quest.Pars[i].Enabled)
                {
     
                    if (Pars[i] > quest.Paths[pathIndx].DPars[i].max)
                        return false;
                    if (Pars[i] < quest.Paths[pathIndx].DPars[i].min)
                        return false;
     
                    if (!IsParametersBitmaskOk(Pars[i], quest.Paths[pathIndx].DPars[i].bitmask))
                        return false;
                    if (!IsValueGatesOk(Pars[i], quest.Paths[pathIndx].DPars[i].ValuesGate))
                        return false;
                    if (!IsModZeroeGatesOk(Pars[i], quest.Paths[pathIndx].DPars[i].ModZeroesGate))
                        return false;
                }
            }

            return true;
        }

        private bool IsModZeroeGatesOk(int parametervalue, QuestValuesList modZeroesGate)
        {
            if (modZeroesGate.Count==0)
              return true;
            
            if (!modZeroesGate.Negation)
            {
              
                for (var i = 0; i < modZeroesGate.Count; ++i)
                    if ((parametervalue % modZeroesGate.Values[i]) == 0) 
                        return false;
            
                return true;
            }
            
            for (var i = 0; i < modZeroesGate.Count; ++i)
                if ((parametervalue % modZeroesGate.Values[i]) == 0) 
                    return true;
            
            return false;
        }

        private bool IsValueGatesOk(int parametervalue, QuestValuesList valuesGate)
        {
            if (valuesGate.Count==0)
              return true;
            
            if (!valuesGate.Negation)
            {
              
                for (var i = 0; i < valuesGate.Count; ++i)
                    if (valuesGate.Values[i]==parametervalue) 
                        return false;
            
                return true;
            }
            
            for (var i = 0; i < valuesGate.Count; ++i)
                if (valuesGate.Values[i]==parametervalue) 
                    return true;
            
            return false;
        }

        private bool IsParametersBitmaskOk(int parametervalue, int bitmask)
        {
            for (var c = 1; c <= 9; ++c)
            {
               if ( ((parametervalue & 1)==0) && ((bitmask & 1)==1) )
                 return false;
               
               parametervalue = parametervalue >> 1;
               bitmask = bitmask >> 1;
            }
            return true;
        }

        public QuestLocation CurrentLocation()
        {
            return quest.Locations[currentLocationIndx];
        }

        public List<QuestPath> PossibleTransitions()
        {
            var result = new List<QuestPath>();

            foreach (var t in CurrentLocation().transitions)
            {
                var canGo = PathesWeCanGo[currentLocationIndx, t.PathIndx]; 
                if (canGo <= 0) continue;
                if (ArePathesStillFromHere(currentLocationIndx) == false) continue;

                bool gatesOk = IsParametersGatesOk(Pars, t.PathIndx);

                bool found = false;
                foreach (var r in result)
                {
                    if (t.StartPathMessage.Trim() == r.StartPathMessage.Trim())
                        found = true;
                }
                
                if (found)
                    continue;
                
                
                if (gatesOk)
                    result.Add(t);
                else if (t.AlwaysShowWhenPlaying)
                    result.Add(t); //TODO: if gatesOk == false then it's disabled
            }

            return result;
        }

        public void DoTransition(QuestPath questPath)
        {
            //if ((accessible_pathes_count <= 0) && (voidpathindex <= 0)) then return
  
                //Выполнение перехода, если выбран переход
            var pathindex = questPath.PathIndx;

//            if (voidpathindex > 0)
//            {
//                pathindex:=voidpathindex;
//            }
//            else
//            {
//                if (CheckedAnswer > 0) then
//                    pathindex = accessible_pathes[CheckedAnswer];
//            }

            if (pathindex == 0)
            {
                //ShowMessage('Ошибка перехода');
                return;
            }

            var CurrPathX = currentLocationIndx;
            var CurrPathY = pathindex;

            PathesWeCanGo[CurrPathX, CurrPathY]--;

            daysPassed += quest.Paths[pathindex].dayscost;
            
            //makedo;
            
            currentLocationIndx = quest.Paths[pathindex].ToLocation;
            
            QuestParameterDelta[] delta = new QuestParameterDelta[Quest.maxparameters];
            for (var i = 0; i < Quest.maxparameters; ++i)
                delta[i] = quest.Paths[pathindex].DPars[i];
    
            ProcessParametersWithDelta(delta);
            ProcessParVisualOpions(delta);
    
            lastpathindex = pathindex;
    
//            if (quest.Paths[pathindex].EndPathMessage.Trim() != "")
//            {
//                Console.WriteLine(quest.Paths[pathindex].EndPathMessage.Trim());
//            }
            
            WeAreInTheLocation();
        }
    }
}