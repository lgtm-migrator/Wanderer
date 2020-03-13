﻿using System;
using System.Linq;
using Wanderer.Actors;
using Wanderer.Extensions;
using Wanderer.Factories.Blueprints;
using Wanderer.Items;
using Wanderer.Rooms;
using Wanderer.Relationships;
using System.Collections.Generic;

namespace Wanderer.Factories
{
    public class ActorFactory : HasStatsFactory<IActor> ,IActorFactory
    {
        public List<ActorBlueprint> Blueprints { get; set; } = new List<ActorBlueprint>();
        
        public SlotCollection DefaultSlots { get; set; } = new SlotCollection();

        public ActorFactory(IAdjectiveFactory adjectiveFactory):base(adjectiveFactory)
        {
        }
        
        public virtual void Create(IWorld world, IRoom room, IFaction faction, RoomBlueprint roomBlueprintIfAny)
        {
            int numberOfNpc = Math.Max(1,world.R.Next(5));

            var pickFrom = Blueprints.Where(b=>b.SuitsFaction(faction)).ToList();

            if (roomBlueprintIfAny != null)
                pickFrom = pickFrom.Union(roomBlueprintIfAny.OptionalActors).ToList();

            if(pickFrom.Any())
                for (int i = 0; i < numberOfNpc; i++)
                    Create(world, room, faction, pickFrom.GetRandom(world.R),roomBlueprintIfAny);
        }

        public IActor Create(IWorld world, IRoom room, IFaction faction, ActorBlueprint blueprint, RoomBlueprint roomBlueprintIfAny)
        {
            var npc = new Npc(blueprint.Name, room);

            AddBasicProperties(npc, blueprint, world,"talk");

            if (faction != null)
                npc.FactionMembership.Add(faction);
            
            if(string.IsNullOrWhiteSpace(npc.Name))
                npc.Name = faction?.NameFactory?.GenerateName(world.R) ?? "Unnamed Npc";

            foreach (var blue in blueprint.MandatoryItems) 
                npc.Equip(npc.SpawnItem(blue));

            //plus give them one more random thing that fits the faction / actor
            var pickFrom = world.ItemFactory.Blueprints.Union(blueprint.OptionalItems).ToArray();

            if (roomBlueprintIfAny != null)
                pickFrom = pickFrom.Union(roomBlueprintIfAny.OptionalItems).ToArray();
            
            if (pickFrom.Any()) 
                npc.Equip(npc.SpawnItem(pickFrom.GetRandom(world.R)));
            
            npc.AvailableSlots = (blueprint.Slots ?? faction?.DefaultSlots ?? DefaultSlots)?.Clone() ?? new SlotCollection();
            
            return npc;
        }
    }
}
