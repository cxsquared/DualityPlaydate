using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DualityPlaydate.Utils
{
    class TextureLoader : ITextureLoader
    {
        readonly ContentManager Content;

        public TextureLoader(ContentManager content)
        {
            Content = content;
        }

        public Texture2D Load(GraphicsDevice device, string info) => Content.Load<Texture2D>(info);
    }
}
