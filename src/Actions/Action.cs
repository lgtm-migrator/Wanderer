﻿using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;

namespace StarshipWanderer.Actions
{
    public abstract class Action : IAction
    {
        public IWorld World { get; set; }
        public IActor PerformedBy { get; set; }
        public string Name { get; set; }

        public CancellationStatus Cancelled { get; set; }  = CancellationStatus.NotCancelled;

        protected Action(IWorld world, IActor performedBy)
        {
            World = world;
            PerformedBy = performedBy;
            Name = GetType().Name.Replace("Action", "");
        }

        /// <summary>
        /// Resets action state to <see cref="CancellationStatus.NotCancelled"/> and
        /// pushes onto <paramref name="stack"/>.  Overrides should prompt for any
        /// additional setup for maybe executing the command (i.e. in <see cref="Pop"/>)
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="stack"></param>
        public virtual void Push(IUserinterface ui, ActionStack stack)
        {
            Cancelled = CancellationStatus.NotCancelled;
            stack.Push(this);
        }

        
        /// <summary>
        /// Override to your action once it is confirmed
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="stack"></param>
        public abstract void Pop(IUserinterface ui,ActionStack stack);
    }
}