﻿using GameLibrary.Dependencies.Entities;
using GameLibrary.Entities.Components.Physics;
using Microsoft.Xna.Framework;
using System;

namespace SpaceHordes.Entities.Components
{
    public enum Targeting
    {
        Constant,
        Closest,
        Strongest,
        Weakest
    }

    /// <summary>
    /// The AI component class
    /// </summary>
    public class AI : Component
    {
        public AI(Body target, Func<Entity, Body, bool> behavior, string targetGroup = "", float searchRadius =10f)
        {
            this.Target = target;
            this.TargetGroup = targetGroup;
            this.SearchRadius = searchRadius;
            this.Behavior = behavior;
        }

        #region Properties

        /// <summary>
        /// The radius in which the AISystem will search for a new target.
        /// </summary>
        public float SearchRadius
        {
            get;
            set;
        }

        /// <summary>
        /// The target entity group in which a close entity must be.
        /// </summary>
        public string TargetGroup
        {
            get;
            set;
        }

        /// <summary>
        /// The way with which targets are targeted
        /// </summary>
        public Targeting Targeting
        {
            get;
            set;
        }

        #endregion

        #region Fields

        /// <summary>
        /// The target which the AI component will process.
        /// </summary>
        public Body Target;
        /// <summary>
        /// Specifies the behavior of the AI component.
        /// Returns true if the AI system is to search for a new target for this specific AI component.
        /// </summary>
        public Func<Entity, Body, bool> Behavior;

        #endregion

        #region Behaviors

        /// <summary>
        /// Creates a follow behavior
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="rotateTo"></param>
        /// <returns></returns>
        public static Func<Entity, Body, bool> CreateFollow(float speed, bool rotateTo = true)
        {
            return
                (ent, target) =>
                {
                    Body b = ent.GetComponent<Body>();
                    Vector2 distance = target.Position - b.Position;
                    distance.Normalize();
                    distance *= speed;

                    if (target != null && target.LinearVelocity != distance && !ent.HasComponent<Slow>())
                    {
                        b.LinearVelocity = distance;
                        if(rotateTo)
                            b.RotateTo(distance);
                    }
                    return false;
                };
        }


        #endregion
    }


    ////TODO: DEPRICATE
    //public class AI : Component
    //{
    //    private Body target;
    //    private Behavior behavior;
    //    private Targeting targeting;

    //    public event Action TargetChangedEvent;

    //    public Body Target
    //    {
    //        get { return target; }
    //        set
    //        {
    //            if (TargetChangedEvent != null)
    //                TargetChangedEvent();
    //            target = value;
    //        }
    //    }

    //    public Behavior Behavior
    //    {
    //        get { return behavior; }
    //        set { behavior = value; }
    //    }

    //    public Targeting Targeting
    //    {
    //        get { return targeting; }
    //        set { targeting = value; }
    //    }

    //    public float SearchRadius
    //    {
    //        get;
    //        set;
    //    }

    //    public string HostileGroup
    //    {
    //        get;
    //        set;
    //    }

    //    public AI()
    //    {
    //    }

    //    public AI(Body target)
    //    {
    //        Target = target;
    //    }
    //}
}