using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorTrees;
using Tree = BehaviorTrees.Tree;  


public class PlayerController : MonoBehaviour
{
    [SerializeField] public GameObject target;
    [SerializeField] public float detectionRange;
    [SerializeField] public float wanderRadius;
    [SerializeField] public float wanderTimer;
    [SerializeField] public GameObject goal;
    [SerializeField] public GameObject cop;
    [SerializeField] public float speed;

    public NavMeshAgent agent;
    public Tree tree;
    public Animator animator;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        tree = new Tree("Player");

        Sequence playerActions = new Sequence("Player Actions");

        Parallel beforeTreasure = new Parallel("Before Treasure");
        playerActions.AddChild(beforeTreasure);

        UntilSuccess targetClose = new UntilSuccess("Target Close");
        targetClose.AddChild(new Leaf("IsTargetClose?", new Condition(() => IsOnRange())));
        beforeTreasure.AddChild(targetClose);

        PrioritySelector moveToTreasure = new PrioritySelector("Move To Treasure");

        Sequence copNear = new Sequence("Cop Near", 100);
        copNear.AddChild(new Leaf("IsCopNear?", new Condition(() => IsCopNear(copNear))));
        copNear.AddChild(new Leaf("Wander", new Wander(agent, wanderRadius, wanderTimer)));

        moveToTreasure.AddChild(copNear);

        Sequence takeTreasure = new Sequence("Take Treasure");
        takeTreasure.AddChild(new Leaf("GoToTreasure", new MoveToTarget(this.transform, agent, target.transform)));
        moveToTreasure.AddChild(takeTreasure);

        beforeTreasure.AddChild(moveToTreasure);
        //Leaf setActive = new Leaf("SetActive", new ActionBehavior(() => target.SetActive(false)));

        tree.AddChild(playerActions);
    }

    
    void Update()
    {
        animator.SetFloat("Speed", agent.velocity.magnitude);
        tree.Evaluate();
    }

    private bool IsOnRange()
    {
        if (target == null) 
            return false;
        return Vector3.Distance(transform.position, target.transform.position) <= detectionRange;
    }

    // Check if cop is near the treasure
    private bool IsCopNear(Node node)
    {
        if (cop == null)
        {
            node.Reset();
            return false;
        }

        float distance = Vector3.Distance(target.transform.position, cop.transform.position);
        if (distance > 30)
        {
            node.Reset();
            return false;
        }

        return true;
    }
}
