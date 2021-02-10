using Akka.Actor;
using Lottery.Actors;
using System;

namespace Lottery.ConsoleRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Init();
        }

        public static void Init()
        {
            using (var system = ActorSystem.Create("lottery-system"))
            {
                // Create top level supervisor
                var supervisor = system.ActorOf(LotterySupervisor.Props(), "lottery-supervisor");
                // Exit the system after ENTER is pressed
                Console.ReadLine();
            }
        }
    }
}
