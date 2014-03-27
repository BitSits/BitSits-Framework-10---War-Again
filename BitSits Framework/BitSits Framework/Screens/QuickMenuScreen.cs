﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BitSits_Framework
{
    class QuickMenuScreen : GameScreen
    {
        Camera2D camera = new Camera2D();


        public override void HandleInput(InputState input)
        {
            PlayerIndex playerIndex;
            if (input.IsMenuSelect(ControllingPlayer, out playerIndex)
                || input.IsMouseLeftButtonClick())
            {
                LoadingScreen.Load(ScreenManager, false, PlayerIndex.One,
                    new GameplayScreen());//, new PauseMenuScreen());

                ExitScreen();
            }
        }


        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            // Darken down any other screens that were drawn beneath the popup.
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 1 / 3);


            // Fade the popup alpha during transitions.
            Color color = Color.White * TransitionAlpha;

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.Transform);

            spriteBatch.Draw(ScreenManager.GameContent.menuBackground, Vector2.Zero, color);

            spriteBatch.End();
        }
    }
}
