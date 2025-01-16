using BehaviorTrees;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Tree = BehaviorTrees.Tree;

public class EnemyController : MonoBehaviour
{
    [SerializeField] public List<Transform> waypoints;
    [SerializeField] public float range;
    [SerializeField] public GameObject target;

    public NavMeshAgent agent;
    public Tree tree; 
    public Animator animator;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        tree = new Tree("Enemy");

        PrioritySelector actions = new PrioritySelector("Agent Logic");

        Sequence chase = new Sequence("Chase", 100);
        
        chase.AddChild(new Leaf("IsPlayerNear?", new Condition(() => IsOnRange(chase))));
        chase.AddChild(new Leaf("ChasePlayer", new MoveToTargetInRange(transform, agent, target.transform, range)));

        actions.AddChild(chase);

        Leaf patrol = new Leaf("Patrol", new Patrol(transform, agent, waypoints));
        actions.AddChild(patrol);

        tree.AddChild(actions);

    }

    
    void Update()
    {
        animator.SetFloat("Speed", agent.velocity.magnitude);
        tree.Evaluate();
    }

    bool IsOnRange(Node node)
    {
        if(target == null)
        {
            node.Reset();
            return false;
        }

        float distance = Vector3.Distance(transform.position, target.transform.position);

        if (distance > range)
        {
            node.Reset();
            return false;
        }

        return true;
    }
}
