﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actors;
using Wanderer.Compilation;
using Wanderer.Factories;
using Wanderer.Factories.Blueprints;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace Tests.ConditionTests
{
    class ConditionSerializationTests : UnitTest
    {
        [TestCase("return false")]
        [TestCase("return true")]
        public void TestConstructors(string condition)
        {
            var yaml =
                @$"
- Name: MedKit
  Require: 
    - Lua: {condition}
";
            var itemFactory = new ItemFactory{Blueprints = Compiler.Instance.Deserializer.Deserialize<List<ItemBlueprint>>(yaml)};
            var createdInstance = itemFactory.Create(new World(), itemFactory.Blueprints.Single());

            Assert.AreEqual(condition == "return true", createdInstance.Require.Single().IsMet(new World(),Mock.Of<IActor>()));

        }
    }
}
