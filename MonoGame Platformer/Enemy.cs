using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame_Platformer
{
    internal class Enemy : Sprite, IEntity
    {
        private int _health;
        private float _speed;
        private int _counter = 0;
        private int _duration;
        public Enemy(Texture2D texture, float speed, Rectangle rect, int duration) : base(texture, rect)
        {
            _speed = speed;
            _duration = duration;
            _health = 1;
        }

        public override void SetGroundedTrue()
        {
            // do nothing for enemies
        }

        public void TakeDamage()
        {
            _health--;
            if (_health <= 0)
            {
                this.rect = new Rectangle(-100, -100, 0, 0); // Move the enemy off-screen
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            rect.X += (int)(_speed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            _counter++;
            if (_counter >= _duration)
            {
                _speed = -_speed; // Reverse direction
                _counter = 0;
            }
        }
    }
}