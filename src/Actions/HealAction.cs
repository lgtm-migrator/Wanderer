﻿using System.Linq;
using Wanderer.Actors;
using Wanderer.Adjectives.ActorOnly;

namespace Wanderer.Actions
{
    public class HealAction : Action
    {
        public override void Push(IUserinterface ui, ActionStack stack, IActor actor)
        {
            if (actor.Decide(ui, "Heal", "Choose who to heal", out IActor target, GetTargets(actor), 10))
                if(actor.Decide(ui,"Injury", "Choose an Injury",out Injured toHeal, target.Adjectives.OfType<Injured>().ToArray(),10))
                    if (toHeal.IsHealableBy(actor,out string reason))
                        stack.Push(new HealFrame(actor, this, target, toHeal, 10));
                    else
                        ShowNarrative(ui,actor,"Cannot Heal","This wound looks too severe for my skill", $"{actor} was unable to heal {target}'s {toHeal} because {reason}",stack.Round);
        }


        public override void Pop(IUserinterface ui, ActionStack stack, Frame frame)
        {
            var f = (HealFrame)frame;

            if (f.TargetIfAny.Adjectives.Contains(f.Injury)) 
                f.Injury.Heal(ui, stack.Round);
        }

        public override bool HasTargets(IActor performer)
        {
            return GetTargets(performer).Any();
        }

        private IActor[] GetTargets(IActor performer)
        {
            return performer.CurrentLocation.Actors.Where(a => a.Has<Injured>(false) && !a.Dead).ToArray();
        }
    }
}