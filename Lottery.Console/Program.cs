using Akka.Actor;
using Lottery.Actors;
using Lottery.Actors.Messages;


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

            Props periodProps = Props.Create<Period>();
            IActorRef period = LotteryActorSystem.ActorOf(periodProps, "Period");

            lotterySupervisor.Tell(new BeginPeriodMessage() {NumberOfTickets=300, NumberOfUsers=200, NumberOfVendors=20});
            period.Tell(new SupervisorPeriodMessage() { NumberOfVendors = 20 });
            LotteryActorSystem.Terminate();
        }

        public static void Init()
        {
            //using (var system = ActorSystem.Create("lottery-system"))
            //{
            //    // Create top level supervisor
            //    var supervisor = system.ActorOf(LotterySupervisor.Props(), "lottery-supervisor");
            //    // Exit the system after ENTER is pressed
            //    Console.ReadLine();
            //}
        }
    }
}
