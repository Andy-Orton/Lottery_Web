using Akka.Actor;
using Akka.Actor.Setup;
using Akka.Configuration;
using Lottery.Actors;
using Lottery.Actors.Messages;
using Npgsql;
using Serilog;
using System;

namespace Lottery.ConsoleRunner
{
    class Program
    {
        private const string hocon = @"
                akka{
                    loggers = [""Akka.Logger.Serilog.SerilogLogger, Akka.Logger.Serilog""]
                    actor{
                        serialize-messages = off
                    }
                    akka.persistence{
                        journal{
                            plugin = ""akka.persistence.journal.postgresql""
                            postgresql{
                                plugin-dispatcher = ""akka.actor.default.dispatcher""
                                class = ""Akka.Persistence.Postgresql.Journal.PostgreSqlJournal, Akka.Persistence.PostgreSql""
                                connection-string = ""{{connection_string}}""
                                auto-initialize = on
                                schema-name = public
                                table-name = event_journal
                                metadata-table-name = metadata
                                stored-as = BYTEA
                                timestamp-provider = ""Akka.Persistence.Sql.Common.Journal.DefaultTimestampProvider, Akka.Persistence.Sql.Common""
                            }    
                        }
                    snapshot-store {
                            plugin = ""akka.persistence.snapshot-store.postgresql""
                            postgresql{
                                class = ""Akka.Persistence.PostgreSql.Snapshot.PostgreSqlSnapshotStore, Akka.Persistence.PostgreSql""
                                plugin-dispatcher = ""akka.actor.default-dispatcher""
                                connection-string = ""{{connection_string}}""
                                schema-name = public
                                table-name = snapshot-store
                                auto-initialize = true
                                stored-as = BYTEA
                            }
                        }
                    }
                }
            ";
        private static ActorSystem LotteryActorSystem;

        static void Main(string[] args)
        {
            var logger = new LoggerConfiguration()
                .WriteTo.File("log.txt", buffered: true, flushToDiskInterval: TimeSpan.FromSeconds(2))
                .MinimumLevel.Information()
                .CreateLogger();

            TestDatabase();

            Serilog.Log.Logger = logger;
            var config = hocon.Replace("{{connection_string}}", Environment.GetEnvironmentVariable("CONNECTION_STRING"));
            var ConfigBootstrap = BootstrapSetup.Create().WithConfig(config);
            var ActorSystemSettings = ActorSystemSetup.Create(ConfigBootstrap);
            LotteryActorSystem = ActorSystem.Create("LotteryActorSystem", ActorSystemSettings);
            Props lotterySupervisorProps = Props.Create<LotterySupervisor>();
            IActorRef lotterySupervisor = LotteryActorSystem.ActorOf(lotterySupervisorProps, "LotterySupervisor");

            lotterySupervisor.Tell(new BeginPeriodMessage() {MinTickets=5, MaxTickets=10, NumberOfUsers=20, NumberOfVendors=150});
            Console.WriteLine("Ticket sales have begun, press enter to end period");
            Console.ReadLine();
            lotterySupervisor.Tell(new SupervisorSalesClosedMessage() { });

            Console.ReadLine();
            LotteryActorSystem.Terminate();

            //Akka.Logger.Serilog.SerilogLogger
        }

        private static void TestDatabase()
        {
            try
            {
                using var conn = new NpgsqlConnection(Environment.GetEnvironmentVariable("CONNECTION_STRING"));
                conn.Open();
            } catch(Exception e)
            {

            }
        }
    }
}
