using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    // Raycast
    public Camera frustum;
    public LayerMask mask;

    //wander
    NavMeshAgent agent;
    private Vector3 worldTarget = Vector3.zero;
    private float freqWander = 0f;
    private float freqChase = 0f;

    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        frustum = gameObject.GetComponentInChildren<Camera>();
        mask = LayerMask.GetMask("Target"); 
        animator = GetComponent<Animator>();
    }

    
    void Update()
    {
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    void chase()
    {
        agent.speed = 5f;

        freqChase += Time.deltaTime;

        if (freqChase > EnemyManager.myManager.freqMaxChase)
        {
            freqChase -= EnemyManager.myManager.freqMaxChase;
            agent.SetDestination(EnemyManager.myManager.target.transform.position);
        }

        if (!EnemyManager.myManager.target.activeInHierarchy)
        {
            EnemyManager.myManager.enemyState = state.WANDER;
        }

        else if ((gameObject.transform.position - EnemyManager.myManager.target.transform.position).sqrMagnitude < EnemyManager.myManager.distanceToKill * EnemyManager.myManager.distanceToKill * 2)
        {
            EnemyManager.myManager.target.SetActive(false);
            EnemyManager.myManager.enemyState = state.WANDER;
        }
    }

    void wander()
    {
        agent.speed = 3f;

        freqWander += Time.deltaTime;

        if (freqWander > EnemyManager.myManager.freqMaxWander)
        {
            freqWander -= EnemyManager.myManager.freqMaxWander;
            Vector3 localTarget = UnityEngine.Random.insideUnitCircle * EnemyManager.myManager.radius;
            localTarget += new Vector3(0, 0, EnemyManager.myManager.offset);
            worldTarget = transform.TransformPoint(localTarget);
            worldTarget.y = 0f;
        }

        agent.destination = worldTarget;
    }
}
