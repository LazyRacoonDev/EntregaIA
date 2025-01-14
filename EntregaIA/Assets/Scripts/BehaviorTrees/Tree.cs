using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTrees
{
    public class Tree : Node
    {
        public Tree(string name) : base(name) { }

        public override NodeState Evaluate()
        {
            while (currentChild < children.Count)
            {
                NodeState result = children[currentChild].Evaluate();
                if(result != NodeState.SUCCESS)
                {
                    return result;
                }
                currentChild++;
            }
            return NodeState.SUCCESS;
        }
    }
}