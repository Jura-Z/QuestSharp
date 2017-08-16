using System;
using System.Collections.Generic;
using System.IO;

namespace SharpQuest
{
    public class QuestLocation
    {
        const int MaxLocationDescriptions = 10;

//        M:PM;
        public int screenx;

        public int screeny; // Координаты локации на экране

        public int DaysCost; // сколько дне прошло

        public int LocationNumber; //Номер локации
        public string LocationName; //Краткое описание локации
        public string LocationDescription; //Длинное описание локации

        public string[] LocationDescriptions; // MaxLocationDescriptions size

        public bool RandomShowLocationDescriptions;
        public int LocDescrOrder;
        public string LocDescrExprOrder;
        public bool VoidLocation;

        public bool ClosedLocationFlag;
        public bool NoWay2LocationFlag;

        public QuestParameterDelta[] DPars; // maxparameters size

        public bool PlayerDeath;

        public bool Money;

        public bool StartLocationFlag; //Флаг, говорящий о том, что локация является ствртовой
        public bool EndLocationFlag; //Флаг, говорящий о том, что локация является конечной

        public bool FailLocationFlag; //Флаг, говорящий о том, что локация является провальной


        public List<QuestPath> transitions = new List<QuestPath>();


        public override string ToString()
        {
            return string.Format("<#{0} > {1}. {2} {3}>", LocationNumber, LocationName, LocationDescription,
                QuestUtils.ArrayToString(LocationDescriptions));
        }

        public void Clear()
        {
            screenx = 100;
            screeny = 100;
            VoidLocation = false;
            DaysCost = 0;

            DPars = new QuestParameterDelta[Quest.maxparameters];
            LocationDescriptions = new string[MaxLocationDescriptions];

            for (var i = 0; i < Quest.maxparameters; ++i)
            {
                DPars[i] = new QuestParameterDelta();
                DPars[i].Clear();
            }

            RandomShowLocationDescriptions = false;
            LocDescrOrder = 1;
            LocDescrExprOrder = "";
            FailLocationFlag = false;
            ClosedLocationFlag = false;
            NoWay2LocationFlag = false;
            PlayerDeath = false;
            LocationNumber = 0;

            LocationName = "(M.Par_Get('LocationClassNewLocationName'))";
            LocationDescription = "(M.Par_Get('LocationClassNewLocationText'))";

            StartLocationFlag = false;
            EndLocationFlag = false;
        }


        public void Load_4_0_1(Stream fs, int xScreenResolution, int YScreenResolution)
        {
            Clear();
            DaysCost = QuestStreamReader.ReadInt(fs);
            screenx = QuestStreamReader.ReadInt(fs);
            screeny = QuestStreamReader.ReadInt(fs);
            LocationNumber = QuestStreamReader.ReadInt(fs);

            StartLocationFlag = QuestStreamReader.ReadBool(fs);
            EndLocationFlag = QuestStreamReader.ReadBool(fs);
            FailLocationFlag = QuestStreamReader.ReadBool(fs);
            PlayerDeath = QuestStreamReader.ReadBool(fs);
            VoidLocation = QuestStreamReader.ReadBool(fs);

            for (var i = 0; i < Quest.maxparameters; ++i) DPars[i].Load_3_9_6(fs);

            for (var i = 0; i < MaxLocationDescriptions; ++i)
            {
                var desc = QuestStreamReader.ReadTextField(fs);
                LocationDescriptions[i] = desc;
            }

            RandomShowLocationDescriptions = QuestStreamReader.ReadBool(fs);
            LocDescrOrder = QuestStreamReader.ReadInt(fs);

            LocationName = QuestStreamReader.ReadTextField(fs);
            LocationDescription = QuestStreamReader.ReadTextField(fs);
            LocDescrExprOrder = QuestStreamReader.ReadTextField(fs);

            LocationDescription = "";
        }


        public void Load_3_9_6(Stream fs, int xScreenResolution, int YScreenResolution)
        {
            Clear();

            DaysCost = QuestStreamReader.ReadInt(fs);
            screenx = QuestStreamReader.ReadInt(fs);
            screeny = QuestStreamReader.ReadInt(fs);
            LocationNumber = QuestStreamReader.ReadInt(fs);

            StartLocationFlag = QuestStreamReader.ReadBool(fs);
            EndLocationFlag = QuestStreamReader.ReadBool(fs);
            FailLocationFlag = QuestStreamReader.ReadBool(fs);
            PlayerDeath = QuestStreamReader.ReadBool(fs);
            VoidLocation = QuestStreamReader.ReadBool(fs);

            for (var i = 0; i < Quest.maxparameters; ++i) DPars[i].Load_3_9_6(fs);

            for (var i = 0; i < MaxLocationDescriptions; ++i)
            {
                var desc = QuestStreamReader.ReadTextField(fs);
                LocationDescriptions[i] = desc;
            }

            RandomShowLocationDescriptions = QuestStreamReader.ReadBool(fs);
            LocDescrOrder = QuestStreamReader.ReadInt(fs);

            LocationName = QuestStreamReader.ReadTextField(fs);
            LocationDescription = QuestStreamReader.ReadTextField(fs);
            LocationDescription = "";
        }

        public void Load_3_9_4(Stream fs, int xScreenResolution, int YScreenResolution)
        {
            Clear();

            DaysCost = QuestStreamReader.ReadInt(fs);
            screenx = QuestStreamReader.ReadInt(fs);
            screeny = QuestStreamReader.ReadInt(fs);
            LocationNumber = QuestStreamReader.ReadInt(fs);

            StartLocationFlag = QuestStreamReader.ReadBool(fs);
            EndLocationFlag = QuestStreamReader.ReadBool(fs);
            FailLocationFlag = QuestStreamReader.ReadBool(fs);
            PlayerDeath = QuestStreamReader.ReadBool(fs);

            for (var i = 0; i < 12; ++i)
            {
                DPars[i].Load_3_9_4(fs);
            }

            for (var i = 0; i < MaxLocationDescriptions; ++i)
            {
                var desc = QuestStreamReader.ReadTextField(fs);
                LocationDescriptions[i] = desc;
            }

            RandomShowLocationDescriptions = QuestStreamReader.ReadBool(fs);
            LocDescrOrder = QuestStreamReader.ReadInt(fs);

            LocationName = QuestStreamReader.ReadTextField(fs);
            LocationDescription = QuestStreamReader.ReadTextField(fs);
            LocationDescription = "";
        }

        public void Load_3_9_3(Stream fs)
        {
            Clear();

            DaysCost = QuestStreamReader.ReadInt(fs);

            screenx = QuestStreamReader.ReadInt(fs);
            screeny = QuestStreamReader.ReadInt(fs);
            LocationNumber = QuestStreamReader.ReadInt(fs);

            StartLocationFlag = QuestStreamReader.ReadBool(fs);
            EndLocationFlag = QuestStreamReader.ReadBool(fs);
            FailLocationFlag = QuestStreamReader.ReadBool(fs);
            PlayerDeath = QuestStreamReader.ReadBool(fs);

            for (var i = 0; i < 12; ++i) DPars[i].Load_3_9_3(fs);

            for (var i = 0; i < MaxLocationDescriptions; ++i)
            {
                var desc = QuestStreamReader.ReadTextField(fs);
                LocationDescriptions[i] = desc;
            }

            RandomShowLocationDescriptions = QuestStreamReader.ReadBool(fs);
            LocDescrOrder = QuestStreamReader.ReadInt(fs);

            LocationName = QuestStreamReader.ReadTextField(fs);
            LocationDescription = QuestStreamReader.ReadTextField(fs);
            LocationDescription = "";
        }

        public void Load_3_9_2(Stream fs)
        {
            Clear();

            screenx = QuestStreamReader.ReadInt(fs);
            screeny = QuestStreamReader.ReadInt(fs);
            LocationNumber = QuestStreamReader.ReadInt(fs);

            StartLocationFlag = QuestStreamReader.ReadBool(fs);
            EndLocationFlag = QuestStreamReader.ReadBool(fs);
            FailLocationFlag = QuestStreamReader.ReadBool(fs);
            PlayerDeath = QuestStreamReader.ReadBool(fs);

            for (var i = 0; i < 12; ++i) DPars[i].Load(fs);

            LocationName = QuestStreamReader.ReadTextField(fs);
            LocationDescription = QuestStreamReader.ReadTextField(fs);
            LocationDescriptions[0] = LocationDescription.Trim();
            LocationDescription = "";
        }

        public void Load(Stream fs)
        {
            Clear();

            screenx = QuestStreamReader.ReadInt(fs);
            screeny = QuestStreamReader.ReadInt(fs);
            LocationNumber = QuestStreamReader.ReadInt(fs);

            StartLocationFlag = QuestStreamReader.ReadBool(fs);
            EndLocationFlag = QuestStreamReader.ReadBool(fs);
            FailLocationFlag = QuestStreamReader.ReadBool(fs);
            PlayerDeath = QuestStreamReader.ReadBool(fs);

            for (var i = 0; i < 9; ++i) DPars[i].Load(fs);

            LocationName = QuestStreamReader.ReadTextField(fs);
            LocationDescription = QuestStreamReader.ReadTextField(fs);
            LocationDescriptions[0] = LocationDescription.Trim();
        }


        // Changes state of the Location
        public string FindLocationDescription(int[] playerPars)
        {
            var found = false;
            var text = "";
            if (RandomShowLocationDescriptions)
            {
                throw new Exception("not implemented");
//                var flag = true;
//                parse = TCalcParse.Create;
//                parse.AssignAndPreprocess(LocDescrExprOrder.Text, 1);
//                if (parse.error or parse.default_expression ) flag = false;
//
//                if (flag)
//                {
//                    parse.Parse(CalcParseClass.TParValues(pars));
//                    if (parse.calc_error) flag = false;
//                }
//
//                if (flag)
//                {
//                    if ((parse.answer > 10)or(parse.answer < 1) ) flag = false;
//                }
//
//                if (flag)
//                {
//                    if (trim(LocationDescriptions[parse.answer].Text) = "") flag = false;
//                }
//
//                if (flag)
//                {
//                    text = trim(LocationDescriptions[parse.answer].Text);
//                }
//
//
//                if (!flag)
//                {
//                    c = 0;
//                    while (!found)
//                    {
//                        i = Random(10) + 1;
//                        text = trim(LocationDescriptions[i].Text);
//                        if (text != "")
//                        {
//                            found = true;
//                            LocationDescription.Text = text;
//                        }
//                        else inc(c);
//                        if (c > MaxLocationDescriptions * 2)
//                        {
//                            text = "";
//                            found = true;
//                        }
//                    }
//                }
            }
            else
            {
                var i = LocDescrOrder;
                var c = 0;
                while (!found)
                {
                    text = LocationDescriptions[i - 1].Trim();
                    if (string.IsNullOrEmpty(text) == false)
                    {
                        found = true;
                        LocationDescription = text;
                        LocDescrOrder = i + 1;
                    }
                    else 
                        ++c;

                    ++i;
                    if (i > MaxLocationDescriptions) i = 1;
                    if (c > MaxLocationDescriptions)
                    {
                        text = "";
                        found = true;
                        LocDescrOrder = i;
                    }
                }
            }

            LocationDescription = Quest.ProcessString(text);
            return LocationDescription;
        }
    }
}