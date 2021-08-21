namespace DualityPlaydate.Component
{
    public struct Animation
    {
        public string Name;
        public int[] Frames;
        public int CurrentFrame;
        public float AnimationTime; // maybe precalculate this and compare to overal ticks instead of 
        public int MsPerFrame;
        public bool Flip;
        public bool Looped;
    }
}
