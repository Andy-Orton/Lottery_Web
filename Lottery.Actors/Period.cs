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
    public class Period : ReceiveActor
    {
        public ILoggingAdapter Log { get; } = Context.GetLogger();

        public Period()
        {
            Become(Initializing);
        }

        private void Initializing()
        {
            Log.Info("Initializing Phase");
            Receive<SupervisorPeriodMessage>(msg =>
            {
                var props = new RoundRobinPool(msg.NumberOfVendors).Props(Props.Create<Vendor>());
                Context.ActorOf(props, ActorTypes.VendorRoundRobin);
                var TicketListProps = Props.Create<TicketListActor>();
                Context.ActorOf(TicketListProps, ActorTypes.TicketListActor);
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
            });

        }

        private void SalesClosed()
        {
            Log.Info("Becoming Closed");
            Receive<TicketBoughtMessage>(msg =>
            {
                Sender.Tell(new BadTicketRequest { Message = "Ticket sales have ended" });
            });
        }
    }
}
