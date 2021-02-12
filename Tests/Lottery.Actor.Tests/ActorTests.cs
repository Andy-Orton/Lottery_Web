using Akka.Actor;
using Akka.TestKit.NUnit3;
using Lottery.Actors;
using Lottery.Actors.Messages;
using NUnit.Framework;
using System;

namespace Tests.Lottery.Actor.Tests
{
    public class ActorTests : TestKit
    {
        private static ActorSystem LotteryActorSystem;

        [SetUp]
        private void SetUp()
        {
            LotteryActorSystem = ActorSystem.Create("Lottery");
            
        }

        [Test]
        public void SupervisorPeriodTest()
        {
            var subject = Sys.ActorOf<LotterySupervisor>();

            var prob = CreateTestProbe();

            var msg = new SupervisorPeriodMessage { vendors = 300 };

            subject.Tell(msg, prob);

            ExpectMsg(msg, TimeSpan.FromSeconds(5));
        }

    }
}
