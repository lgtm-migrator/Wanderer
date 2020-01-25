﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using StarshipWanderer.Actors;
using StarshipWanderer.Dialogues;
using StarshipWanderer.Dialogues.Tokens;
using StarshipWanderer.Extensions;
using YamlDotNet.Serialization;

namespace StarshipWanderer.Systems
{
    public class DialogueSystem : IDialogueSystem
    {
        public IList<DialogueNode> AllDialogues { get; set; } = new List<DialogueNode>();

        [JsonIgnore] 
        protected DialogueTokenCollection Substitutions = new DialogueTokenCollection();

        public DialogueSystem(params string[] dialogueYaml)
        {
            var de = new Deserializer();

            foreach (string yaml in dialogueYaml)
            {
                try
                {
                    foreach (var dialogueNode in de.Deserialize<DialogueNode[]>(yaml)) 
                        AllDialogues.Add(dialogueNode);
                }
                catch (Exception e)
                {
                    throw new ArgumentException("Error in dialogue yaml:" + e.Message);
                }
            }
            
            Substitutions.Add(a=>a.AggressorIfAny?.ToString(), "aggressor");
            Substitutions.Add(a=>a.Recipient.ToString(), "this");
            Substitutions.Add(new DescribeRelationshipToken());
        }

        public void Apply(SystemArgs args)
        {
            if (args.AggressorIfAny == null)
                return;

            if (args.AggressorIfAny is You)
            {
                var d = GetDialogue(args.Recipient.Dialogue.Next) ?? GetBanter(args);
                
                if (d != null)
                    Run(args,d);
                else
                {
                    args.UserInterface.ShowMessage("Dialogue", $"{args.Recipient} had nothing interesting to say");
                }
                
            }

        }

        public DialogueNode GetDialogue(Guid? g)
        {
            return g.HasValue ? AllDialogues.SingleOrDefault(d => d.Identifier == g) : null;
        }

        public void Run(SystemArgs args, DialogueNode node)
        {
            if(node.Options.Any())
            {
                if (args.UserInterface.GetChoice("Dialogue", FormatString(args,node.Body), out DialogueOption chosen, node.Options.ToArray()))
                    Run(args, chosen);
                else
                {
                    //if user hits Escape just pick the first option for them :)
                    Run(args,node.Options.First());
                }

            }
            else
                args.UserInterface.ShowMessage("Dialogue",FormatString(args,node.Body));
        }

        protected virtual string FormatString(SystemArgs args,string body)
        {
            StringBuilder sb = new StringBuilder(body);
            foreach (var sub in Substitutions) 
                sb = sb.Replace(
                    '{' + string.Join(' ',sub.Tokens) + '}'
                    , sub.GetReplacement(args));

            return sb.ToString();
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

        public DialogueNode GetBanter(SystemArgs args)
        {
            var world = args.AggressorIfAny.CurrentLocation.World;

            var a = args.Recipient as IActor;

            if (a == null)
                return null;

            var relationship = world.Relationships.SumBetween(a,args.AggressorIfAny);

            var suitable = AllDialogues.Where(a => a.Suits == Banter.Neutral
                                    || a.Suits == Banter.Friend && relationship >= 0
                                    || a.Suits == Banter.Foe && relationship < 0);
            
            return suitable.ToList().Shuffle(world.R).FirstOrDefault();
        }
    }
}