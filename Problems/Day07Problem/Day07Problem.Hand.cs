namespace AdventOfCode2023.Problems
{
    internal partial class Day07Problem
    {
        private class Hand : IComparable<Hand>
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

            protected virtual HandType GetHandType()
            {
                // Finds the largest set of the same card
                // Returns the size of that set and how many sets exists of this size
                var maxSetSize = Cards
                    .Where(x => x != 1)
                    .GroupBy(x => x)
                    .Select(x => x.Count())
                    .GroupBy(x => x, (y, z) => new
                    {
                        Value = z.Max(),
                        NumSets = z.Count(i => i == z.Max()),
                    })
                    .MaxBy(x => x.Value)!;

                var numGroups = Cards.GroupBy(x => x).Count();

                return ((maxSetSize.Value, maxSetSize.NumSets, numGroups)) switch
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

            protected virtual int GetCardValue(char card) => card switch
            {
                'A' => 14,
                'K' => 13,
                'Q' => 12,
                'J' => 11,
                'T' => 10,
                _ => (int)char.GetNumericValue(card),
            };
        }
    }
}
