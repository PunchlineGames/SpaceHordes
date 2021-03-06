﻿using GameLibrary.GameStates;
using GameLibrary.Helpers;
using GameLibrary.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace SpaceHordes.GameStates.Screens
{
    public class GameOverScreen : GameScreen
    {
        #region Fields

        private string text = "";
        private string text2 = "";

        private PlayerIndex[] players;
        private long score;

        private InitialEntryScreen[] initialEntryScreens = new InitialEntryScreen[4];

        private Vector2 titleLocation;
        private Vector2 subtitleLocation;
        private Vector2[] screenLocations = new Vector2[4];

        private SpriteFont titleFont;
        private SpriteFont textFont;

        private List<string> initials = new List<string>();

        private bool awaitCancel = false;
        private bool expired = false;

        private bool victory;

        private InputAction cancel;

        #endregion Fields

        #region Properties

        public List<string> Initials
        {
            get { return initials; }
        }

        #endregion Properties

        #region Initialization

        public GameOverScreen(PlayerIndex[] players, long score)
            : this(players, score, false)
        {
        }

        public GameOverScreen(PlayerIndex[] players, long score, bool victory)
        {
            this.score = score;
            this.players = players;
            this.IsPopup = true;
            this.victory = victory;

            TransitionOnTime = TimeSpan.FromSeconds(0.5f);
            TransitionOffTime = TimeSpan.FromSeconds(0.5f);

            cancel = new InputAction(
                new Buttons[] { Buttons.Back, Buttons.A, Buttons.B },
                new Keys[] { Keys.Space, Keys.Escape, Keys.Tab },
                true);
        }

        public override void Activate()
        {
            base.Activate();

            text = "GAME OVER";

            if (victory)
                text = "VICTORY!";

            Rectangle viewport = new Rectangle(0, 0, ScreenHelper.Viewport.Width, ScreenHelper.Viewport.Height);

            titleFont = Manager.TitleFont;
            textFont = Manager.Font;

            titleLocation = new Vector2(
                    viewport.Center.X, viewport.Center.Y - viewport.Height * 0.1f);

#if XBOX
            if (!StorageHelper.CheckStorage()) return;
#endif

            if (HighScoreScreen.IsHighScore(players.Length, score))
            {
                text2 = "High score! Enter your initials.";
                titleLocation = new Vector2(
                    viewport.Center.X, viewport.Height * 0.1736111111111111f);
            }

            subtitleLocation = new Vector2(
                viewport.Center.X, titleLocation.Y + titleFont.MeasureString(text).Y / 2);

            screenLocations[0] = new Vector2(
                viewport.Left + viewport.Width / 3, viewport.Top + viewport.Height / 3);
            screenLocations[1] = new Vector2(
                viewport.Right - viewport.Width / 3, viewport.Top + viewport.Height / 3);
            screenLocations[2] = new Vector2(
                viewport.Left + viewport.Width / 3, viewport.Bottom - viewport.Height / 3);
            screenLocations[3] = new Vector2(
                viewport.Right - viewport.Width / 3, viewport.Bottom - viewport.Height / 3);

            if (HighScoreScreen.IsHighScore(players.Length, score))
            {
                for (int i = 0; i < players.Length; ++i)
                {
                    initialEntryScreens[i] = new InitialEntryScreen(screenLocations[(int)players[i]], this);
                    Manager.AddScreen(initialEntryScreens[i], players[i]);
                }
            }
        }

        #endregion Initialization

        #region Update & Draw

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (!expired)
            {
                bool allExpired = true;
                bool allNull = true;

                foreach (InitialEntryScreen screen in initialEntryScreens)
                {
                    if (screen != null)
                    {
                        screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
                        allNull = false;

                        if (!screen.Expired)
                            allExpired = false;
                    }
                }

                if (allNull)
                {
                    awaitCancel = true;
                    allExpired = false;
                }

                if (allExpired)
                {
                    string names = "";

                    for (int i = 0; i < initials.Count; ++i)
                    {
                        names += initials[i];
                        if (i < initials.Count - 1)
                            names += ", ";
                    }

                    Manager.AddScreen(new BackgroundScreen("Textures/hiscore", TransitionType.Fade), ControllingPlayer);
                    Manager.AddScreen(new MainMenuScreen("Space Hordes"), null);
                    Manager.AddScreen(new HighScoreScreen(players.Length, HighScoreScreen.AddScore(players.Length, names, score)), null);
                    ExitScreen();
                    expired = true;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            SpriteBatch spriteBatch = Manager.SpriteBatch;
            Color titleColor = new Color(100, 77, 45) * TransitionAlpha;

            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            Vector2 titlePosition = titleLocation;
            Vector2 titleOrigin = titleFont.MeasureString(text) / 2;
            titlePosition.Y -= transitionOffset * 100;

            Vector2 subtitlePosition = subtitleLocation;
            Vector2 subtitleOrigin = textFont.MeasureString(text2) / 2;
            subtitlePosition.Y -= transitionOffset * 100;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            spriteBatch.DrawString(titleFont, text, titlePosition, titleColor, 0,
                titleOrigin, 1f, SpriteEffects.None, 0);

            spriteBatch.DrawString(textFont, text2, subtitleLocation, Color.White,
                0, subtitleOrigin, 1f, SpriteEffects.None, 0);

            spriteBatch.End();
        }

        #endregion Update & Draw

        #region Input

        public override void HandleInput(GameTime gameTime, GameLibrary.Input.InputState input)
        {
            base.HandleInput(gameTime, input);

            if (awaitCancel)
            {
                PlayerIndex index;

                if (cancel.Evaluate(input, ControllingPlayer, out index))
                {
                    Manager.AddScreen(new BackgroundScreen("Textures/hiscore", TransitionType.Fade), ControllingPlayer);
                    Manager.AddScreen(new MainMenuScreen("Space Hordes"), null);
                    ExitScreen();
                    SoundManager.Play("MenuCancel");
                }
            }

            foreach (InitialEntryScreen screen in initialEntryScreens)
            {
                if (screen != null)
                    screen.HandleInput(gameTime, input);
            }
        }

        #endregion Input
    }
}