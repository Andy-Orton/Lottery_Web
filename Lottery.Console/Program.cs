using Akka.Actor;
using Akka.Actor.Setup;
using Akka.Configuration;
using Lottery.Actors;
using Lottery.Actors.Messages;
using Serilog;
using System;

namespace Lottery.ConsoleRunner
{
    class Program
    {

        private static ActorSystem LotteryActorSystem;

        static void Main(string[] args)
        {
            var logger = new LoggerConfiguration()
                .WriteTo.File("log.txt", buffered: true, flushToDiskInterval: TimeSpan.FromSeconds(2))
                .MinimumLevel.Information()
                .CreateLogger();

            Serilog.Log.Logger = logger;
            LotteryActorSystem = ActorSystem.Create("LotteryActorSystem", ActorSystemSettings);
            Props lotterySupervisorProps = Props.Create<LotterySupervisor>();
            IActorRef lotterySupervisor = LotteryActorSystem.ActorOf(lotterySupervisorProps, "LotterySupervisor");

            lotterySupervisor.Tell(new BeginPeriodMessage() {MinTickets=1000, MaxTickets=2000, NumberOfUsers=1000, NumberOfVendors=150});
            Console.WriteLine("Ticket sales have begun, press enter to end period");
            Console.ReadLine();
            lotterySupervisor.Tell(new SupervisorSalesClosedMessage() { });

            Console.ReadLine();
            LotteryActorSystem.Terminate();

            //Akka.Logger.Serilog.SerilogLogger
        }


        //https://github.com/akkadotnet/Akka.Persistence.PostgreSql
        public static readonly BootstrapSetup Bootstrap = BootstrapSetup.Create().WithConfig(
            ConfigurationFactory.ParseString(@"
                akka{
                    loggers = [""Akka.Logger.Serilog.SerilogLogger, Akka.Logger.Serilog""]
                    actor{
                        serialize-messages = off
                    }
                    akka.persistence{
                        journal{
                            plugin = ""akka.persistence.journal.postgresql""
                            postgresql{
                                class = ""Akka.Persistence.Postgresql.Journal.PostgreSqlJournal, Akka.Persistence.PostgreSql""
                                connection-string = """"
                            }    
                        }
                    }
                }
            "));

        // Merges the SerializationSetup and BootstrapSetup together into a unified ActorSystemSetup class
        public static readonly ActorSystemSetup ActorSystemSettings = ActorSystemSetup.Create(Bootstrap);
    }
}
