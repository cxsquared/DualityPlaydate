using System;

namespace DualityPlaydate.Utils
{
    static class NoiseUtils
    {
        public static float[] GenerateMapNoice(int width, int height, int seed)
        {
            var whiteNoise = GenerateWhiteNoise(width, height, seed);
            var smoothNoise = GenerateSmoothNoise(whiteNoise, width, height, 2);
            return GeneratePerlinNoise(smoothNoise, width, height, 2);
        }

        public static float[] GenerateWhiteNoise(int width, int height, int seed)
        {
            Random random = new(seed); //Seed to 0 for testing
            var arraySize = width * height;
            float[] noise = new float[arraySize];

            for (int i = 0; i < arraySize; i++)
            {
                noise[i] = (float)random.NextDouble() % 1;
            }

            return noise;
        }

        public static float[] GenerateSmoothNoise(float[] baseNoise, int width, int height, int octave)
        {
            float[] smoothNoise = new float[baseNoise.Length];

            int samplePeriod = 1 << octave; // calculates 2 ^ k
            float sampleFrequency = 1.0f / samplePeriod;

            for (int i = 0; i < smoothNoise.Length; i++)
            {
                var x = i % width;
                var y = i / width;

                //calculate the horizontal sampling indices
                int sample_x0 = (x / samplePeriod) * samplePeriod;
                int sample_x1 = (sample_x0 + samplePeriod) % width; //wrap around
                float horizontal_blend = (x - sample_x0) * sampleFrequency;

                //calculate the vertical sampling indices
                int sample_y0 = (y / samplePeriod) * samplePeriod;
                int sample_y1 = (sample_y0 + samplePeriod) % height; //wrap around
                float vertical_blend = (y - sample_y0) * sampleFrequency;

                //blend the top two corners
                float top = Interpolate(baseNoise[sample_y0 * width + sample_x0],
                   baseNoise[sample_y0 * width + sample_x1], horizontal_blend);

                //blend the bottom two corners
                float bottom = Interpolate(baseNoise[sample_y1 * width + sample_x0],
                      baseNoise[sample_y1 * width + sample_x0], horizontal_blend);

                //final blend
                smoothNoise[i] = Interpolate(top, bottom, vertical_blend);
            }

            return smoothNoise;
        }

        public static float[] GeneratePerlinNoise(float[] baseNoise, int width, int height, int octaveCount)
        {
            var smoothNoise = new float[octaveCount][]; //an array of 2D arrays containing

            float persistance = 0.5f;

            //generate smooth noise
            for (int i = 0; i < octaveCount; i++)
            {
                smoothNoise[i] = GenerateSmoothNoise(baseNoise, width, height, i);
            }

            var perlinNoise = new float[baseNoise.Length];
            float amplitude = 1.0f;
            float totalAmplitude = 0.0f;

            //blend noise together
            for (int octave = octaveCount - 1; octave >= 0; octave--)
            {
                amplitude *= persistance;
                totalAmplitude += amplitude;

                for (int i = 0; i < perlinNoise.Length; i++)
                {
                    perlinNoise[i] += smoothNoise[octave][i] * amplitude;
                }
            }

            //normalisation
            for (int i = 0; i < perlinNoise.Length; i++)
            {
                perlinNoise[i] /= totalAmplitude;
            }

            return perlinNoise;
        }

        static float Interpolate(float x0, float x1, float alpha)
        {
            return x0 * (1 - alpha) + alpha * x1;
        }
    }
}
