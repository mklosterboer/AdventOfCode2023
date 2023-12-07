namespace AdventOfCode2023.Problems
{
    internal partial class Day07Problem
    {
        private class JokerHand(string cards, int bid) : Hand(cards, bid)
        {
            protected override int GetCardValue(char card) => card switch
            {
                'A' => 14,
                'K' => 13,
                'Q' => 12,
                'J' => 1,
                'T' => 10,
                _ => (int)char.GetNumericValue(card),
            };

            protected override HandType GetHandType()
            {
                if (!Cards.Any(x => x == 1))
                {
                    // No Jokers, don't mess with the below logic, just find it normally
                    return GetHandTypeNoJokers();
                }

                var maxGroupSize = Cards
                    .Where(x => x != 1)
                    .GroupBy(x => x)
                    .Select(x => x.Count())
                    .GroupBy(x => x, (y, z) => new
                    {
                        Max = z.Max(),
                        CountOfMax = z.Count(i => i == z.Max())
                    })
                    .MaxBy(x => x.Max)!;

                var numJokers = Cards.Count(x => x == 1);
                switch (numJokers)
                {
                    case 5:
                    case 4:
                        // JJJJJ
                        // 1 JJJJ
                        return HandType.FiveOfAKind;
                    case 3:
                        {
                            if (maxGroupSize.Max == 2)
                            {
                                // 11 JJJ
                                return HandType.FiveOfAKind;
                            }
                            // 1 2 JJJ
                            return HandType.FourOfAKind;
                        }
                    case 2:
                        {
                            if (maxGroupSize.Max == 3)
                            {
                                // 111 JJ
                                return HandType.FiveOfAKind;
                            }

                            if (maxGroupSize.Max == 2)
                            {
                                // 11 2 JJ
                                return HandType.FourOfAKind;
                            }

                            // 1 2 3 JJ
                            return HandType.ThreeofAKind;
                        }
                    case 1:
                        {
                            if (maxGroupSize.Max == 4)
                            {
                                // 1111 J
                                return HandType.FiveOfAKind;
                            }

                            if (maxGroupSize.Max == 3)
                            {
                                // 111 2 J
                                return HandType.FourOfAKind;
                            }

                            if (maxGroupSize.Max == 2)
                            {
                                if (maxGroupSize.CountOfMax == 2)
                                {
                                    // 11 22 J
                                    return HandType.FullHouse;
                                }

                                // 11 2 3 J
                                return HandType.ThreeofAKind;
                            }

                            // 1 2 3 4 J 
                            return HandType.OnePair;
                        }
                }

                // We should never get here...
                throw new NotImplementedException();
            }
        }
    }
}
