using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpQuest;
using System.IO;
using NUnit.Framework;

namespace QuestUnitTest
{
    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void TestMethod1()
        {
            var filename = "/Users/Iurii/Projects/mine/_Rangers/SharpQuest/SharpQuest/Data/TestPrison3.9.4.json";
            using (var sr = new StreamReader(filename))
            {
                var reader = new JsonTextReader(sr);
                var jObject = JObject.Load(reader);

                var questFilename = jObject.GetValue("Quest").Value<string>();
                var questSteps = jObject.GetValue("Steps").Value<JArray>();

                questFilename = "/Users/Iurii/Projects/mine/_Rangers/SharpQuest/SharpQuest/Data/" + questFilename;
                var q = new Quest(questFilename);
                var player = new QuestPlayer(q);

                foreach (JObject step in questSteps)
                {
                    var description = step.GetValue("Description").Value<string>();
                    var dayspassed = step.GetValue("dayspassed").Value<int>();
                    var CustomCriticalMessage = step.GetValue("CustomCriticalMessage").Value<string>();
                    var CurrentCriticalParameter = step.GetValue("CurrentCriticalParameter").Value<int>();

                    var StrPars = step.GetValue("StrPars").Value<JArray>();
                    var Pars = step.GetValue("Pars").Value<JArray>();
                    var ParVisState = step.GetValue("ParVisState").Value<JArray>();
                    var PathesWeCanGo = step.GetValue("PathesWeCanGo").Value<JArray>();
                    var Answers = step.GetValue("Answers").Value<JArray>();

                    var Answer = step.GetValue("Answer").Value<JObject>();

                    // ---------------------------------------------

                    Assert.AreEqual(description, player.CurrentLocation().LocationDescription, "Invalid description");
                    Assert.AreEqual(dayspassed, player.daysPassed, "Invalid dayspassed");
                    //Assert.AreEqual(CustomCriticalMessage, );
                    //Assert.AreEqual(CurrentCriticalParameter, );
                    int i;

                    // Pars -------------------------------------------------
                    i = 0;
                    Assert.AreEqual(Pars.Count, player.Pars.Length);
                    foreach (JValue item in Pars)
                    {
                        int tmp1 = item.ToObject<int>();
                        int tmp2 = player.Pars[i];
                        Assert.AreEqual(tmp1, tmp2);
                        i++;
                    }
                    // ParVisState -------------------------------------------------
                    i = 0;
                    Assert.AreEqual(ParVisState.Count, player.ParVisState.Length);
                    foreach (JValue item in ParVisState)
                    {
                        bool tmp1 = item.ToObject<bool>();
                        bool tmp2 = player.ParVisState[i];
                        Assert.AreEqual(tmp1, tmp2);
                        i++;
                    }
                    // StrPars -------------------------------------------------
                    i = 0;
                    foreach (JValue item in StrPars)
                    {
                        string tmp = item.ToString();
                        Assert.AreEqual(tmp, player.ShowParameters(i));
                        i++;
                    }
                }
            }



            //var q = new Quest(filename);

            //var player = new QuestPlayer(q, "211112113114213113113212121142145111311421211");

        }
    }
}
