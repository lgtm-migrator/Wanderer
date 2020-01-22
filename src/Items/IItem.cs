﻿using System;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Places;

namespace StarshipWanderer.Items
{

    public interface IItem : IHasStats
    {
        /// <summary>
        /// If the item must be equipped to use then this indicates what slot it fills
        /// otherwise it will be null
        /// </summary>
        IItemSlot Slot { get; set; }

        /// <summary>
        /// True if the item has been equipped by someone
        /// </summary>
        bool IsEquipped { get; set; }

        /// <summary>
        /// Set on items that should have been removed from the game to prevent
        /// later reprocessing e.g. when one behaviour deletes another behaviours
        /// owner before that behaviour has itself resolved
        /// </summary>
        bool IsErased { get; set; }
        
        /// <summary>
        /// If you can read the item this should be populated
        /// </summary>
        Guid? NextDialogue { get; set; }

        /// <summary>
        /// Forces the owner to drops the item in the supplied <paramref name="owner"/> location
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="owner"></param>
        /// <param name="round"></param>
        void Drop(IUserinterface ui, IActor owner, Guid round);

        /// <summary>
        /// True if there is an active adjective
        /// </summary>
        /// <param name="owner"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool Has<T>(IActor owner) where T : IAdjective;


        /// <summary>
        /// True if there is an active adjective matching the <paramref name="condition"/>
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="condition"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool Has<T>(IActor owner,Func<T,bool> condition) where T : IAdjective;

    }
}