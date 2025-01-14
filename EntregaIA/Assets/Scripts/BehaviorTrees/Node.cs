using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTrees
{
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }
    public class Node
    {
        public NodeState nodeState;

        public string name; 
        public List<Node> children = new List<Node>();
        protected int currentChild;
        public Node(string name)
        {
            this.name = name;
        }
        public void AddChild(Node child)
        {
            children.Add(child);
        }

        public virtual NodeState Evaluate() => children[currentChild].Evaluate();

        public virtual void Reset()
        {
            currentChild = 0;
            foreach (Node child in children)
                child.Reset();
        }

    }
}