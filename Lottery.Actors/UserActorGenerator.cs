﻿using Akka.Actor;
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
    public class UserActorGenerator : ReceiveActor
    {
        private Random random = new Random();
        public ILoggingAdapter Log { get; } = Context.GetLogger();

        public UserActorGenerator()
        {
            Receive<SupervisorUserGeneratorMessage>(msg =>
            {
                for (int i = 0; i < msg.NumberOfUsers; i++)
                {
                    Context.ActorOf(Props.Create(() => new UserActor(random.Next(msg.MinTickets, msg.MaxTickets))), "User" + i);
                }
                Sender.Tell(new UserGenerationCompleteMessage() { CreatedChildren = Context.GetChildren().Count() });
            });
        }
    }
}
