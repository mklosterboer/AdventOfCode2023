namespace AdventOfCode2023.Problems
{
    internal partial class Day08Problem
    {
        private class Node(string Value, Node? Left = null, Node? Right = null)
        {
            public string Value { get; } = Value;
            public Node? Left { get; set; } = Left;
            public Node? Right { get; set; } = Right;

            public Node GetNextNode(Direction direction) => direction switch
            {
                Direction.Left => Left!,
                Direction.Right => Right!,
                _ => throw new NotImplementedException(),
            };

            public override string ToString() => $"{Value} = ({Left!.Value}, {Right!.Value})";
        }
    }
}
