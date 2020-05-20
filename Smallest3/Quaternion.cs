using System;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

using Unity.Mathematics;

namespace Smallest3
{
    public static class Quaternion
    {
        public static async Task<uint> NormalizeAndCompressAsync(quaternion quaternion) { return await Task.Run(() => NormalizeAndCompress(quaternion)); }
        public static async Task<uint> CompressAsync(quaternion quaternion) { return await Task.Run(() => Compress(quaternion)); }

        public static uint NormalizeAndCompress(quaternion quaternion) { return Compress(Normalize(quaternion)); }
        public static uint Compress(quaternion quaternion) { return Compress(quaternion.value, LargestAbsoluteIndex(quaternion.value)); }

        public static async Task<quaternion> DecompressAsync(uint compressed) { return await Task.Run(() => Decompress(compressed)); }
        public static async Task<quaternion> DecompressAndNormalizeAsync(uint compressed) { return await Task.Run(() => DecompressAndNormalize(compressed)); }

        public static quaternion Decompress(uint compressed) { return Decompress((int)(compressed >> 30), compressed); }
        public static quaternion DecompressAndNormalize(uint compressed) { return Normalize(Decompress(compressed)); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Sq(float value) { return math.pow(value, 2); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Sqrt(float value) { return math.sqrt(value); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Abs(float value) { return math.abs(value); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static quaternion Normalize(quaternion value) { return math.normalize(value); }

        // Find largest index
        [MethodImpl(MethodImplOptions.AggressiveInlining)] static uint LargestAbsoluteIndex(float4 value) { return LargestIndex(Abs(value.x), Abs(value.y), Abs(value.z), Abs(value.w)); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] static uint LargestIndex(float x, float y, float z, float w) { return x > y ? LargestIndex(x, z, w, 0, 2, 3) : LargestIndex(y, z, w, 1, 2, 3); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] static uint LargestIndex(float a, float b, float c, uint i, uint j, uint k) { return a > b ? (a > c ? i : k) : (b > c ? j : k); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] static uint Compress(float4 value, uint largest) { return 2 > largest ? CompressXY(value, largest) : CompressZW(value, largest); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] static uint CompressXY(float4 value, uint largest) { return 0 == largest ? CompressYZW(value) : CompressXZW(value); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] static uint CompressZW(float4 value, uint largest) { return 2 == largest ? CompressXYW(value) : CompressXYZ(value); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] static uint CompressYZW(float4 value) { return CompressYZW(value.y, value.z, value.w, GetScale(value, 0)); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] static uint CompressXZW(float4 value) { return CompressXZW(value.x, value.z, value.w, GetScale(value, 1)); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] static uint CompressXYW(float4 value) { return CompressXYW(value.x, value.y, value.w, GetScale(value, 2)); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] static uint CompressXYZ(float4 value) { return CompressXYZ(value.x, value.y, value.z, GetScale(value, 3)); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] static uint CompressYZW(float y, float z, float w, float scale) { return CompressYZW(scale * y, scale * z, scale * w); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] static uint CompressXZW(float x, float z, float w, float scale) { return CompressXZW(scale * x, scale * z, scale * w); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] static uint CompressXYW(float x, float y, float w, float scale) { return CompressXYW(scale * x, scale * y, scale * w); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] static uint CompressXYZ(float x, float y, float z, float scale) { return CompressXYZ(scale * x, scale * y, scale * z); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] static uint CompressYZW(float y, float z, float w) { return CompressLargestIndex(0) | Compress1st(y) | Compress2nd(z) | Compress3rd(w); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] static uint CompressXZW(float x, float z, float w) { return CompressLargestIndex(1) | Compress1st(x) | Compress2nd(z) | Compress3rd(w); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] static uint CompressXYW(float x, float y, float w) { return CompressLargestIndex(2) | Compress1st(w) | Compress2nd(x) | Compress3rd(y); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] static uint CompressXYZ(float x, float y, float z) { return CompressLargestIndex(3) | Compress1st(z) | Compress2nd(x) | Compress3rd(y); }

        // Compress individual component
        [MethodImpl(MethodImplOptions.AggressiveInlining)] static uint CompressLargestIndex(uint index) { return (0x3 & index) << 30; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] static uint Compress1st(float value) { return Mask(Convert.ToUInt32(Scale0To1024(value))) << 20; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] static uint Compress2nd(float value) { return Mask(Convert.ToUInt32(Scale0To1024(value))) << 10; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] static uint Compress3rd(float value) { return Mask(Convert.ToUInt32(Scale0To1024(value))); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] static float GetScale(float4 value, uint index) { return 0.0F > value[(int)index] ? -1 : 1; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] static float Scale0To1024(float value) { return (value + oneOverSqrt2) * oneOverSqrt2 * 1024.0F; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] static uint Mask(uint value) { return value & mask1024; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] static quaternion Decompress(int largest, uint compressed) { return 2 > largest ? DecompressXY(largest, compressed) : DecompressZW(largest, compressed); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] static quaternion DecompressXY(int largest, uint compressed) { return 0 == largest ? DecompressYZW(compressed) : DecompressXZW(compressed); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] static quaternion DecompressZW(int largest, uint compressed) { return 2 == largest ? DecompressXYW(compressed) : DecompressXYZ(compressed); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] static quaternion DecompressYZW(uint compressed) { return DecompressYZW(Decompress1st(compressed), Decompress2nd(compressed), Decompress3rd(compressed)); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] static quaternion DecompressXZW(uint compressed) { return DecompressXZW(Decompress1st(compressed), Decompress2nd(compressed), Decompress3rd(compressed)); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] static quaternion DecompressXYW(uint compressed) { return DecompressXYW(Decompress2nd(compressed), Decompress3rd(compressed), Decompress1st(compressed)); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] static quaternion DecompressXYZ(uint compressed) { return DecompressXYZ(Decompress2nd(compressed), Decompress3rd(compressed), Decompress1st(compressed)); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] static quaternion DecompressYZW(float y, float z, float w) { return new quaternion(X(y,z,w), y, z, w); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] static quaternion DecompressXZW(float x, float z, float w) { return new quaternion(x, Y(x, z, w), z, w); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] static quaternion DecompressXYW(float x, float y, float w) { return new quaternion(x, y, Z(x, y, w), w); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] static quaternion DecompressXYZ(float x, float y, float z) { return new quaternion(x, y, z, W(x,y,z)); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] static float X(float y, float z, float w) { return Sqrt(1.0F - (Sq(y) + Sq(z) + Sq(w))); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] static float Y(float x, float z, float w) { return Sqrt(1.0F - (Sq(x) + Sq(z) + Sq(w))); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] static float Z(float x, float y, float w) { return Sqrt(1.0F - (Sq(x) + Sq(y) + Sq(w))); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] static float W(float x, float y, float z) { return Sqrt(1.0F - (Sq(x) + Sq(y) + Sq(z))); }

        static float Decompress1st(uint compressed) { return Decompress((float)((compressed >> 20) & mask1024)); }
        static float Decompress2nd(uint compressed) { return Decompress((float)((compressed >> 10) & mask1024)); }
        static float Decompress3rd(uint compressed) { return Decompress((float)(compressed & mask1024)); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] static float Decompress(float value) { return value * oneOver1024BySqrt2 - oneOverSqrt2; }

        // Constants
        const uint mask1024 = 0x3FF;
        const float oneOverSqrt2 = 0.70710678118654746F;
        const float oneOver1024BySqrt2 = 0.00138106793200497563359539914474F;
    }
}
