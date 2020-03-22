using System;
using Wanderer.Actions;
using Wanderer.Compilation;
using Wanderer.Factories.Blueprints;
using Action = Wanderer.Actions.Action;

namespace Wanderer.Factories
{

    public class ActionFactory : HasStatsFactory<ActionBlueprint, IAction>, IActionFactory
    {
        private TypeCollection _types;

        public ActionFactory()
        {
            _types = Compilation.Compiler.Instance.TypeFactory.Create<IAction>();
        }

        public void Create(IWorld world, IHasStats onto, ActionBlueprint blueprint)
        {
            HandleInheritance(blueprint);
            IAction action;

            if(string.IsNullOrWhiteSpace(blueprint.Type))
                action = new Action(onto);
            else
            try
            {
                var type = _types.GetTypeNamed(blueprint.Type);
                action = (IAction)Activator.CreateInstance(type,onto);   
            }
            catch (Exception ex)
            {
                
                throw new Exception($"Error creating base Type for ActionBlueprint {blueprint}",ex);
            }

            AddBasicProperties(world,action,blueprint,"use");

            action.HotKey = blueprint.HotKey;
            action.Effect = blueprint.Effect;

            onto.BaseActions.Add(action);
        }
    }
}