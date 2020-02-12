﻿using System.Collections.Generic;
using Wanderer.Actors;

namespace Wanderer.Adjectives
{
    public class Tough : Adjective
    {
        public Tough(IHasStats owner) : base(owner)
        {
        }

        public override IEnumerable<string> GetDescription()
        {
            yield return "Wounds do not get infected";
        }
    }
}