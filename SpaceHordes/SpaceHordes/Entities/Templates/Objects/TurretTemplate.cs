﻿using GameLibrary.Dependencies.Entities;
using GameLibrary.Dependencies.Physics.Factories;
using GameLibrary.Entities.Components;
using GameLibrary.Entities.Components.Physics;
using GameLibrary.Helpers;
using Microsoft.Xna.Framework;
using SpaceHordes.Entities.Components;

namespace SpaceHordes.Entities.Templates.Objects
{
    public class TurretTemplate : IEntityTemplate
    {
        private SpriteSheet _SpriteSheet;
        private EntityWorld _World;

        public TurretTemplate(SpriteSheet spriteSheet, EntityWorld world)
        {
            _SpriteSheet = spriteSheet;
            _World = world;
        }

        public Entity BuildEntity(Entity e, params object[] args)
        {
            #region Body

            Body Body = e.AddComponent<Body>(new Body(_World, e));
            {
                FixtureFactory.AttachEllipse(//Add a basic bounding box (rectangle status)
                    ConvertUnits.ToSimUnits(_SpriteSheet.Animations["miniturret"][0].Width / 2f),
                    ConvertUnits.ToSimUnits(_SpriteSheet.Animations["miniturret"][0].Height / 2f),
                    20,
                    1,
                    Body);
                Body.Position = ConvertUnits.ToSimUnits((Vector2)args[0]);
                Body.BodyType = GameLibrary.Dependencies.Physics.Dynamics.BodyType.Static;
                Body.CollisionCategories = GameLibrary.Dependencies.Physics.Dynamics.Category.Cat1;
                Body.FixedRotation = false;

                Body.SleepingAllowed = false;
            }

            #endregion Body

            #region Sprite

            Sprite s = new Sprite(_SpriteSheet, "miniturret");
            e.AddComponent<Sprite>(s);

            #endregion Sprite

            #region AI/GUN

            Inventory inv = e.AddComponent<Inventory>(new Inventory(0, 0, 0, 0));

            AI ai = e.AddComponent<AI>(new AI(null,
                (turret, target) => //AI FUNCTION
                {
                    Gun g = inv.CurrentGun;
                    g.BulletsToFire = true;

                    /* Aiming *\
                     * X = v*t + x_o therefore
                     *  let X_aim = v_tar*t + x_tar
                     *            = v_bul*t + x_bul
                     *  therefore
                     *      0 = v_tar*t + x_tar - (v_bul*t + x_bul) =>
                     *  therefore
                     *      x_bul - x_tar = t(v_tar - v_bul)
                     *  so that
                     *      t = (x_bul - x_tar)/(v_tar - v_bul)
                     *  therefore X = (v_tar)*(x_bul - x_tar)/(v_tar - v_bul) + x_tar
                     */

                    //Vector2 zero = target.LinearVelocity*time - inv.CurrentGun.BulletVelocity*time + target.Position - Body.Position;
                    Vector2 time = (Body.Position - target.Position) /
                        (target.LinearVelocity - inv.CurrentGun.BulletVelocity);

                    Vector2 XFinal = (target.LinearVelocity) * time + target.Position;
                    Body.RotateTo(XFinal - Body.Position);

                    return true;

                    // Console.WriteLine((target.UserData as Entity).Tag + ": " + time);
                },
                "Enemies",
                3000));
            ai.Targeting = Targeting.Closest;

            #endregion AI/GUN

            #region Health

            e.AddComponent<Health>(new Health(1)).OnDeath +=
                ent =>
                {
                    Vector2 poss = e.GetComponent<ITransform>().Position;
                    _World.CreateEntity("Explosion", 0.5f, poss, ent, 3).Refresh();

                    int splodeSound = 1;
                    SoundManager.Play("Explosion" + splodeSound.ToString());
                };

            #endregion Health

            e.Group = "Structures";
            return e;
        }
    }
}