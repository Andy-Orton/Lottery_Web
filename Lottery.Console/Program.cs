using Akka.Actor;
using Lottery.Actors;
using Lottery.Actors.Messages;
using System;

namespace Lottery.ConsoleRunner
{
    class Program
    {

        private static ActorSystem LotteryActorSystem;

        static void Main(string[] args)
        {
            LotteryActorSystem = ActorSystem.Create("LotteryActorSystem");
            Props lotterySupervisorProps = Props.Create<LotterySupervisor>();
            IActorRef lotterySupervisor = LotteryActorSystem.ActorOf(lotterySupervisorProps, "LotterySupervisor");


            lotterySupervisor.Tell(new BeginPeriodMessage() {MinTickets=25, MaxTickets=50, NumberOfUsers=100, NumberOfVendors=150});
            Console.WriteLine("Ticket sales have begun, press enter to end period");
            Console.ReadLine();
            lotterySupervisor.Tell(new SupervisorSalesClosedMessage() { });
            Console.ReadLine();
            LotteryActorSystem.Terminate();
        }
    }
}
