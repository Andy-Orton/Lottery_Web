using Akka.Actor;
using Akka.Event;
using ClassLib;
using Lottery.Actors.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lottery.Actors
{
    public class LotterySupervisor : ReceiveActor
    {
        public ILoggingAdapter Log { get; } = Context.GetLogger();
        public bool ReadyToMoveToNextPhase { get; set; }

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
                Context.ActorOf(Props.Create(() => new PeriodActor()), ActorTypes.PeriodActor);
                Log.Info("Period Actor has been created");
                Context.ActorOf(Props.Create(() => new UserActorGenerator()), ActorTypes.UserGenerator);
                Log.Info("User Generator Actor has been created");

                Context.Child(ActorTypes.UserGenerator).Tell(new SupervisorUserGeneratorMessage() { MinTickets = msg.MinTickets, MaxTickets = msg.MaxTickets, NumberOfUsers = msg.NumberOfUsers });
                Context.Child(ActorTypes.PeriodActor).Tell(new SupervisorPeriodMessage() { NumberOfVendors = msg.NumberOfVendors });
                Become(PeriodOpen);
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
                Context.Child(ActorTypes.PeriodActor).Tell(new EndPeriodMessage() { });
            });

            Receive<SupervisorSalesClosedMessage>(msg =>
            {
                Context.Child(ActorTypes.PeriodActor).Tell(new EndPeriodMessage { });
                Become(PeriodClosed);
            });
        }

        private void CheckForNextPeriodPhase()
        {
            if (ReadyToMoveToNextPhase)
            {
                Context.Child(ActorTypes.PeriodActor).Tell(new SupervisorSalesOpenMessage { });
                Context.ActorSelection(ActorTypes.AllUsers).Tell(new LotterySalesOpen());
            }
            else
            {
                ReadyToMoveToNextPhase = true;
            }
        }
    }
}
