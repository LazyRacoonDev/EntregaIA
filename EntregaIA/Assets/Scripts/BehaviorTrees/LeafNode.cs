using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BehaviorTrees
{
    public class Leaf : Node
    {
        IBehavior behavior;
        public Leaf(string name, IBehavior behavior) : base(name)
        {
            this.behavior = behavior;
        }
        public override NodeState Evaluate() => behavior.Evaluate();
        public override void Reset() => behavior.Reset();
    }
}

