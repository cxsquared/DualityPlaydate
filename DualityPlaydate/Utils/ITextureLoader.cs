using Microsoft.Xna.Framework.Graphics;

namespace DualityPlaydate.Utils
{
    public interface ITextureLoader
    {
        Texture2D Load(GraphicsDevice device, string info);
    }
}
