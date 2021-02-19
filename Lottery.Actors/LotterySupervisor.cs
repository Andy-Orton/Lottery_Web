using Akka.Actor;
using Akka.Event;
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
                Context.ActorOf(Props.Create(() => new Period()), "PeriodActor");
                Log.Info("Period Actor has been created");
                Context.ActorOf(Props.Create(() => new UserGenerator()), "UserGenerator");
                Log.Info("User Generator Actor has been created");


                Context.Child("UserGenerator").Tell(new SupervisorUserGeneratorMessage() { NumberOfTickets = msg.NumberOfTickets, NumberOfUsers = msg.NumberOfUsers });
                Context.Child("PeriodActor").Tell(new SupervisorPeriodMessage() { NumberOfVendors = msg.NumberOfVendors });
                Become(PeriodOpen);
            });

            Receive<PeriodCompleteMessage>(msg =>
            {
                var topTen = Context.Child("Stats").Ask(new TopTenWinnersMessage());
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
                Context.Child("PeriodActor").Tell(new SupervisorSalesClosedMessage() { });
            });

            Receive<EndPeriodMessage>(msg =>
            {
                Context.Child("PeriodActor").Tell(new SupervisorSalesClosedMessage { });
                Become(PeriodClosed);
            });
        }

        private void CheckForNextPeriodPhase()
        {
            if (ReadyToMoveToNextPhase)
            {
                Context.Child("PeriodActor").Tell(new SupervisorSalesOpenMessage { });
                Context.ActorSelection("UserGenerator/*").Tell(new LotterySalesOpen());
            }
            else
            {
                ReadyToMoveToNextPhase = true;
            }
        }
    }
}
