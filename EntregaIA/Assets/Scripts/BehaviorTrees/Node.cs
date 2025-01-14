using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public int priority;
        public List<Node> children = new List<Node>();
        protected int currentChild;
        public Node(string name, int priority = 0)
        {
            this.name = name;
            this.priority = priority;
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
    public class Leaf : Node
    {
        IBehavior behavior;
        public Leaf(string name, IBehavior behavior, int priority = 0) : base(name, priority)
        {
            this.behavior = behavior;
        }
        public override NodeState Evaluate() => behavior.Evaluate();
        public override void Reset() => behavior.Reset();
    }

    public class Sequence : Node
    {
        public Sequence(string name, int priority = 0) : base(name, priority) { }
        public override NodeState Evaluate()
        {
            if (currentChild < children.Count)
            {
                switch (children[currentChild].Evaluate())
                {
                    case NodeState.RUNNING:
                        return NodeState.RUNNING;
                    case NodeState.FAILURE:
                        Reset();
                        return NodeState.FAILURE;
                    default:
                        currentChild++;
                        return currentChild == children.Count ? NodeState.SUCCESS : NodeState.RUNNING;
                }
            }

            Reset();
            return NodeState.SUCCESS;
        }
    }

    public class Selector : Node
    {
        public Selector(string name, int priority = 0) : base(name, priority) { }
        public override NodeState Evaluate()
        {
            if (currentChild < children.Count)
            {
                switch (children[currentChild].Evaluate())
                {
                    case NodeState.RUNNING:
                        return NodeState.RUNNING;
                    case NodeState.SUCCESS:
                        Reset();
                        return NodeState.SUCCESS;
                    default:
                        currentChild++;
                        return NodeState.RUNNING;
                }
            }
            Reset();
            return NodeState.FAILURE;
        }
    }

    public class PrioritySelector : Node
    {
        List<Node> sortedChildren;
        List<Node> SortedChildren => sortedChildren ??= SortChildren();

        protected virtual List<Node> SortChildren() => children.OrderByDescending(child => child.priority).ToList();

        public PrioritySelector(string name) : base(name) { }

        public override void Reset()
        {
            base.Reset();
            sortedChildren = null;
        }
        public override NodeState Evaluate()
        {
            foreach (Node child in SortedChildren)
            {
                switch (child.Evaluate())
                {
                    case NodeState.RUNNING:
                        return NodeState.RUNNING;
                    case NodeState.SUCCESS:
                        Reset();
                        return NodeState.SUCCESS;
                    default:
                        continue;
                }
            }
            return NodeState.FAILURE;
        }
    }

    public class UntilFail : Node
    {
        public UntilFail(string name) : base(name) { }
        public override NodeState Evaluate()
        {
            if(children[0].Evaluate() == NodeState.FAILURE)
            {
                Reset();
                return NodeState.FAILURE;
            }
            return NodeState.RUNNING;
        }
    }

    public class UntilSuccess : Node
    {
        public UntilSuccess(string name) : base(name) { }
        public override NodeState Evaluate()
        {
            if (children[0].Evaluate() == NodeState.SUCCESS)
            {
                Reset();
                return NodeState.SUCCESS;
            }
            return NodeState.RUNNING;
        }
    }

    public class Parallel : Node
    {
        public Parallel(string name) : base(name) { }

        public override NodeState Evaluate()
        {
            bool anyRunning = false;
            bool allSuccess = true;

            foreach (var child in children)
            {
                switch (child.Evaluate())
                {
                    case NodeState.SUCCESS:
                        allSuccess = allSuccess && true;
                        break;
                    case NodeState.RUNNING:
                        anyRunning = true;
                        break;
                    case NodeState.FAILURE:
                        return NodeState.FAILURE;
                }
            }

            if (anyRunning)
            {
                return NodeState.RUNNING;
            }

            if (allSuccess)
            {
                Reset();
                return NodeState.SUCCESS;
            }

            return NodeState.RUNNING;
        }

        public override void Reset()
        {
            base.Reset();
            foreach (var child in children)
            {
                child.Reset();
            }
        }
    }

    public class Repeat : Node
    {
        private int repetitions; 
        private int currentRepetition;

        // Finite repetitions
        public Repeat(string name, int repetitions) : base(name)
        {
            this.repetitions = repetitions;
            this.currentRepetition = 0;
        }
        // Infinite repetitions
        public Repeat(string name) : base(name)
        {
            this.repetitions = -1; 
            this.currentRepetition = 0;
        }

        public override NodeState Evaluate()
        {
            if (repetitions > 0 && currentRepetition >= repetitions)
            {
                Reset();
                return NodeState.SUCCESS;
            }

            NodeState childState = children[0].Evaluate();

            if (childState == NodeState.SUCCESS || childState == NodeState.FAILURE)
            {
                currentRepetition++;
                children[0].Reset();
            }

            return NodeState.RUNNING;
        }

        public override void Reset()
        {
            base.Reset();
            currentRepetition = 0;
        }
    }
}