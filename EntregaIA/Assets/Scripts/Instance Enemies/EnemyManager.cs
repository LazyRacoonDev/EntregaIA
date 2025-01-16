using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum state { WANDER, CHASE };
public class EnemyManager : MonoBehaviour
{
    public state enemyState = state.WANDER;
    [Header("Enemy Spawn Settings")]
    public static EnemyManager myManager;
    public int distanceToKill = 4;
    public GameObject zombiePrefab;
    public int numZombies = 5;
    public List<GameObject> allZombies;
    public Vector3 limits = new Vector3(5, 5, 5);

    public GameObject target;

    
    [Header("Enemy Wander Settings")]
    [Range(0.0f, 10.0f)]
    public float radius = 10.0f;
    [Range(0.0f, 10.0f)]
    public float offset = 2.0f;
    [Range(0.0f, 10.0f)]
    public float freqMaxWander = 2.5f;

    
    [Header("Enemy Wander Settings")]
    [Range(0.0f, 10.0f)]
    public float freqMaxChase = 0.0f;

    void Start()
    {
        myManager = this;

        StartCoroutine(spawnEnemies());
    }

    // Update is called once per frame
    void Update()
    {
        switch (enemyState)
        {
            case state.WANDER:
                Debug.Log("wander");
                BroadcastMessage("wander", SendMessageOptions.DontRequireReceiver);
                break;
            case state.CHASE:
                Debug.Log("chase");
                BroadcastMessage("chase", SendMessageOptions.DontRequireReceiver);
                break;
            default:
                break;
        }
    }

    IEnumerator spawnEnemies()
    {
        for (int i = 0; i < numZombies; ++i)
        {
            Vector3 pos = this.transform.position + new Vector3(Random.Range(-limits.x, limits.x),
                1, Random.Range(-limits.z, limits.z)); // random position
            allZombies.Add((GameObject)Instantiate(zombiePrefab, pos, Quaternion.identity, GameObject.Find("EnemyManager").transform));
            yield return new WaitForSeconds(0.5f);
        }
    }
}
