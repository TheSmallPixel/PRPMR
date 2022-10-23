// See https://aka.ms/new-console-template for more information

using PatternFromRandom;

Console.WriteLine("Hello, World!");

object syncObj = new object();
var random = new Random();
var wheel = new int[] { 20, 1, 3, 1, 5, 1, 3, 1, 10, 1, 3, 1, 5, 1, 5, 3, 1, 10, 1, 3, 1, 5, 1, 3, 1 };
var numbers = new int[] { 1, 3, 5, 10 };
var payout = new Dictionary<int, int>()
{
    {1,2},
    {3,4},
    {5,6},
    {10,12},
    {20,25}
};
var Tasks = new List<Task>();

Combination();

var marginalSym = new MarginalSymulator(random);

var t3 = Task.Run(() => marginalSym.Run(ReadInput(), Bet, betInit: 1354, retryisCount: 4, betNumber: 1, betFailMultiply: 2));




Task.WaitAll(marginalSym.RunPlots(), t3);





async IAsyncEnumerator<int> ReadInput()
{
    string line;

    while (null != (line = Console.ReadLine()))
        yield return int.Parse(line);
}
async IAsyncEnumerator<int> ReadInputRandom()
{

    Console.WriteLine("Press ESC to stop");

    while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape))
    {
        int num = 0;
        lock (syncObj)
        {
            num = wheel[random.Next(0, wheel.Length)];
        }

        yield return num;
    }
}
async IAsyncEnumerator<int> ReadInputRandomLimit(int p)
{

    Console.WriteLine("Press ESC to stop");

    for (int i = 0; i < p; i++)
    {
        int num = 0;
        lock (syncObj)
        {
            num = wheel[random.Next(0, wheel.Length)];
        }

        yield return num;
        await Task.Delay(400);
    }
}


void Combination()
{
    foreach (var numer in numbers)
    {
        float probability = (float)wheel.Where(x => x == numer).Count() / (float)wheel.Length;
        Console.WriteLine($"Probability for: {numer} is {probability}");
    }
}

async Task<(int, bool)> Bet(int betNumber, int amount, IAsyncEnumerator<int> input)
{
    Console.WriteLine($"[BOT] bet on {betNumber} this {amount}");
    Console.Write("Number: ");
    if (await input.MoveNextAsync())
    {
        Console.WriteLine($"{input.Current}");
        return ((betNumber == input.Current) ? (amount * payout[input.Current])-amount : -amount, false);
    }
    else
    {
        Console.WriteLine("Ended");
    }
    return (0, true);
}




