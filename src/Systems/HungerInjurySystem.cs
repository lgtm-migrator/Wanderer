using System;
using System.Collections.Generic;
using Wanderer.Actors;
using Wanderer.Adjectives;

namespace Wanderer.Systems
{
    public class HungerInjurySystem : InjurySystem
    {
        public override Guid Identifier {get;set;} = new Guid("89c18233-5250-4445-8799-faa9a888fb7f");

        public override IEnumerable<Injured> GetAvailableInjuries(IActor actor)
        {
            for(double i = 1 ; i <=5;i++)
                yield return new Injured(
                    GetDescription(i),actor,1,InjuryRegion.None,this){

                        //mark the injury as comming from hunger system
                        Identifier = Identifier
                    };
        }

        private string GetDescription(double severity)
        {
            if(severity <= 1.0001)
                return "Peckish";
            if(severity <= 2.0001)
                return "Hungry";
            if(severity <= 3.0001)
                return "Famished";
            if(severity <= 4.0001)
                return "Ravenous";

            return "Starved";
        }

        public override void Heal(Injured injured, IUserinterface ui, Guid round)
        {
            injured.Severity -= 2;
            if(injured.Severity <= 0)
                injured.Owner.Adjectives.Remove(injured);
            else
                injured.Name = GetDescription(injured.Severity);
        }

        public override bool IsHealableBy(IActor actor, Injured injured, out string reason)
        {
            //only food heals injuries
            reason = "You need to eat something";
            return false;
        }

        public override void Worsen(Injured injured, IUserinterface ui, Guid round)
        {
            injured.Severity++;
            injured.Name = GetDescription(injured.Severity);
        }

        public override bool HasFatalInjuries(IInjured injured, out string diedOf)
        {
            diedOf = "Hunger";
            return injured.Severity>=7;
        }

        protected override IEnumerable<InjuryRegion> GetAvailableInjuryLocations(SystemArgs args)
        {
            yield return InjuryRegion.None;
        }

        protected override bool ShouldNaturallyHealImpl(Injured injured, int roundsSeenCount)
        {
            return false;
        }

        protected override bool IsWithinNaturalHealingThreshold(Injured injured)
        {
            return false;
        }

        protected override bool ShouldWorsenImpl(Injured injury, int roundsSeen)
        {
            return roundsSeen > injury.Severity * 2.001;
        }
    }
}