﻿using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Dialogues;
using StarshipWanderer.Factories;
using StarshipWanderer.Factories.Blueprints;
using StarshipWanderer.Places;
using StarshipWanderer.Relationships;

namespace Tests.Actors
{
    class YamlRoomFactoryTests : UnitTest
    {
        [Test]
        public void TestCreatingRoomFromBlueprint_WithDialogue()
        {
            var yaml = @"- Name: Gun Bay
  Dialogue:
    Next: 193506ab-11bc-4de2-963e-e2f55a38d006";

            var roomFactory = new YamlRoomFactory(yaml, new AdjectiveFactory());

            var w = new World();
            w.Dialogue.AllDialogues.Add(new DialogueNode()
            {
                Identifier = new Guid("193506ab-11bc-4de2-963e-e2f55a38d006"),
                Body = new TextBlock[]{new TextBlock("This room is rank"), }
            });

            var room = roomFactory.Create(w, roomFactory.Blueprints.Single());
            var you = new You("Wanderer",room);

            var ui = GetUI("look:Gun Bay");

            w.RunRound(ui,new DialogueAction());

            Assert.Contains("This room is rank",ui.MessagesShown);


        }

        [Test]
        public void TestCreatingRoomFromBlueprint_NoFaction()
        {
            var w = new World();

            var yaml = 
@"
- Name: Tunnels
";
            var roomFactory = new YamlRoomFactory(yaml, new AdjectiveFactory());
            var room = roomFactory.Create(w);

            Assert.IsNotNull(room);
            Assert.AreEqual("Tunnels",room.Name);

            Assert.IsEmpty(room.Actors,"Expected that because there are no factions there are no actor factories");
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TestCreatingRoomFromBlueprint_WithFaction(bool explicitRoomColor)
        {
            var w = new World();

            var adj = new AdjectiveFactory();

            w.Factions.Add(
                new Faction("Techno Wizards",FactionRole.Establishment)
                {
                    Identifier = new Guid("bb70f169-e0f7-40e8-927b-1c181eb8740b"),
                    Color = ConsoleColor.Cyan,
                    ActorFactory = new ActorFactory(new ItemFactory(adj),adj)
                    {
                        Blueprints = new []
                        {
                            new ActorBlueprint()
                            {
                                Name = "Sandman"
                            }, 
                        }
                    }
                }
            );

            var yaml = 
                @$"
- Name: Tunnels
  {(explicitRoomColor ? "Color: 2" : "")}
  Faction: bb70f169-e0f7-40e8-927b-1c181eb8740b
";
            var roomFactory = new YamlRoomFactory(yaml, new AdjectiveFactory());
            var room = roomFactory.Create(w);

            Assert.IsNotNull(room);
            Assert.AreEqual("Tunnels",room.Name);

            Assert.Greater(room.Actors.Count(),0);
            Assert.IsTrue(room.Actors.All(a=>a.Name.Equals("Sandman")));

            //if the room has no set color and it is owned by the faction it should inherit the faction color
            Assert.AreEqual(explicitRoomColor ? ConsoleColor.DarkGreen : ConsoleColor.Cyan ,room.Color);
        }
        
        [Test]
        public void Test_UniqueRooms()
        {
            var g = new Guid("1f0eb057-edac-4eaa-b61b-778b75463cb9");

            var yaml =
                @"
- Identifier: 1f0eb057-edac-4eaa-b61b-778b75463cb9
  Name: BossRoom
  Unique: true
- Name: RegularRoom

";
            var room =  new YamlRoomFactory(yaml, new AdjectiveFactory());

            var w = new World();

            var rooms = new List<IPlace>();
            for (int i = 0; i < 100; i++) 
                rooms.Add(room.Create(w));

            Assert.AreEqual(99,rooms.Count(r=>r.Name.Equals("RegularRoom")));
            Assert.AreEqual(1,rooms.Count(r=>r.Name.Equals("BossRoom")));
        }
    }
}