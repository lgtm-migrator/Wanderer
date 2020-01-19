﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Items;
using StarshipWanderer.Relationships;

namespace StarshipWanderer.Places
{
    public interface IPlace : IHasStats
    {
        /// <summary>
        /// Who currently controls this place (can be null)
        /// </summary>
        IFaction ControllingFaction { get; set; }

        /// <summary>
        /// True if the player has uncovered this location yet
        /// </summary>
        bool IsExplored { get; set; }

        IWorld World { get; set; }
        
        /// <summary>
        /// The single character to render for this location in maps.
        /// </summary>
        char Tile { get; }
        
        /// <summary>
        /// Items that are not owned by anyone yet
        /// </summary>
        HashSet<IItem> Items { get;set; }

        [JsonIgnore]
        IEnumerable<IActor> Actors { get; }

        Point3 GetPoint();

        bool Has<T>() where T:IAdjective;

        bool Has<T>(Func<T,bool> condition) where T : IAdjective;

        /// <summary>
        /// Determines which directions you can leave the room in
        /// </summary>
        HashSet<Direction> LeaveDirections { get; set; }
    }
}