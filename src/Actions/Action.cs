﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Wanderer.Actors;
using Wanderer.Behaviours;
using Wanderer.Compilation;
using Wanderer.Factories.Blueprints;
using Wanderer.Systems;

namespace Wanderer.Actions
{
    public class Action : HasStats,IAction
    {
        public char HotKey {get; set;}
        
        public IHasStats Owner { get; set; }


        /// <summary>
        /// How kind is the action? before picking any targets
        /// </summary>
        public double Attitude {get;set;}

        /// <summary>
        /// What can be targetted by the action
        /// </summary>
        public List<IActionTarget>  Targets {get;set;}
        
        public List<IEffect> Effect {get;set;} = new List<IEffect>();
        
        /// <summary>
        /// Initializes action with a default <see cref="HasStats.Name"/> based on the class name
        /// </summary>
        public Action(IHasStats owner)
        {
            Name = GetType().Name.Replace("Action", "");
            Owner = owner;
        }

        /// <summary>
        /// Prompts for all choices and then pushes onto <paramref name="stack"/>
        /// a suitable <see cref="Frame"/> (or not if there are no valid options picked / option
        /// picking is cancelled.
        /// 
        /// <para>Actual resolution of the action should be reserved for the <see cref="Pop"/> method</para>
        /// </summary>
        /// <param name="world"></param>
        /// <param name="ui"></param>
        /// <param name="stack"></param>
        /// <param name="actor"></param>
        public virtual void Push(IWorld world,IUserinterface ui, ActionStack stack, IActor actor)
        {
            var targets = GetTargets(actor).ToArray();
            IHasStats target = null;

            if(targets.Length == 1)
                target = targets[0];
            else if(targets.Length >= 1)
                if(!actor.Decide(ui,Name,"Target",out target, targets,Attitude))
                    return;
            
            stack.Push(new Frame(actor,this,Attitude){TargetIfAny = target});
        }


        /// <summary>
        /// Override to your action once it is confirmed
        /// </summary>
        /// <param name="world"></param>
        /// <param name="ui"></param>
        /// <param name="stack"></param>
        /// <param name="frame"></param>
        public virtual void Pop(IWorld world, IUserinterface ui, ActionStack stack, Frame frame)
        {
            foreach (var e in Effect) 
                e.Apply(new SystemArgs(world, ui, 0, frame.PerformedBy, frame.TargetIfAny ?? Owner, stack.Round));

            PopImpl(world, ui, stack, frame);
        }

        protected virtual void PopImpl(IWorld world, IUserinterface ui, ActionStack stack, Frame frame)
        {
            
        }

        public virtual bool HasTargets(IActor performer)
        {
            return GetTargets(performer).Any();
        }
        
        public virtual IEnumerable<IHasStats> GetTargets(IActor performer)
        {
            if(Targets == null || !Targets.Any())
            {
                if(Owner != null)
                    return new []{Owner};
            }
            else
            {

                var args = new SystemArgs(performer.CurrentLocation.World,null,0,performer,Owner,Guid.Empty);

                return Targets.SelectMany(t=>t.Get(args)).Distinct();
            }

            return new IHasStats[0];
        }

        public virtual IAction Clone()
        {
            //TODO preserve Owner
            return (IAction) Activator.CreateInstance(GetType(),true);
        }

        public virtual ActionDescription ToActionDescription()
        {
            return new ActionDescription(){HotKey = HotKey, Name = Name};
        }

        public bool AreIdentical(IAction other)
        {
            if (other == null)
                return false;

            return this.Name == other.Name;
        }

        public override string ToString()
        {
            return $"{Name} [{Owner}]";
        }

        
        protected void ShowNarrative(IUserinterface ui,IActor actor,string title, string fluff, string technical,Guid round)
        {
            var narrative = new Narrative(actor, title, fluff, technical,round);
            narrative.Show(ui);
        }

        /// <summary>
        /// Returns all values of the Enum T except for "None"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T[] GetValues<T>() where T:Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>()
                .Where(t => !string.Equals(t.ToString(), "None", StringComparison.CurrentCultureIgnoreCase)).ToArray();
        }
    }
}