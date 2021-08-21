using Microsoft.Xna.Framework.Graphics;

namespace DualityPlaydate
{
    public interface ITextureLoader
    {
        Texture2D Load(GraphicsDevice device, string info);
    }
}
