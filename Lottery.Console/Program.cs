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


            lotterySupervisor.Tell(new BeginPeriodMessage() {NumberOfTickets=300, NumberOfUsers=200, NumberOfVendors=20});
            Console.ReadLine();
        }
    }
}
