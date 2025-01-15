using UnityEngine;
using UnityEngine.AI;

public class WaypointNavigator : MonoBehaviour
{
    [Header("Waypoints Settings")]
    [Tooltip("Lista de waypoints por los que el objeto debe moverse.")]
    public Transform[] waypoints;

    [Tooltip("¿Debe repetirse el ciclo de waypoints?")]
    public bool loop = true;

    [Header("NavMesh Agent Settings")]
    [Tooltip("Velocidad del agente.")]
    public float agentSpeed = 3.5f;

    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;

    void Start()
    {
        // Obtener el componente NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("No se encontró un componente NavMeshAgent en el objeto.");
            return;
        }

        // Configurar velocidad inicial
        agent.speed = agentSpeed;

        // Validar waypoints
        if (waypoints.Length == 0)
        {
            Debug.LogError("No se han asignado waypoints al script.");
            return;
        }

        // Iniciar movimiento hacia el primer waypoint
        MoveToNextWaypoint();
    }

    void Update()
    {
        // Si el agente ha llegado al destino, pasar al siguiente waypoint
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            MoveToNextWaypoint();
        }
    }

    void MoveToNextWaypoint()
    {
        if (waypoints.Length == 0) return;

        // Establecer el destino del agente al waypoint actual
        agent.SetDestination(waypoints[currentWaypointIndex].position);

        // Avanzar al siguiente waypoint
        currentWaypointIndex++;

        // Reiniciar el índice si se habilita el loop
        if (loop && currentWaypointIndex >= waypoints.Length)
        {
            currentWaypointIndex = 0;
        }
        else if (currentWaypointIndex >= waypoints.Length)
        {
            agent.isStopped = true; // Detener el agente si no hay más waypoints
        }
    }
}
