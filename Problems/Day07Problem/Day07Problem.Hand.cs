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
                var groups = Cards.GroupBy(x => x);
                switch (groups.Count())
                {
                    case 1:
                        // All the cards are the same, must be five of a kind
                        return HandType.FiveOfAKind;
                    case 2:
                        {
                            if (groups.Any(x => x.Count() == 2))
                            {
                                return HandType.FullHouse;
                            }

                            return HandType.FourOfAKind;
                        }
                    case 3:
                        {
                            if (groups.Any(x => x.Count() == 3))
                            {
                                return HandType.ThreeofAKind;
                            }

                            return HandType.TwoPair;
                        }
                    case 4:
                        return HandType.OnePair;
                    default:
                        return HandType.HighCard;
                }
            }

            protected abstract int GetCardValue(char card);

            protected abstract HandType GetHandType();
        }
    }
}
