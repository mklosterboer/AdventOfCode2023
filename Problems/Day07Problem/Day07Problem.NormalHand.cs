namespace AdventOfCode2023.Problems
{
    internal partial class Day07Problem
    {
        private class NormalHand(string cards, int bid) : Hand(cards, bid)
        {
            protected override int GetCardValue(char card) => card switch
            {
                'A' => 14,
                'K' => 13,
                'Q' => 12,
                'J' => 11,
                'T' => 10,
                _ => (int)char.GetNumericValue(card),
            };

            protected override HandType GetHandType()
            {
                return GetHandTypeNoJokers();
            }
        }
    }
}
