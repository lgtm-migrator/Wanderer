﻿using System;
using System.Collections.Generic;
using System.Linq;
using StarshipWanderer;
using StarshipWanderer.Actors;
using StarshipWanderer.Systems;

namespace StarshipWanderer.Systems
{
    public class DialogueSystem : IDialogueSystem
    {
        public IList<DialogueNode> AllDialogues { get; set; } = new List<DialogueNode>();

        public void Apply(SystemArgs args)
        {
            if (args.AggressorIfAny == null)
                return;

            if (args.AggressorIfAny is You)
            {
                var d = GetDialogue(args.Recipient.NextDialog);
                
                if (d != null)
                    Run(args,d);
                else
                {
                    args.UserInterface.ShowMessage("Dialogue", $"{args.Recipient} had nothing interesting to say");
                }
                
            }

        }

        private DialogueNode GetDialogue(Guid? g)
        {
            return g.HasValue ? AllDialogues.SingleOrDefault(d => d.Identifier == g) : null;
        }

        private void Run(SystemArgs args, DialogueNode node)
        {
            if(node.Options.Any())
            {
                if (args.UserInterface.GetChoice("Dialogue", node.Body, out DialogueOption chosen, node.Options.ToArray()))
                    Run(args, chosen);
                else
                {
                    //if user hits Escape just pick the first option for them :)
                    Run(args,node.Options.First());
                }

            }
            else
                args.UserInterface.ShowMessage("Dialogue",node.Body);
        }

        private void Run(SystemArgs args, DialogueOption option)
        {
            if (option.Attitude.HasValue)
            {
                var w = args.AggressorIfAny.CurrentLocation.World;
                w.Relationships.Apply(new SystemArgs(args.UserInterface,option.Attitude.Value,args.AggressorIfAny,args.Recipient,args.Round));
            }

            var d = GetDialogue(option.Destination);

            if (d != null) 
                Run(args,d);
        }


        public bool CanTalk(IActor actor, IActor other)
        {
            return true;
        }

        public IEnumerable<IActor> GetAvailableTalkTargets(IActor actor)
        {
            return actor.GetCurrentLocationSiblings().Where(o => CanTalk(actor, o));
        }
    }
}