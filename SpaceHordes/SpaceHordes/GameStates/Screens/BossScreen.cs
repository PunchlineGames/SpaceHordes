﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using GameLibrary.Helpers;
using GameLibrary.Input;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace SpaceHordes.GameStates.Screens
{
    internal struct BossInfo
    {
        public string SpriteKey;
        public string BossName;

        public BossInfo(string key, string name)
        {
            SpriteKey = key;
            BossName = name;
        }
    }

    public class BossScreen : GameScreen
    {
        #region Fields

        static BossInfo[] bosses = new BossInfo[]
        {
            new BossInfo("smasher", "The Smasher"),
            new BossInfo("greenbossship", "Big Green"),
            new BossInfo("clawbossthing", "Clawdia"),
            new BossInfo("minikillerhead", "Mini Killer"),
            new BossInfo("eye", "The Eye"),
            new BossInfo("brain", "Father Brain"),
            new BossInfo("blimp", "Blimpy"),
            new BossInfo("bigredblobboss", "Big Red"),
            new BossInfo("giantgraybossship", "The Mother Ship"),
            new BossInfo("birdbody", "Big Blue"),
            new BossInfo("redgunship", "The Gunner"),
            new BossInfo("flamer", "The Flamer"),
            new BossInfo("massivebluemissile", "The Jabber-W0K"),
            new BossInfo("killerhead", "The Killer")
        };

        ContentManager content;
        SpriteSheet spriteSheet;
        Texture2D backgroundTexture;
        string filename;

        int index = 0;
        string currentKey;

        InputAction next;
        InputAction previous;
        InputAction cancel;

        #endregion

        #region Static Properties

        /// <summary>
        /// The folder path where save files will be stored for PC.
        /// </summary>
        public static string FolderPath
        {
            get
            {
#if WINDOWS
                return Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\Space Hordes";
#endif

#if XBOX
                return "";
#endif
            }
        }

        /// <summary>
        /// The path of the scores text file
        /// </summary>
        public static string FilePath
        {
            get
            {
#if WINDOWS
                return FolderPath + @"\bosses.txt";
#endif

#if XBOX
                return "";
#endif
            }
        }

        public static bool[] ClearedBosses
        {
            get { return clearedBosses; }
            set
            {
                clearedBosses = value;
                WriteData(clearedBosses);
            }
        }
        static bool[] clearedBosses;

        #endregion

        #region Initialization

        public BossScreen(string filename, SpriteSheet sheet)
        {
            //TransitionOnTime = TimeSpan.FromSeconds(0.5);
            //TransitionOffTime = TimeSpan.FromSeconds(0.5);
            this.filename = filename;

            spriteSheet = sheet;
            currentKey = bosses[0].SpriteKey;

            next = new InputAction(
                new Buttons[]
                {
                    Buttons.LeftThumbstickRight,
                    Buttons.DPadRight
                },
                new Keys[]
                {
                    Keys.Right
                },
                true);

            previous = new InputAction(
                new Buttons[]
                {
                    Buttons.LeftThumbstickLeft,
                    Buttons.DPadLeft
                },
                new Keys[]
                {
                    Keys.Left
                },
                true);

            cancel = new InputAction(
                new Buttons[]
                {
                    Buttons.Back,
                    Buttons.B
                },
                new Keys[]
                {
                    Keys.Escape
                },
                true);
        }

        public override void Activate()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            backgroundTexture = content.Load<Texture2D>(filename);
        }

        /// <summary>
        /// Unloads graphics content for this screen.
        /// </summary>
        public override void Unload()
        {
            content.Unload();
        }

        #endregion

        #region Update & Draw

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            bool[] data = ReadData();
            bool current = data[index];

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();

            spriteBatch.Draw(backgroundTexture, ScreenHelper.Viewport.Bounds, Color.White);

            float scale = 5f;

            Rectangle source = spriteSheet[currentKey][0];
            Vector2 spriteLoc = ScreenHelper.Center;

            spriteLoc -= (new Vector2(source.Width/2, source.Height/2) * scale);
            Rectangle destination;

            if (currentKey.Equals("birdbody"))
            {
                //bird contains two sprites so needs special draw handling
            }

            else if (currentKey.Equals("minikillerhead"))
            {
            }

            else if (currentKey.Equals("killerhead"))
            {
                //likewise here
            }

            else
            {
                Color color = current ? Color.White : Color.Black;
                destination = new Rectangle((int)spriteLoc.X, (int)spriteLoc.Y, (int)(source.Width * scale), (int)(source.Height * scale));
                spriteBatch.Draw(spriteSheet.Texture, destination, source, color);
            }

            string text = current ? bosses[index].BossName : "?????";
            Vector2 textDest = new Vector2(ScreenHelper.Viewport.Width/2, ScreenHelper.Viewport.Height * 0.80f);
            Vector2 size = ScreenManager.Font.MeasureString(bosses[index].BossName);
            Vector2 origin = size / 2;
            spriteBatch.DrawString(ScreenManager.Font, bosses[index].BossName, textDest, Color.White, 0f, origin, 1f, SpriteEffects.None, 0);
            spriteBatch.End();
        }

        #endregion

        #region Handle Input

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            PlayerIndex indx;

            if (next.Evaluate(input, ControllingPlayer, out indx))
            {
                if (index != bosses.Count() - 1)
                    index++;

                currentKey = bosses[index].SpriteKey;
            }

            if (previous.Evaluate(input, ControllingPlayer, out indx))
            {
                if (index != 0)
                    index--;

                currentKey = bosses[index].SpriteKey;
            }

            if (cancel.Evaluate(input, ControllingPlayer, out indx))
            {
                ExitScreen();
            }
        }

        #endregion

        #region Static Methods

        public static bool[] ReadData()
        {
            List<bool> data = new List<bool>();

            if (File.Exists(FilePath))
            {
                using (StreamReader reader = new StreamReader(FilePath))
                {
                    for (int x = 0; x < bosses.Length; x++)
                    {
                        string next = reader.ReadLine();

                        switch (next)
                        {
                            case "True":
                                data.Add(true);
                                break;
                            case "False":
                                data.Add(false);
                                break;
                            default:
                                data.Add(false);
                                break;
                        }
                    }
                }
            }
            else
            {
                WriteInitialData();
                return ReadData();
            }

            return data.ToArray();
        }

        public static void WriteData(bool[] data)
        {
            bool[] dat = new bool[bosses.Count()];

            using (StreamWriter writer = new StreamWriter(FilePath))
            {
                for (int i = 0; i < bosses.Count(); i++)
                {
                    writer.WriteLine(dat[i].ToString());
                }

                writer.Close();
            }
        }

        public static void WriteInitialData()
        {
            if (!Directory.Exists(FolderPath))
                Directory.CreateDirectory(FolderPath);

            if (!File.Exists(FilePath))
            {
                //If there is no scores file, make a new one
                using (FileStream fs = File.Create(FilePath))
                {
                    fs.Close();
                }
            }

            bool[] data = new bool[bosses.Length];

            for (int i = 0; i < bosses.Length; i++)
            {
                data[i] = false;
            }

            WriteData(data);
        }

        #endregion
    }
}
