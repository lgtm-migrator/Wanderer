﻿using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Behaviours;
using Wanderer.Compilation;
using Wanderer.Dialogues;
using Wanderer.Relationships;
using Wanderer.Stats;
using Wanderer.Systems;
using YamlDotNet.Serialization;

namespace Tests.Systems
{
    class DialogueTests : UnitTest
    {
        [TestCase(true)]
        [TestCase(false)]
        public void TestDialogue_ChoicesAffectRelationships(bool pickFriendly)
        {
            TwoInARoom(out You you, out IActor them, out IWorld w);

            var g = Guid.NewGuid();

            var tree = new DialogueNode()
            {
                Identifier = g,
                Body = new List<TextBlock>{new TextBlock("Hello World") }
            };
            var o1 = new DialogueOption()
            {
                Attitude = 10,
                Text = "Hello to you friend"
            };

            var o2 = new DialogueOption()
            {
                Attitude = -10,
                Text = "Go to hell"
            };

            tree.Options = new List<DialogueOption> {o1, o2};

            them.Dialogue.Verb = "talk";
            them.Dialogue.Next = tree.Identifier;
            w.Dialogue.AllDialogues.Add(tree);

            var yaml = new Serializer().Serialize(new DialogueNode[]{tree});
            TestContext.Out.Write(yaml);

            var ui = GetUI(pickFriendly ? o1 : o2);

            w.RunRound(ui,you.GetFinalActions().OfType<DialogueAction>().Single());

            var r = w.Relationships.OfType<PersonalRelationship>().Single(r => r.AppliesTo(them, you));

            Assert.AreEqual(pickFriendly ? 10 : -10,r.Attitude);

        }

        [TestCase(true)]
        [TestCase(false)]
        public void TestBanterDialogueCondition_AreFriends(bool areFriends)
        {
            TwoInARoomWithRelationship(areFriends ? 10 : -10,false, out You you, out IActor them, out IWorld w);

            var friend = new DialogueNode()
            {
                Identifier = new Guid("4abbc8e5-880c-44d3-ba0e-a9f13a0522d0"),
                Body = new List<TextBlock>{new TextBlock("Hello Friend") },
                Condition = new List<ICondition<SystemArgs>>()
                {
                    new ConditionCode<SystemArgs>("return Recipient:AttitudeTo(AggressorIfAny) > 5")
                }

            };
            var foe = new DialogueNode()
            {
                Identifier = new Guid("00d77067-da1c-4c34-96ee-8a74353e4839"),
                Body = new List<TextBlock>{new TextBlock("Hello Foe") },
                Condition = new List<ICondition<SystemArgs>>()
                {
                    new ConditionCode<SystemArgs>("return Recipient:AttitudeTo(AggressorIfAny) < -4")
                }
            };

            them.Dialogue.Verb = "talk";
            them.Dialogue.Banter = new[]
            {
                new Guid("4abbc8e5-880c-44d3-ba0e-a9f13a0522d0"),
                new Guid("00d77067-da1c-4c34-96ee-8a74353e4839")
            };

            w.Dialogue.AllDialogues.Add(friend);
            w.Dialogue.AllDialogues.Add(foe);
            
            var ui = GetUI("talk:Chaos Sam");
            w.RunRound(ui,you.GetFinalActions().OfType<DialogueAction>().Single());

            Assert.Contains(areFriends ? "Hello Friend" : "Hello Foe",ui.MessagesShown);

        }

        [Test]
        public void Test_SimpleSubstitution()
        {
            var you = YouInARoom(out IWorld world);
            you.Name = "Flash";
            var npc = new Npc("Space Crab",you.CurrentLocation);
            npc.Dialogue.Next = new Guid("339271e0-7b11-4aba-a9e2-2776f6c5a197");

            var yaml = @"- Identifier: 339271e0-7b11-4aba-a9e2-2776f6c5a197
  Body: 
    - Text: ""Greetings {AggressorIfAny} I am {Recipient}""";
         
            
            var dlg = new DialogueSystem{AllDialogues = Compiler.Instance.Deserializer.Deserialize<List<DialogueNode>>(yaml)};

            var ui = GetUI();
            dlg.Apply(new SystemArgs(world,ui,0,you,npc,Guid.Empty));
            Assert.Contains("Greetings Flash I am Space Crab",ui.MessagesShown);
            
        }

        [Test]
        public void Test_AdvancedSubstitution()
        {
            TwoInARoomWithRelationship(-50,false,out You you,out IActor npc, out IWorld world);
            you.Name = "Flash";
            npc.Dialogue.Next = new Guid("339271e0-7b11-4aba-a9e2-2776f6c5a197");

            var yaml = @"- Identifier: 339271e0-7b11-4aba-a9e2-2776f6c5a197
  Body: 
    - Text: ""I really hate {Recipient:WorstEnemy(false,-10)}""";
         
            var dlg = new DialogueSystem{AllDialogues = Compiler.Instance.Deserializer.Deserialize<List<DialogueNode>>(yaml)};

            var ui = GetUI();
            dlg.Apply(new SystemArgs(world,ui,0,you,npc,Guid.Empty));
            Assert.Contains("I really hate Flash",ui.MessagesShown);
            
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Test_Substitutions(bool areFriends)
        {
            TwoInARoomWithRelationship(areFriends ? 10:-10,false,out You you, out IActor them,out IWorld world);
            
            them.Name = "Space Crab";
            them.Dialogue.Next = new Guid("339271e0-7b11-4aba-a9e2-2776f6c5a197");

            var yaml = @"- Identifier: 339271e0-7b11-4aba-a9e2-2776f6c5a197
  Body: 
    - Text: Screeeee (this creature seems friendly)
      Condition: 
        - return Recipient:AttitudeTo(AggressorIfAny) > 0
    - Text: Screeeee (this creature seems hostile)
      Condition: 
        - return Recipient:AttitudeTo(AggressorIfAny)  < 0
    - Text: Screeeee (this creature seems indifferent)
      Condition: 
        - return Recipient:AttitudeTo(AggressorIfAny) == 0";
            
            var dlg = new DialogueSystem{AllDialogues = Compiler.Instance.Deserializer.Deserialize<List<DialogueNode>>(yaml)};

            var ui = GetUI();
            dlg.Apply(new SystemArgs(world,ui,0,you,them,Guid.Empty));

            if(areFriends)
                Assert.Contains("Screeeee (this creature seems friendly)",ui.MessagesShown);
            else
                Assert.Contains("Screeeee (this creature seems hostile)",ui.MessagesShown);
        }
        
        [Test]
        public void TestConditionalDialogue_RoomHasLight()
        {
            string yaml = @"
- Identifier: ce16ae16-4de8-4e33-8d52-ace4543ada20
  Body: 
    - Text: This room is
    - Text: Pitch Black
      Condition: 
        - return Room:Has('Light') == false
    - Text: Dimly Illuminated
      Condition: 
        - return Room:Has('Light')";

            var system = new DialogueSystem{AllDialogues = Compiler.Instance.Deserializer.Deserialize<List<DialogueNode>>(yaml)};
            Assert.IsNotNull(system);

            var ui = GetUI();

            var room = InARoom(out IWorld world);

            system.Run(new SystemArgs(world,ui,0,
                Mock.Of<IActor>(a=> a.CurrentLocation == room),room,
                Guid.NewGuid()),system.AllDialogues.Single());

            Assert.Contains("This room is Pitch Black",ui.MessagesShown);

            room.Adjectives.Add(world.AdjectiveFactory.Create(room,"Light"));

            system.Run(new SystemArgs(world,ui,0,
                Mock.Of<IActor>(a=> a.CurrentLocation == room),room,
                Guid.NewGuid()),system.AllDialogues.Single());

            Assert.Contains("This room is Dimly Illuminated",ui.MessagesShown);
        }
        
        [Test]
        public void TestConditionalDialogue_ActorHasStat()
        {
            string yaml = @"
- Identifier: ce16ae16-4de8-4e33-8d52-ace4543ada20
  Body: 
    - Text: The denizens of this degenerate bar 
    - Text: make you nervous
      Condition: 
        - return AggressorIfAny:GetFinalStats()[Stat.Corruption] <= 5
    - Text: seem like your kind of people
      Condition: 
        - return AggressorIfAny:GetFinalStats()[Stat.Corruption] > 5";

            var system = new DialogueSystem{AllDialogues = Compiler.Instance.Deserializer.Deserialize<List<DialogueNode>>(yaml)};
            Assert.IsNotNull(system);

            var ui = GetUI();

            var you = YouInARoom(out IWorld world);
            you.BaseStats[Stat.Corruption] = 0;

            system.Run(new SystemArgs(world,ui,0,you,you.CurrentLocation, Guid.NewGuid()),system.AllDialogues.Single());

            Assert.Contains("The denizens of this degenerate bar make you nervous",ui.MessagesShown);

            you.BaseStats[Stat.Corruption] = 10;
            
            system.Run(new SystemArgs(world,ui,0,you,you.CurrentLocation, Guid.NewGuid()),system.AllDialogues.Single());

            Assert.Contains("The denizens of this degenerate bar seem like your kind of people",ui.MessagesShown);
        }

        [Test]
        public void TestSingleUse_DialogueOption()
        {
                string yaml = @"
- Identifier: ce16ae16-4de8-4e33-8d52-ace4543ada20
  Body: 
    - Text: You want some Death Sticks?  
  Options:
    - Text: Yes give me 500
      SingleUse: true
    - Text: Sure give me 1";

                var system = new DialogueSystem{AllDialogues = Compiler.Instance.Deserializer.Deserialize<List<DialogueNode>>(yaml)};
                Assert.IsNotNull(system);
                var ui = GetUI("Yes give me 500");
                var you = YouInARoom(out IWorld world);

                Assert.IsFalse(system.AllDialogues.First().Options.First().Exhausted);

                //option should be allowed
                system.Run(new SystemArgs(world,ui,0,you,you.CurrentLocation, Guid.NewGuid()),system.AllDialogues.Single());

                ui = GetUI("Yes give me 500");
                
                //next time around you shouldn't be able to pick it
                Assert.Throws<Exception>(()=>system.Run(new SystemArgs(world,ui,0,you,you.CurrentLocation, Guid.NewGuid()),system.AllDialogues.Single()));

                //because it is exhausted
                Assert.IsTrue(system.AllDialogues.First().Options.First().Exhausted);

                //but you should be able to still pick this one
                ui = GetUI("Sure give me 1");
                
                //next time around you shouldn't be able to pick it
                system.Run(new SystemArgs(world, ui, 0, you, you.CurrentLocation, Guid.NewGuid()),
                    system.AllDialogues.Single());


        }

    }
}
