﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Items;
using StarshipWanderer.Relationships;

namespace StarshipWanderer.Places
{
    public class RoomFactory:IRoomFactory
    {
        public IActorFactory ActorFactory { get; set; }
        public IItemFactory ItemFactory { get; }
        public IAdjectiveFactory AdjectiveFactory { get; set; }

        public RoomFactory(IActorFactory actorFactory,IItemFactory itemFactory, IAdjectiveFactory adjectiveFactory)
        {
            ActorFactory = actorFactory;
            ItemFactory = itemFactory;
            AdjectiveFactory = adjectiveFactory;
        }

        public IPlace Create(IWorld world)
        {
            var room = _buildersList[world.R.Next(_buildersList.Count)](world);

            //give the room a random adjective
            var availableAdjectives = AdjectiveFactory.GetAvailableAdjectives(room).ToArray();
            room.Adjectives.Add(availableAdjectives[world.R.Next(0, availableAdjectives.Length)]);
            
            //some friends in the room with you
            ActorFactory.Create(world, room);

            //some free items
            ItemFactory.Create(room);

            return room;
        }

        private IReadOnlyList<Func<IWorld, IPlace>> _buildersList = new ReadOnlyCollection<Func<IWorld, IPlace>>(
            new List<Func<IWorld, IPlace>>
            {
                w =>
                    new Room("Gun Bay " + w.R.Next(5000), w, 'g')
                        {
                            ControllingFaction = w.Factions.GetRandomFaction(w.R)
                        }
                        .With(new LoadGunsAction()),
                w =>
                    new Room("Stair" + w.R.Next(5000), w, 's')
                        .AllowUpDown(true)
            });
    }
}