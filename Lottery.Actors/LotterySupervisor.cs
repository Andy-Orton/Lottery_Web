﻿using Akka.Actor;
using Akka.Event;
using Lottery.Actors.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lottery.Actors
{
    public class LotterySupervisor : UntypedActor
    {
        public ILoggingAdapter Log { get; } = Context.GetLogger();

        protected override void PreStart() => Log.Info("Lottery Application started");
        protected override void PostStop() => Log.Info("Lottery Application stopped");

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case BeginPeriodMessage sup:

                    Context.ActorOf(Props.Create(() => new Period()), "PeriodActor");
                    Log.Info("Period Actor has been created");
                    Context.ActorOf(Props.Create(() => new UserGenerator()), "UserGenerator");
                    Log.Info("User Generator Actor has been created");


                    Context.Child("PeriodActor").Tell(new SupervisorPeriodMessage() { NumberOfVendors = sup.NumberOfVendors });
                    Context.Child("UserGenerator").Tell(new SupervisorUserGeneratorMessage() { NumberOfTickets = sup.NumberOfTickets, NumberOfUsers = sup.NumberOfUsers });
                    break;
                default:
                    Log.Info("Got Message that I didn't know how to do anything with");
                    break;
            }
            
        }
       
    }
}
