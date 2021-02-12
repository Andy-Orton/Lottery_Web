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
            Init();
            Props lotterySupervisorProps = Props.Create<LotterySupervisor>();
            Props periodProps = Props.Create<Period>();
            IActorRef lotterySupervisor = LotteryActorSystem.ActorOf(lotterySupervisorProps, "LotterySupervisor");
            IActorRef period = LotteryActorSystem.ActorOf(periodProps, "Period");
            lotterySupervisor.Tell(new SupervisorUserGeneratorMessage() {tickets=300, users=200});
            period.Tell(new SupervisorPeriodMessage() { vendors = 20 });
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
