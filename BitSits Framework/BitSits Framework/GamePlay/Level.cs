using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;
using System.IO;
using Box2D.XNA;
using GameDataLibrary;

namespace BitSits_Framework
{
    class Level : IDisposable
    {
        #region Fields


        public int Score { get; private set; }

        public bool IsLevelUp { get; private set; }
        public bool ReloadLevel { get; private set; }
        int levelIndex;

        GameContent gameContent;
        LevelData levelData;

        World world = new World(new Vector2(0, 2), false);

        List<Soldier> soldiers = new List<Soldier>();
        public static Soldier selectedSoldier = null;
        Soldier mouseOverSoldier;

        #endregion

        #region Initialization


        public Level(ScreenManager screenManager, int levelIndex)
        {
            this.gameContent = screenManager.GameContent;
            this.levelIndex = levelIndex;

            Load();
        }


        private void Load()
        {
            levelData = gameContent.content.Load<LevelData>("Levels/level" + levelIndex);

            soldiers.Add(new Soldier(Shape.Triangle, new Vector2(702, 185), gameContent, world));
            soldiers.Add(new Soldier(Shape.Triangle, new Vector2(752, 185), gameContent, world));
            soldiers.Add(new Soldier(Shape.Triangle, new Vector2(400, 160), gameContent, world));
            soldiers.Add(new Soldier(Shape.Triangle, new Vector2(440, 171), gameContent, world));
            soldiers.Add(new Soldier(Shape.Square, new Vector2(109, 316), gameContent, world));
            soldiers.Add(new Soldier(Shape.Square, new Vector2(346, 444), gameContent, world));

            PolygonShape pShape = new PolygonShape();

            BodyDef bd = new BodyDef();
            bd.type = BodyType.Static;

            for (int i = 0; i < levelData.Rectangles.Count; i++)
            {
                GroundData g = levelData.Rectangles[i];
                Rectangle r = new Rectangle(0, 0, g.Width, g.Height);
                pShape.SetAsBox((float)r.Width / 2 / gameContent.b2Scale, (float)r.Height / 2 / gameContent.b2Scale);

                bd.position = g.Position / gameContent.b2Scale;
                
                Body b = world.CreateBody(bd);
                b.CreateFixture(pShape, 1);
                b.Rotation = g.Rotation / 180 * (float)Math.PI;
            }
        }


        public void Dispose() { }


        #endregion

        #region Update and HandleInput


        public void Update(GameTime gameTime)
        {
            world.Step((float)gameTime.ElapsedGameTime.TotalSeconds, 10, 10);

            for (int i = 0; i < soldiers.Count; i++) soldiers[i].Update(gameTime);
        }


        public void HandleInput(InputState input, int playerIndex)
        {
            Vector2 mousePos = new Vector2(input.CurrentMouseState.X, input.CurrentMouseState.Y);
            Point mouseP = new Point(input.CurrentMouseState.X, input.CurrentMouseState.Y);

            mouseOverSoldier = null;
            for (int i = 0; i < soldiers.Count; i++)
            {
                if (selectedSoldier == null && soldiers[i].BoundingRect.Contains(mouseP) 
                    && input.LastMouseState.LeftButton == ButtonState.Released)
                    mouseOverSoldier = soldiers[i];
            }

            if (input.IsMouseLeftButtonClick())
            {
                selectedSoldier = null;
                if (mouseOverSoldier != null) selectedSoldier = mouseOverSoldier;
            }

            if (input.LastMouseState.LeftButton == ButtonState.Pressed
                && input.CurrentMouseState.LeftButton == ButtonState.Pressed)
            {
                if (selectedSoldier != null) selectedSoldier.ShowMove(mousePos);
            }

            if (input.LastMouseState.LeftButton == ButtonState.Pressed
                && input.CurrentMouseState.LeftButton == ButtonState.Released)
            {
                if (selectedSoldier != null && !selectedSoldier.BoundingRect.Contains(mouseP))
                    selectedSoldier.Move(mousePos);
            }
        }


        #endregion

        #region Draw


        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(gameContent.grid, Vector2.Zero, Color.White);

            spriteBatch.Draw(gameContent.bg0, Vector2.Zero, Color.White);

            spriteBatch.Draw(gameContent.bg1, Vector2.Zero, Color.White);

            for (int i = 0; i < levelData.Rectangles.Count; i++)
            {
                GroundData g = levelData.Rectangles[i];
                spriteBatch.Draw(gameContent.blank, new Rectangle((int)g.Position.X, (int)g.Position.Y, g.Width, g.Height), null,
                    Color.Black * .2f, g.Rotation / 180 * (float)Math.PI, new Vector2(.5f), SpriteEffects.None, 1);
            }

            if (mouseOverSoldier != null && mouseOverSoldier != selectedSoldier) mouseOverSoldier.DrawSelected(spriteBatch);
            if (selectedSoldier != null) selectedSoldier.DrawSelected(spriteBatch);

            for (int i = 0; i < soldiers.Count; i++) soldiers[i].DrawHealth(spriteBatch);
            for (int i = 0; i < soldiers.Count; i++) soldiers[i].Draw(spriteBatch, gameTime);

            spriteBatch.Draw(gameContent.water, new Rectangle(0, levelData.WaterLevel, gameContent.water.Width,
                (int)Camera2D.BaseScreenSize.Y - levelData.WaterLevel), Color.White);

#if DEBUG
            for (Joint j = world.GetJointList(); j != null; j = j.GetNext())
            {
                float len = Vector2.Distance(j.GetAnchorA(), j.GetAnchorB()) * gameContent.b2Scale;
                float theta = (float)Math.Atan((j.GetAnchorB().Y - j.GetAnchorA().Y) / (j.GetAnchorB().X - j.GetAnchorA().X));

                spriteBatch.Draw(gameContent.blank, Vector2.Add(j.GetAnchorA(), j.GetAnchorB()) / 2 * gameContent.b2Scale,
                    null, Color.Red, theta, new Vector2(.5f), new Vector2(len, 1), SpriteEffects.None, 1);
            }
#endif
        }


        #endregion
    }
}
