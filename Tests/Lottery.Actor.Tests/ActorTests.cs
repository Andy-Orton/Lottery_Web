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

        //[SetUp]
        //private void SetUp()
        //{
        //    LotteryActorSystem = ActorSystem.Create("Lottery");
            
        //}

        [TestCase(100)]
        [TestCase(50)]
        [TestCase(30)]
        [TestCase(20)]
        [TestCase(10)]
        public void SupervisorUserGeneratorTest(int users)
        {
            var ugMsg = new SupervisorUserGeneratorMessage() { NumberOfTickets = 200, NumberOfUsers = users };
            var userGenerator = ActorOfAsTestActorRef(() => new UserActorGenerator(), TestActor);

            userGenerator.Tell(ugMsg);

            Assert.AreEqual(users, ExpectMsg<UserGenerationCompleteMessage>().CreatedChildren);
        }


    }
}
