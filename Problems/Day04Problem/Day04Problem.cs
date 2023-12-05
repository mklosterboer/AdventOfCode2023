using AdventOfCode2023.Utilities;
using System.Text.RegularExpressions;

namespace AdventOfCode2023.Problems
{
    internal partial class Day04Problem : Problem
    {
        protected override string InputName => "Actual";

        private IEnumerable<string> Input { get; init; }

        public Day04Problem()
        {
            Input = GetInputStringList();
        }

        public override object PartOne()
        {
            return Input
                .Select(x => new Card(x))
                .Sum(x => x.Points);
        }

        public override object PartTwo()
        {
            var cards = Input
                .Select(x => new Card(x))
                .ToDictionary(x => x.Number);

            for (var i = 1; i < cards.Count; i++)
            {
                var card = cards[i];

                foreach (var instance in card.CardsWon)
                {
                    cards[instance].AddInstances(1 * card.InstanceCount);
                }
            }

            return cards.Values.Sum(x => x.InstanceCount);
        }

        private partial class Card
        {
            public int Number { get; init; }

            private int NumWinningNumbers { get; init; }

            public int InstanceCount { get; private set; }

            [GeneratedRegex(@"\d+")]
            private static partial Regex NumberRegex();

            public Card(string input)
            {
                InstanceCount = 1;

                var splitString = input.Split(':');

                Number = int.Parse(NumberRegex().Match(splitString[0]).Value);

                var numberString = splitString[1].Split("|");

                var winningNumbers = NumberRegex()
                    .Matches(numberString[0])
                    .Select(match => int.Parse(match.Value))
                    .ToHashSet();

                var myNumbers = NumberRegex()
                     .Matches(numberString[1])
                     .Select(match => int.Parse(match.Value))
                     .ToHashSet();

                NumWinningNumbers = winningNumbers.Intersect(myNumbers).Count();
            }

            public int Points => (int)Math.Pow(2, NumWinningNumbers - 1);

            public IEnumerable<int> CardsWon => NumWinningNumbers == 0
                ? []
                : Enumerable.Range(Number + 1, NumWinningNumbers);

            public void AddInstances(int instancesToAdd) =>
                InstanceCount += instancesToAdd;
        }
    }
}
