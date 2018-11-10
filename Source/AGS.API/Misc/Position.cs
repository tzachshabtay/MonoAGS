using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace AGS.API
{
    /// <summary>
    /// A 3D location in the world.
    /// </summary>
    [DataContract]
    public struct Position : IEquatable<Position>
    {
        private readonly float? _z;
        private readonly PointF _xy;

        public Position(PointF point, float? z = null)
        {
            _xy = point;
            _z = z;
        }

        [JsonConstructor]
        public Position(float x, float y, float? z = null) : this(new PointF(x, y), z)
        { }

        public static Position Empty = new Position(new PointF(), null);

        /// <summary>
        /// Gets the x coordinate.
        /// </summary>
        /// <value>The x.</value>
        [DataMember(Name = "X")]
        public float X => XY.X;

        /// <summary>
        /// Gets the y coordinate.
        /// </summary>
        /// <value>The y.</value>
        [DataMember(Name = "Y")]
        public float Y => XY.Y;

        /// <summary>
        /// Gets the z coordinate.
        /// In a 2D world, Z is actually used to decide what gets rendered behind what.
        /// By default, Z will equal Y and will change whenever Y changes, so that characters/objects which are
        /// more to the bottom of the screen will appear in the front, which is the desired behavior in most scenarios.
        /// By setting a location with Z different than Y, this behavior breaks, making Z an independent value (this can
        /// be reverted again by explicitly setting Z to be Y).
        /// </summary>
        /// <value>The z.</value>
        [DataMember(Name = "Z")]
        public float Z => _z.HasValue ? _z.Value : Y;

        /// <summary>
        /// Gets the (x,y) as a point.
        /// </summary>
        /// <value>The xy.</value>
        public PointF XY => _xy;

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:AGS.API.Position"/>.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:AGS.API.Position"/>.</returns>
        public override string ToString()
        {
            return $"{XY.ToString()},{Z:0.##}";
        }

        public static implicit operator Position((float x, float y) pos) => new Position(pos.x, pos.y);

        public static implicit operator Position((float x, float y, float? z) pos) => new Position(pos.x, pos.y, pos.z);

        /// <summary>
        /// Deconstruct the specified x, y and z (can be used for tuple deconstruction).
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="z">The z coordinate.</param>
        public void Deconstruct(out float x, out float y, out float z)
        {
            x = this.X;
            y = this.Y;
            z = this.Z;
        }

        public override bool Equals(object obj) => obj is Position other && Equals(other);

        public override int GetHashCode() => XY.GetHashCode();

        public bool Equals(Position other) => XY.Equals(other.XY) && MathUtils.FloatEquals(Z, other.Z);
    }
}