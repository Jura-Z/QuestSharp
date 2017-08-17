using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpQuest;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace QuestUnitTest
{
    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void TestMethod1()
        {
            //var basepath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var basepath = Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            
            var filename = basepath + "/../../../Data/Prison3.9.4.result";
            filename = Path.GetFullPath(filename);

            using (var sr = new StreamReader(filename))
            {
                var reader = new JsonTextReader(sr);
                var jObject = JObject.Load(reader);

                var questFilename = jObject.GetValue("Quest").Value<string>();
                
                var userName = jObject.GetValue("UserName").Value<string>();
                
                var randomSeqJson = jObject.GetValue("Random").Value<JArray>();
                var randomSeq = randomSeqJson.Select(jv => (int)jv).ToArray();
                QuestRandom.SetSeq(ref randomSeq);
                QuestRandom.useArrayBased = true; //????????????????????????????????????
                var questSteps = jObject.GetValue("Steps").Value<JArray>();

                
                questFilename = basepath + "/../../../Data/" + questFilename;
                var q = new Quest(questFilename);
                var player = new QuestPlayer(q);

                int stepcount = 0;
                foreach (JObject step in questSteps)
                {
                    stepcount++;

                    var description = step.GetValue("Description").Value<string>();

                    var EndPathMessage = "";
                    try
                    {
                        EndPathMessage = step.GetValue("EndPathMessage").Value<string>();
                    }
                    catch { }

                    var dayspassed = step.GetValue("dayspassed").Value<int>();
                    var CustomCriticalMessage = step.GetValue("CustomCriticalMessage").Value<string>();
                    var CurrentCriticalParameter = step.GetValue("CurrentCriticalParameter").Value<int>();

                    var StrPars = step.GetValue("StrPars").Value<JArray>();
                    var Pars = step.GetValue("Pars").Value<JArray>();
                    var ParVisState = step.GetValue("ParVisState").Value<JArray>();
                    var PathesWeCanGo = step.GetValue("PathesWeCanGo").Value<JArray>();
                    var Answers = step.GetValue("Answers").Value<JArray>();

                    // ---------------------------------------------
                    int i, j;

                    // Pars -------------------------------------------------
                    i = 0;
                    Assert.AreEqual(Pars.Count, player.Pars.Length);
                    foreach (JValue item in Pars)
                    {
                        int tmp1 = item.ToObject<int>();
                        int tmp2 = player.Pars[i];
                        Assert.AreEqual(tmp1, tmp2, string.Format("Invalid Pars (step: {0})", stepcount));
                        i++;
                    }
                    
                    string s = player.CurrentLocation().LocationDescription;
                    s = s.Replace("\r", "");
                    description = description.Replace("\r", "");
                    Assert.AreEqual(description, s , string.Format("Invalid description (step: {0})", stepcount));
                    
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
                        string tmp = item.ToString();
                        Assert.AreEqual(tmp, player.ShowParameters(i), string.Format("Invalid StrPars[{0}] (step: {1})", i, stepcount));
                        i++;
                    }
                    /**/

                    // Answers --------------------------------
                    var trans = player.PossibleTransitions();
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

                        Assert.AreEqual(index1, index2, string.Format("Invalid Answer index[{0}] (step: {1})", i, stepcount));
                        Assert.AreEqual(value1, value2, string.Format("Invalid Answer value[{0}] (step: {1})", i, stepcount));
                        //Assert.AreEqual(number1, number2, string.Format("Invalid Answer number[{0}] (step: {1})", i, stepcount));
                        
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

                    if (Answer != null)
                    {
                        int step_index = Answer.GetValue("Index").Value<int>();
                        string step_string = Answer.GetValue("Value").Value<string>();

                        QuestPath qp = trans[step_index - 1];
                        Assert.AreEqual(EndPathMessage, qp.EndPathMessage, string.Format("Invalid EndPathMessage (step: {0})", stepcount));
                        Assert.AreEqual(step_string, qp.StartPathMessage, string.Format("Invalid StartPathMessage (step: {0})", stepcount));

                        Console.WriteLine("Step done: {0}", qp);

                        player.DoTransition(qp);
                    }
                }
            }



            //var q = new Quest(filename);

            //var player = new QuestPlayer(q, "211112113114213113113212121142145111311421211");

        }
    }
}
