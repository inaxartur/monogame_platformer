using Microsoft.Xna.Framework;

namespace MonoGame_Platformer
{
    internal interface IEntity
    {
        void Update(GameTime gameTime);
        void TakeDamage();
        void SetGroundedTrue();
    }
}