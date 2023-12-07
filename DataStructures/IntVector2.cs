namespace AdventOfCode2023.DataStructures
{
    public record IntVector2(int X, int Y) : IEquatable<IntVector2>
    {
        public static IntVector2 operator +(IntVector2 a, IntVector2 b) => new(a.X + b.X, a.Y + b.Y);
        public static IntVector2 operator -(IntVector2 a, IntVector2 b) => new(a.X - b.X, a.Y - b.Y);

        public override string ToString()
        {
            return $"[{X}, {Y}]";
        }

        bool IEquatable<IntVector2>.Equals(IntVector2? other) => X == other?.X && Y == other.Y;
    }
}
