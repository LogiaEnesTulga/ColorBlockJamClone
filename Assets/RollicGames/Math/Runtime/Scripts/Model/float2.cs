using System;

namespace RollicGames.Math.Runtime.Model
{
    [Serializable]
    public readonly struct float2 : IEquatable<float2>
    {
        public static readonly float2 Zero = new(0f, 0f);
        public static readonly float2 One = new(1f, 1f);

        public readonly float X;
        public readonly float Y;

        public float2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static float2 operator +(float2 a, float2 b) => new(a.X + b.X, a.Y + b.Y);
        public static float2 operator -(float2 a, float2 b) => new(a.X - b.X, a.Y - b.Y);
        public static float2 operator *(float2 a, float scalar) => new(a.X * scalar, a.Y * scalar);
        public static bool operator ==(float2 a, float2 b) => a.Equals(b);
        public static bool operator !=(float2 a, float2 b) => !a.Equals(b);

        public bool Equals(float2 other) => X == other.X && Y == other.Y;
        public override bool Equals(object obj) => obj is float2 other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(X, Y);
        public override string ToString() => $"({X}, {Y})";
    }
}
