using Akka.Actor;
using Akka.Event;
using Akka.Routing;
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
                Context.ActorOf(props, "VendorRoundRobin");
                Sender.Tell(new VendorGenerationCompleteMessage { CreatedVendors = Context.GetChildren().Count() });
            });

            Receive<SupervisorSalesOpenMessage>(msg =>
            {
                Become(SalesOpen);
            });

            Receive<TicketBoughtMessage>(msg =>
            {
                Sender.Tell(new BadTicketRequest() { });
            });
        }

        private void SalesOpen()
        {
            Log.Info("Becoming Open");
            Receive<TicketBoughtMessage>(msg =>
            {
                Context.Child("VendorRoundRobin").Tell(msg);
            });

            Receive<SupervisorSalesClosedMessage>(msg =>
            {
                Become(SalesClosed);
            });

        }

        private void SalesClosed()
        {
            Log.Info("Becoming Closed");
        }
    }
}
