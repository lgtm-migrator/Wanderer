﻿using System.Text;
using StarshipWanderer.Actions;

namespace StarshipWanderer.Behaviours
{
    public interface IBehaviour
    {
        void OnPush(IUserinterface ui, ActionStack stack);
    }
}
