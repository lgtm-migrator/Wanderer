﻿using System;
using System.Collections.Generic;
using Wanderer.Actions;
using Wanderer.Adjectives;
using Wanderer.Behaviours;
using Wanderer.Factories.Blueprints;
using Wanderer.Items;
using Wanderer.Rooms;
using Wanderer.Relationships;
using Wanderer.Stats;
using Wanderer.Systems;

namespace Wanderer.Actors
{
    /// <summary>
    /// An entity in a single location (at once) capable of performing actions (this includes the human player)
    /// </summary>
    public interface IActor : IHasStats
    {
        
        /// <summary>
        /// True if actor is dead
        /// </summary>
        bool Dead { get; set; }

        /// <summary>
        /// The word that describes how the actor fights when not
        /// equipped with a weapon e.g. fisticuffs
        /// </summary>
        string FightVerb { get; set; }

        /// <summary>
        /// True if they can perform the <see cref="DialogueAction"/> i.e. enter the dialogue system and start talking to people (typically true for the player but false for <see cref="Npc"/>)
        /// </summary>
        bool CanInitiateDialogue {get;set;}
        
        /// <summary>
        /// True if they can perform the <see cref="InspectAction"/> (typically true for the player but false for <see cref="Npc"/>)
        /// </summary>
        bool CanInspect {get;set;}

        /// <summary>
        /// Where the <see cref="Actor"/> currently is
        /// </summary>
        IRoom CurrentLocation { get; set; }

        /// <summary>
        /// Items that the actor owns
        /// </summary>
        List<IItem> Items { get;set; }

        /// <summary>
        /// How many of each body part does the actor have in which he can equip stuff
        /// </summary>
        SlotCollection AvailableSlots { get; set; }
        
        /// <summary>
        /// All factions which you belong to
        /// </summary>
        HashSet<IFaction> FactionMembership { get; set; }

        /// <summary>
        /// Asks the actor to pick a target for T.  This could be direction to move
        /// someone to attack etc. <paramref name="attitude"/> indicates how naughty
        /// the act is 0 neutral (won't hurt anyone), high numbers are friendly, negative
        /// numbers are hostile actions.
        /// 
        /// <para>Returning false indicates no desire to make a decision in which circumstances <paramref name="chosen"/> should be default(T) </para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ui"></param>
        /// <param name="title"></param>
        /// <param name="body"></param>
        /// <param name="chosen">The target the actor picked</param>
        /// <param name="options"></param>
        /// <param name="attitude">0 for neutral actions, positive for actions that are helpful (to <paramref name="chosen"/>), negative for actions that are hostile to <paramref name="chosen"/> </param>
        /// <returns>True if the actor wants to go ahead</returns>
        bool Decide<T>(IUserinterface ui, string title, string body, out T chosen, T[] options, double attitude);

        /// <summary>
        /// Move the actor from it's <see cref="CurrentLocation"/> to a <paramref name="newLocation"/>
        /// </summary>
        /// <param name="newLocation"></param>
        void Move(IRoom newLocation);

        /// <summary>
        /// Ends the life of the <see cref="Actor"/>
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="round"></param>
        /// <param name="reason"></param>
        void Kill(IUserinterface ui, Guid round, string reason);

        /// <summary>
        /// Returns all other people in the <see cref="CurrentLocation"/>
        /// </summary>
        /// <returns></returns>
        IActor[] GetCurrentLocationSiblings(bool includeDead);

        
        /// <summary>
        /// Returns true if the <see cref="IActor"/> has the supplied adjective (or optionally
        /// an item) that matches the <paramref name="name"/> which can be an <see cref="IHasStats.Identifier"/>.
        /// </summary>
        /// <returns></returns>
        bool Has(string name, bool includeItems);

        /// <summary>
        /// Returns the actors <see cref="IHasStats.BaseStats"/> adjusted for current equipment, adjectives (e.g. sick) and room (e.g. dark)
        /// </summary>
        /// <returns></returns>
        StatsCollection GetFinalStats();

        /// <summary>
        /// Returns all actions that the actor can perform.  This includes actions granted by the room they are in or items they have
        /// </summary>
        /// <returns></returns>
        IEnumerable<IAction> GetFinalActions();

        /// <summary>
        /// Returns all <see cref="IBehaviour"/> for this actor including any granted by items they are holding or rooms they are in etc
        /// </summary>
        /// <returns></returns>
        IEnumerable<IBehaviour> GetFinalBehaviours();

        /// <summary>
        /// Returns true if the current actor is able to observe freely the actions of <paramref name="other"/> (typically requires them to be in the same <see cref="Room"/>)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool IsAwareOf(IActor other);

        /// <summary>
        /// Return true if the item is one you can equip (have enough slots, are not already equipping it and meet the equip conditions)
        /// </summary>
        /// <param name="item"></param>
        /// <param name="reason">Reason that you cannot equip it</param>
        /// <returns></returns>
        bool CanEquip(IItem item, out string reason);


        /// <summary>
        /// Return true if the item can be taken off
        /// </summary>
        /// <param name="item"></param>
        /// <param name="reason">Reason that you cannot equip it</param>
        /// <returns></returns>
        bool CanUnEquip(IItem item, out string reason);
        
        /// <summary>
        /// Returns this actors perception of <paramref name="other"/> (bear in mind you
        /// might like them but they might hate you back)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        double AttitudeTo(IActor other);

        
        /// <summary>
        /// Returns the distance between the current actor and <paramref name="actor"/> in a straight line (i.e. not a path or zigzag)
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        double DistanceTo(IActor actor);

        /// <summary>
        /// Returns the actor which this actor has the strongest relationship to (the feeling might not be mutual)
        /// </summary>
        /// <param name="inSameLocation">Restrict the answer to only those actors in the same room</param>
        /// <param name="threshold">The relationship must be at least this strong</param>
        /// <returns></returns>
        IActor BestFriend(bool inSameLocation, double threshold);

        
        /// <summary>
        /// Returns the actor which this actor has the most negative relationship to (the feeling might not be mutual)
        /// </summary>
        /// <param name="inSameLocation">Restrict the answer to only those actors in the same room</param>
        /// <param name="threshold">The relationship must be at least this strong</param>
        /// <returns></returns>
        IActor WorstEnemy(bool inSameLocation, double threshold);


        /// <summary>
        /// Spawn a new item for the <see cref="IActor"/>
        /// </summary>
        /// <param name="blue"></param>
        /// <returns></returns>
        IItem SpawnItem(ItemBlueprint blue);

        
        /// <summary>
        /// Spawn a new item for the <see cref="IActor"/>.
        /// </summary>
        /// <param name="g"></param>
        /// <exception cref="GuidNotFoundException"></exception>
        /// <returns></returns>
        IItem SpawnItem(Guid g);
        
        /// <summary>
        /// Spawn a new item for the <see cref="IActor"/>
        /// </summary>
        /// <param name="name"></param>
        ///  <exception cref="NamedObjectNotFoundException"></exception>
        /// <returns></returns>
        IItem SpawnItem(string name);
        
        
        /// <summary>
        /// Spawn a new action for the object
        /// </summary>
        /// <param name="blue"></param>
        /// <returns></returns>
        IAction SpawnAction(ActionBlueprint blue);

        
        /// <summary>
        /// Spawn a new action for the object
        /// </summary>
        /// <param name="g"></param>
        /// <exception cref="GuidNotFoundException"></exception>
        /// <returns></returns>
        IAction SpawnAction(Guid g);
        
        /// <summary>
        /// Spawn a new action onto the object
        /// </summary>
        /// <param name="name"></param>
        ///  <exception cref="NamedObjectNotFoundException"></exception>
        /// <returns></returns>
        IAction SpawnAction(string name);

        /// <summary>
        /// Adds a new <see cref="IBehaviour"/> onto <see cref="IHasStats.BaseBehaviours"/>
        /// </summary>
        /// <param name="name">Name of <see cref="BehaviourBlueprint"/></param>
        /// <returns></returns>
        IBehaviour SpawnBehaviour(string name);
        
        /// <summary>
        /// Adds a new <see cref="IBehaviour"/> onto <see cref="IHasStats.BaseBehaviours"/>
        /// </summary>
        /// <param name="g">Guid of <see cref="HasStatsBlueprint.Identifier"/></param>
        /// <returns></returns>
        IBehaviour SpawnBehaviour(Guid g);

        /// <summary>
        /// Automatically equips the given <paramref name="item"/> if possible
        /// without performing any formal actions (e.g. <see cref="EquipmentAction"/>)
        /// </summary>
        /// <param name="item"></param>
        void Equip(IItem item);


        /// <summary>
        /// Returns the injury system of your currently held weapon or innate form (i.e. <see cref="InjurySystem"/>)
        /// </summary>
        /// <returns></returns>
        IInjurySystem GetBestInjurySystem();

        /// <summary>
        /// Heals the first injury of the given type
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="round"></param>
        /// <param name="s"></param>
        void Heal(IUserinterface ui, Guid round, string s);
    }
}
