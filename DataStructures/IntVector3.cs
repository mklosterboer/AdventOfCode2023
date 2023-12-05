namespace AdventOfCode2023.DataStructures
{
    internal record IntVector3(int X, int Y, int Z)
    {
        public static IntVector3 operator +(IntVector3 a, IntVector3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static IntVector3 operator -(IntVector3 a, IntVector3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        public IntVector3(string x, string y, string z)
            : this(int.Parse(x), int.Parse(y), int.Parse(z))
        { }

        public override string ToString()
        {
            return $"[{X}, {Y}, {Z}]";
        }
    }
}
