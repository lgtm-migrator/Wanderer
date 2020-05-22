﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actors;
using Wanderer.Factories.Blueprints;
using Wanderer.Systems;

namespace Tests.Factories.Blueprints
{
    class ConditionBlueprintTests : UnitTest
    {
        private SystemArgs GetSystemArgs(IActor forActor)
        {
            return new SystemArgs(forActor.CurrentLocation.World,null,0,forActor,forActor,Guid.Empty);
        }

        [Test]
        public void TestCondition_RoomHas_PassBecauseRoomHasName()
        {
            var you = YouInARoom(out IWorld world);
            var condition = new ConditionBlueprint() {RoomHas = "Fish"}.Create().Single();
            
            Assert.IsFalse(condition.IsMet(world, GetSystemArgs(you)));
            you.CurrentLocation.Name = "Fish";
            Assert.IsTrue(condition.IsMet(world, GetSystemArgs(you)));
        }
        [Test]
        public void TestCondition_RoomNotHas_PassBecauseRoomHasName()
        {
            var you = YouInARoom(out IWorld world);
            var condition = new ConditionBlueprint() {RoomHasNot = "Fish"}.Create().Single();
            
            Assert.IsTrue(condition.IsMet(world, GetSystemArgs(you)));
            you.CurrentLocation.Name = "Fish";
            Assert.IsFalse(condition.IsMet(world, GetSystemArgs(you)));
        }
    }
}
