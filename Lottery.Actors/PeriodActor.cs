using Akka.Actor;
using Akka.Event;
using Akka.Routing;
using ClassLib;
using Lottery.Actors.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Actors
{
    public class PeriodActor : ReceiveActor
    {
        public ILoggingAdapter Log { get; } = Context.GetLogger();

        public PeriodActor()
        {
            Become(Initializing);
        }

        private void Initializing()
        {
            Log.Info("Initializing Phase");
            Receive<InitializeNewPeriodMessage>(msg =>
            {
                Context.ActorOf(new RoundRobinPool(msg.NumberOfVendors).Props(Props.Create<VendorActor>()), ActorTypes.VendorRoundRobin);
                Context.ActorOf(Props.Create<TicketListActor>(), ActorTypes.TicketListActor);
                Context.ActorOf(Props.Create<LotteryStatisticsActor>(), ActorTypes.StatsActor);
                Sender.Tell(new VendorGenerationCompleteMessage { CreatedVendors = Context.GetChildren().Count() });
            });

            Receive<SupervisorSalesOpenMessage>(msg =>
            {
                Become(SalesOpen);
            });

            Receive<TicketBoughtMessage>(msg =>
            {
                Sender.Tell(new BadTicketRequest() {Message = "Ticket sales have not yet begun"});
            });
        }

        private void SalesOpen()
        {
            Log.Info("Becoming Open");
            Receive<BuyTicketMessage>(msg =>
            {
                Context.Child(ActorTypes.VendorRoundRobin).Forward(msg);
            });

            Receive<EndPeriodMessage>(msg =>
            {
                Become(SalesClosed);
                Context.Child(ActorTypes.StatsActor).Tell(new GenerateStatisticsMessage());
            });

        }

        private void SalesClosed()
        {
            Log.Info("Becoming Closed");
            Receive<TicketBoughtMessage>(msg =>
            {
                Sender.Tell(new BadTicketRequest { Message = "Ticket sales have ended" });
            });

            Receive<AllTicketsScoredMessage>(msg =>
            {
                Context.Parent.Forward(msg);
            });

        }
    }
    public record GenerateStatisticsMessage;
}
