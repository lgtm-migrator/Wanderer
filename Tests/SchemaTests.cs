﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using NJsonSchema.Generation;
using Wanderer.Dialogues;
using Wanderer.Factories.Blueprints;
using Wanderer.Systems;

namespace Tests
{
    class SchemaTests
    {
        readonly JsonSchemaGenerator _generator = 
            new JsonSchemaGenerator(new JsonSchemaGeneratorSettings()
            {
                FlattenInheritanceHierarchy = true,
                GenerateEnumMappingDescription = true
            });
        


        [Test]
        public void ActionBlueprintSchema()
        {
            TestContext.Out.WriteLine(_generator.Generate(typeof(List<ActionBlueprint>)).ToJson());
        }


        [Test]
        public void AdjectivesBlueprintSchema()
        {
            TestContext.Out.WriteLine(_generator.Generate(typeof(List<AdjectiveBlueprint>)).ToJson());
        }

        [Test]
        public void ItemBlueprintSchema()
        {
            TestContext.Out.WriteLine(_generator.Generate(typeof(List<ItemBlueprint>)).ToJson());
        }
        [Test]
        public void ActorBlueprintSchema()
        {
            TestContext.Out.WriteLine(_generator.Generate(typeof(List<ActorBlueprint>)).ToJson());
        }
        
        [Test]
        public void RoomBlueprintSchema()
        {
            TestContext.Out.WriteLine(_generator.Generate(typeof(List<RoomBlueprint>)).ToJson());
        }

        [Test]
        public void DialogueSchema()
        {
            TestContext.Out.WriteLine(_generator.Generate(typeof(List<DialogueNode>)).ToJson());
        }

        [Test]
        public void InjurySystemSchema()
        {
            TestContext.Out.WriteLine(_generator.Generate(typeof(InjurySystem)).ToJson());
        }
    }
}
