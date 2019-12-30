﻿using System.Linq;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Places;

namespace Tests.Actors
{
    class ActorTests
    {
        [Test]
        public void TestAddOccupant()
        {

            var world = new World();
            var roomFactory = new RoomFactory(new ActorFactory());
            var you = new You(roomFactory.Create(world));
            world.Player = you;

            var room1 = world.Player.CurrentLocation;

            Assert.Contains(you,world.Population.ToArray());

            var frank = new Npc("Frank",room1);

            
            Assert.AreEqual(room1,you.CurrentLocation);
            Assert.AreEqual(room1,frank.CurrentLocation);

            Assert.Contains(you,world.Population.ToArray());
            Assert.Contains(frank,world.Population.ToArray());
        }
    }
}
