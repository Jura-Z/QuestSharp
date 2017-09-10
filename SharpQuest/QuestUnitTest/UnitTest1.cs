using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpQuest;
using System.IO;
using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;

namespace QuestUnitTest
{
    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void TestExpr()
        {
            int[] Pars = new int[Quest.maxparameters];
            for (int i = 0; i < Pars.Count(); i++)
                Pars[i] = i;

            string tstr = "";
            int index = 0;

            QuestRandom.Clear();
            QuestRandom.AddSeq("A", 10, 31);
            QuestRandom.AddSeq("B", 13, 31);
            QuestRandom.AddSeq("A", 16, 19);
            QuestRandom.AddSeq("B", 13, 19);
            QuestRandom.AddSeq("A", 2, 19);
            QuestRandom.AddSeq("B", 2, 19);

            QuestRandom.AddSeq("A", 2, 5);
            QuestRandom.AddSeq("B", 2, 5);

            QuestRandom.AddSeq("A", 3, 5);
            QuestRandom.AddSeq("B", 3, 5);

            QuestRandom.AddSeq("A", 1, 5);
            QuestRandom.AddSeq("B", 1, 5);

            QuestRandom.AddSeq("A", 6, 20);
            QuestRandom.AddSeq("B", 18, 20);

            QuestRandom.AddSeq("A", 1, 3);
            QuestRandom.AddSeq("B", 2, 3);
            QuestRandom.FinishSeq();


            tstr = "[60..90]";
            var parse = new QuestCalcParse();
            parse.Parse(tstr, Pars);
            Assert.IsFalse(parse.error);
            Assert.AreEqual(73, parse.answer);

            tstr = "[81..99]";
            parse = new QuestCalcParse();
            parse.Parse(tstr, Pars);
            Assert.AreEqual(94, parse.answer);

            tstr = "[41..59]";
            parse = new QuestCalcParse();
            parse.Parse(tstr, Pars);
            Assert.AreEqual(43, parse.answer);

            // 21+3*100+2*10+1;
            tstr = "[p22]+[0..4]*100+[0..4]*10+[0..4]";
            parse = new QuestCalcParse();
            parse.Parse(tstr, Pars);
            Assert.IsFalse(parse.error);
            Assert.AreEqual(342, parse.answer);


            tstr = "[p4]*10>20+[p5]*0,8+[1..20]";
            Pars[4] = 100;
            parse = new QuestCalcParse();
            parse.Parse(tstr, Pars);
            Assert.IsFalse(parse.error);
            Assert.AreEqual(0, parse.answer);

            tstr = "XXXX([p24]<>0)+1";
            Pars[23] = 100;
            parse = new QuestCalcParse();
            tstr = parse.AssignAndPreprocess(tstr, 1);
            Assert.IsFalse(parse.error);
            Assert.AreEqual(0, parse.answer);

            tstr = "[p1] mod 7+1";
            Pars[0] = 1;
            parse = new QuestCalcParse();
            tstr = parse.AssignAndPreprocess(tstr, 1);
            parse.Parse(tstr, Pars);
            Assert.IsFalse(parse.error);
            Assert.AreEqual(2, parse.answer);

            tstr = "0,01*200";
            parse = new QuestCalcParse();
            parse.Parse(tstr, Pars);
            Assert.IsFalse(parse.error);
            Assert.AreEqual(2, parse.answer);

            tstr = "[p3]+0,01*[30..32]*[p2]+1";
            Pars[2] = 0;
            Pars[1] = 83;
            parse = new QuestCalcParse();
            parse.Parse(tstr, Pars);
            Assert.IsFalse(parse.error);
            Assert.AreEqual(28, parse.answer);

            tstr = "(0<90)";
            parse = new QuestCalcParse();
            parse.Parse(tstr, Pars);
            Assert.IsFalse(parse.error);
            Assert.AreEqual(1, parse.answer);

            tstr = "([p3]<90) or ([p4]<90)";
            Pars[2] = 50;
            Pars[3] = 100;
            parse = new QuestCalcParse();
            parse.Parse(tstr, Pars);
            Assert.IsFalse(parse.error);
            Assert.AreEqual(1, parse.answer);

            tstr = "(0-8)";
            parse = new QuestCalcParse();
            parse.Parse(tstr, Pars);
            Assert.IsFalse(parse.error);
            Assert.AreEqual(-8, parse.answer);

        }
        class SourceClass
        {
            static object[] FileCases()
            {
                var list = new List<object>();

                var basepath = Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath);
                basepath += "/../../../Data/";
                basepath = Path.GetFullPath(basepath);

                var fileList = new DirectoryInfo(basepath).GetFiles("*.result", SearchOption.AllDirectories);
                foreach (var item in fileList)
                {
                    string filename = item.FullName;
                    string path = Path.GetDirectoryName(filename);
                    filename = Path.GetFileName(filename);
                    list.Add(new object[] { path, filename});
                }


                object[] result = list.ToArray<object>();
                return result;
            }
        }

        [TestCaseSource(typeof(SourceClass), "FileCases")]
        public void TestNew(string basepath, string filename)
        {
            TestQuest(basepath, filename);
        }

        /*
        [TestCase("Bank.result")]
        [TestCase("Boat.result")]
        [TestCase("Bondiana.result")]
        [TestCase("Build.result")]
        [TestCase("Casino.result")]
        [TestCase("Commando.result")]
        [TestCase("Diamond.result")]
        [TestCase("Diehard.result")]
        [TestCase("Energy.result")]
        [TestCase("Examen.result")]
        [TestCase("Fishing.result")]
        [TestCase("Galaxy.result")]
        [TestCase("Gladiator.result")]
        [TestCase("Gobsaur.result")]
        [TestCase("Hachball.result")]
        [TestCase("Ikebana.result")]
        [TestCase("Menzols.result")]
        [TestCase("Murder.result")]
        [TestCase("Newflora.result")]
        [TestCase("Penetrator.result")]
        [TestCase("Poroda.result")]
        [TestCase("Prison.result")]
        [TestCase("Rush.result")]
        [TestCase("Siege.result")]
        [TestCase("Spy.result")]
        [TestCase("Tomb.result")]
        public void TestOther(string filename)
        {
            var basepath = Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            basepath += "/../../../Data/Other/";
            basepath = Path.GetFullPath(basepath);
            TestQuest(basepath, filename);
        }
        [TestCase("Prison3.9.4.result")]
        [TestCase("Prison3.9.4(1).result")]
        [TestCase("Prison3.9.4(2).result")]
        [TestCase("Prison3.9.4(3).result")]
        [TestCase("Prison3.9.4(4).result")]
        [TestCase("Prison3.9.4(5).result")]
        [TestCase("Prison3.9.4(6).result")]
        [TestCase("Prison3.9.4(7).result")]
        [TestCase("Prison3.9.4(8).result")]
        [TestCase("Prison3.9.4(9).result")]
        [TestCase("Prison3.9.4(10).result")]
        [TestCase("Prison3.9.4(11).result")]
        [TestCase("Prison3.9.4(12).result")]
        [TestCase("Prison3.9.4(13).result")]
        [TestCase("Prison3.9.4(14).result")]
        [TestCase("Prison3.9.4(15).result")]
        [TestCase("Prison3.9.4(16).result")]
        [TestCase("Prison3.9.4(17).result")]
        [TestCase("Prison3.9.4(18).result")]
        [TestCase("Prison3.9.4(19).result")]
        [TestCase("Prison3.9.4(20).result")]
        public void TestQuest(string filename)
        {
            var basepath = Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            basepath += "/../../../Data/";
            basepath = Path.GetFullPath(basepath);
            TestQuest(basepath, filename);
        }
        */
        public void TestQuest(string basepath, string filename)
        {
            int stepCounter = 0;
            filename = Path.Combine(basepath, filename);
            
            

            using (var sr = new StreamReader(filename))
            {
                var reader = new JsonTextReader(sr);
                var jObject = JObject.Load(reader);

                var questFilename = jObject.GetValue("Quest").Value<string>();
                
                var userName = jObject.GetValue("UserName").Value<string>();
                
                var randomSeqJson = jObject.GetValue("Random").Value<JArray>();

                QuestRandom.Clear();
                foreach (JObject item in randomSeqJson)
                {
                    var name = item.Properties().First().Name;
                    int value = (int)item.Properties().First().Value;
                    var max = (int)item.Properties().Last().Value;
                    QuestRandom.AddSeq(name, value, max);
                }
                QuestRandom.FinishSeq();
                
                var questSteps = jObject.GetValue("Steps").Value<JArray>();

                questFilename = Path.Combine(basepath, questFilename);
                
                var q = new Quest(questFilename);
                var player = new QuestPlayer(q);

                int stepcount = 0;
                foreach (JObject step in questSteps)
                {
                    stepcount++;

                    string description = "";
                    try
                    {
                        description = step.GetValue("Description").Value<string>();
                        description = description.Replace("\r", "");
                    }
                    catch { }
                    var EndPathMessage = "";
                    try
                    {
                        EndPathMessage = step.GetValue("EndPathMessage").Value<string>();
                        EndPathMessage = EndPathMessage.Replace("\r", "");
                    }
                    catch { }

                    var dayspassed = step.GetValue("dayspassed").Value<int>();
                    var CustomCriticalMessage = step.GetValue("CustomCriticalMessage").Value<string>();
                    CustomCriticalMessage = CustomCriticalMessage.Replace("\r", "");
                    var CurrentCriticalParameter = step.GetValue("CurrentCriticalParameter").Value<int>();

                    var CriticalMessage = "";
                    try
                    {
                        CriticalMessage = step.GetValue("CriticalMessage").Value<string>();
                        CriticalMessage = CriticalMessage.Replace("\r", "");
                    }
                    catch { }
                    

                    var StrPars = step.GetValue("StrPars").Value<JArray>();
                    var Pars = step.GetValue("Pars").Value<JArray>();
                    var ParVisState = step.GetValue("ParVisState").Value<JArray>();
                    var PathesWeCanGo = step.GetValue("PathesWeCanGo").Value<JArray>();
                    var Answers = step.GetValue("Answers").Value<JArray>();
                    var RandomCount = step.GetValue("RandomCount").Value<int>();
                    // ---------------------------------------------
                    int i, j;

                    if (CurrentCriticalParameter > 0)
                        CurrentCriticalParameter--;
                    if (player.failFlag || player.successFlag)
                    {
                        Assert.IsTrue(CriticalMessage != "");
                        Assert.AreEqual(CriticalMessage.Trim(), player.quest.Pars[CurrentCriticalParameter].CriticalMessage);
                        return;
                    }

                    var trans = player.PossibleTransitions();

                    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    string s = player.CurrentLocation().LocationDescription;
                    s = s.Replace("\r", "");
                    description = description.Replace("\r", "");
                    Assert.AreEqual(description, s, string.Format("Invalid description (step: {0})", stepcount));
                    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


                    // Pars -------------------------------------------------
                    i = 0;
                    Assert.AreEqual(Pars.Count, player.Pars.Length);
                    foreach (JValue item in Pars)
                    {
                        int tmp1 = item.ToObject<int>();
                        int tmp2 = player.Pars[i];
                        Assert.AreEqual(tmp1, tmp2, string.Format("Invalid Pars[{1}] (step: {0})", stepcount, i));
                        i++;
                    }
                    
                    Assert.AreEqual(dayspassed, player.daysPassed, string.Format("Invalid dayspassed (step: {0})", stepcount));
                    Assert.AreEqual(CustomCriticalMessage, player.CustomCriticalMessage, string.Format("Invalid CustomCriticalMessage (step: {0})", stepcount));

                    Assert.AreEqual(CurrentCriticalParameter, player.CurrentCriticalParameter, string.Format("Invalid CurrentCriticalParameter(step: {0})", stepcount));
                    
                    /**/
                    // ParVisState -------------------------------------------------
                    i = 0;
                    Assert.AreEqual(ParVisState.Count, player.ParVisState.Length, string.Format("Invalid ParVisState.Count(step: {0})", stepcount));
                    foreach (JValue item in ParVisState)
                    {
                        bool tmp1 = item.ToObject<bool>();
                        bool tmp2 = player.ParVisState[i];
                        Assert.AreEqual(tmp1, tmp2, string.Format("Invalid ParVisState [{0}](step: {1})", i, stepcount));
                        i++;
                    }
                    // StrPars -------------------------------------------------
                    i = 0;
                    foreach (JValue item in StrPars)
                    {
                        var itemStr = item.ToString();
                        Assert.AreEqual(itemStr, player.ShowParameters(i), string.Format("Invalid StrPars[{0}] (step: {1})", i, stepcount));
                        i++;
                    }
                    /**/

                    if (Answers.Count != trans.Count)
                    {
                        Console.WriteLine("Expected:");
                        foreach (JObject item in Answers)
                        {
                            int index = item.GetValue("Index").Value<int>();
                            string value = item.GetValue("Value").Value<string>();
                            int number = item.GetValue("Number").Value<int>();
                            Console.WriteLine(string.Format("i: {0}, v:{1}, n:{2}", index, value, number));
                        }
                        Console.WriteLine("Actualy:");
                        foreach (var item in trans)
                        {
                            Console.WriteLine(trans);
                        }
                    }
                    Assert.AreEqual(Answers.Count, trans.Count, string.Format("Invalid Answers.Count (step: {0})", stepcount));
                    i = 0;
                    foreach (JObject item in Answers)
                    {
                        int index1 = item.GetValue("Index").Value<int>();
                        string value1 = item.GetValue("Value").Value<string>();
                        int number1 = item.GetValue("Number").Value<int>();
                        
                        int index2 = i + 1;
                        string value2 = trans[i].StartPathMessage;
                        int number2 = trans[i].PathIndx + 1;

                        value1 = value1.Replace("\r", "");
                        value2 = value2.Replace("\r", "");
                        value2 = player.quest.ProcessString(value2, player.Pars);
                        Assert.AreEqual(index1, index2, string.Format("Invalid Answer index[{0}] (step: {1})", i, stepcount));
                        Assert.AreEqual(number1, number2, string.Format("Invalid Answer number[{0}] (step: {1})", i, stepcount));
                        Assert.AreEqual(value1, value2, string.Format("Invalid Answer value[{0}] (step: {1})", i, stepcount));
                        
                        i++;
                    }
                    // PathesWeCanGo --------------------------------
                    i = 0;
                    j = 0;

                    Assert.AreEqual(PathesWeCanGo.Count, player.quest.LocationsValue, string.Format("Invalid PathesWeCanGo.Locations count (step: {0})", stepcount));
                    foreach (JArray items in PathesWeCanGo)
                    {
                        Assert.AreEqual(items.Count, player.quest.PathesValue, string.Format("Invalid PathesWeCanGo.Paths count (step: {0})", stepcount));
                        j = 0;
                        foreach (JValue item in items)
                        {

                            int tmp1 = item.ToObject<int>();
                            int tmp2 = player.PathesWeCanGo[i, j];

                            Assert.AreEqual(tmp1, tmp2, string.Format("Invalid PathesWeCanGo[{0}, {1}] (step: {0})", i, j, stepcount));
                            j++;
                        }
                        i++;
                    }

                    // ---------------------------------

                    JObject Answer = null;
                    try
                    {
                        Answer = step.GetValue("Answer").Value<JObject>();
                    }
                    catch (Exception e)
                    {
                        
                    }

                    // -------------------------------

                    if (QuestRandom.RamdomCallCount() != RandomCount)
                    {
                        for (var ki = QuestRandom.RamdomCallCount() + 1; ki <= RandomCount; ++ki)
                        {
                            try
                            {
                                Console.Write(ki + " ");
                                QuestRandom.DebugPrint(ki);
                            }
                            catch { }
                        }
                        Console.WriteLine();
                    }

                    Assert.AreEqual(RandomCount, QuestRandom.RamdomCallCount(), string.Format("Invalid Ramdom call count (step: {0})", stepcount));

                    if (Answer != null)
                    {
                        int step_index = Answer.GetValue("Index").Value<int>();
                        string step_string = Answer.GetValue("Value").Value<string>();
                        step_string = step_string.Replace("\r", "");
                        QuestPath qp = trans[step_index - 1];
                        EndPathMessage = player.quest.ProcessString(EndPathMessage, player.Pars);
                        string ss = player.quest.ProcessString(qp.EndPathMessage, player.Pars);
                        Assert.AreEqual(EndPathMessage, ss, string.Format("Invalid EndPathMessage (step: {0})", stepcount));
                        ss = player.quest.ProcessString(qp.StartPathMessage, player.Pars);
                        Assert.AreEqual(step_string, ss, string.Format("Invalid StartPathMessage (step: {0})", stepcount));

                        Console.WriteLine("#{1} Step done: {0}", qp, ++stepCounter);

                        player.DoTransition(qp);
                    }

                    
                }
            }



            //var q = new Quest(filename);

            //var player = new QuestPlayer(q, "211112113114213113113212121142145111311421211");

        }
    }
}
