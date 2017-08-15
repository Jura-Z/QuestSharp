using System;
using System.Collections.Generic;
using System.Text;

namespace SharpQuest
{
    internal class Program
    {

        public static void Main(string[] args)
        {
            //var filename = "/Users/Iurii/Desktop/Prison3.9.4.qm";
            var filename = "/Users/Iurii/Desktop/Prison4.0.2.qm";
            var filename2 = "/Users/Iurii/Projects/mine/_Rangers/tge2/Quests/Game/Fishing.qm";
            var q = new Quest(filename);

            var player = new QuestPlayer(q, "21111111311421111211321111211211311");

            

         
        }

        public static void Main2(string[] args)
        {
            var filename = "/Users/Iurii/Projects/mine/_Rangers/TGE/Prison.qm";
            var filename2 = "/Users/Iurii/Projects/mine/_Rangers/tge2/Quests/Game/Fishing.qm";
            var q = new Quest(filename);

            Console.WriteLine(q);
            var player = new QuestPlayer(q);
            
            Console.Clear();

            for (;;)
            {

                var loc = player.CurrentLocation();
                Console.WriteLine(loc);

                Console.WriteLine();
                Console.WriteLine("===");                
                player.ShowParameters();
                Console.WriteLine("===");
                
                Console.WriteLine();

                if (loc.EndLocationFlag)
                {
                    Console.WriteLine("EndLocationFlag");
                    return;
                }

                if (loc.FailLocationFlag)
                {
                    Console.WriteLine("FailLocationFlag");
                    return;
                }
                
                if (loc.PlayerDeath)
                {
                    Console.WriteLine("PlayerDeath");
                    return;
                }
                
                var trans = player.PossibleTransitions();
                for (var i = 0; i < trans.Count; ++i)
                {
                    Console.WriteLine("{0})\t{1}", i + 1, trans[i].StartPathMessage);
                }

                for (;;)
                {
                    Console.Write(">");
                    var answer = Console.ReadLine();
                    
                    var indx = -1;
                    if (int.TryParse(answer, out indx) == false) continue;
                    indx--;
                    if (indx < 0 || indx >= trans.Count) continue;

                    Console.WriteLine();
                    Console.WriteLine(trans[indx].EndPathMessage);

                    player.DoTransition(trans[indx]);

                    break;
                }
            }
        }
    }
}