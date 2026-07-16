using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGame_Platformer
{
    internal class Player : Sprite, IEntity
    {
        public const int TILESIZE = 64;
        private int _health;
        private float _speed;
        private bool _isGrounded;
        private bool _isJumping = false;
        public Player(Texture2D texture, Rectangle rect, float speed) : base(texture, rect)
        {
            _speed = speed;
            _health = 2;
        }

        public void TakeDamage()
        {
            _health--;
            if (_health <= 0)
            {
                _health = 2; // Reset health if died
                rect = new Rectangle(0, 0, TILESIZE, TILESIZE);  //reset pos if died
            }
        }

        public override void SetGroundedTrue()
        {
            _isGrounded = true;
            _isJumping = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            velocity = Vector2.Zero; // Reset velocity each frame

            // horizontal movement
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                velocity.X = -_speed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                velocity.X = _speed;
            }

            // vertical movement
            velocity.Y = 5; // gravity
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && _isGrounded)
            {
 //               rect.Y += -(int)_speed * 20; // jump v1


                _isGrounded = false; // player is no longer grounded after jumping
                _isJumping = true;
            }
            HandleJump();
        }

        private readonly int _jumpHeight = 20;
        private int _jumpCounter = 21;
        private void HandleJump()
        {
            if (_isJumping)
            {
                _jumpCounter--;
                if (_jumpCounter > 0)
                {
                    velocity.Y = -(int)(Math.Sqrt(_jumpCounter * 4) ); // jump v2
                }
                else
                {
                    _isJumping = false;
                    _jumpCounter = _jumpHeight; // reset jump counter for next jump
                }
            }
        }
    }
}