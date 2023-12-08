namespace AdventOfCode2023.Problems
{
    internal partial class Day07Problem
    {
        private class JokerHand(string cards, int bid) : Hand(cards, bid)
        {
            private static readonly int JokerValue = 1;

            protected override int GetCardValue(char card) => card switch
            {
                'A' => 14,
                'K' => 13,
                'Q' => 12,
                'J' => JokerValue,
                'T' => 10,
                _ => (int)char.GetNumericValue(card),
            };

            protected override HandType GetHandType()
            {
                var numJokers = Cards.Count(x => x == JokerValue);

                if (numJokers == 0)
                {
                    // No Jokers, don't mess with the below logic, just find it normally
                    return base.GetHandType();
                }

                // Finds the largest set of the same card that is not a Joker
                // Returns the size of that set and how many sets exists of this size
                var maxSetSize = Cards
                    .Where(x => x != JokerValue)
                    .GroupBy(x => x)
                    .Select(x => x.Count())
                    .GroupBy(x => x, (y, z) => new
                    {
                        Value = z.Max(),
                        NumSets = z.Count(i => i == z.Max())
                    })
                    .MaxBy(x => x.Value);
                
                // Total number of the potentially same card when including jokers
                var numSameCard = numJokers + (maxSetSize?.Value ?? 0);

                return (numSameCard, maxSetSize?.NumSets) switch
                {
                    // JJJJJ
                    // 1 JJJJ
                    // 11 JJJ
                    // 111 JJ
                    // 1111 J
                    (5, _) => HandType.FiveOfAKind,

                    // 1 2 JJJ
                    // 11 2 JJ
                    // 111 2 J
                    (4, _) => HandType.FourOfAKind,

                    // 11 22 J
                    (3, 2) => HandType.FullHouse,

                    // 1 2 3 JJ
                    // 11 2 3 J
                    (3, _) => HandType.ThreeofAKind,

                    // 1 2 3 4 J 
                    (_, _) => HandType.OnePair,
                };
            }
        }
    }
}
