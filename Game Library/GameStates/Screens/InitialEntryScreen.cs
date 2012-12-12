﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Game_Library;
using Game_Library.Input;

namespace Game_Library.GameStates.Screens
{
    class InitialEntryScreen : GameScreen
    {
        #region Fields

        Vector2 position;

        int selectedChar = 0;
        MenuEntry[] initials = new MenuEntry[3];

        InputAction up;
        InputAction down;
        InputAction left;
        InputAction right;
        InputAction accept;

        #endregion

        #region Properties

        /// <summary>
        /// The initials that have been entered by the player.
        /// </summary>
        public string Initials
        {
            get
            {
                string toReturn = "";

                for (int i = 0; i < initials.Length; i++)
                    toReturn += initials[i].Text;

                return toReturn;
            }
        }

        #endregion

        #region Initialization

        public InitialEntryScreen(Vector2 position)
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            IsPopup = true;
            this.position = position;

            up = new InputAction(
                new Buttons[] { Buttons.LeftThumbstickUp, Buttons.DPadUp },
                new Keys[] { Keys.Up },
                true);

            down = new InputAction(
                new Buttons[] { Buttons.LeftThumbstickDown, Buttons.DPadDown },
                new Keys[] { Keys.Down },
                true);

            left = new InputAction(
                new Buttons[] { Buttons.LeftThumbstickLeft, Buttons.DPadLeft },
                new Keys[] { Keys.Left },
                true);

            right = new InputAction(
                new Buttons[] { Buttons.LeftThumbstickRight, Buttons.DPadRight },
                new Keys[] { Keys.Right },
                true);

            accept = new InputAction(
                new Buttons[] { Buttons.Start },
                new Keys[] { Keys.Enter },
                true);
        }

        public override void Activate()
        {
            base.Activate();

            Vector2 loc = position;

            for (int i = 0; i < initials.Length; i++)
            {
                initials[i] = new MenuEntry("A");
                initials[i].Position = loc;

                loc += new Vector2(ScreenManager.Font.MeasureString("A").X + 10, 0);
            }
        }

        #endregion

        #region Update & Draw

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            foreach (MenuEntry entry in initials)
            {
                if (initials[selectedChar] == entry)
                    entry.Update(true, gameTime);
                else
                    entry.Update(false, gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            ScreenManager.SpriteBatch.Begin();

            foreach (MenuEntry entry in initials)
            {
                if (initials[selectedChar] == entry)
                    entry.Draw(this, true, gameTime);
                else
                    entry.Draw(this, false, gameTime);
            }

            ScreenManager.SpriteBatch.End();
        }

        #endregion

        #region Input

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            base.HandleInput(gameTime, input);

            foreach (MenuEntry entry in initials)
            {
                if (input.MouseHoverIn(entry.ClickRectangle))
                {
                    selectedChar = Array.IndexOf(initials, entry);

                    char ch = entry.Text.ToCharArray()[0];
                    ch = (char)((int)ch + input.RelativeScrollValue);
                    entry.Text = ch.ToString();
                }
            }

            PlayerIndex index;

            if (up.Evaluate(input, ControllingPlayer, out index))
            {
                char ch = initials[selectedChar].Text.ToCharArray()[0];
                ch += (char)1;
                initials[selectedChar].Text = ch.ToString();
            }

            if (down.Evaluate(input, ControllingPlayer, out index))
            {
                char ch = initials[selectedChar].Text.ToCharArray()[0];
                ch -= (char)1;
                initials[selectedChar].Text = ch.ToString();
            }

            char c = initials[selectedChar].Text.ToCharArray()[0];

            int num = (int)c;

            if (num < 65)
                num = 90;

            if (num > 90)
                num = 65;
            
            c = (char)num;

            initials[selectedChar].Text = c.ToString();

            if (right.Evaluate(input, ControllingPlayer, out index))
            {
                selectedChar++;
            }

            if (left.Evaluate(input, ControllingPlayer, out index))
            {
                selectedChar--;
            }

            selectedChar = (int)MathHelper.Clamp((float)selectedChar, 0f, 2f);

            if (accept.Evaluate(input, ControllingPlayer, out index))
                ExitScreen();
        }

        #endregion
    }
}
