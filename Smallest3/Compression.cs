using System;
using System.Runtime.CompilerServices;

using UnityEngine;
using Unity.Mathematics;

/// <summary>Static class for compression extensions related to rotation.</summary>
public static class Compression
{
    /// <summary>Implemented as Smallest 3 (2 bits [largest index] 10 bits per value).</summary>
    /// <param name="rotation">Rotation to convert.</param>
    /// <returns>Rotation as 32 bit unsigned.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static UInt32 ToUInt32(this Quaternion rotation) { return ToUInt32(rotation.x, rotation.y, rotation.z, rotation.w); }
    /// <summary>Implemented as Smallest 3 (2 bits [largest index] 10 bits per value).</summary>
    /// <param name="rotation">Rotation to convert.</param>
    /// <returns>Rotation as 32 bit unsigned.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static UInt32 ToUInt32(this quaternion rotation) { return rotation.value.ToUInt32(); }
    /// <summary>Implemented as Smallest 3 (2 bits [largest index] 10 bits per value).</summary>
    /// <param name="rotation">Rotation to convert.</param>
    /// <returns>Rotation as 32 bit unsigned.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static UInt32 ToUInt32(this float4 rotation) { return ToUInt32(rotation.x, rotation.y, rotation.z, rotation.w); }
    /// <summary>Decompresses a 32 bit unsigned integer and assigns it to the rotation.</summary>
    /// <param name="quaternion">The rotation to which the value will be assigned.</param>
    /// <param name="value">The value to decompress into a rotation.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void FromUInt32(ref this Quaternion quaternion, UInt32 value) { quaternion.FromUInt32(value, (value >> 20).ToSingle(), (value >> 10).ToSingle(), value.ToSingle()); }
    /// <summary>Takes a <see cref="uint"/> and creates a rotation.</summary>
    /// <param name="UInt32">the compressed rotation.</param>
    /// <returns>The uncompressed rotation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static quaternion ToQuaternion(this UInt32 UInt32) { return ToQuaternion(UInt32 >> 30, (UInt32 >> 20).ToSingle(), (UInt32 >> 10).ToSingle(), UInt32.ToSingle()); }
    /// <summary>Takes a <see cref="uint"/> and creates a rotation.</summary>
    /// <param name="UInt32">the compressed rotation.</param>
    /// <returns>The uncompressed rotation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float4 ToFloat4(this UInt32 UInt32) { return ToFloat4(UInt32 >> 30, (UInt32 >> 20).ToSingle(), (UInt32 >> 10).ToSingle(), UInt32.ToSingle()); }
    /// <summary>Compressed to 2 Bytes per channel.</summary>
    /// <param name="rotation">The rotation to compress.</param>
    /// <returns>The rotation as an unsigned 64 bit integer.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static UInt64 ToUInt64(this Quaternion rotation) { return ToUInt64(rotation.x, rotation.y, rotation.z, rotation.w); }
    /// <summary>Compressed to 2 Bytes per channel.</summary>
    /// <param name="rotation">The rotation to compress.</param>
    /// <returns>The rotation as an unsigned 64 bit integer.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static UInt64 ToUInt64(this quaternion rotation) { return rotation.value.ToUInt64(); }
    /// <summary>Compressed to 2 Bytes per channel.</summary>
    /// <param name="rotation">The rotation to compress.</param>
    /// <returns>The rotation as an unsigned 64 bit integer.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static UInt64 ToUInt64(this float4 rotation) { return ToUInt64(rotation.x, rotation.y, rotation.z, rotation.w); }
    /// <summary>Takes a <see cref="UInt64"/> and creates a rotation.</summary>
    /// <param name="UInt32">the compressed rotation.</param>
    /// <returns>The uncompressed rotation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static quaternion ToQuaternion(this UInt64 UInt64) { return ToQuaternion(UInt64 >> 48, UInt64 >> 32, UInt64 >> 16, UInt64); }
    /// <summary>Takes a <see cref="UInt64"/> and creates a rotation.</summary>
    /// <param name="UInt32">the compressed rotation.</param>
    /// <returns>The uncompressed rotation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float4 ToFloat4(this UInt64 UInt64) { return ToFloat4(UInt64 >> 48, UInt64 >> 32, UInt64 >> 16, UInt64); }
    /// <summary>Converts a 64 bit unsigned and assigns it to the rotation.</summary>
    /// <param name="quaternion">The rotation to which the value will be assigned.</param>
    /// <param name="value">The value to decompress into a rotation.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void FromUInt64(ref this Quaternion quaternion, UInt64 value)
    {
        quaternion.x = (value >> 48).ToSingle();
        quaternion.y = (value >> 32).ToSingle();
        quaternion.z = (value >> 16).ToSingle();
        quaternion.w = value.ToSingle();
    }
    /// <summary>Helper for squaring a value.</summary>
    /// <param name="value">The value to square</param>
    /// <returns>The squared value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Sq(float value) { return math.pow(value, 2); }
    /// <summary>Helper for getting the square root of a value.</summary>
    /// <param name="value">The value from which to find the root.</param>
    /// <returns>The square root of the value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Sqrt(float value) { return math.sqrt(value); }
    /// <summary>Helper for getting the absolute value of a number.</summary>
    /// <param name="value">The number from which to find the absolute value.</param>
    /// <returns>The absolute value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Abs(float value) { return math.abs(value); }

    static void FromUInt32(ref this Quaternion quaternion, UInt32 largest, float a, float b, float c)
    {
        switch (largest & 0xC0000000)
        {
            case xIndex: { quaternion.FromUInt32(X(a, b, c), a, b, c); } break;
            case yIndex: { quaternion.FromUInt32(a, Y(a, b, c), b, c); } break;
            case zIndex: { quaternion.FromUInt32(b, c, Z(b, c, a), a); } break;
            case wIndex: { quaternion.FromUInt32(b, c, a, W(b, c, a)); } break;
        }
    }
    static void FromUInt32(ref this Quaternion quaternion, float x, float y, float z, float w)
    {
        quaternion.x = x;
        quaternion.y = y;
        quaternion.z = z;
        quaternion.w = w;
    }

    static quaternion ToQuaternion(UInt32 largest, float a, float b, float c) { return 2 > largest ? ToQuaternionXY(largest, a, b, c) : ToQuaternionZW(largest, a, b, c); }
    static float4 ToFloat4(UInt32 largest, float a, float b, float c) { return 2 > largest ? ToFloat4XY(largest, a, b, c) : ToFloat4ZW(largest, a, b, c); }
    static quaternion ToQuaternionXY(UInt32 largest, float a, float b, float c) { return 0 == largest ? new quaternion(X(a, b, c), a, b, c) : new quaternion(a, Y(a, b, c), b, c); }
    static float4 ToFloat4XY(UInt32 largest, float a, float b, float c) { return 0 == largest ? new float4(X(a, b, c), a, b, c) : new float4(a, Y(a, b, c), b, c); }
    static quaternion ToQuaternionZW(UInt32 largest, float a, float b, float c) { return 2 == largest ? new quaternion(b, c, Z(b, c, a), a) : new quaternion(b, c, a, W(b, c, a)); }
    static float4 ToFloat4ZW(UInt32 largest, float a, float b, float c) { return 2 == largest ? new float4(b, c, Z(b, c, a), a) : new float4(b, c, a, W(b, c, a)); }

    static float X(float y, float z, float w) { return Sqrt(1.0F - (Sq(y) + Sq(z) + Sq(w))); }
    static float Y(float x, float z, float w) { return Sqrt(1.0F - (Sq(x) + Sq(z) + Sq(w))); }
    static float Z(float x, float y, float w) { return Sqrt(1.0F - (Sq(x) + Sq(y) + Sq(w))); }
    static float W(float x, float y, float z) { return Sqrt(1.0F - (Sq(x) + Sq(y) + Sq(z))); }

    static UInt32 ToUInt32(float x, float y, float z, float w) { return ToUInt32(LargestIndexAbs(x, y, z, w), x, y, z, w); }
    static UInt32 ToUInt32(UInt32 largest, float x, float y, float z, float w) { return 2 > largest ? ToUInt32XY(largest, x, y, z, w) : ToUInt32ZW(largest, x, y, z, w); }
    static UInt32 ToUInt32XY(UInt32 largest, float x, float y, float z, float w) { return 0 == largest ? ToUInt32(xIndex, y, z, w) : ToUInt32(yIndex, x, z, w); }
    static UInt32 ToUInt32ZW(UInt32 largest, float x, float y, float z, float w) { return 2 == largest ? ToUInt32(zIndex, x, y, w) : ToUInt32(wIndex, x, y, z); }

    static UInt32 ToUInt32(UInt32 largest, float a, float b, float c) { return largest | a.ToUInt32() << 20 | b.ToUInt32() << 10 | c.ToUInt32(); }
    static UInt32 ToUInt32(this float value) { return Mask10b(Convert.ToUInt32(Map10b(value))); }

    static UInt32 LargestIndexAbs(float a, float b, float c, float d) { return LargestIndex(Abs(a), Abs(b), Abs(c), Abs(d)); }
    static UInt32 LargestIndex(float a, float b, float c, float d) { return a > d ? LargestIndex(a, b, c) : LargestIndex(b, c, d) + 1; }
    static UInt32 LargestIndex(float a, float b, float c) { return a >= c ? LargestIndex(a, b) : LargestIndex(b, c) + 1; }
    static UInt32 LargestIndex(float a, float b) { unchecked { return a >= b ? (UInt32)0 : (UInt32)1; } }

    static quaternion ToQuaternion(UInt64 x, UInt64 y, UInt64 z, UInt64 w) { return new quaternion(x.ToSingle(), y.ToSingle(), z.ToSingle(), w.ToSingle()); }
    static float4 ToFloat4(UInt64 x, UInt64 y, UInt64 z, UInt64 w) { return new float4(x.ToSingle(), y.ToSingle(), z.ToSingle(), w.ToSingle()); }

    static float ToSingle(this UInt64 UInt64) { return Unmap2B(Convert.ToSingle(Mask2B(UInt64))); }
    static float ToSingle(this UInt32 UInt32) { return Unmap10b(Convert.ToSingle(Mask10b(UInt32))); }

    static UInt64 ToUInt64(float x, float y, float z, float w) { return x.ToUInt64() << 48 | y.ToUInt64() << 32 | z.ToUInt64() << 16 | w.ToUInt64(); }
    static UInt64 ToUInt64(this float value) { return Mask2B(Convert.ToUInt64(Map2B(value))); }

    static float Map(float value, float offset, float scale, float range) { return (value + offset) * scale * range; }
    static float Map(float value, float range) { return (value + 1) * 0.5F * range; }
    static float Map2B(float value) { return Map(value, maximum2B); }
    static float Map10b(float value) { return Map(value, oneOverSqrt2, oneOverSqrt2, maximum10b); }
    
    static float Unmap(float value, float oneOverRange, float oneOverScale, float inverseOffset) { return value * oneOverRange * oneOverScale + inverseOffset; }
    static float Unmap(float value, float oneOverRange) { return value * oneOverRange * 2 - 1; }
    static float Unmap2B(float value) { return Unmap(value, oneOver2B); }
    static float Unmap10b(float value) { return Unmap(value, oneOver1024, sqrt2, -oneOverSqrt2); }

    static UInt64 Mask2B(UInt64 value) { return value & mask2B; }
    static UInt32 Mask10b(UInt32 value) { return value & mask10b; }

    const UInt64 mask2B = 0xFFFF;
    const UInt32 mask10b = 0x3FF;

    const float maximum2B = 65536;
    const float oneOver2B = 0.0000152587890625F;

    const float maximum10b = 1024;
    const float oneOver1024 = 0.0009765625F;

    const float sqrt2 = 1.4142135623730950488016887242097F;
    const float oneOverSqrt2 = 0.70710678118654746F;

    const UInt32 xIndex = 0b0000_0000_0000_0000;
    const UInt32 yIndex = 0b0100_0000_0000_0000;
    const UInt32 zIndex = 0b1000_0000_0000_0000;
    const UInt32 wIndex = 0b1100_0000_0000_0000;
}
