﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DualityPlaydate.Component
{
    public struct DrawInfo
    {
        public Color Color;
        public Texture2D Texture;
        public Rectangle SourceLocation;
        public Vector2 SourceOrigin;
        public float ZIndex; 

        public DrawInfo(Texture2D texture, Color color, Rectangle locaiton, Vector2 origin)
        {
            Texture = texture;
            Color = color;
            SourceLocation = locaiton;
            SourceOrigin = origin;
            ZIndex = default;
        }
    }
}