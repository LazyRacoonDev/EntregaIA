using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


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
    public class Wander : IBehavior
    {
        public NavMeshAgent agent;
        public float wanderRadius;
        public float wanderTimer;

        private float timer;

        public Wander(NavMeshAgent agent, float wanderRadius, float wanderTimer)
        {
            this.agent = agent;
            this.wanderRadius = wanderRadius;
            this.wanderTimer = wanderTimer;
            timer = wanderTimer;
        }

        public NodeState Evaluate()
        {
            timer += Time.deltaTime;
            if (timer >= wanderTimer)
            {
                Vector3 randDirection = Random.insideUnitSphere * wanderRadius;
                randDirection += agent.transform.position;
                NavMeshHit hit;
                NavMesh.SamplePosition(randDirection, out hit, wanderRadius, -1);
                Vector3 newPos = hit.position;
                agent.SetDestination(newPos);
                timer = 0;
            }

            if (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            {
                return NodeState.RUNNING;
            }

            return NodeState.SUCCESS;
        }
    }

    public class MoveToTarget : IBehavior
    {
        public Transform entity;
        public NavMeshAgent agent;
        public Transform target;
        public bool isPathSet;

        public MoveToTarget(Transform entity, NavMeshAgent agent, Transform target)
        {
            this.entity = entity;
            this.agent = agent;
            this.target = target;
        }

        public NodeState Evaluate()
        {
            if (target == null)
            {
                return NodeState.FAILURE;
            }

            if (Vector3.Distance(entity.position, target.position) < 1.0f)
            {
                Debug.Log("Target reached");
                return NodeState.SUCCESS;
            }

            agent.SetDestination(target.position);
            entity.LookAt(target.position);

            if (agent.pathPending)
            {
                isPathSet = true;
            }

            if (!isPathSet)
            {
                return NodeState.FAILURE;
            }

            return NodeState.RUNNING;
        }

        public void Reset() => isPathSet = false;
    }
    
    public class MoveToTargetInRange : IBehavior
    {
        public Transform entity;
        public NavMeshAgent agent;
        public Transform target;
        public float range; 
        public bool isPathSet;

        public MoveToTargetInRange(Transform entity, NavMeshAgent agent, Transform target, float range)
        {
            this.entity = entity;
            this.agent = agent;
            this.target = target;
            this.range = range;
        }

        public NodeState Evaluate()
        {
            if (target == null)
            {
                return NodeState.FAILURE;
            }
            float distance = Vector3.Distance(entity.position, target.position);

            if (distance > range)
            {
                return NodeState.FAILURE;
            }

            if (distance < 1.0f)
            {
                Debug.Log("Target reached");
                return NodeState.SUCCESS;
            }

            agent.SetDestination(target.position);
            entity.LookAt(target.position);

            if (agent.pathPending)
            {
                isPathSet = true;
            }
            return NodeState.RUNNING;
        }

        public void Reset() => isPathSet = false;
    }

    public class RunAway : IBehavior
    {
        private NavMeshAgent agent; 
        private float escapeRange;
        private LayerMask enemyLayer;

        public RunAway(NavMeshAgent agent, float escapeRange, LayerMask enemyLayer)
        {
            this.agent = agent;
            this.escapeRange = escapeRange;
            this.enemyLayer = enemyLayer;
        }

        public NodeState Evaluate()
        {
            
            Collider[] enemies = Physics.OverlapSphere(agent.transform.position, escapeRange, enemyLayer);

            if (enemies.Length == 0)
            {
                return NodeState.SUCCESS;
            }

            Vector3 escapeDirection = Vector3.zero;

            foreach (Collider enemy in enemies)
            {
                escapeDirection += (agent.transform.position - enemy.transform.position).normalized;
            }

            escapeDirection = escapeDirection.normalized;

            Vector3 escapePosition = agent.transform.position + escapeDirection * escapeRange;

            if (NavMesh.SamplePosition(escapePosition, out NavMeshHit hit, escapeRange, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                return NodeState.RUNNING;
            }

            return NodeState.FAILURE;
        }
    }

}
