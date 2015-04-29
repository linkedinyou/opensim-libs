/*
 * Copyright (c) 2006-2007, Second Life Reverse Engineering Team
 * All rights reserved.
 *
 * - Redistribution and use in source and binary forms, with or without 
 *   modification, are permitted provided that the following conditions are met:
 *
 * - Redistributions of source code must retain the above copyright notice, this
 *   list of conditions and the following disclaimer.
 * - Neither the name of the Second Life Reverse Engineering Team nor the names 
 *   of its contributors may be used to endorse or promote products derived from
 *   this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
 * POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using libsecondlife.StructuredData;

namespace libsecondlife
{
    /// <summary>
    /// A 128-bit Universally Unique Identifier, used throughout the Second
    /// Life networking protocol
    /// </summary>
    public struct LLUUID : IComparable
    {
        /// <summary>The System.Guid object this struct wraps around</summary>
        public Guid UUID;

        #region Properties

        /// <summary>Get a byte array of the 16 raw bytes making up the UUID</summary>
        public byte[] Data { get { return GetBytes(); } }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Constructor that takes a string UUID representation
        /// </summary>
        /// <param name="val">A string representation of a UUID, case 
        /// insensitive and can either be hyphenated or non-hyphenated</param>
        /// <example>LLUUID("11f8aa9c-b071-4242-836b-13b7abe0d489")</example>
        public LLUUID(string val)
        {
            if (val == null)
                UUID = new Guid();
            else
                UUID = new Guid(val);
        }

        /// <summary>
        /// Constructor that takes a System.Guid object
        /// </summary>
        /// <param name="val">A Guid object that contains the unique identifier
        /// to be represented by this LLUUID</param>
        public LLUUID(Guid val)
        {
            UUID = val;
        }

        /// <summary>
        /// Constructor that takes a byte array containing a UUID
        /// </summary>
        /// <param name="source">Byte array containing a 16 byte UUID</param>
        /// <param name="pos">Beginning offset in the array</param>
        public LLUUID(byte[] source, int pos)
        {
            UUID = LLUUID.Zero.UUID;
            FromBytes(source, pos);
        }

        /// <summary>
        /// Constructor that takes an unsigned 64-bit unsigned integer to 
        /// convert to a UUID
        /// </summary>
        /// <param name="val">64-bit unsigned integer to convert to a UUID</param>
        public LLUUID(ulong val)
        {
            UUID = new Guid(0, 0, 0, BitConverter.GetBytes(val));
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pos"></param>
        public void FromBytes(byte[] source, int pos)
        {
            UUID = new Guid(
                (source[pos + 0] << 24) | (source[pos + 1] << 16) | (source[pos + 2] << 8) | source[pos + 3],
                (short)((source[pos + 4] << 8) | source[pos + 5]),
                (short)((source[pos + 6] << 8) | source[pos + 7]),
                source[pos + 8], source[pos + 9], source[pos + 10], source[pos + 11],
                source[pos + 12], source[pos + 13], source[pos + 14], source[pos + 15]);
        }

        /// <summary>
        /// IComparable.CompareTo implementation.
        /// </summary>
        public int CompareTo(object obj)
        {
            if (obj is LLUUID)
            {
                LLUUID ID = (LLUUID)obj;
                return this.UUID.CompareTo(ID.UUID);
            }

            throw new ArgumentException("object is not a LLUUID");
        }

        /// <summary>
        /// Returns the raw bytes for this UUID
        /// </summary>
        /// <returns>A 16 byte array containing this UUID</returns>
        public byte[] GetBytes()
        {
            byte[] bytes = UUID.ToByteArray();

            byte[] output = new byte[16];
            output[0] = bytes[3];
            output[1] = bytes[2];
            output[2] = bytes[1];
            output[3] = bytes[0];
            output[4] = bytes[5];
            output[5] = bytes[4];
            output[6] = bytes[7];
            output[7] = bytes[6];
            Buffer.BlockCopy(bytes, 8, output, 8, 8);
            
            return output;
        }

		/// <summary>
		/// Calculate an LLCRC (cyclic redundancy check) for this LLUUID
		/// </summary>
		/// <returns>The CRC checksum for this LLUUID</returns>
		public uint CRC() 
		{
			uint retval = 0;
            byte[] bytes = GetBytes();

            retval += (uint)((bytes[ 3] << 24) + (bytes[ 2] << 16) + (bytes[ 1] << 8) + bytes[ 0]);
            retval += (uint)((bytes[ 7] << 24) + (bytes[ 6] << 16) + (bytes[ 5] << 8) + bytes[ 4]);
            retval += (uint)((bytes[11] << 24) + (bytes[10] << 16) + (bytes[ 9] << 8) + bytes[ 8]);
            retval += (uint)((bytes[15] << 24) + (bytes[14] << 16) + (bytes[13] << 8) + bytes[12]);

			return retval;
		}

        /// <summary>
        /// Get a 64-bit integer representation of the first half of this UUID
        /// </summary>
        /// <returns>An integer created from the first eight bytes of this UUID</returns>
        public ulong ToULong()
        {
            return Helpers.BytesToUInt64(UUID.ToByteArray());
        }

        #endregion Public Methods

        #region Static Methods

        /// <summary>
        /// Generate a LLUUID from a string
        /// </summary>
        /// <param name="val">A string representation of a UUID, case 
        /// insensitive and can either be hyphenated or non-hyphenated</param>
        /// <example>LLUUID.Parse("11f8aa9c-b071-4242-836b-13b7abe0d489")</example>
        public static LLUUID Parse(string val)
        {
            return new LLUUID(val);
        }

        /// <summary>
        /// Generate a LLUUID from a string
        /// </summary>
        /// <param name="val">A string representation of a UUID, case 
        /// insensitive and can either be hyphenated or non-hyphenated</param>
        /// <param name="result">Will contain the parsed UUID if successful,
        /// otherwise null</param>
        /// <returns>True if the string was successfully parse, otherwise false</returns>
        /// <example>LLUUID.TryParse("11f8aa9c-b071-4242-836b-13b7abe0d489", result)</example>
        public static bool TryParse(string val, out LLUUID result)
        {
            try
            {
                result = Parse(val);
                return true;
            }
            catch (Exception)
            {
                result = LLUUID.Zero;
                return false;
            }
        }

        /// <summary>
        /// Combine two UUIDs together by taking the MD5 hash of a byte array
        /// containing both UUIDs
        /// </summary>
        /// <param name="first">First LLUUID to combine</param>
        /// <param name="second">Second LLUUID to combine</param>
        /// <returns>The UUID product of the combination</returns>
        public static LLUUID Combine(LLUUID first, LLUUID second)
        {
            // Construct the buffer that MD5ed
            byte[] input = new byte[32];
            Buffer.BlockCopy(first.GetBytes(), 0, input, 0, 16);
            Buffer.BlockCopy(second.GetBytes(), 0, input, 16, 16);

            return new LLUUID(Helpers.MD5Builder.ComputeHash(input), 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public static LLUUID Random()
		{
			return new LLUUID(Guid.NewGuid());
        }

        #endregion Static Methods

        #region Overrides

        /// <summary>
        /// Return a hash code for this UUID, used by .NET for hash tables
        /// </summary>
        /// <returns>An integer composed of all the UUID bytes XORed together</returns>
		public override int GetHashCode()
		{
            return UUID.GetHashCode();
		}

        /// <summary>
        /// Comparison function
        /// </summary>
        /// <param name="o">An object to compare to this UUID</param>
        /// <returns>False if the object is not an LLUUID, true if it is and
        /// byte for byte identical to this</returns>
		public override bool Equals(object o)
		{
			if (!(o is LLUUID)) return false;

			LLUUID uuid = (LLUUID)o;
            return UUID == uuid.UUID;
        }

        /// <summary>
        /// Get a hyphenated string representation of this UUID
        /// </summary>
        /// <returns>A string representation of this UUID, lowercase and 
        /// with hyphens</returns>
        /// <example>11f8aa9c-b071-4242-836b-13b7abe0d489</example>
        public override string ToString()
        {
            return UUID.ToString();
        }

        #endregion Overrides

        #region Operators

        /// <summary>
        /// Equals operator
        /// </summary>
        /// <param name="lhs">First LLUUID for comparison</param>
        /// <param name="rhs">Second LLUUID for comparison</param>
        /// <returns>True if the UUIDs are byte for byte equal, otherwise false</returns>
		public static bool operator==(LLUUID lhs, LLUUID rhs)
		{
            return lhs.UUID == rhs.UUID;
		}

        /// <summary>
        /// Not equals operator
        /// </summary>
        /// <param name="lhs">First LLUUID for comparison</param>
        /// <param name="rhs">Second LLUUID for comparison</param>
        /// <returns>True if the UUIDs are not equal, otherwise true</returns>
		public static bool operator!=(LLUUID lhs, LLUUID rhs)
		{
			return !(lhs == rhs);
		}

        /// <summary>
        /// XOR operator
        /// </summary>
        /// <param name="lhs">First LLUUID</param>
        /// <param name="rhs">Second LLUUID</param>
        /// <returns>A UUID that is a XOR combination of the two input UUIDs</returns>
        public static LLUUID operator ^(LLUUID lhs, LLUUID rhs)
        {
            byte[] lhsbytes = lhs.GetBytes();
            byte[] rhsbytes = rhs.GetBytes();
            byte[] output = new byte[16];

            for (int i = 0; i < 16; i++)
            {
                output[i] = (byte)(lhsbytes[i] ^ rhsbytes[i]);
            }

            return new LLUUID(output, 0);
        }

        /// <summary>
        /// String typecasting operator
        /// </summary>
        /// <param name="val">A UUID in string form. Case insensitive, 
        /// hyphenated or non-hyphenated</param>
        /// <returns>A UUID built from the string representation</returns>
        public static implicit operator LLUUID(string val)
		{
			return new LLUUID(val);
        }

        #endregion Operators

        /// <summary>An LLUUID with a value of all zeroes</summary>
        public static readonly LLUUID Zero = new LLUUID();
	}

    /// <summary>
    /// A two-dimensional vector with floating-point values
    /// </summary>
    public struct LLVector2
    {
        /// <summary>X value</summary>
        public float X;
        /// <summary>Y value</summary>
        public float Y;

        #region Constructors

        /// <summary>
        /// Constructor, copies a single-precision vector
        /// </summary>
        /// <param name="vector">Single-precision vector to copy</param>
        public LLVector2(LLVector2 vector)
        {
            X = vector.X;
            Y = vector.Y;
        }

        /// <summary>
        /// Constructor, builds a vector for individual float values
        /// </summary>
        /// <param name="x">X value</param>
        /// <param name="y">Y value</param>
        /// <param name="z">Z value</param>
		public LLVector2(float x, float y)
		{
			X = x;
			Y = y;
        }

        #endregion Constructors

        #region Overrides

        /// <summary>
        /// A hash of the vector, used by .NET for hash tables
        /// </summary>
        /// <returns>The hashes of the individual components XORed together</returns>
        public override int GetHashCode()
        {
            return (X.GetHashCode() ^ Y.GetHashCode());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public override bool Equals(object o)
        {
            if (!(o is LLVector2)) return false;

            LLVector2 vector = (LLVector2)o;

            return (X == vector.X && Y == vector.Y);
        }

        /// <summary>
        /// Get a formatted string representation of the vector
        /// </summary>
        /// <returns>A string representation of the vector, similar to the LSL
        /// vector to string conversion in Second Life</returns>
        public override string ToString()
        {
            return String.Format(Helpers.EnUsCulture, "<{0}, {1}>", X, Y);
        }

        #endregion Overrides

        #region Operators

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator ==(LLVector2 lhs, LLVector2 rhs)
        {
            return (lhs.X == rhs.X && lhs.Y == rhs.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator !=(LLVector2 lhs, LLVector2 rhs)
        {
            return !(lhs == rhs);
        }

        public static LLVector2 operator +(LLVector2 lhs, LLVector2 rhs)
        {
            return new LLVector2(lhs.X + rhs.X, lhs.Y + rhs.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static LLVector2 operator -(LLVector2 lhs, LLVector2 rhs)
        {
            return new LLVector2(lhs.X - rhs.X, lhs.Y - rhs.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vec"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static LLVector2 operator *(LLVector2 vec, float val)
        {
            return new LLVector2(vec.X * val, vec.Y * val);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static LLVector2 operator *(float val, LLVector2 vec)
        {
            return new LLVector2(vec.X * val, vec.Y * val);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static LLVector2 operator *(LLVector2 lhs, LLVector2 rhs)
        {
            return new LLVector2(lhs.X * rhs.X, lhs.Y * rhs.Y);
        }

        #endregion Operators

        /// <summary>An LLVector2 with a value of 0,0,0</summary>
        public readonly static LLVector2 Zero = new LLVector2();
    }

    /// <summary>
    /// A three-dimensional vector with floating-point values
    /// </summary>
	public struct LLVector3
	{
        /// <summary>X value</summary>
        public float X;
		/// <summary>Y value</summary>
        public float Y;
        /// <summary>Z value</summary>
        public float Z;

        // Used for little to big endian conversion on big endian architectures
        private byte[] conversionBuffer;

        #region Constructors

        /// <summary>
        /// Constructor, copies a single-precision vector
        /// </summary>
        /// <param name="vector">Single-precision vector to copy</param>
        public LLVector3(LLVector3 vector)
        {
            conversionBuffer = null;
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
        }

        /// <summary>
        /// Constructor, builds a single-precision vector from a 
        /// double-precision one
        /// </summary>
        /// <param name="vector">A double-precision vector</param>
		public LLVector3(LLVector3d vector)
		{
            conversionBuffer = null;
			X = (float)vector.X;
			Y = (float)vector.Y;
			Z = (float)vector.Z;
		}

        /// <summary>
        /// Constructor, builds a vector from a byte array
        /// </summary>
        /// <param name="byteArray">Byte array containing a 12 byte vector</param>
        /// <param name="pos">Beginning position in the byte array</param>
		public LLVector3(byte[] byteArray, int pos)
		{
            conversionBuffer = null;
            X = Y = Z = 0;
            FromBytes(byteArray, pos);
        }

        /// <summary>
        /// Constructor, builds a vector for individual float values
        /// </summary>
        /// <param name="x">X value</param>
        /// <param name="y">Y value</param>
        /// <param name="z">Z value</param>
		public LLVector3(float x, float y, float z)
		{
            conversionBuffer = null;
			X = x;
			Y = y;
			Z = z;
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Builds a vector from a byte array
        /// </summary>
        /// <param name="byteArray">Byte array containing a 12 byte vector</param>
        /// <param name="pos">Beginning position in the byte array</param>
        public void FromBytes(byte[] byteArray, int pos)
        {
            if (!BitConverter.IsLittleEndian)
            {
                // Big endian architecture
                if (conversionBuffer == null)
                    conversionBuffer = new byte[12];

                Buffer.BlockCopy(byteArray, pos, conversionBuffer, 0, 12);

                Array.Reverse(conversionBuffer, 0, 4);
                Array.Reverse(conversionBuffer, 4, 4);
                Array.Reverse(conversionBuffer, 8, 4);

                X = BitConverter.ToSingle(conversionBuffer, 0);
                Y = BitConverter.ToSingle(conversionBuffer, 4);
                Z = BitConverter.ToSingle(conversionBuffer, 8);
            }
            else
            {
                // Little endian architecture
                X = BitConverter.ToSingle(byteArray, pos);
                Y = BitConverter.ToSingle(byteArray, pos + 4);
                Z = BitConverter.ToSingle(byteArray, pos + 8);
            }
        }

        /// <summary>
        /// Test if this vector is composed of all finite numbers
        /// </summary>
        public bool IsFinite()
        {
            if (Helpers.IsFinite(X) && Helpers.IsFinite(Y) && Helpers.IsFinite(Z))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Returns the raw bytes for this vector
        /// </summary>
        /// <returns>A 12 byte array containing X, Y, and Z</returns>
		public byte[] GetBytes()
		{
			byte[] byteArray = new byte[12];

            Buffer.BlockCopy(BitConverter.GetBytes(X), 0, byteArray, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(Y), 0, byteArray, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(Z), 0, byteArray, 8, 4);

			if(!BitConverter.IsLittleEndian) {
				Array.Reverse(byteArray, 0, 4);
				Array.Reverse(byteArray, 4, 4);
				Array.Reverse(byteArray, 8, 4);
			}

			return byteArray;
        }

        public LLSD ToLLSD()
        {
            LLSDArray array = new LLSDArray();
            array.Add(LLSD.FromReal(X));
            array.Add(LLSD.FromReal(Y));
            array.Add(LLSD.FromReal(Z));
            return array;
        }

        #endregion Public Methods

        #region Static Methods

        public static LLVector3 FromLLSD(LLSD llsd)
        {
            if (llsd.Type == LLSDType.Array)
            {
                LLSDArray array = (LLSDArray)llsd;

                if (array.Count == 3)
                {
                    return new LLVector3((float)array[0].AsReal(), (float)array[1].AsReal(), (float)array[2].AsReal());
                }
            }

            return LLVector3.Zero;
        }

        /// <summary>
        /// Calculate the magnitude of the supplied vector
        /// </summary>
        public static float Mag(LLVector3 v)
        {
            return (float)Math.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z);
        }

        /// <summary>
        /// Calculate the squared magnitude of the supplied vector
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static float MagSquared(LLVector3 v)
        {
            return v.X * v.X + v.Y * v.Y + v.Z * v.Z;
        }

        /// <summary>
        /// Returns a normalized version of the supplied vector
        /// </summary>
        /// <param name="vector">The vector to normalize</param>
        /// <returns>A normalized version of the vector</returns>
        public static LLVector3 Norm(LLVector3 vector)
        {
            float mag = Mag(vector);
            return new LLVector3(vector.X / mag, vector.Y / mag, vector.Z / mag);
        }

        /// <summary>
        /// Return the cross product of two vectors
        /// </summary>
        /// <param name="v1">First vector</param>
        /// <param name="v2">Second vector</param>
        /// <returns>Cross product of first and second vector</returns>
        public static LLVector3 Cross(LLVector3 v1, LLVector3 v2)
        {
            return new LLVector3
            (
                v1.Y * v2.Z - v1.Z * v2.Y,
                v1.Z * v2.X - v1.X * v2.Z,
                v1.X * v2.Y - v1.Y * v2.X
            );
        }

        /// <summary>
        /// Returns the dot product of two vectors
        /// </summary>
        public static float Dot(LLVector3 v1, LLVector3 v2)
        {
            return (v1.X * v2.X) + (v1.Y * v2.Y) + (v1.Z * v2.Z);
        }

        /// <summary>
        /// Calculates the distance between two vectors
        /// </summary>
        public static float Dist(LLVector3 pointA, LLVector3 pointB)
        {
            float xd = pointB.X - pointA.X;
            float yd = pointB.Y - pointA.Y;
            float zd = pointB.Z - pointA.Z;
            return (float)Math.Sqrt(xd * xd + yd * yd + zd * zd);
        }

        public static LLVector3 Rot(LLVector3 vector, LLQuaternion rotation)
        {
            return vector * rotation;
        }

        public static LLVector3 Rot(LLVector3 vector, LLMatrix3 rotation)
        {
            return vector * rotation;
        }

        /// <summary>
        /// Calculate the rotation between two vectors
        /// </summary>
        /// <param name="a">Directional vector, such as 1,0,0 for the forward face</param>
        /// <param name="b">Target vector - normalize first with VecNorm</param>
        public static LLQuaternion RotBetween(LLVector3 a, LLVector3 b)
        {
            //A and B should both be normalized

            float dotProduct = Dot(a, b);
            LLVector3 crossProduct = Cross(a, b);
            float magProduct = Mag(a) * Mag(b);
            double angle = Math.Acos(dotProduct / magProduct);
            LLVector3 axis = Norm(crossProduct);
            float s = (float)Math.Sin(angle / 2);

            LLQuaternion quat = new LLQuaternion();
            quat.X = axis.X * s;
            quat.Y = axis.Y * s;
            quat.Z = axis.Z * s;
            quat.W = (float)Math.Cos(angle / 2);

            return quat;
        }

        /// <summary>
        /// Converts a vector style rotation to a quaternion
        /// </summary>
        /// <param name="a">Axis rotation, such as 0,0,90 for 90 degrees to the right</param>
        /// <returns>A quaternion representing the axes of the supplied vector</returns>
        public static LLQuaternion Axis2Rot(LLVector3 a)
        {
            if (a.X > 180) a.X -= 360; if (a.Y > 180) a.Y -= 360; if (a.Z > 180) a.Z -= 360;
            if (a.X < -180) a.X += 360; if (a.Y < -180) a.Y += 360; if (a.Z < -180) a.Z += 360;

            LLQuaternion rot = LLQuaternion.Identity;
            rot.X = (float)(a.X * Helpers.DEG_TO_RAD);
            rot.Y = (float)(a.Y * Helpers.DEG_TO_RAD);
            rot.Z = (float)(a.Z * Helpers.DEG_TO_RAD);
            if (a.Z > 180) rot.W = 0;

            return rot;
        }

        /// <summary>
        /// Generate an LLVector3 from a string
        /// </summary>
        /// <param name="val">A string representation of a 3D vector, enclosed 
        /// in arrow brackets and separated by commas</param>
        public static LLVector3 Parse(string val)
        {
            char[] splitChar = { ',', ' ' };
            string[] split = val.Replace("<", String.Empty).Replace(">", String.Empty).Split(splitChar);
            return new LLVector3(
                float.Parse(split[0].Trim(), Helpers.EnUsCulture),
                float.Parse(split[1].Trim(), Helpers.EnUsCulture),
                float.Parse(split[2].Trim(), Helpers.EnUsCulture));
        }

        public static bool TryParse(string val, out LLVector3 result)
        {
            try
            {
                result = Parse(val);
                return true;
            }
            catch (Exception)
            {
                result = new LLVector3();
                return false;
            }
        }

        #endregion Static Methods

        #region Overrides

        /// <summary>
        /// A hash of the vector, used by .NET for hash tables
        /// </summary>
        /// <returns>The hashes of the individual components XORed together</returns>
        public override int GetHashCode()
        {
            return (X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
		public override bool Equals(object o)
		{
			if (!(o is LLVector3)) return false;

			LLVector3 vector = (LLVector3)o;

			return (X == vector.X && Y == vector.Y && Z == vector.Z);
        }

        /// <summary>
        /// Get a formatted string representation of the vector
        /// </summary>
        /// <returns>A string representation of the vector, similar to the LSL
        /// vector to string conversion in Second Life</returns>
        public override string ToString()
        {
            return String.Format(Helpers.EnUsCulture, "<{0}, {1}, {2}>", X, Y, Z);
        }

        #endregion Overrides

        #region Operators

		public static bool operator==(LLVector3 lhs, LLVector3 rhs)
		{
			return (lhs.X == rhs.X && lhs.Y == rhs.Y && lhs.Z == rhs.Z);
		}

		public static bool operator!=(LLVector3 lhs, LLVector3 rhs)
		{
			return !(lhs == rhs);
		}

        public static LLVector3 operator +(LLVector3 lhs, LLVector3 rhs)
        {
            return new LLVector3(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z);
        }

        public static LLVector3 operator -(LLVector3 lhs, LLVector3 rhs)
        {
            return new LLVector3(lhs.X - rhs.X,lhs.Y - rhs.Y, lhs.Z - rhs.Z);
        }

        public static LLVector3 operator *(LLVector3 vec, float val)
        {
            return new LLVector3(vec.X * val, vec.Y * val, vec.Z * val);
        }

        public static LLVector3 operator *(float val, LLVector3 vec)
        {
            return new LLVector3(vec.X * val, vec.Y * val, vec.Z * val);
        }

        public static LLVector3 operator *(LLVector3 lhs, LLVector3 rhs)
        {
            return new LLVector3(lhs.X * rhs.X, lhs.Y * rhs.Y, lhs.Z * rhs.Z);
        }

        public static LLVector3 operator *(LLVector3 vec, LLQuaternion rot)
        {
            float rw = -rot.X * vec.X - rot.Y * vec.Y - rot.Z * vec.Z;
            float rx =  rot.W * vec.X + rot.Y * vec.Z - rot.Z * vec.Y;
            float ry =  rot.W * vec.Y + rot.Z * vec.X - rot.X * vec.Z;
            float rz =  rot.W * vec.Z + rot.X * vec.Y - rot.Y * vec.X;

            float nx = -rw * rot.X + rx * rot.W - ry * rot.Z + rz * rot.Y;
            float ny = -rw * rot.Y + ry * rot.W - rz * rot.X + rx * rot.Z;
            float nz = -rw * rot.Z + rz * rot.W - rx * rot.Y + ry * rot.X;

            return new LLVector3(nx, ny, nz);
        }

        #endregion Operators

        /// <summary>An LLVector3 with a value of 0,0,0</summary>
        public readonly static LLVector3 Zero = new LLVector3();
	}

    /// <summary>
    /// A double-precision three-dimensional vector
    /// </summary>
	public struct LLVector3d
	{
        /// <summary>X value</summary>
        public double X;
        /// <summary>Y value</summary>
        public double Y;
        /// <summary>Z value</summary>
        public double Z;

        // Used for little to big endian conversion on big endian architectures
        private byte[] conversionBuffer;

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
		public LLVector3d(double x, double y, double z)
		{
            conversionBuffer = null;
			X = x;
			Y = y;
			Z = z;
		}

        /// <summary>
        /// Create a double precision vector from a float vector
        /// </summary>
        /// <param name="llv3"></param>
        public LLVector3d(LLVector3 llv3)
        {
            conversionBuffer = null;
            X = llv3.X;
            Y = llv3.Y;
            Z = llv3.Z;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="byteArray"></param>
        /// <param name="pos"></param>
		public LLVector3d(byte[] byteArray, int pos)
		{
            conversionBuffer = null;
            X = Y = Z = 0;
            FromBytes(byteArray, pos);
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="byteArray"></param>
        /// <param name="pos"></param>
        public void FromBytes(byte[] byteArray, int pos)
        {
            if (!BitConverter.IsLittleEndian)
            {
                // Big endian architecture
                if (conversionBuffer == null)
                    conversionBuffer = new byte[24];

                Buffer.BlockCopy(byteArray, pos, conversionBuffer, 0, 24);

                Array.Reverse(conversionBuffer, 0, 8);
                Array.Reverse(conversionBuffer, 8, 8);
                Array.Reverse(conversionBuffer, 16, 8);

                X = BitConverter.ToDouble(conversionBuffer, 0);
                Y = BitConverter.ToDouble(conversionBuffer, 8);
                Z = BitConverter.ToDouble(conversionBuffer, 16);
            }
            else
            {
                // Little endian architecture
                X = BitConverter.ToDouble(byteArray, pos);
                Y = BitConverter.ToDouble(byteArray, pos + 8);
                Z = BitConverter.ToDouble(byteArray, pos + 16);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            byte[] byteArray = new byte[24];

            Buffer.BlockCopy(BitConverter.GetBytes(X), 0, byteArray, 0, 8);
            Buffer.BlockCopy(BitConverter.GetBytes(Y), 0, byteArray, 8, 8);
            Buffer.BlockCopy(BitConverter.GetBytes(Z), 0, byteArray, 16, 8);

            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(byteArray, 0, 8);
                Array.Reverse(byteArray, 8, 8);
                Array.Reverse(byteArray, 16, 8);
            }

            return byteArray;
        }

        public LLSD ToLLSD()
        {
            LLSDArray array = new LLSDArray();
            array.Add(LLSD.FromReal(X));
            array.Add(LLSD.FromReal(Y));
            array.Add(LLSD.FromReal(Z));
            return array;
        }

        #endregion Public Methods

        #region Static Methods

        public static LLVector3d FromLLSD(LLSD llsd)
        {
            if (llsd.Type == LLSDType.Array)
            {
                LLSDArray array = (LLSDArray)llsd;

                if (array.Count == 3)
                {
                    return new LLVector3d(array[0].AsReal(), array[1].AsReal(), array[2].AsReal());
                }
            }

            return LLVector3d.Zero;
        }

        /// <summary>
        /// Calculates the distance between two vectors
        /// </summary>
        public static double Dist(LLVector3d pointA, LLVector3d pointB)
        {
            double xd = pointB.X - pointA.X;
            double yd = pointB.Y - pointA.Y;
            double zd = pointB.Z - pointA.Z;
            return Math.Sqrt(xd * xd + yd * yd + zd * zd);
        }

        #endregion Static Methods

        #region Overrides

        /// <summary>
        /// A hash of the vector, used by .NET for hash tables
        /// </summary>
        /// <returns>The hashes of the individual components XORed together</returns>
        public override int GetHashCode()
        {
            return (X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public override bool Equals(object o)
        {
            if (!(o is LLVector3d)) return false;

            LLVector3d vector = (LLVector3d)o;

            return (X == vector.X && Y == vector.Y && Z == vector.Z);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("<{0}, {1}, {2}>", X, Y, Z);
        }

        #endregion Overrides

        #region Operators

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator ==(LLVector3d lhs, LLVector3d rhs)
        {
            return (lhs.X == rhs.X && lhs.Y == rhs.Y && lhs.Z == rhs.Z);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator !=(LLVector3d lhs, LLVector3d rhs)
        {
            return !(lhs == rhs);
        }

        #endregion Operators

        /// <summary>An LLVector3d with a value of 0,0,0</summary>
        public static readonly LLVector3d Zero = new LLVector3d();
	}

    /// <summary>
    /// A four-dimensional vector
    /// </summary>
	public struct LLVector4
	{
        /// <summary></summary>
        public float X;
        /// <summary></summary>
        public float Y;
        /// <summary></summary>
        public float Z;
        /// <summary></summary>
        public float S;

        // Used for little to big endian conversion on big endian architectures
        private byte[] conversionBuffer;

        #region Constructors

        /// <summary>
        /// Constructor, sets the vector members according to parameters
        /// </summary>
        /// <param name="x">X value</param>
        /// <param name="y">Y value</param>
        /// <param name="z">Z value</param>
        /// <param name="s">S value</param>
        public LLVector4(float x, float y, float z, float s)
        {
            conversionBuffer = null;
            X = x;
            Y = y;
            Z = z;
            S = s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="byteArray"></param>
        /// <param name="pos"></param>
		public LLVector4(byte[] byteArray, int pos)
		{
            conversionBuffer = null;
            X = Y = Z = S = 0;
            FromBytes(byteArray, pos);
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="byteArray"></param>
        /// <param name="pos"></param>
        public void FromBytes(byte[] byteArray, int pos)
        {
            if (!BitConverter.IsLittleEndian)
            {
                // Big endian architecture
                if (conversionBuffer == null)
                    conversionBuffer = new byte[16];

                Buffer.BlockCopy(byteArray, pos, conversionBuffer, 0, 16);

                Array.Reverse(conversionBuffer, 0, 4);
                Array.Reverse(conversionBuffer, 4, 4);
                Array.Reverse(conversionBuffer, 8, 4);
                Array.Reverse(conversionBuffer, 12, 4);

                X = BitConverter.ToSingle(conversionBuffer, 0);
                Y = BitConverter.ToSingle(conversionBuffer, 4);
                Z = BitConverter.ToSingle(conversionBuffer, 8);
                S = BitConverter.ToSingle(conversionBuffer, 12);
            }
            else
            {
                // Little endian architecture
                X = BitConverter.ToSingle(byteArray, pos);
                Y = BitConverter.ToSingle(byteArray, pos + 4);
                Z = BitConverter.ToSingle(byteArray, pos + 8);
                S = BitConverter.ToSingle(byteArray, pos + 12);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public byte[] GetBytes()
		{
			byte[] byteArray = new byte[16];

            Buffer.BlockCopy(BitConverter.GetBytes(X), 0, byteArray, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(Y), 0, byteArray, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(Z), 0, byteArray, 8, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(S), 0, byteArray, 12, 4);

			if(!BitConverter.IsLittleEndian)
            {
				Array.Reverse(byteArray, 0, 4);
				Array.Reverse(byteArray, 4, 4);
				Array.Reverse(byteArray, 8, 4);
				Array.Reverse(byteArray, 12, 4);
			}

			return byteArray;
        }

        public LLSD ToLLSD()
        {
            LLSDArray array = new LLSDArray();
            array.Add(LLSD.FromReal(X));
            array.Add(LLSD.FromReal(Y));
            array.Add(LLSD.FromReal(Z));
            array.Add(LLSD.FromReal(S));
            return array;
        }

        #endregion Public Methods

        #region Static Methods

        public static LLVector4 FromLLSD(LLSD llsd)
        {
            if (llsd.Type == LLSDType.Array)
            {
                LLSDArray array = (LLSDArray)llsd;

                if (array.Count == 4)
                {
                    return new LLVector4(
                        (float)array[0].AsReal(),
                        (float)array[1].AsReal(),
                        (float)array[2].AsReal(),
                        (float)array[3].AsReal());
                }
            }

            return LLVector4.Zero;
        }

        #endregion Static Methods

        #region Overrides

        /// <summary>
        /// A hash of the vector, used by .NET for hash tables
        /// </summary>
        /// <returns>The hashes of the individual components XORed together</returns>
        public override int GetHashCode()
        {
            return (X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ S.GetHashCode());
        }
         /// <summary>
         /// 
         /// </summary>
         /// <param name="o"></param>
         /// <returns></returns>
        public override bool Equals(object o)
        {
            if (!(o is LLVector4)) return false;

            LLVector4 vector = (LLVector4)o;
            return (X == vector.X && Y == vector.Y && Z == vector.Z && S == vector.S);
        }
        /// <summary>        
        /// 
        /// </summary>
        /// <returns></returns>
		public override string ToString()
		{
            return String.Format("<{0}, {1}, {2}, {3}>", X, Y, Z, S);
        }

        #endregion Overrides

        #region Operators

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator ==(LLVector4 lhs, LLVector4 rhs)
        {
            return (lhs.X == rhs.X && lhs.Y == rhs.Y && lhs.Z == rhs.Z && lhs.S == rhs.S);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator !=(LLVector4 lhs, LLVector4 rhs)
        {
            return !(lhs == rhs);
        }

        public static LLVector4 operator +(LLVector4 lhs, LLVector4 rhs)
        {
            return new LLVector4(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z, lhs.S + rhs.S);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static LLVector4 operator -(LLVector4 lhs, LLVector4 rhs)
        {
            return new LLVector4(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z, lhs.S - rhs.S);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vec"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static LLVector4 operator *(LLVector4 vec, float val)
        {
            return new LLVector4(vec.X * val, vec.Y * val, vec.Z * val, vec.S * val);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static LLVector4 operator *(float val, LLVector4 vec)
        {
            return new LLVector4(vec.X * val, vec.Y * val, vec.Z * val, vec.S * val);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static LLVector4 operator *(LLVector4 lhs, LLVector4 rhs)
        {
            return new LLVector4(lhs.X * rhs.X, lhs.Y * rhs.Y, lhs.Z * rhs.Z, lhs.S * rhs.S);
        }

        #endregion Operators

        /// <summary>An LLVector4 with a value of 0,0,0,0</summary>
        public readonly static LLVector4 Zero = new LLVector4();
	}

    /// <summary>
    /// An 8-bit color structure including an alpha channel
    /// </summary>
    public struct LLColor
    {
        /// <summary>Red</summary>
        public float R;
        /// <summary>Green</summary>
        public float G;
        /// <summary>Blue</summary>
        public float B;
        /// <summary>Alpha</summary>
        public float A;

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        public LLColor(byte r, byte g, byte b, byte a)
        {
            float quanta = 1.0f / 255.0f;

            R = (float)r * quanta;
            G = (float)g * quanta;
            B = (float)b * quanta;
            A = (float)a * quanta;
        }

        public LLColor(float r, float g, float b, float a)
        {
            if (r > 1f || g > 1f || b > 1f || a > 1f)
                SecondLife.LogStatic(
                    String.Format("Attempting to initialize LLColor with out of range values <{0},{1},{2},{3}>",
                    r, g, b, a), Helpers.LogLevel.Warning);

            // Valid range is from 0.0 to 1.0
            R = Helpers.Clamp(r, 0f, 1f);
            G = Helpers.Clamp(g, 0f, 1f);
            B = Helpers.Clamp(b, 0f, 1f);
            A = Helpers.Clamp(a, 0f, 1f);
        }

        public LLColor(byte[] byteArray, int pos, bool inverted)
        {
            const float quanta = 1.0f / 255.0f;

            if (inverted)
            {
                R = (float)(255 - byteArray[pos]) * quanta;
                G = (float)(255 - byteArray[pos + 1]) * quanta;
                B = (float)(255 - byteArray[pos + 2]) * quanta;
                A = (float)(255 - byteArray[pos + 3]) * quanta;
            }
            else
            {
                R = (float)byteArray[pos] * quanta;
                G = (float)byteArray[pos + 1] * quanta;
                B = (float)byteArray[pos + 2] * quanta;
                A = (float)byteArray[pos + 3] * quanta;
            }
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            byte[] byteArray = new byte[4];

            byteArray[0] = Helpers.FloatToByte(R, 0f, 1f);
            byteArray[1] = Helpers.FloatToByte(G, 0f, 1f);
            byteArray[2] = Helpers.FloatToByte(B, 0f, 1f);
            byteArray[3] = Helpers.FloatToByte(A, 0f, 1f);

            return byteArray;
        }

        public byte[] GetInvertedBytes()
        {
            byte[] byteArray = GetBytes();

            byteArray[0] = (byte)(255 - byteArray[0]);
            byteArray[1] = (byte)(255 - byteArray[1]);
            byteArray[2] = (byte)(255 - byteArray[2]);
            byteArray[3] = (byte)(255 - byteArray[3]);

            return byteArray;
        }

        public byte[] GetFloatBytes()
        {
            byte[] bytes = new byte[16];
            Buffer.BlockCopy(BitConverter.GetBytes(R), 0, bytes, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(G), 0, bytes, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(B), 0, bytes, 8, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(A), 0, bytes, 12, 4);
            return bytes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToStringRGB()
        {
            return String.Format("<{0}, {1}, {2}>", R, G, B);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public LLSD ToLLSD()
        {
            LLSDArray array = new LLSDArray();
            array.Add(LLSD.FromReal(R));
            array.Add(LLSD.FromReal(G));
            array.Add(LLSD.FromReal(B));
            array.Add(LLSD.FromReal(A));
            return array;
        }

        #endregion Public Methods

        #region Static Methods

        public static LLColor FromLLSD(LLSD llsd)
        {
            if (llsd.Type == LLSDType.Array)
            {
                LLSDArray array = (LLSDArray)llsd;

                if (array.Count == 4)
                {
                    return new LLColor(
                        (float)array[0].AsReal(),
                        (float)array[1].AsReal(),
                        (float)array[2].AsReal(),
                        (float)array[3].AsReal());
                }
            }

            return LLColor.Black;
        }

        #endregion Static Methods

        #region Overrides

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("<{0}, {1}, {2}, {3}>", R, G, B, A);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is LLColor)
            {
                LLColor c = (LLColor)obj;
                return (R == c.R) && (G == c.G) && (B == c.B) && (A == c.A);
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return R.GetHashCode() ^ G.GetHashCode() ^ B.GetHashCode() ^ A.GetHashCode();
        }

        #endregion Overrides

        #region Operators

        /// <summary>
        /// Comparison operator
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator ==(LLColor lhs, LLColor rhs)
        {
            // Return true if the fields match:
            return lhs.R == rhs.R && lhs.G == rhs.G && lhs.B == rhs.B && lhs.A == rhs.A;
        }

        /// <summary>
        /// Not comparison operator
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator !=(LLColor lhs, LLColor rhs)
        {
            return !(lhs == rhs);
        }

        #endregion Operators

        /// <summary>An LLColor with zero RGB values and full alpha</summary>
        public readonly static LLColor Black = new LLColor(0f, 0f, 0f, 1f);
    }

    /// <summary>
    /// A quaternion, used for rotations
    /// </summary>
	public struct LLQuaternion
	{
        /// <summary>X value</summary>
        public float X;
        /// <summary>Y value</summary>
        public float Y;
        /// <summary>Z value</summary>
        public float Z;
        /// <summary>W value</summary>
        public float W;

        // Used for little to big endian conversion on big endian architectures
        private byte[] conversionBuffer;

        #region Constructors

        /// <summary>
        /// Constructor, builds a quaternion object from a byte array
        /// </summary>
        /// <param name="byteArray">The source byte array</param>
        /// <param name="pos">Offset in the byte array to start reading at</param>
        /// <param name="normalized">Whether the source data is normalized or
        /// not. If this is true 12 bytes will be read, otherwise 16 bytes will
        /// be read.</param>
        public LLQuaternion(byte[] byteArray, int pos, bool normalized)
        {
            conversionBuffer = null;
            X = Y = Z = W = 0;
            FromBytes(byteArray, pos, normalized);
        }

        /// <summary>
        /// Build a quaternion from normalized float values
        /// </summary>
        /// <param name="x">X value from -1.0 to 1.0</param>
        /// <param name="y">Y value from -1.0 to 1.0</param>
        /// <param name="z">Z value from -1.0 to 1.0</param>
        public LLQuaternion(float x, float y, float z)
        {
            conversionBuffer = null;
            X = x;
            Y = y;
            Z = z;

            float xyzsum = 1 - X * X - Y * Y - Z * Z;
            W = (xyzsum > 0) ? (float)Math.Sqrt(xyzsum) : 0;
        }

        /// <summary>
        /// Build a quaternion from individual float values
        /// </summary>
        /// <param name="x">X value</param>
        /// <param name="y">Y value</param>
        /// <param name="z">Z value</param>
        /// <param name="w">W value</param>
        public LLQuaternion(float x, float y, float z, float w)
        {
            conversionBuffer = null;
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Build a quaternion from an angle and a vector
        /// </summary>
        /// <param name="angle">Angle value</param>
        /// <param name="vector">Vector value</param>
        public LLQuaternion(float angle, LLVector3 vector)
        {
            conversionBuffer = null;

            vector = LLVector3.Norm(vector);
            angle *= 0.5f;

            float c = (float)Math.Cos(angle);
            float s = (float)Math.Sin(angle);

            X = vector.X * s;
            Y = vector.Y * s;
            Z = vector.Z * s;
            W = c;

            this = Norm(this);
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Builds a quaternion object from a byte array
        /// </summary>
        /// <param name="byteArray">The source byte array</param>
        /// <param name="pos">Offset in the byte array to start reading at</param>
        /// <param name="normalized">Whether the source data is normalized or
        /// not. If this is true 12 bytes will be read, otherwise 16 bytes will
        /// be read.</param>
        public void FromBytes(byte[] byteArray, int pos, bool normalized)
        {
            if (!normalized)
            {
                if (!BitConverter.IsLittleEndian)
                {
                    // Big endian architecture
                    if (conversionBuffer == null)
                        conversionBuffer = new byte[16];

                    Buffer.BlockCopy(byteArray, pos, conversionBuffer, 0, 16);

                    Array.Reverse(conversionBuffer, 0, 4);
                    Array.Reverse(conversionBuffer, 4, 4);
                    Array.Reverse(conversionBuffer, 8, 4);
                    Array.Reverse(conversionBuffer, 12, 4);

                    X = BitConverter.ToSingle(conversionBuffer, 0);
                    Y = BitConverter.ToSingle(conversionBuffer, 4);
                    Z = BitConverter.ToSingle(conversionBuffer, 8);
                    W = BitConverter.ToSingle(conversionBuffer, 12);
                }
                else
                {
                    // Little endian architecture
                    X = BitConverter.ToSingle(byteArray, pos);
                    Y = BitConverter.ToSingle(byteArray, pos + 4);
                    Z = BitConverter.ToSingle(byteArray, pos + 8);
                    W = BitConverter.ToSingle(byteArray, pos + 12);
                }
            }
            else
            {
                if (!BitConverter.IsLittleEndian)
                {
                    // Big endian architecture
                    if (conversionBuffer == null)
                        conversionBuffer = new byte[16];

                    Buffer.BlockCopy(byteArray, pos, conversionBuffer, 0, 12);

                    Array.Reverse(conversionBuffer, 0, 4);
                    Array.Reverse(conversionBuffer, 4, 4);
                    Array.Reverse(conversionBuffer, 8, 4);

                    X = BitConverter.ToSingle(conversionBuffer, 0);
                    Y = BitConverter.ToSingle(conversionBuffer, 4);
                    Z = BitConverter.ToSingle(conversionBuffer, 8);
                }
                else
                {
                    // Little endian architecture
                    X = BitConverter.ToSingle(byteArray, pos);
                    Y = BitConverter.ToSingle(byteArray, pos + 4);
                    Z = BitConverter.ToSingle(byteArray, pos + 8);
                }

                float xyzsum = 1 - X * X - Y * Y - Z * Z;
                W = (xyzsum > 0) ? (float)Math.Sqrt(xyzsum) : 0;
            }
        }

        /// <summary>
        /// Normalize this quaternion and serialize it to a byte array
        /// </summary>
        /// <returns>A 12 byte array containing normalized X, Y, and Z floating
        /// point values in order using little endian byte ordering</returns>
        public byte[] GetBytes()
        {
            byte[] bytes = new byte[12];
            float norm;

            norm = (float)Math.Sqrt(X * X + Y * Y + Z * Z + W * W);

            if (norm != 0)
            {
                norm = 1 / norm;

                float x, y, z;
                if (W >= 0)
                {
                    x = X; y = Y; z = Z;
                }
                else
                {
                    x = -X; y = -Y; z = -Z;
                }

                Buffer.BlockCopy(BitConverter.GetBytes(norm * x), 0, bytes, 0, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(norm * y), 0, bytes, 4, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(norm * z), 0, bytes, 8, 4);

                if (!BitConverter.IsLittleEndian)
                {
                    Array.Reverse(bytes, 0, 4);
                    Array.Reverse(bytes, 4, 4);
                    Array.Reverse(bytes, 8, 4);
                }
            }
            else
            {
                throw new Exception("Quaternion " + this.ToString() + " normalized to zero");
            }

            return bytes;
        }


        public LLSD ToLLSD()
        {
            LLSDArray array = new LLSDArray();
            array.Add(LLSD.FromReal(X));
            array.Add(LLSD.FromReal(Y));
            array.Add(LLSD.FromReal(Z));
            array.Add(LLSD.FromReal(W));
            return array;
        }

        #endregion Public Methods

        #region Static Methods

        public static LLQuaternion FromLLSD(LLSD llsd)
        {
            if (llsd.Type == LLSDType.Array)
            {
                LLSDArray array = (LLSDArray)llsd;

                if (array.Count == 4)
                {
                    return new LLQuaternion(
                        (float)array[0].AsReal(),
                        (float)array[1].AsReal(),
                        (float)array[2].AsReal(),
                        (float)array[3].AsReal());
                }
            }

            return LLQuaternion.Identity;
        }

        /// <summary>
        /// Calculate the magnitude of the supplied quaternion
        /// </summary>
        public static float Mag(LLQuaternion q)
        {
            return (float)Math.Sqrt(q.W * q.W + q.X * q.X + q.Y * q.Y + q.Z * q.Z);
        }

        /// <summary>
        /// Returns a normalized version of the supplied quaternion
        /// </summary>
        /// <param name="q">The quaternion to normalize</param>
        /// <returns>A normalized version of the quaternion</returns>
        public static LLQuaternion Norm(LLQuaternion q)
        {
            const float MAG_THRESHOLD = 0.0000001f;
            float mag = (float)Math.Sqrt(q.X * q.X + q.Y * q.Y + q.Z * q.Z + q.W * q.W);

            if (mag > MAG_THRESHOLD)
            {
                float oomag = 1.0f / mag;
                q.X *= oomag;
                q.Y *= oomag;
                q.Z *= oomag;
                q.W *= oomag;
            }
            else
            {
                q.X = 0.0f;
                q.Y = 0.0f;
                q.Z = 0.0f;
                q.W = 1.0f;
            }

            return q;
        }

        /// <summary>
        /// Returns the inverse matrix from a quaternion, or the correct
        /// matrix if the quaternion is inverse
        /// </summary>
        /// <param name="q">Quaternion to convert to a matrix</param>
        /// <returns>A matrix representation of the quaternion</returns>
        public static LLMatrix3 GetMatrix(LLQuaternion q)
        {
            LLMatrix3 m;
            float xx, xy, xz, xw, yy, yz, yw, zz, zw;

            xx = q.X * q.X;
            xy = q.X * q.Y;
            xz = q.X * q.Z;
            xw = q.X * q.W;

            yy = q.Y * q.Y;
            yz = q.Y * q.Z;
            yw = q.Y * q.W;

            zz = q.Z * q.Z;
            zw = q.Z * q.W;

            m.M11 = 1f - 2f * (yy + zz);
            m.M12 = 2f * (xy + zw);
            m.M13 = 2f * (xz - yw);

            m.M21 = 2f * (xy - zw);
            m.M22 = 1f - 2f * (xx + zz);
            m.M23 = 2f * (yz + xw);

            m.M31 = 2f * (xz + yw);
            m.M32 = 2f * (yz - xw);
            m.M33 = 1f - 2f * (xx + yy);

            return m;
        }

        public static LLQuaternion SetQuaternion(float angle, float x, float y, float z)
        {
            LLVector3 vec = new LLVector3(x, y, z);
            return SetQuaternion(angle, vec);
        }

        public static LLQuaternion SetQuaternion(float angle, LLVector3 vec)
        {
            LLQuaternion quat = new LLQuaternion();
            vec = LLVector3.Norm(vec);

            angle *= 0.5f;
            float c = (float)Math.Cos(angle);
            float s = (float)Math.Sin(angle);

            quat.X = vec.X * s;
            quat.Y = vec.Y * s;
            quat.Z = vec.Z * s;
            quat.W = c;

            quat = LLQuaternion.Norm(quat);
            return quat;
        }

        #endregion Static Methods

        #region Overrides

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ W.GetHashCode());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public override bool Equals(object o)
        {
            if (!(o is LLQuaternion)) return false;

            LLQuaternion quaternion = (LLQuaternion)o;

            return X == quaternion.X && Y == quaternion.Y && Z == quaternion.Z && W == quaternion.W;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "<" + X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ", " + W.ToString() + ">";
        }

        #endregion Overrides

        #region Operators

        /// <summary>
        /// Comparison operator
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator ==(LLQuaternion lhs, LLQuaternion rhs)
        {
            // Return true if the fields match:
            return lhs.X == rhs.X && lhs.Y == rhs.Y && lhs.Z == rhs.Z && lhs.W == rhs.W;
        }

        /// <summary>
        /// Not comparison operator
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator !=(LLQuaternion lhs, LLQuaternion rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// Multiplication operator
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static LLQuaternion operator *(LLQuaternion lhs, LLQuaternion rhs)
        {
            LLQuaternion ret = new LLQuaternion();
            ret.W = lhs.W * rhs.W - lhs.X * rhs.X - lhs.Y * rhs.Y - lhs.Z * rhs.Z;
            ret.X = lhs.W * rhs.X + lhs.X * rhs.W + lhs.Y * rhs.Z - lhs.Z * rhs.Y;
            ret.Y = lhs.W * rhs.Y + lhs.Y * rhs.W + lhs.Z * rhs.X - lhs.X * rhs.Z;
            ret.Z = lhs.W * rhs.Z + lhs.Z * rhs.W + lhs.X * rhs.Y - lhs.Y * rhs.X;
            return ret;
        }

        #endregion Operators

        /// <summary>An LLQuaternion with a value of 0,0,0,1</summary>
        public readonly static LLQuaternion Identity = new LLQuaternion(0f, 0f, 0f, 1f);
	}

    /// <summary>
    /// 
    /// </summary>
    public struct LLMatrix3
    {
        public float M11, M12, M13;
        public float M21, M22, M23;
        public float M31, M32, M33;

        #region Properties

        public float Trace
        {
            get
            {
                return M11 + M22 + M33;
            }
        }

        public float Determinant
        {
            get
            {
                return M11 * M22 * M33 + M12 * M23 * M31 + M13 * M21 * M32 - 
                       M13 * M22 * M31 - M11 * M23 * M32 - M12 * M21 * M33;
            }
        }

        #endregion Properties

        #region Constructors

        public LLMatrix3(float m11, float m12, float m13, float m21, float m22, float m23, float m31, float m32, float m33)
        {
            M11 = m11;
            M12 = m12;
            M13 = m13;
            M21 = m21;
            M22 = m22;
            M23 = m23;
            M31 = m31;
            M32 = m32;
            M33 = m33;
        }

        public LLMatrix3(LLMatrix3 m)
        {
            M11 = m.M11;
            M12 = m.M12;
            M13 = m.M13;
            M21 = m.M21;
            M22 = m.M22;
            M23 = m.M23;
            M31 = m.M31;
            M32 = m.M32;
            M33 = m.M33;
        }

        public LLMatrix3(LLQuaternion q)
        {
            this = LLQuaternion.GetMatrix(q);
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Transposes this matrix
        /// </summary>
        public void Transpose()
        {
            Helpers.Swap<float>(ref M12, ref M21);
            Helpers.Swap<float>(ref M13, ref M31);
            Helpers.Swap<float>(ref M23, ref M32);
        }

        public void Orthogonalize()
        {
            LLVector3 xAxis = this[0];
            LLVector3 yAxis = this[1];
            LLVector3 zAxis = this[2];

            xAxis = LLVector3.Norm(xAxis);
            yAxis -= xAxis * (xAxis * yAxis);
            yAxis = LLVector3.Norm(yAxis);
            zAxis = LLVector3.Cross(xAxis, yAxis);

            this[0] = xAxis;
            this[1] = yAxis;
            this[2] = zAxis;
        }

        public void GetEulerAngles(out float roll, out float pitch, out float yaw)
        {
            // From the Matrix and Quaternion FAQ: http://www.j3d.org/matrix_faq/matrfaq_latest.html

            double angleX, angleY, angleZ;
            double cx, cy, cz; // cosines
            double sx, sz; // sines

            angleY = Math.Asin(Helpers.Clamp(M31, -1f, 1f));
            cy = Math.Cos(angleY);

            if (Math.Abs(cy) > 0.005f)
            {
                // No gimbal lock
                cx = M33 / cy;
                sx = (-M32) / cy;

                angleX = (float)Math.Atan2(sx, cx);

                cz = M11 / cy;
                sz = (-M21) / cy;

                angleZ = (float)Math.Atan2(sz, cz);
            }
            else
            {
                // Gimbal lock
                angleX = 0;
                
                cz = M22;
                sz = M12;

                angleZ = Math.Atan2(sz, cz);
            }

            roll = (float)angleX;
            pitch = (float)angleY;
            yaw = (float)angleZ;
        }

        #endregion Public Methods

        #region Static Methods

        public static LLMatrix3 Add(LLMatrix3 left, LLMatrix3 right)
        {
            return new LLMatrix3(
                left.M11 + right.M11, left.M12 + right.M12, left.M13 + right.M13,
                left.M21 + right.M21, left.M22 + right.M22, left.M23 + right.M23,
                left.M31 + right.M31, left.M32 + right.M32, left.M33 + right.M33
            );
        }

        public static LLMatrix3 Add(LLMatrix3 matrix, float scalar)
        {
            return new LLMatrix3(
                matrix.M11 + scalar, matrix.M12 + scalar, matrix.M13 + scalar,
                matrix.M21 + scalar, matrix.M22 + scalar, matrix.M23 + scalar,
                matrix.M31 + scalar, matrix.M32 + scalar, matrix.M33 + scalar
            );
        }

        public static LLMatrix3 Subtract(LLMatrix3 left, LLMatrix3 right)
        {
            return new LLMatrix3(
                left.M11 - right.M11, left.M12 - right.M12, left.M13 - right.M13,
                left.M21 - right.M21, left.M22 - right.M22, left.M23 - right.M23,
                left.M31 - right.M31, left.M32 - right.M32, left.M33 - right.M33
                );
        }

        public static LLMatrix3 Subtract(LLMatrix3 matrix, float scalar)
        {
            return new LLMatrix3(
                matrix.M11 - scalar, matrix.M12 - scalar, matrix.M13 - scalar,
                matrix.M21 - scalar, matrix.M22 - scalar, matrix.M23 - scalar,
                matrix.M31 - scalar, matrix.M32 - scalar, matrix.M33 - scalar
                );
        }

        public static LLMatrix3 Multiply(LLMatrix3 left, LLMatrix3 right)
        {
            return new LLMatrix3(
                left.M11 * right.M11 + left.M12 * right.M21 + left.M13 * right.M31,
                left.M11 * right.M12 + left.M12 * right.M22 + left.M13 * right.M32,
                left.M11 * right.M13 + left.M12 * right.M23 + left.M13 * right.M33,

                left.M21 * right.M11 + left.M22 * right.M21 + left.M23 * right.M31,
                left.M21 * right.M12 + left.M22 * right.M22 + left.M23 * right.M32,
                left.M21 * right.M13 + left.M22 * right.M23 + left.M23 * right.M33,

                left.M31 * right.M11 + left.M32 * right.M21 + left.M33 * right.M31,
                left.M31 * right.M12 + left.M32 * right.M22 + left.M33 * right.M32,
                left.M31 * right.M13 + left.M32 * right.M23 + left.M33 * right.M33
            );
        }

        public static LLVector3 Transform(LLVector3 vector, LLMatrix3 matrix)
        {
            // Operates "from the right" on row vector
            return new LLVector3(
                vector.X * matrix.M11 + vector.Y * matrix.M21 + vector.Z * matrix.M31,
                vector.X * matrix.M12 + vector.Y * matrix.M22 + vector.Z * matrix.M32,
                vector.X * matrix.M13 + vector.Y * matrix.M23 + vector.Z * matrix.M33
            );
        }

        public static LLMatrix3 Transpose(LLMatrix3 m)
        {
            LLMatrix3 t = new LLMatrix3(m);
            t.Transpose();
            return t;
        }

        #endregion Static Methods

        #region Overrides

        public override int GetHashCode()
        {
            return
                M11.GetHashCode() ^ M12.GetHashCode() ^ M13.GetHashCode() ^
                M21.GetHashCode() ^ M22.GetHashCode() ^ M23.GetHashCode() ^
                M31.GetHashCode() ^ M32.GetHashCode() ^ M33.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is LLMatrix3)
            {
                LLMatrix3 m = (LLMatrix3)obj;
                return
                    (M11 == m.M11) && (M12 == m.M12) && (M13 == m.M13) &&
                    (M21 == m.M21) && (M22 == m.M22) && (M23 == m.M23) &&
                    (M31 == m.M31) && (M32 == m.M32) && (M33 == m.M33);
            }
            return false;
        }

        public override string ToString()
        {
            return string.Format("[{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}]",
                M11, M12, M13, M21, M22, M23, M31, M32, M33);
        }

        #endregion Overrides

        #region Operators

        public static bool operator ==(LLMatrix3 left, LLMatrix3 right)
        {
            return ValueType.Equals(left, right);
        }

        public static bool operator !=(LLMatrix3 left, LLMatrix3 right)
        {
            return !ValueType.Equals(left, right);
        }

        public static LLMatrix3 operator +(LLMatrix3 left, LLMatrix3 right)
        {
            return LLMatrix3.Add(left, right);
        }

        public static LLMatrix3 operator +(LLMatrix3 matrix, float scalar)
        {
            return LLMatrix3.Add(matrix, scalar);
        }

        public static LLMatrix3 operator +(float scalar, LLMatrix3 matrix)
        {
            return LLMatrix3.Add(matrix, scalar);
        }

        public static LLMatrix3 operator -(LLMatrix3 left, LLMatrix3 right)
        {
            return LLMatrix3.Subtract(left, right); ;
        }

        public static LLMatrix3 operator -(LLMatrix3 matrix, float scalar)
        {
            return LLMatrix3.Subtract(matrix, scalar);
        }

        public static LLMatrix3 operator *(LLMatrix3 left, LLMatrix3 right)
        {
            return LLMatrix3.Multiply(left, right); ;
        }

        public static LLVector3 operator *(LLVector3 vector, LLMatrix3 matrix)
        {
            return LLMatrix3.Transform(vector, matrix);
        }

        public LLVector3 this[int row]
        {
            get
            {
                switch (row)
                {
                    case 0:
                        return new LLVector3(M11, M12, M13);
                    case 1:
                        return new LLVector3(M21, M22, M23);
                    case 2:
                        return new LLVector3(M31, M32, M33);
                    default:
                        throw new IndexOutOfRangeException("LLMatrix3 row index must be from 0-2");
                }
            }
            set
            {
                switch (row)
                {
                    case 0:
                        M11 = value.X;
                        M12 = value.Y;
                        M13 = value.Z;
                        break;
                    case 1:
                        M21 = value.X;
                        M22 = value.Y;
                        M23 = value.Z;
                        break;
                    case 2:
                        M31 = value.X;
                        M32 = value.Y;
                        M33 = value.Z;
                        break;
                    default:
                        throw new IndexOutOfRangeException("LLMatrix3 row index must be from 0-2");
                }
            }
        }

        public float this[int row, int column]
        {
            get
            {
                switch (row)
                {
                    case 0:
                        switch (column)
                        {
                            case 0:
                                return M11;
                            case 1:
                                return M12;
                            case 2:
                                return M13;
                            default:
                                throw new IndexOutOfRangeException("LLMatrix3 row and column values must be from 0-2");
                        }
                    case 1:
                        switch (column)
                        {
                            case 0:
                                return M21;
                            case 1:
                                return M22;
                            case 2:
                                return M23;
                            default:
                                throw new IndexOutOfRangeException("LLMatrix3 row and column values must be from 0-2");
                        }
                    case 2:
                        switch (column)
                        {
                            case 0:
                                return M31;
                            case 1:
                                return M32;
                            case 2:
                                return M33;
                            default:
                                throw new IndexOutOfRangeException("LLMatrix3 row and column values must be from 0-2");
                        }
                    default:
                        throw new IndexOutOfRangeException("LLMatrix3 row and column values must be from 0-2");
                }
            }
            set
            {
                //FIXME:
                throw new NotImplementedException();
            }
        }

        #endregion Operators

        /// <summary>A 3x3 matrix set to all zeroes</summary>
        public static readonly LLMatrix3 Zero = new LLMatrix3();
        /// <summary>A 3x3 identity matrix</summary>
        public static readonly LLMatrix3 Identity = new LLMatrix3(
            1f, 0f, 0f,
            0f, 1f, 0f,
            0f, 0f, 1f
        );
    }
}
