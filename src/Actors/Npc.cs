﻿using System;
using System.Collections.Generic;
using System.Linq;
using StarshipWanderer.Places;

namespace StarshipWanderer.Actors
{
    public class Npc : Actor
    {
        public Npc(string name,IPlace currentLocation) : base( name,currentLocation)
        {
        }

        public override bool Decide<T>(IUserinterface ui, string title, string body, out T chosen, T[] options, int attitude)
        {
            List<T> narrowOptions = new List<T>(options);
            
            //if it's something bad
            if (attitude < 0 && this is T npcAsT)
            {
                //don't pick yourself
                narrowOptions.Remove(npcAsT);
            }

            //If there are no options pick null return false
            if (!narrowOptions.Any())
            {
                chosen = default(T);
                return false;
            }
            
            //pick random option
            chosen = narrowOptions[CurrentLocation.World.R.Next(0, options.Length)];

            //if picked option was default (e.g. None Enums) return false
            return !chosen.Equals(default(T));
        }

        public override void Kill(IUserinterface ui)
        {
            CurrentLocation.World.Population.Remove(this);
        }
    }
}