﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarshipWanderer.Actions;

namespace StarshipWanderer.Behaviours
{
    public class ForbidBehaviour<T> : Behaviour<T> where T:IAction
    {
        public Func<T, bool> Condition { get; }

        public ForbidBehaviour(Func<T,bool> condition)
        {
            Condition = condition;
        }

        public override void OnPush(IUserinterface ui, ActionStack stack)
        {
            foreach (var matchingAction in stack.Where(a => a is T t && Condition(t)).ToArray())
                stack.Push(new ForbidAction(matchingAction));
        }
    }
}
