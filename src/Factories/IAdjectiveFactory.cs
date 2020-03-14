﻿using System;
using System.Collections.Generic;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Factories.Blueprints;

namespace Wanderer.Factories
{
    public interface IAdjectiveFactory
    {
        List<AdjectiveBlueprint> Blueprints { get; set; }

        IAdjective Create(IHasStats owner,AdjectiveBlueprint blueprint);
        IAdjective Create(IHasStats s, Type adjectiveType);
        IAdjective Create(IHasStats s, Guid guid);

        IAdjective Create(IHasStats s, string name);

        /// <summary>
        /// Adds all <see cref="HasStatsBlueprint.MandatoryAdjectives"/> and
        /// some <see cref="HasStatsBlueprint.OptionalAdjectives"/> to
        /// <paramref name="owner"/>
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="ownerBlueprint">the owners blueprint (NOT an <see cref="AdjectiveBlueprint"/>)</param>
        /// <param name="r"></param>
        void AddAdjectives(IHasStats owner, HasStatsBlueprint ownerBlueprint, Random r);
    }
}