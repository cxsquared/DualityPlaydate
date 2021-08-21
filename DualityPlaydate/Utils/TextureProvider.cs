using DefaultEcs;
using DefaultEcs.Resource;
using DualityPlaydate.Component;
using Microsoft.Xna.Framework.Graphics;

namespace DualityPlaydate.Utils
{
    // TInfo is string, the name of the texture and TResource is Texture2D
    public sealed class TextureResourceManager : AResourceManager<string, Texture2D>
    {
        private readonly GraphicsDevice _device;
        private readonly ITextureLoader _loader;

        // ITextureLoader is the actual loader, not shown here
        public TextureResourceManager(GraphicsDevice device, ITextureLoader loader)
        {
            _device = device;
            _loader = loader;
        }

        // this will only be called if the texture with the key info has never been loaded yet or it has previously been disposed because it was not used anymore
        protected override Texture2D Load(string info) => _loader.Load(_device, info);

        // this is the callback method where the entity with the ManagedResource<string, Texture2D> component is set, the TInfo and the resource are given do act as needed
        protected override void OnResourceLoaded(in Entity entity, string info, Texture2D resource)
        {
            // here we just set the texture to a field of an other component of the entity which contains all the information needed to draw it (position, size, origin, rotation, texture, ...)
            entity.Get<DrawInfo>().Texture = resource;
        }
    }
}
