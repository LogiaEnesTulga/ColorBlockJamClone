using System;

namespace RollicGames.Math.Runtime.Model
{
    [Serializable]
    public readonly struct int2 : IEquatable<int2>
    {
        public static readonly int2 Zero = new(0, 0);
        public static readonly int2 One = new(1, 1);

        public readonly int X;
        public readonly int Y;

        public int2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static int2 operator +(int2 a, int2 b) => new(a.X + b.X, a.Y + b.Y);
        public static int2 operator -(int2 a, int2 b) => new(a.X - b.X, a.Y - b.Y);
        public static int2 operator *(int2 a, int scalar) => new(a.X * scalar, a.Y * scalar);
        public static bool operator ==(int2 a, int2 b) => a.Equals(b);
        public static bool operator !=(int2 a, int2 b) => !a.Equals(b);

        public bool Equals(int2 other) => X == other.X && Y == other.Y;
        public override bool Equals(object obj) => obj is int2 other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(X, Y);
        public override string ToString() => $"({X}, {Y})";
    }
}
