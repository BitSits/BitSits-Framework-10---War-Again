using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GameDataLibrary
{
    public class Settings
    {
        public static Point[] Resolutions = { new Point(640, 480), new Point(800, 600), new Point(1024, 768) };
        public static string[] ResolutionStrings = { "Small 640 x 480", "Medium 800 x 600", "HD 1024 x 768" };

        public int CurrentResolution;
        public bool IsFullScreen;
        public float SoundVolume;
        public float MusicVolume;

        string fileName = "settings.xml";

        public Settings Load()
        {
            Settings s = Storage.LoadXml<Settings>(fileName);

            if (s == null)
            {
                s = new Settings();
                s.LoadDefault();
            }

            return s;
        }

        void LoadDefault()
        {
#if WINDOWS
            CurrentResolution = 1;
#endif

#if DEBUG
            IsFullScreen = false;
#else
            IsFullScreen = true;
#endif

            SoundVolume = 1f;
            MusicVolume = 1f;
        }

        public void Save()
        {
            Storage.SaveXml<Settings>(fileName, this);
        }
    }
}
