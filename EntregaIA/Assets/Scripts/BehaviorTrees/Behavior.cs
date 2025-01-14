using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviorTrees
{
    public interface IBehavior
    {
        NodeState Evaluate();
        void Reset() { }
    }

    public class ActionBehavior : IBehavior
    {
        public Action action; 

        public ActionBehavior(Action action)
        {
            this.action = action;
        }

        public NodeState Evaluate() 
        {
            action();
            return NodeState.SUCCESS;
        }
    }

    public class Condition : IBehavior
    {
        public Func<bool> condition;

        public Condition(Func<bool> condition)
        {
            this.condition = condition;
        }

        public NodeState Evaluate()
        {
            return condition() ? NodeState.SUCCESS : NodeState.FAILURE;
        }
    }
    public class Patrol : IBehavior
    {
        public Transform entity; 
        public NavMeshAgent agent;
        public List<Transform> waypoints;
        public float speed;
        public int currentWaypoint;
        bool isPathSet; 

        public Patrol(Transform entity, NavMeshAgent agent, List<Transform> waypoints, float speed = 2.0f)
        {
            this.entity = entity;
            this.agent = agent;
            this.waypoints = waypoints;
            this.speed = speed;
        }

        public NodeState Evaluate()
        {
            if (currentWaypoint == waypoints.Count)
            {
                return NodeState.SUCCESS;
            }
            Transform target = waypoints[currentWaypoint];
            agent.SetDestination(target.position);
            entity.LookAt(target);

            if (isPathSet && agent.remainingDistance < 0.1f)
            {
                currentWaypoint++;
                isPathSet = false;
            }

            if (agent.pathPending)
            {
                isPathSet = true;
            }
            return NodeState.RUNNING;
        }

        public void Reset()
        {
            currentWaypoint = 0;
        }
    }
}
