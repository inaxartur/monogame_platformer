using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGame_Platformer
{
    internal class Sprite
    {
        public Texture2D texture;
        public Vector2 position;
        public Rectangle rect;
        public Vector2 velocity;

        public Sprite(Texture2D texture, Rectangle rect)
        {
            this.texture = texture;
            this.rect = rect;
            velocity = new();
        }

        public virtual void Update(GameTime gameTime)
        {
            // Default implementation does nothing
        }
        public virtual void SetGroundedTrue()
        {
            // Default implementation does nothing
        }
        
    }
}