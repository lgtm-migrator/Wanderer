﻿using System;
using System.Collections.Generic;
using System.Linq;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Adjectives.ActorOnly;
using Wanderer.Adjectives.RoomOnly;
using Wanderer.Stats;

namespace Wanderer.Systems
{
    public class InjurySystem : IInjurySystem
    {
        public void Apply(SystemArgs args, InjuryRegion region)
        {
            if(args.Intensity < 0 || region == InjuryRegion.None)
                return;

            var a = (IActor) args.Recipient;

            //currently you can't damage rooms or burn books
            if (a == null)
                return;

            var available = GetAvailableInjuries(a).ToArray();

            var worst = available.Max(i => i.Severity);

            var newInjury = available.FirstOrDefault(a =>
                (int)a.Severity == (int)Math.Min(worst, args.Intensity / 10) && a.Region == region);

            if(newInjury == null)
                throw new Exception("No Injury  found for severity " + args.Intensity);

            args.Recipient.Adjectives.Add(newInjury);
            args.UserInterface.Log.Info(new LogEntry($"{args.Recipient} gained {newInjury}", args.Round,a));
        }

        public void Apply(SystemArgs args)
        {
            var regions = Enum.GetValues(typeof(InjuryRegion)).Cast<InjuryRegion>().ToArray();

            //Generate a random region excluding None
            Apply(args,regions[(int) Math.Abs(args.Intensity % (regions.Length-1)+1)]);
        }

        public virtual IEnumerable<Injured> GetAvailableInjuries(IActor actor)
        {
            foreach (InjuryRegion region in Enum.GetValues(typeof(InjuryRegion)))
            {
                double severity = 0;
                foreach (var s in new string[]{"Bruised","Cut","Lacerated","Fractured","Broken","Detached"})
                    yield return new Injured(s + " " + region, actor, severity++, region,this);
            }
        }

        public bool HasFatalInjuries(IActor owner, out string diedOf)
        {
            //Combined total of serious wounds (2 or higher) severity is 10
            if (owner.Adjectives.OfType<Injured>().Where(i => i.Severity > 1).Sum(i => i.Severity) >= 10)
            {
                diedOf = "injuries";
                return true;
            }

            diedOf = null;
            return false;
        }

        public bool ShouldWorsen(Injured injury, int roundsSeen)
        {
            if (IsWithinNaturalHealingThreshold(injury) || injury.OwnerActor.Dead)
                return false;

            double worsenRate = 1;

            if (injury.OwnerActor.Has<Tough>(true))
                worsenRate--;

            if (injury.OwnerActor.CurrentLocation.Has<Stale>())
                worsenRate++;

            return Math.Abs(worsenRate) > 0.0001 && Math.Abs(roundsSeen % (injury.Severity*2 / worsenRate)) < 0.0001;
        
        }

        public bool IsHealableBy(IActor actor, Injured injured, out string reason)
        {
            var requiredSavvy = injured.Severity * 5;

            //harder to heal giant things
            if (injured.OwnerActor.Has<Giant>(false))
                requiredSavvy *= 1.5;

            if (actor.GetFinalStats()[Stat.Savvy] > requiredSavvy)
            {
                reason = null;
                return true;
            }

            reason = $"Savvy was too low (required {requiredSavvy})";
            return false;
        }

        public bool ShouldNaturallyHeal(Injured injured, in int roundsSeen)
        {
            return !injured.OwnerActor.Dead && IsWithinNaturalHealingThreshold(injured) && roundsSeen >= 10;
        }

        private bool IsWithinNaturalHealingThreshold(Injured injured)
        {
            return injured.Severity <= 1;
        }
    }
}