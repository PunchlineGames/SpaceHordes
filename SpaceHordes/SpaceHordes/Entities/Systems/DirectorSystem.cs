﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary.Dependencies.Entities;
using Microsoft.Xna.Framework;
using GameLibrary.Entities.Components.Physics;

namespace SpaceHordes.Entities.Systems
{
    public class DirectorSystem : IntervalEntitySystem
    {
        Random r = new Random();

        Entity Base;

        int difficulty = 0;

        //Start off with a minute worth of time so spawns don't delay by a minute due to casting
        float elapsedSeconds;
        float elapsedMinutes;

        int playerDeaths = 0;
        int baseDamage = 0;

        int mooksToSpawn = 0;
        int gunnersToSpawn = 0;
        int huntersToSpawn = 0;
        int destroyersToSpawn = 0;

        public DirectorSystem()
            : base(500)
        {
            elapsedSeconds = 60f;
            elapsedMinutes = 1f;
        }

        public void LoadContent(Entity Base)
        {
            this.Base = Base;
        }

        protected override void ProcessEntities(Dictionary<int, Entity> entities)
        {
            base.ProcessEntities(entities);

            elapsedSeconds += .5f;
            elapsedMinutes += .5f/60f;

            difficulty = (int)(elapsedMinutes - (playerDeaths / 2 + baseDamage));

            //Spawn mooks per second equal to Difficulty
            if (elapsedSeconds % 5 == 0)
            {
                mooksToSpawn = 3 * difficulty;
                gunnersToSpawn = (int)(difficulty / 3);
                huntersToSpawn = (int)(difficulty / 5);
                destroyersToSpawn = (int)(difficulty / 10);

                for (int i = 0; i < mooksToSpawn; i++)
                {
                    int type = r.Next(9);
                    World.CreateEntity("Mook", type, Base.GetComponent<Body>()).Refresh();
                }

                for (int i = 0; i < gunnersToSpawn; i++)
                {
                }

                for (int i = 0; i < huntersToSpawn; i++)
                {
                }

                for (int i = 0; i < destroyersToSpawn; i++)
                {
                }
            }
        }

        public static float ClampInverse(float value, float min, float max)
        {
            if (value > min && value < max)
            {
                if (MathHelper.Distance(min, value) < MathHelper.Distance(max, value))
                    return min;
                else
                    return max;
            }
            else
                return value;
        }
    }
}
