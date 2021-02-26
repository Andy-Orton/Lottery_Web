using Akka.Actor;
using ClassLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Actors
{
    public class TicketScorerActor : ReceiveActor
    {
        public LotteryTicket WinningTicket { get; private set; }
        public decimal GrandPrizeAmount { get; set; } = 300;

        public TicketScorerActor()
        {
            Receive<TicketListMessage>(msg =>
            {
                GrandPrizeAmount = msg.lotteryTickets.Count * 2;
                WinningTicket = msg.WinningLotteryTicket;
                
                
                foreach(var ticket in msg.lotteryTickets)
                {
                    CheckWinningTicket(ticket);
                }

                Sender.Tell(new TicketListMessage(msg.lotteryTickets, msg.WinningLotteryTicket));
            });
        }

        public int NumberMatchingWhiteBalls(LotteryTicket lt)
        {
            int numMatches = 0;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (lt.balls[i] == WinningTicket.balls[j])
                    {
                        numMatches++;
                    }
                }
            }
            return numMatches;
        }
        public void CheckWinningTicket(LotteryTicket lt)
        {
            int whiteMatches = NumberMatchingWhiteBalls(lt);
            if (lt.powerBall == WinningTicket.powerBall && whiteMatches == 5)
            {
                lt.winLevel = 1;
                lt.winAmtDollars = GrandPrizeAmount;
            }
            else if (whiteMatches == 5)
            {
                lt.winLevel = 2;
                lt.winAmtDollars = 1000000;//$1M
            }
            else if (lt.powerBall == WinningTicket.powerBall && whiteMatches == 4)
            {
                lt.winLevel = 3;
                lt.winAmtDollars = 50000;//$50k
            }
            else if (whiteMatches == 4)
            {
                lt.winLevel = 4;
                lt.winAmtDollars = 100;//$100
            }
            else if (lt.powerBall == WinningTicket.powerBall && whiteMatches == 3)
            {
                lt.winLevel = 5;
                lt.winAmtDollars = 100;//$100
            }
            else if (whiteMatches == 3)
            {
                lt.winLevel = 6;
                lt.winAmtDollars = 7;//$7
            }
            else if (lt.powerBall == WinningTicket.powerBall && whiteMatches == 2)
            {
                lt.winLevel = 7;
                lt.winAmtDollars = 7;//$7
            }
            else if (lt.powerBall == WinningTicket.powerBall && whiteMatches == 1)
            {
                lt.winLevel = 8;
                lt.winAmtDollars = 4;//$4
            }
            else if (lt.powerBall == WinningTicket.powerBall && whiteMatches == 0)
            {
                lt.winLevel = 9;
                lt.winAmtDollars = 4;//$4
            }
            else
            {
                lt.winLevel = 0;
                lt.winAmtDollars = 0;
            }

            lt.isGraded = true;

        }

    }




}
