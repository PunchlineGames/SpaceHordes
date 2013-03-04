﻿using GameLibrary.GameStates.Screens;
using GameLibrary.Helpers;
using GameLibrary.Input;
using Microsoft.Xna.Framework;
using System;

namespace SpaceHordes.GameStates.Screens
{
    /// <summary>
    /// The main menu screen is the first thing displayed when
    /// </summary>
    public class MainMenuScreen : MenuScreen
    {
        #region Initialization

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen(string text)
            : base(text)
        {
            //Create our menu entries.
#if WINDOWS
            MenuEntry playGameMenuEntry = new MenuEntry("Play Game");
#endif

#if XBOX
            MenuEntry playGameMenuEntry = new MenuEntry("Play Solo");
#endif
            MenuEntry playMultiplayerMenuEntry = new MenuEntry("Play Game");

            MenuEntry highScoresMenuEntry = new MenuEntry("Multiplayer");
            MenuEntry bossEntry = new MenuEntry("High Scores");

            MenuEntry optionsMenuEntry = new MenuEntry("Options");
            MenuEntry exitMenuEntry = new MenuEntry("Exit");

            //Hook up menu event handlers.
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
//#if XBOX
            playMultiplayerMenuEntry.Selected += PlayMultiplayerMenuEntrySelected;
//#endif

            highScoresMenuEntry.Selected += HighScoresMenuEntrySelected;
            bossEntry.Selected += BossMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            //Add entries to the menu.
            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(playMultiplayerMenuEntry);
            MenuEntries.Add(highScoresMenuEntry);
            MenuEntries.Add(bossEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }

        #endregion Initialization

        #region Events

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        private void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, e.PlayerIndex, new GameplayScreen("Textures/gamefont", false));
        }

        private void PlayMultiplayerMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, e.PlayerIndex, new GameplayScreen("Textures/gamefont", true));
        }

        private void HighScoresMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new HighScoreScreen(), e.PlayerIndex);
        }

        static BackgroundScreen bossBackdrop;
        private void BossMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            bossBackdrop = new BackgroundScreen("Textures/GameMenu", TransitionType.Slide);
            ScreenManager.AddScreen(bossBackdrop, ControllingPlayer);
            BossScreen bosses = new BossScreen(ScreenHelper.SpriteSheet);
            bosses.OnExit += new EventHandler(BossScreenExited);
            ScreenManager.AddScreen(bosses, e.PlayerIndex);
        }

        public static void BossScreenExited(object sender, EventArgs e)
        {
            bossBackdrop.ExitScreen();
        }

        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        private void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
        }

        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            ScreenManager.Game.Exit();
        }

        #endregion Events
    }
}