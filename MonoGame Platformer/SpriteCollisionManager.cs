using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame_Platformer
{
    internal static class SpriteCollisionManager
    {
        public static void CheckCollisionsForEntities(List<Sprite> sprites)
        {
            // Check each sprite against every other sprite
            for (int i = 0; i < sprites.Count; i++)
            {
                for (int j = i + 1; j < sprites.Count; j++)
                {
                    // Only check collision if both are IEntity
                    if (sprites[i] is IEntity entity1 && sprites[j] is IEntity entity2)
                    {
                        if (sprites[i].rect.Intersects(sprites[j].rect))
                        {
                            entity1.TakeDamage();
                            entity2.TakeDamage();
                        }
                    }
                }
            }
        }
    }
}
