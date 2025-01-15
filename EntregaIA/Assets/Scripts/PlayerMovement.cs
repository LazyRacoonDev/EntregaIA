using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float speed = 20f; // Velocidad normal
    [SerializeField] private float slowSpeed = 2f; // Velocidad lenta (cuando se presiona Control)
    [SerializeField] private bool useSmoothMovement = true; // Movimiento suave
    [SerializeField] private float smoothTime = 0.1f; // Tiempo de suavizado

    private Vector3 targetVelocity; // Dirección de movimiento
    private Vector3 currentVelocity; // Velocidad actual para suavizado
    private Rigidbody rb; // Referencia al Rigidbody (opcional)
    private bool isSlowed; // Indica si se está moviendo más lento

    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Intenta obtener un Rigidbody

        // Si hay un Rigidbody, desactiva el efecto de la gravedad
        if (rb != null)
        {
            rb.useGravity = false; // Desactiva la gravedad
            rb.constraints = RigidbodyConstraints.FreezePositionY; // Bloquea el movimiento en Y
        }
    }

    private void Update()
    {
        // Verifica si se está presionando la tecla Control
        isSlowed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        // Obtiene el input de las teclas WASD
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Calcula la velocidad dependiendo de si Control está presionado
        float currentSpeed = isSlowed ? slowSpeed : speed;

        // Calcula la dirección del movimiento, forzando Y a ser 0
        targetVelocity = new Vector3(horizontal, 0f, vertical).normalized * currentSpeed;

        // Si no usamos Rigidbody, aplicamos movimiento directamente
        if (rb == null)
        {
            MoveCharacterDirectly();
        }
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            MoveCharacterWithRigidbody();
        }
    }

    private void MoveCharacterDirectly()
    {
        if (useSmoothMovement)
        {
            // Movimiento suave
            currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, smoothTime);
            transform.Translate(currentVelocity * Time.deltaTime, Space.World);
        }
        else
        {
            // Movimiento directo
            transform.Translate(targetVelocity * Time.deltaTime, Space.World);
        }
    }

    private void MoveCharacterWithRigidbody()
    {
        if (useSmoothMovement)
        {
            // Movimiento suave con Rigidbody, forzando Y a ser 0
            currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, smoothTime);
            rb.velocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);
        }
        else
        {
            // Movimiento directo con Rigidbody, forzando Y a ser 0
            rb.velocity = new Vector3(targetVelocity.x, 0f, targetVelocity.z);
        }
    }
}