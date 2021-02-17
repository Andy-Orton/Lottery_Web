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

        public ActorTests() : base()
        {

        }
        private static ActorSystem LotteryActorSystem;

        //[SetUp]
        //private void SetUp()
        //{
        //    LotteryActorSystem = ActorSystem.Create("Lottery");
            
        //}

        [Test]
        public void SupervisorPeriodTest()
        {
            var msg = new BeginPeriodMessage() { NumberOfTickets = 200, NumberOfUsers = 10, NumberOfVendors = 20 };
            var ugMsg = new SupervisorUserGeneratorMessage() { NumberOfTickets = 200, NumberOfUsers = 10 };
            var userGenerator = ActorOfAsTestActorRef(() => new UserGenerator(), TestActor);

            userGenerator.Tell(ugMsg);

            var result = ExpectMsg<int>(TimeSpan.FromSeconds(3)) == 10;
            Assert.IsTrue(result);
        }


    }
}
