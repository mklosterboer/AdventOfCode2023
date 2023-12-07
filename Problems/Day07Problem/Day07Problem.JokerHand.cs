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
                    .MaxBy(x => x.Max);

                var numJokers = Cards.Count(x => x == 1);

                return (numJokers, maxGroupSize?.Max, maxGroupSize?.CountOfMax) switch
                {
                    // JJJJJ
                    // 1 JJJJ
                    ( >= 4, _, _) => HandType.FiveOfAKind,

                    // 11 JJJ
                    (3, 2, _) => HandType.FiveOfAKind,

                    // 1 2 JJJ
                    (3, _, _) => HandType.FourOfAKind,

                    // 111 JJ
                    (2, 3, _) => HandType.FiveOfAKind,

                    // 11 2 JJ
                    (2, 2, _) => HandType.FourOfAKind,

                    // 1 2 3 JJ
                    (2, _, _) => HandType.ThreeofAKind,

                    // 1111 J
                    (1, 4, _) => HandType.FiveOfAKind,

                    // 111 2 J
                    (1, 3, _) => HandType.FourOfAKind,

                    // 11 22 J
                    (1, 2, 2) => HandType.FullHouse,

                    // 11 2 3 J
                    (1, 2, _) => HandType.ThreeofAKind,

                    // 1 2 3 4 J 
                    (_, _, _) => HandType.OnePair,
                };
            }
        }
    }
}
