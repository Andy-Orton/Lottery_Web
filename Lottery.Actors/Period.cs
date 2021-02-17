﻿using Akka.Actor;
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
                    for(int i = 0; i < supPM.NumberOfVendors; i++)
                    {
                        Context.ActorOf(Props.Create(() => new Vendor()), "Vendor" + i);
                    }
                    Sender.Tell(new VendorGenerationCompleteMessage { CreatedVendors = Context.GetChildren().Count() });
                    break;
            }
        }
    }
}
