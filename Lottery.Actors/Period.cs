using Akka.Actor;
using Akka.Event;
using Lottery.Actors.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Actors
{
    public class Period : UntypedActor
    {
        public ILoggingAdapter Log { get; } = Context.GetLogger();

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case SupervisorPeriodMessage supPM:
                    Log.Info($"Starting period with {supPM.NumberOfVendors} vendors.");
                    break;
            }
        }
    }
}
