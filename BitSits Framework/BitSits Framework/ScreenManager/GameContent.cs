using System;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GameDataLibrary;

namespace BitSits_Framework
{
    /// <summary>
    /// All the Contents of the Game is loaded and stored here
    /// so that all other screen can copy from here
    /// </summary>
    public class GameContent
    {
        public ContentManager content;

        public Random random = new Random();

        public readonly float b2Scale = 30;

        // Textures
        public Texture2D blank;
        public Texture2D menuBackground;

        public Texture2D grid;

        public Texture2D[] idle = new Texture2D[2], walk = new Texture2D[2];

        public Texture2D moveArrow;
        public Texture2D ring, ringDashed;
        public Vector2 ringOrigin;

        public Texture2D bg0, bg1;

        public Texture2D water;

        // Fonts
        public SpriteFont debugFont, gameFont;
        public int gameFontSize;
        

        /// <summary>
        /// Load GameContents
        /// </summary>
        public GameContent(GameComponent screenManager)
        {
            content = screenManager.Game.Content;

            blank = content.Load<Texture2D>("Graphics/blank");
            menuBackground = content.Load<Texture2D>("Graphics/menuBackground");

            grid = content.Load<Texture2D>("Graphics/grid");

            for (int i = 0; i < 2; i++)
            {
                idle[i] = content.Load<Texture2D>("Graphics/" + ((Shape)i).ToString() + "Idle");
                walk[i] = content.Load<Texture2D>("Graphics/" + ((Shape)i).ToString() + "Walk");
            }

            moveArrow = content.Load<Texture2D>("Graphics/moveArrow");
            
            ring = content.Load<Texture2D>("Graphics/ring");
            ringDashed = content.Load<Texture2D>("Graphics/ringDashed");
            ringOrigin = new Vector2(ring.Width, ring.Height) / 2;

            bg0 = content.Load<Texture2D>("Graphics/bg0");
            bg1 = content.Load<Texture2D>("Graphics/bg1");

            water = content.Load<Texture2D>("Graphics/water");

            debugFont = content.Load<SpriteFont>("Fonts/debugFont");

            gameFontSize = 60;
            gameFont = content.Load<SpriteFont>("Fonts/chunky" + gameFontSize.ToString());

            MediaPlayer.IsRepeating = true;

#if DEBUG
            MediaPlayer.Volume = .4f; SoundEffect.MasterVolume = .4f;
#else
            MediaPlayer.Volume = 1; SoundEffect.MasterVolume = 1;
#endif

            // Initialize audio objects.
            //audioEngine = new AudioEngine("Content/Audio/Audio.xgs");
            //soundBank = new SoundBank(audioEngine, "Content/Audio/Sound Bank.xsb");
            //waveBank = new WaveBank(audioEngine, "Content/Audio/Wave Bank.xwb");
            //soundBank.GetCue("music").Play();


            //Thread.Sleep(1000);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            screenManager.Game.ResetElapsedTime();
        }


        /// <summary>
        /// Unload GameContents
        /// </summary>
        public void UnloadContent() { content.Unload(); }
    }
}
