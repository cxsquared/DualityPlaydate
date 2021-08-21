namespace DualityPlaydate.Utils
{
    static class AnimationUtils
    {
        public static int FpsToMs(int fps)
        {
            return (int)(1.0f / fps * 1000.0f / 1.0f);
        }
    }
}
