using DualityPlaydate.Component;
using Microsoft.Xna.Framework;

namespace DualityPlaydate.Utils
{
    class CameraUtils
    {
        public static bool IsOnScreen(Vector2 worldLocation, int width, int height, in FollowCamera camera)
        {
            var bounds = ScreenBoundsInWorldCoords(camera);
            return worldLocation.X + width / camera.Zoom > bounds.X &&
                worldLocation.X - width / camera.Zoom < bounds.X + bounds.Width &&
                worldLocation.Y + height / camera.Zoom > bounds.Y &&
                worldLocation.Y - height / camera.Zoom < bounds.Y + bounds.Height;
            /*
            return !(position.X + bounds. < -camera.Offset.X / camera.Zoom / 2 ||
                position.X > camera.Offset.X / camera.Zoom * 2 ||
                position.Y < -camera.Offset.Y / camera.Zoom / 2 ||
                position.Y > camera.Offset.Y / camera.Zoom * 2);
            */
        }

        public static Rectangle ScreenBoundsInWorldCoords(in FollowCamera camera)
        {
            var returnRect = new Rectangle();

            var topLeft = camera.Position - (camera.Offset / camera.Zoom);
            var worldScreenSize = camera.Offset * 2 / camera.Zoom;

            returnRect.Width = (int)worldScreenSize.X;
            returnRect.Height = (int)worldScreenSize.Y;
            returnRect.X = (int)MathHelper.Clamp(topLeft.X, camera.WorldBounds.X, camera.WorldBounds.Width - camera.Offset.X);
            returnRect.Y = (int)MathHelper.Clamp(topLeft.Y, camera.WorldBounds.Y, camera.WorldBounds.Height - camera.Offset.Y);

            return returnRect;
        }

        public static Vector2 SetDrawLocation(FollowCamera camera, ref Vector2 drawLocation)
        {
            drawLocation.X = MathHelper.Clamp(camera.Position.X - (camera.Offset.X / camera.Zoom), camera.WorldBounds.X, camera.WorldBounds.Width);
            drawLocation.Y = MathHelper.Clamp(camera.Position.Y - (camera.Offset.Y / camera.Zoom), camera.WorldBounds.Y, camera.WorldBounds.Height);

            return drawLocation;
        }
    }
}
