using AdventOfCode2023.Utilities;

namespace AdventOfCode2023.Problems
{
    internal partial class Day07Problem : Problem
    {
        protected override string InputName => "Actual";

        public override object PartOne()
        {
            var input = GetInputStringList().Select(x => x.Split(" "));

            var hands = input
                .Select(x => new Hand(x[0], int.Parse(x[1])))
                .OrderBy(x => x)
                .ToList();

            var totalWinnings = 0;

            for (int i = 1; i < hands.Count + 1; i++)
            {
                var hand = hands[i - 1];

                totalWinnings += i * hand.Bid;
            }

            return totalWinnings;
        }

        public override object PartTwo()
        {
            var input = GetInputStringList().Select(x => x.Split(" "));

            var hands = input
                .Select(x => new JokerHand(x[0], int.Parse(x[1])))
                .OrderBy(x => x)
                .ToList();

            var totalWinnings = 0;

            for (int i = 1; i < hands.Count + 1; i++)
            {
                var hand = hands[i - 1];

                totalWinnings += i * hand.Bid;
            }

            return totalWinnings;
        }
    }
}
