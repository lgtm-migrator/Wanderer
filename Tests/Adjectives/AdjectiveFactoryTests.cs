﻿using Moq;
using NUnit.Framework;
using StarshipWanderer;
using StarshipWanderer.Factories;
using StarshipWanderer.Factories.Blueprints;
using StarshipWanderer.Places;

namespace Tests.Adjectives
{
    class AdjectiveFactoryTests
    {
        [TestCase("Strong")]
        [TestCase("Tough")]
        [TestCase("Rusty")]
        public void TestCreate_AdjectiveBlueprint(string typeName)
        {
            var adj = new AdjectiveFactory();
            Assert.IsNotNull(adj.Create(Mock.Of<IHasStats>(),new AdjectiveBlueprint(){Type = typeName}));
        }

        [TestCase("Dark")]
        public void TestCreateRoomOnly_AdjectiveBlueprint(string typeName)
        {
            var adj = new AdjectiveFactory();
            Assert.IsNotNull(adj.Create(Mock.Of<IPlace>(),new AdjectiveBlueprint(){Type = typeName}));
        }
    }
}