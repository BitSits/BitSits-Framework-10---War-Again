using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GameDataLibrary
{
    public struct GroundData
    {
        public Vector2 Position;
        public int Width, Height;
        public float Rotation;
    }

    public class LevelData
    {
        public List<GroundData> Rectangles, Triangles, Circles;

        public List<Vector2> Ground;

        public int WaterLevel;
    }
}
