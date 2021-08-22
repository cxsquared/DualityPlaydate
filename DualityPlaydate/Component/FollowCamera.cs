using Microsoft.Xna.Framework;

namespace DualityPlaydate.Component
{
    public struct FollowCamera
    {
        public Vector2 Position;
        public Vector2 Offset;
        public Rectangle WorldBounds;
        public float FollowDampening;
        public float Zoom;

        // This follows the transform of what ever entity it's on
        // If the entity doesn't have a transform this camera won't change from
        // the default position.
        public FollowCamera(int screenWidth, int screenHeight)
        {
            Position = Vector2.Zero;
            Offset = new Vector2(screenWidth / 2, screenHeight / 2);
            WorldBounds = new Rectangle(0, 0, 1000, 1000);
            FollowDampening = 0.05f;
            Zoom = 1;
        }
    }
}
