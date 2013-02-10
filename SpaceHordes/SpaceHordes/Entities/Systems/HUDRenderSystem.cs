﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GameLibrary;
using GameLibrary.Dependencies;
using GameLibrary.Entities;
using GameLibrary.Dependencies.Entities;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameLibrary.Helpers;
using SpaceHordes.Entities.Components;

namespace SpaceHordes.Entities.Systems
{
    public class HUDRenderSystem : EntityProcessingSystem
    {
        SpriteBatch _SpriteBatch;
        ImageFont _Font;
        Texture2D _Hud;

        #region Locations

        Vector2 hudDimmensions;
        Vector2 radarDimmensions;
        Rectangle[] hudLocations;
        Rectangle radarLocation;
        Rectangle radarSource;
        Rectangle hudSource;
        Rectangle buildMenuSource;
        Rectangle selectionSource;

        Vector2[] boxOffsets;
        Rectangle box = new Rectangle(0, 0, 21, 21);

        #endregion

        public HUDRenderSystem()
            : base(typeof(Inventory))
        {
            hudDimmensions = new Vector2(96, 51);
            radarDimmensions = new Vector2(87, 51);

            hudLocations = new Rectangle[]
            {
                new Rectangle(ScreenHelper.Viewport.X, ScreenHelper.Viewport.Y, (int)hudDimmensions.X, (int)hudDimmensions.Y),
                new Rectangle(ScreenHelper.Viewport.Width - (int)hudDimmensions.X, ScreenHelper.Viewport.Y, (int)hudDimmensions.X, (int)hudDimmensions.Y),
                new Rectangle(ScreenHelper.Viewport.X, ScreenHelper.Viewport.Height - (int)radarDimmensions.Y, (int)hudDimmensions.X, (int)hudDimmensions.Y),
                new Rectangle(ScreenHelper.Viewport.Width - (int)hudDimmensions.X, ScreenHelper.Viewport.Height - (int)hudDimmensions.Y, (int)hudDimmensions.X, (int)hudDimmensions.Y)
            };

            radarLocation = new Rectangle((int)ScreenHelper.Center.X - (int)radarDimmensions.X / 2, ScreenHelper.Viewport.Height - (int)radarDimmensions.Y, (int)radarDimmensions.X, (int)radarDimmensions.Y);

            radarSource = new Rectangle(0, 0, 87, 51);
            hudSource = new Rectangle(86, 0, 96, 51);
            buildMenuSource = new Rectangle(181, 0, 96, 51);
            selectionSource = new Rectangle(277, 0, 25, 26);

            boxOffsets = new Vector2[8];
            boxOffsets[0] = new Vector2(3, 3);
            boxOffsets[1] = new Vector2(26, 3);
            boxOffsets[2] = new Vector2(49, 3);
            boxOffsets[3] = new Vector2(72, 3);
            boxOffsets[4] = new Vector2(3, 26);
            boxOffsets[5] = new Vector2(26, 26);
            boxOffsets[6] = new Vector2(49, 26);
            boxOffsets[7] = new Vector2(72, 26);
        }

        public void LoadContent(ImageFont font, Texture2D texture)
        {
            _SpriteBatch = new SpriteBatch(ScreenHelper.GraphicsDevice);
            _Font = font;
            _Hud = texture;
        }

        public override void Process()
        {
            _SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            _SpriteBatch.Draw(_Hud, radarLocation, radarSource,  Color.White);
            base.Process();

            _SpriteBatch.End();
        }

        public override void  Process(Entity e)
        {
            Inventory i = e.GetComponent<Inventory>();

            int playerIndex = int.Parse(e.Tag.Replace("Player","")) - 1;
            if (!i.BuildMode)
            {
                _SpriteBatch.Draw(_Hud, hudLocations[playerIndex], hudSource, Color.White);

                int blue = i.BLUE.Ammunition;
                int green = i.GREEN.Ammunition;
                int red = i.RED.Ammunition;
                int yellow = (int)i.YELLOW;

                Vector2 topLeft = new Vector2(hudLocations[playerIndex].X, hudLocations[playerIndex].Y);

                box.Location = new Point((int)(topLeft.X + boxOffsets[4].X), (int)(topLeft.Y + boxOffsets[4].Y));
                _Font.DrawString(_SpriteBatch, box, blue.ToString());

                box.Location = new Point((int)(topLeft.X + boxOffsets[5].X), (int)(topLeft.Y + boxOffsets[5].Y));
                _Font.DrawString(_SpriteBatch, box, green.ToString());

                box.Location = new Point((int)(topLeft.X + boxOffsets[6].X), (int)(topLeft.Y + boxOffsets[6].Y));
                _Font.DrawString(_SpriteBatch, box, red.ToString());

                box.Location = new Point((int)(topLeft.X + boxOffsets[7].X), (int)(topLeft.Y + boxOffsets[7].Y));
                _Font.DrawString(_SpriteBatch, box, yellow.ToString());
            }
            else
            {
                _SpriteBatch.Draw(_Hud, hudLocations[playerIndex], buildMenuSource, Color.White);
            }

        }
    }
}
