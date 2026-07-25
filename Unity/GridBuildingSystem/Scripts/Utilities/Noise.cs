using System;
using UnityEngine;

namespace Noise
{
    /// <summary>
    /// Implementation of the Perlin simplex noise, an improved Perlin noise algorithm.
    /// Based loosely on SimplexNoise1234 by Stefan Gustavson: http://staffwww.itn.liu.se/~stegu/aqsis/aqsis-newnoise/
    /// </summary>
    public static class SimplexNoise
    {
        // Simplex Noise for C#
        // Copyright © Benjamin Ward 2019
        // See LICENSE
        // Simplex Noise implementation offering 1D, 2D, and 3D forms w/ values in the range of 0 to 255.
        // Based on work by Heikki Törmälä (2012) and Stefan Gustavson (2006).

        /// <summary>
        /// Creates 1D Simplex noise
        /// </summary>
        /// <param name="width">The number of points to generate</param>
        /// <param name="scale">The scale of the noise. The greater the scale, the denser the noise gets</param>
        /// <returns>An array containing 1D Simplex noise. All the values are in the range [-1,1]</returns>
        public static float[] Calc1D(int width, float scale)
        {
            var values = new float[width];
            for (var i = 0; i < width; i++)
                values[i] = Generate(i, scale);
            return values;
        }

        /// <summary>
        /// Creates 2D Simplex noise
        /// </summary>
        /// <param name="width">The number of points to generate in the 1st dimension</param>
        /// <param name="height">The number of points to generate in the 2nd dimension</param>
        /// <param name="scale">The scale of the noise. The greater the scale, the denser the noise gets</param>
        /// <returns>An array containing 2D Simplex noise. All the values are in the range [-1,1]</returns>
        public static float[,] Calc2D(int width, int height, float scale)
        {
            var values = new float[width, height];
            for (var i = 0; i < width; i++)
                for (var j = 0; j < height; j++)
                    values[i, j] = Generate(i, j, scale);
            return values;
        }

        /// <summary>
        /// Creates 3D Simplex noise
        /// </summary>
        /// <param name="width">The number of points to generate in the 1st dimension</param>
        /// <param name="height">The number of points to generate in the 2nd dimension</param>
        /// <param name="length">The number of points to generate in the 3nd dimension</param>
        /// <param name="scale">The scale of the noise. The greater the scale, the denser the noise gets</param>
        /// <returns>An array containing 3D Simplex noise. All the values are in the range [-1,1]</returns>
        public static float[,,] Calc3D(int width, int height, int length, float scale)
        {
            var values = new float[width, height, length];
            for (var i = 0; i < width; i++)
                for (var j = 0; j < height; j++)
                    for (var k = 0; k < length; k++)
                        values[i, j, k] = Generate(i, j, k, scale);
            return values;
        }

        /// <summary>
        /// Gets the value of an index of 1D simplex noise
        /// </summary>
        /// <param name="x">Index</param>
        /// <param name="scale">The scale of the noise. The greater the scale, the denser the noise gets</param>
        /// <returns>The value of an index of 1D simplex noise. The value is in the range [-1,1]</returns>
        public static float CalcPixel1D(float x, float scale)
        {
            return Generate(x, scale);
        }

        /// <summary>
        /// Gets the value of an index of 2D simplex noise
        /// </summary>
        /// <param name="x">1st dimension index</param>
        /// <param name="y">2st dimension index</param>
        /// <param name="scale">The scale of the noise. The greater the scale, the denser the noise gets</param>
        /// <returns>The value of an index of 2D simplex noise. The value is in the range [-1,1]</returns>
        public static float CalcPixel2D(float x, float y, float scale)
        {
            return Generate(x, y, scale);
        }


        /// <summary>
        /// Gets the value of an index of 3D simplex noise
        /// </summary>
        /// <param name="x">1st dimension index</param>
        /// <param name="y">2nd dimension index</param>
        /// <param name="z">3rd dimension index</param>
        /// <param name="scale">The scale of the noise. The greater the scale, the denser the noise gets</param>
        /// <returns>The value of an index of 3D simplex noise. The value is in the range [-1,1]</returns>
        public static float CalcPixel3D(float x, float y, float z, float scale)
        {
            return Generate(x, y, z, scale);
        }

        static SimplexNoise()
        {
            _perm = new byte[PermOriginal.Length];
            PermOriginal.CopyTo(_perm, 0);
        }

        /// <summary>
        /// Arbitrary integer seed used to generate lookup table used internally
        /// </summary>
        public static int Seed
        {
            get => _seed;
            set
            {
                if (value == 0)
                {
                    _perm = new byte[PermOriginal.Length];
                    PermOriginal.CopyTo(_perm, 0);
                }
                else
                {
                    _perm = new byte[512];
                    var random = new System.Random(value);
                    random.NextBytes(_perm);
                }

                _seed = value;
            }
        }

        private static int _seed;

        /// <summary>
        /// 1D simplex noise
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float Generate(float x, in float scale = 1)
        {
            x *= scale;
            var i0 = FastFloor(x);
            var i1 = i0 + 1;
            var x0 = x - i0;
            var x1 = x0 - 1.0f;

            var t0 = 1.0f - (x0 * x0);
            t0 *= t0;
            var n0 = t0 * t0 * Grad(_perm[i0 & 0xff], x0);

            var t1 = 1.0f - (x1 * x1);
            t1 *= t1;
            var n1 = t1 * t1 * Grad(_perm[i1 & 0xff], x1);
            // The maximum value of this noise is 8*(3/4)^4 = 2.53125
            // A factor of 0.395 scales to fit exactly within [-1,1]
            return 0.395f * (n0 + n1);
        }

        /// <summary>
        /// 2D simplex noise
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static float Generate(float x, float y, in float scale = 1)
        {
            x *= scale;
            y *= scale;

            const float F2 = 0.366025403f; // F2 = 0.5*(sqrt(3.0)-1.0)
            const float G2 = 0.211324865f; // G2 = (3.0-Math.sqrt(3.0))/6.0

            float n0, n1, n2; // Noise contributions from the three corners

            // Skew the input space to determine which simplex cell we're in
            var s = (x + y) * F2; // Hairy factor for 2D
            var xs = x + s;
            var ys = y + s;
            var i = FastFloor(xs);
            var j = FastFloor(ys);

            var t = (i + j) * G2;
            var X0 = i - t; // Unskew the cell origin back to (x,y) space
            var Y0 = j - t;
            var x0 = x - X0; // The x,y distances from the cell origin
            var y0 = y - Y0;

            // For the 2D case, the simplex shape is an equilateral triangle.
            // Determine which simplex we are in.
            int i1, j1; // Offsets for second (middle) corner of simplex in (i,j) coords
            if (x0 > y0) { i1 = 1; j1 = 0; } // lower triangle, XY order: (0,0)->(1,0)->(1,1)
            else { i1 = 0; j1 = 1; }      // upper triangle, YX order: (0,0)->(0,1)->(1,1)

            // A step of (1,0) in (i,j) means a step of (1-c,-c) in (x,y), and
            // a step of (0,1) in (i,j) means a step of (-c,1-c) in (x,y), where
            // c = (3-sqrt(3))/6

            var x1 = x0 - i1 + G2; // Offsets for middle corner in (x,y) unskewed coords
            var y1 = y0 - j1 + G2;
            var x2 = x0 - 1.0f + (2.0f * G2); // Offsets for last corner in (x,y) unskewed coords
            var y2 = y0 - 1.0f + (2.0f * G2);

            // Wrap the integer indices at 256, to avoid indexing perm[] out of bounds
            var ii = Mod(i, 256);
            var jj = Mod(j, 256);

            // Calculate the contribution from the three corners
            var t0 = 0.5f - (x0 * x0) - (y0 * y0);
            if (t0 < 0.0f) n0 = 0.0f;
            else
            {
                t0 *= t0;
                n0 = t0 * t0 * Grad(_perm[ii + _perm[jj]], x0, y0);
            }

            var t1 = 0.5f - (x1 * x1) - (y1 * y1);
            if (t1 < 0.0f) n1 = 0.0f;
            else
            {
                t1 *= t1;
                n1 = t1 * t1 * Grad(_perm[ii + i1 + _perm[jj + j1]], x1, y1);
            }

            var t2 = 0.5f - (x2 * x2) - (y2 * y2);
            if (t2 < 0.0f) n2 = 0.0f;
            else
            {
                t2 *= t2;
                n2 = t2 * t2 * Grad(_perm[ii + 1 + _perm[jj + 1]], x2, y2);
            }

            // Add contributions from each corner to get the final noise value.
            // The result is scaled to return values in the interval [-1,1].
            return 40.0f * (n0 + n1 + n2); // TODO: The scale factor is preliminary!
        }

        public static float Generate(float x, float y, float z, in float scale = 1)
        {
            x *= scale;
            y *= scale;
            z *= scale;

            // Simple skewing factors for the 3D case
            const float F3 = 0.333333333f;
            const float G3 = 0.166666667f;

            float n0, n1, n2, n3; // Noise contributions from the four corners

            // Skew the input space to determine which simplex cell we're in
            var s = (x + y + z) * F3; // Very nice and simple skew factor for 3D
            var xs = x + s;
            var ys = y + s;
            var zs = z + s;
            var i = FastFloor(xs);
            var j = FastFloor(ys);
            var k = FastFloor(zs);

            var t = (i + j + k) * G3;
            var X0 = i - t; // Unskew the cell origin back to (x,y,z) space
            var Y0 = j - t;
            var Z0 = k - t;
            var x0 = x - X0; // The x,y,z distances from the cell origin
            var y0 = y - Y0;
            var z0 = z - Z0;

            // For the 3D case, the simplex shape is a slightly irregular tetrahedron.
            // Determine which simplex we are in.
            int i1, j1, k1; // Offsets for second corner of simplex in (i,j,k) coords
            int i2, j2, k2; // Offsets for third corner of simplex in (i,j,k) coords

            /* This code would benefit from a backport from the GLSL version! */
            if (x0 >= y0)
            {
                if (y0 >= z0)
                { i1 = 1; j1 = 0; k1 = 0; i2 = 1; j2 = 1; k2 = 0; } // X Y Z order
                else if (x0 >= z0) { i1 = 1; j1 = 0; k1 = 0; i2 = 1; j2 = 0; k2 = 1; } // X Z Y order
                else { i1 = 0; j1 = 0; k1 = 1; i2 = 1; j2 = 0; k2 = 1; } // Z X Y order
            }
            else
            { // x0<y0
                if (y0 < z0) { i1 = 0; j1 = 0; k1 = 1; i2 = 0; j2 = 1; k2 = 1; } // Z Y X order
                else if (x0 < z0) { i1 = 0; j1 = 1; k1 = 0; i2 = 0; j2 = 1; k2 = 1; } // Y Z X order
                else { i1 = 0; j1 = 1; k1 = 0; i2 = 1; j2 = 1; k2 = 0; } // Y X Z order
            }

            // A step of (1,0,0) in (i,j,k) means a step of (1-c,-c,-c) in (x,y,z),
            // a step of (0,1,0) in (i,j,k) means a step of (-c,1-c,-c) in (x,y,z), and
            // a step of (0,0,1) in (i,j,k) means a step of (-c,-c,1-c) in (x,y,z), where
            // c = 1/6.

            var x1 = x0 - i1 + G3; // Offsets for second corner in (x,y,z) coords
            var y1 = y0 - j1 + G3;
            var z1 = z0 - k1 + G3;
            var x2 = x0 - i2 + (2.0f * G3); // Offsets for third corner in (x,y,z) coords
            var y2 = y0 - j2 + (2.0f * G3);
            var z2 = z0 - k2 + (2.0f * G3);
            var x3 = x0 - 1.0f + (3.0f * G3); // Offsets for last corner in (x,y,z) coords
            var y3 = y0 - 1.0f + (3.0f * G3);
            var z3 = z0 - 1.0f + (3.0f * G3);

            // Wrap the integer indices at 256, to avoid indexing perm[] out of bounds
            var ii = Mod(i, 256);
            var jj = Mod(j, 256);
            var kk = Mod(k, 256);

            // Calculate the contribution from the four corners
            var t0 = 0.6f - (x0 * x0) - (y0 * y0) - (z0 * z0);
            if (t0 < 0.0f) n0 = 0.0f;
            else
            {
                t0 *= t0;
                n0 = t0 * t0 * Grad(_perm[ii + _perm[jj + _perm[kk]]], x0, y0, z0);
            }

            var t1 = 0.6f - (x1 * x1) - (y1 * y1) - (z1 * z1);
            if (t1 < 0.0f) n1 = 0.0f;
            else
            {
                t1 *= t1;
                n1 = t1 * t1 * Grad(_perm[ii + i1 + _perm[jj + j1 + _perm[kk + k1]]], x1, y1, z1);
            }

            var t2 = 0.6f - (x2 * x2) - (y2 * y2) - (z2 * z2);
            if (t2 < 0.0f) n2 = 0.0f;
            else
            {
                t2 *= t2;
                n2 = t2 * t2 * Grad(_perm[ii + i2 + _perm[jj + j2 + _perm[kk + k2]]], x2, y2, z2);
            }

            var t3 = 0.6f - (x3 * x3) - (y3 * y3) - (z3 * z3);
            if (t3 < 0.0f) n3 = 0.0f;
            else
            {
                t3 *= t3;
                n3 = t3 * t3 * Grad(_perm[ii + 1 + _perm[jj + 1 + _perm[kk + 1]]], x3, y3, z3);
            }

            // Add contributions from each corner to get the final noise value.
            // The result is scaled to stay just inside [-1,1]
            return 32.0f * (n0 + n1 + n2 + n3); // TODO: The scale factor is preliminary!
        }

        private static byte[] _perm;

        private static readonly byte[] PermOriginal = {
            151,160,137,91,90,15,
            131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
            190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
            88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
            77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
            102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
            135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
            5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
            223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
            129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
            251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
            49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
            138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180,
            151,160,137,91,90,15,
            131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
            190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
            88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
            77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
            102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
            135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
            5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
            223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
            129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
            251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
            49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
            138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
        };

        private static int FastFloor(float x)
        {
            return (x > 0) ? ((int)x) : (((int)x) - 1);
        }

        private static int Mod(int x, int m)
        {
            var a = x % m;
            return a < 0 ? a + m : a;
        }

        private static float Grad(int hash, float x)
        {
            var h = hash & 15;
            var grad = 1.0f + (h & 7);   // Gradient value 1.0, 2.0, ..., 8.0
            if ((h & 8) != 0) grad = -grad;         // Set a random sign for the gradient
            return grad * x;           // Multiply the gradient with the distance
        }

        private static float Grad(int hash, float x, float y)
        {
            var h = hash & 7;      // Convert low 3 bits of hash code
            var u = h < 4 ? x : y;  // into 8 simple gradient directions,
            var v = h < 4 ? y : x;  // and compute the dot product with (x,y).
            return ((h & 1) != 0 ? -u : u) + ((h & 2) != 0 ? -2.0f * v : 2.0f * v);
        }

        private static float Grad(int hash, float x, float y, float z)
        {
            var h = hash & 15;     // Convert low 4 bits of hash code into 12 simple
            var u = h < 8 ? x : y; // gradient directions, and compute dot product.
            var v = h < 4 ? y : h == 12 || h == 14 ? x : z; // Fix repeats at h = 12 to 15
            return ((h & 1) != 0 ? -u : u) + ((h & 2) != 0 ? -v : v);
        }

        private static float Grad(int hash, float x, float y, float z, float t)
        {
            var h = hash & 31;      // Convert low 5 bits of hash code into 32 simple
            var u = h < 24 ? x : y; // gradient directions, and compute dot product.
            var v = h < 16 ? y : z;
            var w = h < 8 ? z : t;
            return ((h & 1) != 0 ? -u : u) + ((h & 2) != 0 ? -v : v) + ((h & 4) != 0 ? -w : w);
        }
    }

    public abstract class Perlin<GradientType>
    {
        //The function we use to smooth the interpolation between the
        //different corners of the cube. With a linear interpolation
        //we'll get hard edges.
        private Func<float, float> SmoothingFunction;

        //PermutationTable, shortened for readability
        protected int[] PT;
        //the defaultPermutationTable is 512 ints long and an contains values 0..255
        private static int[] defaultPermutationTable = {151,160,137,91,90,15,
            131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
            190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
            88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
            77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
            102,143,54,65,25,63,161,1,216,80,73,209,76,132,187,208,89,18,169,200,196,
            135,130,116,188,159,86,164,100,109,198,173,186,3,64,52,217,226,250,124,123,
            5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
            223,183,170,213,119,248,152,2,44,154,163,70,221,153,101,155,167,43,172,9,
            129,22,39,253,19,98,108,110,79,113,224,232,178,185,112,104,218,246,97,228,
            251,34,242,193,238,210,144,12,191,179,162,241,81,51,145,235,249,14,239,107,
            49,192,214,31,181,199,106,157,184,84,204,176,115,121,50,45,127,4,150,254,
            138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180,
             151,160,137,91,90,15,
            131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
            190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
            88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
            77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
            102,143,54,65,25,63,161,1,216,80,73,209,76,132,187,208,89,18,169,200,196,
            135,130,116,188,159,86,164,100,109,198,173,186,3,64,52,217,226,250,124,123,
            5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
            223,183,170,213,119,248,152,2,44,154,163,70,221,153,101,155,167,43,172,9,
            129,22,39,253,19,98,108,110,79,113,224,232,178,185,112,104,218,246,97,228,
            251,34,242,193,238,210,144,12,191,179,162,241,81,51,145,235,249,14,239,107,
            49,192,214,31,181,199,106,157,184,84,204,176,115,121,50,45,127,4,150,254,
            138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
        };

        private GradientType[] gradients;
        //Function that performs the dot product (inner product) of two 3D vectors
        //where one of the vectors is stored in the GradientType type.
        private Func<GradientType, float, float, float, float> Dot;

        protected Perlin(GradientType[] gradients, Func<GradientType, float, float, float, float> dot, Func<float, float> smoothingFunction)
        {
            this.gradients = gradients;
            Dot = dot;
            SmoothingFunction = smoothingFunction;
            PT = defaultPermutationTable;
        }

        /// <summary>
        /// Standard Perlin Noise function, returns smooth noise in the range (-1,1)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="between0And1"></param>
        /// <returns></returns>
        public float Noise(float x, float y = 0.5f, float z = 0.5f, in float scale = 1, bool between0And1 = false)
        {
            x *= scale;
            y *= scale;
            z *= scale;

            //determine what cube we are in
            int cubeX = ((int)x) & ((PT.Length / 2) - 1);
            int cubeY = ((int)y) & ((PT.Length / 2) - 1);
            int cubeZ = ((int)z) & ((PT.Length / 2) - 1);

            /*Find the gradients for the 8 corners of the cube

                        *V011---------*V111
                        |\            |\
                        | \           | \
                        |  \          |  \
                        |   *V010---------*V110
                    V001*---|---------*V101 
                         \  |          \  |
                          \ |           \ |
                           \|            \|
                        V000*-------------*V100
            **/
            int XIndex = PT[cubeX] + cubeY;
            int X1Index = PT[cubeX + 1] + cubeY;
            //indexes for the gradients
            GradientType V000 = gradients[PT[PT[XIndex] + cubeZ] % gradients.Length];
            GradientType V001 = gradients[PT[PT[XIndex] + cubeZ + 1] % gradients.Length];
            GradientType V010 = gradients[PT[PT[XIndex + 1] + cubeZ] % gradients.Length];
            GradientType V011 = gradients[PT[PT[XIndex + 1] + cubeZ + 1] % gradients.Length];
            GradientType V100 = gradients[PT[PT[X1Index] + cubeZ] % gradients.Length];
            GradientType V101 = gradients[PT[PT[X1Index] + cubeZ + 1] % gradients.Length];
            GradientType V110 = gradients[PT[PT[X1Index + 1] + cubeZ] % gradients.Length];
            GradientType V111 = gradients[PT[PT[X1Index + 1] + cubeZ + 1] % gradients.Length];

            //calculate the local x, y and z coordinates (0..1)
            x -= Mathf.Floor(x);
            y -= Mathf.Floor(y);
            z -= Mathf.Floor(z);

            //calculate dot products
            float V000Dot = Dot(V000, x, y, z);
            float V001Dot = Dot(V001, x, y, z - 1);
            float V010Dot = Dot(V010, x, y - 1, z);
            float V011Dot = Dot(V011, x, y - 1, z - 1);
            float V100Dot = Dot(V100, x - 1, y, z);
            float V101Dot = Dot(V101, x - 1, y, z - 1);
            float V110Dot = Dot(V110, x - 1, y - 1, z);
            float V111Dot = Dot(V111, x - 1, y - 1, z - 1);

            //calculate smoothed x, y and z values. These are used to get
            //a smoother interpolation between the dot products of the 
            //gradients and local coords
            float smoothedX = SmoothingFunction(x);
            float smoothedY = SmoothingFunction(y);
            float smoothedZ = SmoothingFunction(z);

            //linearly interpolate the dot products
            float V000V100Val = LinearlyInterpolate(V000Dot, V100Dot, smoothedX);
            float V001V101Val = LinearlyInterpolate(V001Dot, V101Dot, smoothedX);
            float V010V110Val = LinearlyInterpolate(V010Dot, V110Dot, smoothedX);
            float V011V111Val = LinearlyInterpolate(V011Dot, V111Dot, smoothedX);

            float ZZeroPlaneVal = LinearlyInterpolate(V000V100Val, V010V110Val, smoothedY);
            float ZOnePlaneVal = LinearlyInterpolate(V001V101Val, V011V111Val, smoothedY);

            float finalValue = LinearlyInterpolate(ZZeroPlaneVal, ZOnePlaneVal, smoothedZ);
            if (between0And1) finalValue = (finalValue * 0.5f) + 0.5f;

            return finalValue;
        }

        //Tile Perlin Noise function, the noise is tiled over a region of tileRegion^3
        public float NoiseTiled(float x, float y = 0.5f, float z = 0.5f, int tileRegion = 2)
        {
            int cubeX = ((int)x) & ((PT.Length / 2) - 1);
            int cubeY = ((int)y) & ((PT.Length / 2) - 1);
            int cubeZ = ((int)z) & ((PT.Length / 2) - 1);
            int XIndex = PT[cubeX % tileRegion] + (cubeY % tileRegion);
            int X1Index = PT[(cubeX + 1) % tileRegion] + (cubeY % tileRegion);
            int XIndex1 = PT[cubeX % tileRegion] + ((cubeY + 1) % tileRegion);
            int X1Index1 = PT[(cubeX + 1) % tileRegion] + ((cubeY + 1) % tileRegion);
            GradientType V000 = gradients[PT[PT[XIndex] + (cubeZ % tileRegion)] % gradients.Length];
            GradientType V001 = gradients[PT[PT[XIndex] + ((cubeZ + 1) % tileRegion)] % gradients.Length];
            GradientType V010 = gradients[PT[PT[XIndex1] + (cubeZ % tileRegion)] % gradients.Length];
            GradientType V011 = gradients[PT[PT[XIndex1] + ((cubeZ + 1) % tileRegion)] % gradients.Length];
            GradientType V100 = gradients[PT[PT[X1Index] + (cubeZ % tileRegion)] % gradients.Length];
            GradientType V101 = gradients[PT[PT[X1Index] + ((cubeZ + 1) % tileRegion)] % gradients.Length];
            GradientType V110 = gradients[PT[PT[X1Index1] + (cubeZ % tileRegion)] % gradients.Length];
            GradientType V111 = gradients[PT[PT[X1Index1] + ((cubeZ + 1) % tileRegion)] % gradients.Length];
            x -= Mathf.Floor(x);
            y -= Mathf.Floor(y);
            z -= Mathf.Floor(z);
            float V000Dot = Dot(V000, x, y, z);
            float V001Dot = Dot(V001, x, y, z - 1);
            float V010Dot = Dot(V010, x, y - 1, z);
            float V011Dot = Dot(V011, x, y - 1, z - 1);
            float V100Dot = Dot(V100, x - 1, y, z);
            float V101Dot = Dot(V101, x - 1, y, z - 1);
            float V110Dot = Dot(V110, x - 1, y - 1, z);
            float V111Dot = Dot(V111, x - 1, y - 1, z - 1);
            float smoothedX = SmoothingFunction(x);
            float smoothedY = SmoothingFunction(y);
            float smoothedZ = SmoothingFunction(z);
            float V000V100Val = LinearlyInterpolate(V000Dot, V100Dot, smoothedX);
            float V001V101Val = LinearlyInterpolate(V001Dot, V101Dot, smoothedX);
            float V010V110Val = LinearlyInterpolate(V010Dot, V110Dot, smoothedX);
            float V011V111Val = LinearlyInterpolate(V011Dot, V111Dot, smoothedX);
            float ZZeroPlaneVal = LinearlyInterpolate(V000V100Val, V010V110Val, smoothedY);
            float ZOnePlaneVal = LinearlyInterpolate(V001V101Val, V011V111Val, smoothedY);
            return LinearlyInterpolate(ZZeroPlaneVal, ZOnePlaneVal, smoothedZ);
        }

        //creates noise combined of multiple noise values at different octaves
        public float NoiseOctaves(float x, float y, float z = 0.5f,
            int numOctaves = 6, float lacunarity = 2f, float persistence = 0.5f)
        {
            float noiseValue = 0f;
            float amp = 1f;
            float freq = 1f;
            float totalAmp = 0f;

            for (int i = 0; i < numOctaves; i++)
            {
                noiseValue += amp * Noise(x * freq, y * freq, z * freq);
                totalAmp += amp;
                amp *= persistence;
                freq *= lacunarity;
            }

            return noiseValue / totalAmp;
        }

        //creates tiled noise with multiple octaves
        public float NoiseTiledOctaves(float x, float y, float z, int tileRegion = 2,
            int numOctaves = 6, float lacunarity = 2f, float persistence = 0.5f)
        {
            float noiseValue = 0f;
            float amp = 1f;
            float freq = 1f;
            float totalAmp = 0f;

            for (int i = 0; i < numOctaves; i++)
            {
                noiseValue += amp * NoiseTiled(x * freq, y * freq, z * freq, tileRegion);
                totalAmp += amp;
                amp *= persistence;
                freq *= lacunarity;
            }

            return noiseValue / totalAmp;
        }

        //use a different permutationTable then the provided default.
        //This will change the look of the noise
        public void SetPermutationTable(int[] newPermutationTable)
        {
            //make sure the new PT has Length = 2^N (this property is 
            //used in the Noise function)
            if ((newPermutationTable.Length & (newPermutationTable.Length - 1)) == 0)
            {
                PT = newPermutationTable;
            }
        }

        private static float LinearlyInterpolate(float valueA, float valueB, float t)
        {
            return valueA + (t * (valueB - valueA));
        }

        //Takes a val in the range 0..1 and returns an s-curve
        //in the range 0..1
        //This is a recommended replacement for the original 3t^2 - 2t^3
        //from https://mrl.nyu.edu/~perlin/paper445.pdf
        protected static float SmoothToSCurve(float val)
        {
            return val * val * val * ((val * ((val * 6f) - 15f)) + 10f);
        }
    }

    public class Perlin : Perlin<Perlin.Vector3>
    {

        private static Vector3[] gradients =
            {
                new Vector3(1,1,0), new Vector3(-1,1,-0),
                new Vector3(1,-1,0), new Vector3(-1,-1,0), new Vector3(1,0,1),
                new Vector3(-1,0,1), new Vector3(1,0,-1), new Vector3(-1,0,-1),
                new Vector3(0,1,1), new Vector3(0,-1,1), new Vector3(0,1,-1),
                new Vector3(0,-1,-1)
        };

        public Perlin(Func<float, float> smoothingFunction) : base(gradients, Dot, smoothingFunction) { }

        public Perlin() : this(SmoothToSCurve) { }

        private static float Dot(Vector3 gradient, float x, float y, float z)
        {
            return (gradient.x * x) + (gradient.y * y) + (gradient.z * z);
        }

        public struct Vector3
        {
            public float x, y, z;

            public Vector3(float x, float y, float z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }
        }
    }
}
