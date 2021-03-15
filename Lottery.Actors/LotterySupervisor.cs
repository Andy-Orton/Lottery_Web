using Akka.Actor;
using Akka.Event;
using ClassLib;
using Lottery.Actors.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Lottery.Actors
{
    public class LotterySupervisor : ReceiveActor
    {
        public ILoggingAdapter Log { get; } = Context.GetLogger();
        public bool ReadyToMoveToNextPhase { get; set; }
        private Stopwatch stopwatch = new Stopwatch();

        protected override void PreStart() => Log.Info("Lottery Application started");
        protected override void PostStop() => Log.Info("Lottery Application stopped");

        public LotterySupervisor()
        {
            Become(PeriodClosed);
        }

        private void PeriodClosed()
        {
            Receive<BeginPeriodMessage>(msg =>
            {
                stopwatch.Start();
                Context.ActorOf(Props.Create(() => new PeriodActor()), Constants.PeriodActor);
                Log.Info("Period Actor has been created");
                Context.ActorOf(Props.Create(() => new UserActorGenerator()), Constants.UserGenerator);
                Log.Info("User Generator Actor has been created");

                Context.Child(Constants.UserGenerator).Tell(new SupervisorUserGeneratorMessage() { MinTickets = msg.MinTickets, MaxTickets = msg.MaxTickets, NumberOfUsers = msg.NumberOfUsers });
                Context.Child(Constants.PeriodActor).Tell(new InitializeNewPeriodMessage() { NumberOfVendors = msg.NumberOfVendors });
                Become(PeriodOpen);
            });

            Receive<AllTicketsScoredMessage>(msg =>
            {
                stopwatch.Stop();

                Console.WriteLine(stopwatch.Elapsed);
            });
        }

        private void PeriodOpen()
        {
            Receive<UserGenerationCompleteMessage>(msg =>
            {
                Log.Info($"{msg.CreatedChildren} children created from User Generator");
                CheckForNextPeriodPhase();
            });

            Receive<VendorGenerationCompleteMessage>(msg => {
                Log.Info($"{msg.CreatedVendors} vendors created from Period");
                CheckForNextPeriodPhase();
            });

            Receive<UserGeneratorUsersCompleteMessage>(msg =>
            {
                Log.Info("Users finished buying tickets");
                Context.Child(Constants.PeriodActor).Tell(new EndPeriodMessage() { });
            });

            Receive<SupervisorSalesClosedMessage>(msg =>
            {
                EndPeriod();
            });

            Receive<AllUserTicketPurchasesCompleteMessage>(msg =>
            {
                EndPeriod();
            });
        }

        private void EndPeriod()
        {
            Context.Child(Constants.PeriodActor).Tell(new EndPeriodMessage { });
            Become(PeriodClosed);
        }

        private void CheckForNextPeriodPhase()
        {
            if (ReadyToMoveToNextPhase)
            {
                Context.Child(Constants.PeriodActor).Tell(new SupervisorSalesOpenMessage { });
                Context.ActorSelection(Constants.AllUsers).Tell(new LotterySalesOpen());
            }
            else
            {
                ReadyToMoveToNextPhase = true;
            }
        }
    }
}
