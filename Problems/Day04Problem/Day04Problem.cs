using AdventOfCode2023.Utilities;
using System.Text.RegularExpressions;

namespace AdventOfCode2023.Problems
{
    internal class Day04Problem : Problem
    {
        protected override string InputName => "Actual";

        private IEnumerable<Card> Cards { get; init; }

        public Day04Problem()
        {
            Cards = GetInputStringList().Select(x => new Card(x));
        }

        public override object PartOne()
        {
            return Cards.Sum(x => x.Points);
        }

        public override object PartTwo()
        {
            var cardsDictionary = Cards
                .ToDictionary(x => x.Number);

            for (var i = 1; i < cardsDictionary.Count; i++)
            {
                var card = cardsDictionary[i];

                foreach (var cardNumber in card.CardsWon)
                {
                    cardsDictionary[cardNumber].AddInstances(card.InstanceCount);
                }
            }

            return cardsDictionary.Values.Sum(x => x.InstanceCount);
        }

        private class Card
        {
            public int Number { get; init; }

            public int InstanceCount { get; private set; }

            private int WinningNumbersCount { get; init; }

            private static Regex NumberRegex = new(@"\d+");

            public Card(string input)
            {
                InstanceCount = 1;

                var splitString = input.Split(':');

                Number = int.Parse(NumberRegex.Match(splitString[0]).Value);

                var numbersString = splitString[1].Split("|");

                var winningNumbers = NumberRegex
                    .Matches(numbersString[0])
                    .Select(match => int.Parse(match.Value));

                var myNumbers = NumberRegex
                     .Matches(numbersString[1])
                     .Select(match => int.Parse(match.Value));

                WinningNumbersCount = winningNumbers.Intersect(myNumbers).Count();
            }

            public int Points => (int)Math.Pow(2, WinningNumbersCount - 1);

            public IEnumerable<int> CardsWon => WinningNumbersCount == 0
                ? []
                : Enumerable.Range(Number + 1, WinningNumbersCount);

            public void AddInstances(int instancesToAdd) =>
                InstanceCount += instancesToAdd;
        }
    }
}
