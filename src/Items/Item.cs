﻿using System;
using System.Collections.Generic;
using System.Linq;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Places;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Items
{
    public class Item : HasStats,IItem
    {
        public void Drop(IUserinterface ui, IActor owner, Guid round)
        {
            //remove us from the owner
            owner.Items.Remove(this);
            //add us to the room
            owner.CurrentLocation.Items.Add(this);

            //log it
            ui.Log.Info(new LogEntry($"{owner} dropped {this}", round,owner));
        }

        public bool Has<T>(IActor owner) where T : IAdjective
        {
            return Adjectives.Any(a => a is T);
        }

        public bool Has<T>(IActor owner, Func<T, bool> condition) where T : IAdjective
        {
            return Adjectives.Any(a => a is T t && condition(t));
        }

        public Item(string name)
        {
            Name = name;
        }
        
        public override StatsCollection GetFinalStats(IActor forActor)
        {
            var clone = BaseStats.Clone();

            foreach (var adjective in Adjectives) 
                clone.Add(adjective.GetFinalStats(forActor));

            return clone;
        }

        public override IActionCollection GetFinalActions(IActor forActor)
        {
            return new ActionCollection(BaseActions.Union(Adjectives.SelectMany(a => a.GetFinalActions(forActor))));
        }

        public override IBehaviourCollection GetFinalBehaviours(IActor forActor)
        {
            return new BehaviourCollection(BaseBehaviours.Union(Adjectives.SelectMany(a => a.GetFinalBehaviours(forActor))));
        }
    }
}