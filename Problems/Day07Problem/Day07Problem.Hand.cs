namespace AdventOfCode2023.Problems
{
    internal partial class Day07Problem
    {
        private abstract class Hand : IComparable<Hand>
        {
            public List<int> Cards { get; init; }

            public int Bid { get; init; }

            public HandType HandType { get; set; }

            private string OriginalHand { get; init; }

            public Hand(string cards, int bid)
            {
                OriginalHand = cards;
                Cards = cards.Select(GetCardValue).ToList();
                Bid = bid;
                HandType = GetHandType();
            }

            public int CompareTo(Hand? other)
            {
                if (other == null) return 1;

                if (this.HandType != other.HandType)
                {
                    return this.HandType.CompareTo(other.HandType);
                }

                for (int i = 0; i < Cards.Count; i++)
                {
                    if (this.Cards[i] != other.Cards[i])
                    {
                        return this.Cards[i].CompareTo(other.Cards[i]);
                    }
                }

                return 0;
            }

            public override string ToString() => $"{OriginalHand} {Bid} {HandType}";

            protected HandType GetHandTypeNoJokers()
            {
                var numGroups = Cards.GroupBy(x => x).Count();
                var maxGroupSize = Cards
                    .Where(x => x != 1)
                    .GroupBy(x => x)
                    .Select(x => x.Count())
                    .GroupBy(x => x, (y, z) => new
                    {
                        Max = z.Max(),
                        CountOfMax = z.Count(i => i == z.Max()),
                    })
                    .MaxBy(x => x.Max)!;

                return ((maxGroupSize.Max, maxGroupSize.CountOfMax, numGroups)) switch
                {
                    (5, _, _) => HandType.FiveOfAKind,
                    (4, _, _) => HandType.FourOfAKind,
                    (3, _, 2) => HandType.FullHouse,
                    (3, _, _) => HandType.ThreeofAKind,
                    (2, 2, _) => HandType.TwoPair,
                    (2, 1, _) => HandType.OnePair,
                    (_, _, _) => HandType.HighCard,
                };
            }

            protected abstract int GetCardValue(char card);

            protected abstract HandType GetHandType();
        }
    }
}
