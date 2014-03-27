using Microsoft.Xna.Framework;

namespace BitSits_Framework
{
    class CreditsMenuScreen : MenuScreen
    {
        public override void LoadContent()
        {
            titleString = "CREDITS";
            titlePosition = new Vector2(80, 200);

            // Create our menu entries.
            MenuEntry exitMenuEntry = new MenuEntry(this, "Exit", new Vector2(500, 400));

            // Hook up menu event handlers.
            exitMenuEntry.Selected += QuitGameMenuEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(exitMenuEntry);
        }

        void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ExitScreen();
        }
    }
}
