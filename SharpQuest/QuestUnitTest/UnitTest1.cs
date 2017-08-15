using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpQuest;
using System.IO;

namespace QuestUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var filename = "../../../Data/TestPrison3.9.4.json";
            using (var sr = new StreamReader(filename))
            {
                var reader = new JsonTextReader(sr);
                var jObject = JObject.Load(reader);

                var questFilename = jObject.GetValue("Quest").Value<string>();
                var questSteps = jObject.GetValue("Steps").Value<JArray>();

                questFilename = "../../../Data/" + questFilename;
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
                }
                /*

                // JObject can be cast into a dynamic
                var dObject = (dynamic)jObject;
                someValue = (string)dObject.someProperty;
                */
            }



            //var q = new Quest(filename);

            //var player = new QuestPlayer(q, "211112113114213113113212121142145111311421211");

        }
    }
}
