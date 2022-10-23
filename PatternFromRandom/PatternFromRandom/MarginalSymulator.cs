using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternFromRandom
{
    public class MarginalSymulator : SymStats
    {
        public MarginalSymulator(Random random) : base("MarginalSystem", random)
        {
        }

        public async Task Run(IAsyncEnumerator<int> numbers, Func<int, int, IAsyncEnumerator<int>, Task<(int, bool)>> placeBet, int betInit, int retryisCount, int betNumber, int betFailMultiply)
        {
            int wallet = betInit;

            int lastbet = 0;
            int failSequence = 0;
            if (retryisCount < 1)
                throw new Exception("The retrys have to be at least 1");

            int retryFactorial = Enumerable.Range(1, retryisCount).Aggregate(1, (p, item) => p * item);
            int step = wallet / retryFactorial;

            wallet = betInit;
            while (true)
            {
                if (failSequence >= retryisCount)
                    failSequence = 0;
                int currentBet = (failSequence == 0) ? step : (int)step * (failSequence+1);
                //wallet -= currentBet;
                //if (failSequence > 0 && failSequence <= retryisCount)
                //{
                //    currentBet = lastbet * betFailMultiply;
                //}
                //else
                //{
                //    failSequence = 0;
                //}

                //if (currentBet > wallet || currentBet == 0)
                //{

                //}
                //cap the amount
                //TODO: remove cap
                //if (currentBet > 2000)
                //    currentBet = 2000;

                if (currentBet <= 1)
                    break;
                

                var (won, ended) = await placeBet(betNumber, currentBet, numbers);
                if (ended)
                    break;
                if (won > 0)
                {
                    step = wallet / retryFactorial;
                }
                failSequence = (won >= 0) ? 0 : failSequence + 1;
                wallet += won;

                RegisterBet(currentBet, won, wallet, failSequence);

                lastbet = currentBet;

            }
        }

    }
}
