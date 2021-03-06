﻿using GameLibrary.Dependencies.Entities;
using GameLibrary.Entities.Components.Physics;
using GameLibrary.Helpers;

namespace SpaceHordes.Entities.Systems
{
    internal class BaseAnimationSystem : TagSystem
    {
        private int _MaxH = 20;
        private float _X = 0;
        private float _Speed = 0.25f;
        private float _Direction;

        public BaseAnimationSystem(float speed, int maxHeight)
            : base("Base")
        {
            _MaxH = maxHeight;
            _Speed = speed;
            _Direction = _Speed;
        }

        public override void Process(Entity e)
        {
            #region Float Effect

            if (e.HasComponent<Body>())
            {
                Body b = e.GetComponent<Body>();

                if (ConvertUnits.ToDisplayUnits(b.Position.Y) >= _MaxH)
                    _Direction = -_Speed;
                if (ConvertUnits.ToDisplayUnits(b.Position.Y) <= 0)
                    _Direction = _Speed;

                b.Position += new Microsoft.Xna.Framework.Vector2(b.Position.X - _X, ConvertUnits.ToSimUnits(_Direction));
            }

            #endregion Float Effect
        }
    }
}