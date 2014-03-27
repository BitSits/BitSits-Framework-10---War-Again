using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Box2D.XNA;

namespace BitSits_Framework
{
    class AirBombs : Soldier
    {
        public AirBombs(Shape shape, Vector2 position, GameContent gameContent, World world)
            : base(shape, position, gameContent, world)
        {

        }

        public override void Update(GameTime gameTime)
        {

            //base.Update(gameTime);
        }
    }
}
