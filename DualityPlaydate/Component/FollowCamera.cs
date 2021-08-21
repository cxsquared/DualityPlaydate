using Microsoft.Xna.Framework;

namespace DualityPlaydate.Component
{
    public struct FollowCamera
    {
        public Vector2 Position;
        public Vector2 Offset;
        public int MaxWorldX;
        public int MinWorldX;
        public float FollowDampening;

        // This follows the transform of what ever entity it's on
        // If the entity doesn't have a transform this camera won't change from
        // the default position.
        public FollowCamera(int screenWidth, int screenHeight)
        {
            Position = Vector2.Zero;
            Offset = new Vector2(screenWidth / 2, screenHeight / 2);
            MaxWorldX = 1000;
            MinWorldX = 0;
            FollowDampening = 0.05f;
        }
    }
}
