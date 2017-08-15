using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SharpQuest
{
    public class Quest
    {
        public const int maxparameters = 24; 
        
        public const int maxlocations=400;
        public const int maxpathes=1200;
        
        public QuestParameter[] Pars = new QuestParameter[maxparameters];
        QuestFileVersion version;

        public int LocationsValue;
        public int PathesValue;

        public string RToStar;
        public string RParsec;
        public string RArtefact;
        public string RToPlanet;
        public string RDate;
        public string RMoney;
        public string RFromPLanet;
        public string RFromStar;
        public string RRanger;
                
        public string QuestSuccessGovMessage;
        public string QuestDescription;
        public string QuestTargetName;

        public List<QuestLocation> Locations = new List<QuestLocation>();
        public List<QuestPath> Paths = new List<QuestPath>();

        Dictionary<int, int> codeToIndx = new Dictionary<int, int>();
        
        public int startLocationIndx = -1;
        
        public override string ToString()
        {
            return string.Format("[Quest] version = {0} | desc = {2} | pars = [{1}] \n locs = {3} \n paths = {4}", 
                version, QuestUtils.ArrayToString(Pars), QuestDescription, QuestUtils.ArrayToString(Locations), QuestUtils.ArrayToString(Paths));
        }

        public int LocationCodeToIndx(int code)
        {
            return codeToIndx[code];
        }
        
        public Quest(string filename)
        {
            using (var fs = new FileStream(filename, FileMode.Open))
            {
                version = QuestStreamReader.ReadInt(fs);
                
                if (version.isOlderThan3_8_5())
                {
                    var race = version;
                    version = QuestFileVersion.FileVersion_Older_than_3_8_5;
                }
                else
                {
                    var race = QuestStreamReader.ReadInt(fs);
                }
                
                if (version.isSince3_9_6())
                {
                    var set = QuestStreamReader.ReadSetInt(fs); // SRace:TOwnerSet;   TOwnerSet = set of TOwner;   TOwner=(Maloc,Peleng,People,Fei,Gaal,Kling,None);// владелец чего-либо раса, клинг, Нет
                }
                else 
                {
                    //SRace:=RaceToSRace(Race);
                }
                
                
                if (version.isSince3_8_5())
                {
                    var needNotToReturn = QuestStreamReader.ReadBool(fs);
                }


                var targetRace = QuestStreamReader.ReadInt(fs);
                
                if (version.isSince3_9_6()) 
                {
                    var set = QuestStreamReader.ReadSetInt(fs);    // SRace:TOwnerSet; 
                }
                else 
                {
                    //STargetRace:=RaceToTOwnerSet(targetRace);
                }


                var rangerStatus = QuestStreamReader.ReadInt(fs);
                
                if (version.isSince4_0_0()) 
                {
                    var set = QuestStreamReader.ReadSetInt(fs);// TStatusSet = set of TStatus; TStatus = (Trader,Pirate,Warrior); //Статус рейнджера
                }
                else 
                {
                    //SRangerStatus:=RangerStatusToTStatusSet(RangerStatus);
                }


                var rangerRace = QuestStreamReader.ReadInt(fs);
                
                if (version.isSince4_0_0()) 
                {
                    var set = QuestStreamReader.ReadSetInt(fs);    // SRangerRace:TOwnerSet;
                }
                else 
                {
                    //SRangerRace:=RangerRaceToTOwnerSet(RangerRace);  
                }

                var planetReaction = QuestStreamReader.ReadInt(fs);  // Отношение после выполнения квеста // -1 - хуже, 0 - также, 1-лучше
                
                var xRes = QuestStreamReader.ReadInt(fs);
                var yRes = QuestStreamReader.ReadInt(fs);
                
                var xGrad = QuestStreamReader.ReadInt(fs);    // 15
                var yGrad = QuestStreamReader.ReadInt(fs);    // 12
                
                var artifactSize = QuestStreamReader.ReadInt(fs);

                if (version.isSince4_0_0())
                {
                    var defPathGoTimesValue = QuestStreamReader.ReadInt(fs); //проходимость перехода по умолчанию
                }

                if (version.isSince4_0_1())
                {
                    var difficulty = QuestStreamReader.ReadInt(fs);
                }

                Pars = new QuestParameter[maxparameters];

                
                if (version.isSince4_0_1())
                {
                    for (var i = 0; i < maxparameters; ++i)
                        Pars[i].Load_4_0_1(fs);
                }
                else if (version.isSince3_9_6())
                {
                    for (var i = 0; i < maxparameters; ++i)
                        Pars[i].Load_3_9_6(fs);
                    
                    //if (FileVersion_Current.isSince4_0_1()) ?? 
                    {
                        //for (var i = 0; i < maxparameters; ++i)
                        //    Pars[i].AltDiapStartValues.Add(Pars[i].value);
                    }
                }
                else if (version.isSince3_9_5())
                {
                    for (var i = 0; i < 12; ++i)
                    {
                        Pars[i].Load_3_9_5(fs);
                    }
                }
                else if (version.isSince3_9_2())
                {
                    for (var i = 0; i < 12; ++i)
                    {
                        Pars[i].Load_3_9_0(fs);
                    }
                }
                else if (version.isSince3_9_0())
                {
                    for (var i = 0; i < 9; ++i)
                    {
                        Pars[i].Load_3_9_0(fs);
                    }
                }
                else
                {
                    for (var i = 0; i < 9; ++i)
                    {
                        Pars[i].Load(fs);
                    }
                }

                RToStar = QuestStreamReader.ReadTextField(fs);
                RParsec = QuestStreamReader.ReadTextField(fs);
                RArtefact = QuestStreamReader.ReadTextField(fs);
                RToPlanet = QuestStreamReader.ReadTextField(fs);
                RDate = QuestStreamReader.ReadTextField(fs);
                RMoney = QuestStreamReader.ReadTextField(fs);
                RFromPLanet = QuestStreamReader.ReadTextField(fs);
                RFromStar = QuestStreamReader.ReadTextField(fs);
                RRanger = QuestStreamReader.ReadTextField(fs);
                
                LocationsValue = QuestStreamReader.ReadInt(fs);
                PathesValue = QuestStreamReader.ReadInt(fs);
                
                QuestSuccessGovMessage = QuestStreamReader.ReadTextField(fs);
                QuestDescription = QuestStreamReader.ReadTextField(fs);
                QuestTargetName = QuestStreamReader.ReadTextField(fs);

                Locations.Capacity = LocationsValue;
                Paths.Capacity = PathesValue;

                for (var i = 0; i < LocationsValue; ++i)
                {
                    Locations.Add(new QuestLocation());
                    
                    if (version.isSince4_0_1())
                        Locations[i].Load_4_0_1(fs, xRes, yRes);
                    else if (version.isSince3_9_6())
                        Locations[i].Load_3_9_6(fs, xRes, yRes);
                    else if (version.isSince3_9_4())
                        Locations[i].Load_3_9_4(fs, xRes, yRes);
                    else if (version.isSince3_9_3())
                        Locations[i].Load_3_9_3(fs);
                    else if (version.isSince3_9_2())
                        Locations[i].Load_3_9_2(fs);
                    else
                        Locations[i].Load(fs);

                    if (Locations[i].StartLocationFlag)
                    {
                        if (startLocationIndx != -1)
                            throw new Exception("Several start locations found");
                        
                        startLocationIndx = i;
                    }

                    codeToIndx[Locations[i].LocationNumber] = i;
                }
                
                if (startLocationIndx == -1)
                    throw new Exception("No start locations found");

                for (var i = 0; i < PathesValue; ++i)
                {
                    Paths.Add(new QuestPath());
                    
                    if (version.isSince4_0_2())
                        Paths[i].Load_4_0_2(fs, xRes, yRes);
                    else if (version.isSince3_9_6())
                        Paths[i].Load_3_9_6(fs, xRes, yRes);
                    else if (version.isSince3_9_4())
                        Paths[i].Load_3_9_4(fs, xRes, yRes);
                    else if (version.isSince3_9_3())
                        Paths[i].Load_3_9_3(fs);
                    else if (version.isSince3_9_2())
                        Paths[i].Load_3_9_2(fs);
                    else if (version.isSince3_9_1())
                        Paths[i].Load_3_9_1(fs);
                    else if (version.isSince3_8_5())
                        Paths[i].Load_3_8_5(fs);
                    else
                        Paths[i].Load(fs);

                    Paths[i].PathIndx = i;
                    Paths[i].FromLocationNumber = Paths[i].FromLocation;
                    Paths[i].ToLocationNumber = Paths[i].ToLocation;
                    Paths[i].FromLocation = LocationCodeToIndx( Paths[i].FromLocation );
                    Paths[i].ToLocation = LocationCodeToIndx( Paths[i].ToLocation );

                    var fromIndx = Paths[i].FromLocation;
                    if (fromIndx < 0) throw new Exception("from < 0. == " + fromIndx);
                    if (fromIndx >= Locations.Count()) throw new Exception("from >= count (" + Locations.Count() + "). == " + fromIndx);
                    
                    Locations[ fromIndx ].transitions.Add(Paths[i]);
                }

            }
        
             
        }
    }
}