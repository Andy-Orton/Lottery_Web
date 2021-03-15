using Akka.Actor;
using Akka.Actor.Setup;
using Akka.Configuration;
using Akka.Routing;
using ClassLib;
using Lottery.Actors;
using Lottery.Actors.Messages;
using Npgsql;
using Serilog;
using System;
using System.Linq;

namespace Lottery.ConsoleRunner
{
    //https://github.com/petabridge/lighthouse
    //using this as a seed node
    class Program
    {
        private const string hocon = @"
                akka{
                    loggers = [""Akka.Logger.Serilog.SerilogLogger, Akka.Logger.Serilog""]
                    actor{
                        serialize-messages = off
                        deployment {
                            /LotterySupervisor/PeriodActor/VendorRoundRobin {
                                router = round-robin-pool
                                nr-of-instances = 20
                                cluster {
                                    enabled = on
                                    max-nr-of-instances-per-node = 20
                                    use-role = lottery
                                }
                            }
                        }
                    }
                    actor.provider = ""Akka.Cluster.ClusterActorRefProvider, Akka.Cluster""
                    remote {
                        log-remote-lifecycle-events = DEBUG
                        dot-netty.tcp {
                            port = {{port}}
                            hostname = {{hostname}}
                        }
                    }
                    cluster {
                        seed-nodes = [""akka.tcp://webcrawler@localhost:4053""]
                        roles = [lottery]
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

            //TestDatabase();

            Serilog.Log.Logger = logger;
            var port = args.Length == 1 ? args[0] : "0";

            var config = hocon
                .Replace("{{connection_string}}", Environment.GetEnvironmentVariable("CONNECTION_STRING"))
                .Replace("{{port}}", port)
                .Replace("{{hostname}}", "144.17.24.19");
            var ConfigBootstrap = BootstrapSetup.Create().WithConfig(config);
            var ActorSystemSettings = ActorSystemSetup.Create(ConfigBootstrap);
            LotteryActorSystem = ActorSystem.Create(Constants.ActorSystemName, ActorSystemSettings);
            LotteryActorSystem.ActorOf(Props.Create(typeof(SimpleClusterListener)), "clusterListener");
            Props lotterySupervisorProps = Props.Create<LotterySupervisor>();
            IActorRef lotterySupervisor = LotteryActorSystem.ActorOf(lotterySupervisorProps, "LotterySupervisor");

            lotterySupervisor.Tell(new BeginPeriodMessage() { MinTickets = 5, MaxTickets = 10, NumberOfUsers = 20, NumberOfVendors = 150 });
            Console.WriteLine("Ticket sales have begun, press enter to end period");
            Console.ReadLine();
            lotterySupervisor.Tell(new SupervisorSalesClosedMessage() { });

            Console.ReadLine();
            LotteryActorSystem.Terminate();

            ////Akka.Logger.Serilog.SerilogLogger
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
