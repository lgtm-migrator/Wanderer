﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Dialogues;
using StarshipWanderer.Dialogues.Conditions;

namespace Tests.Systems
{
    class ConditionalDialogueTests : UnitTest
    {
        [TestCase(true)]
        [TestCase(false)]
        public void TestMainDialogueCondition_AreFriends(bool friends)
        {
            TwoInARoomWithRelationship(friends ? 10 : -10,false,out You you, out IActor them, out IWorld w);
            them.BaseActions.Clear();

            var g1 = new Guid("93d68a59-d0ef-4df7-97af-fa3db0840bad");
            var n1 = new DialogueNode()
            {
                Identifier = g1,
                Body = "Hey I want to give you all the space bucks!",
                Conditions = new DialogueConditionCollection(new RelationshipCondition(Comparison.GreaterThanOrEqual,5))

            };

            w.Dialogue.AllDialogues = new List<DialogueNode>(new []{n1});
            them.Dialogue.Next = g1;
            
            for (int i = 0; i < 100; i++)
            {
                var ui = new FixedChoiceUI("talk:Chaos Sam");
                w.RunRound(ui,new DialogueAction());

                if(friends)
                    Assert.Contains("Hey I want to give you all the space bucks!",ui.MessagesShown);
                else
                    Assert.Contains("Chaos Sam had nothing interesting to say", ui.MessagesShown);
            }

        }

    }
}