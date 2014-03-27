using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Box2D.XNA;

namespace BitSits_Framework
{
    class Bazooka : Soldier
    {
        public Bazooka(Shape shape, Vector2 position, GameContent gameContent, World world)
            : base(shape, position, gameContent, world)
        {

        }
    }
}
