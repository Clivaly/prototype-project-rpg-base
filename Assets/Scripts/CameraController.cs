using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform target;     // Player
    public Transform pivot;  // CameraPivot (segue o Player, gira horizontalmente)
    public Transform holder; // CameraHolder (rotaciona verticalmente)
    public Transform cam;        // Main Camera
    public LayerMask collisionMask; // Define quais camadas o Raycast vai considerar


    public float distance = 10f;
    public float rotationSpeed = 10f;
    public float zoomSpeed = 0.3f;
    public float minZoom = 5f;
    public float maxZoom = 15f;

    public float verticalMin = -5f;
    public float verticalMax = 30f;
    public float cameraHeight = 4f;

    private Vector2 lookInput;
    private float zoomInput;
    private float currentDistance;
    private Quaternion targetPivotRotation;
    private Quaternion targetHolderRotation;


    private InputActions inputActions;

    void Awake()
    {
        inputActions = new InputActions();
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled += _ => lookInput = Vector2.zero;

        inputActions.Player.Zoom.performed += ctx => zoomInput = ctx.ReadValue<float>();
        inputActions.Player.Zoom.canceled += _ => zoomInput = 0f;
    }

    void OnDisable()
    {
        inputActions.Player.Disable();
    }

    void Start()
    {
        // Centraliza o pivot no player no in�cio
        pivot.position = target.position;

        // Garante que todas as rota��es comecem zeradas (evita tilt ou tor��o indevida)
        pivot.rotation = Quaternion.identity;
        holder.localRotation = Quaternion.identity;

        // Define rota��o vertical inicial (opcional)
        holder.localEulerAngles = new Vector3(30f, 0, 0); // vis�o "por cima"

        // Posi��o inicial da c�mera
        cam.localPosition = new Vector3(0, 0, -distance);

        currentDistance = distance;

        targetPivotRotation = pivot.rotation;
        targetHolderRotation = holder.localRotation;

    }

    void LateUpdate()
    {
        // Atualiza o Pivot para sempre seguir o player
        pivot.position = Vector3.Lerp(pivot.position, target.position, Time.deltaTime * 10f);

        // Se o bot�o direito do mouse estiver pressionado, rotaciona a c�mera
        if (Mouse.current.rightButton.isPressed)
        {
            // Rota��o horizontal do pivot (em torno do Y)
            float yaw = lookInput.x * rotationSpeed * Time.deltaTime;
            targetPivotRotation *= Quaternion.Euler(0, yaw, 0);

            // Rota��o vertical do holder (em torno do X)
            float pitch = -lookInput.y * rotationSpeed * Time.deltaTime;
            Quaternion verticalRotation = Quaternion.Euler(pitch, 0, 0);
            targetHolderRotation *= verticalRotation;

            // Clamp da rota��o vertical para n�o passar dos limites
            Vector3 holderEuler = targetHolderRotation.eulerAngles;
            holderEuler.x = (holderEuler.x > 180) ? holderEuler.x - 360 : holderEuler.x;
            holderEuler.x = Mathf.Clamp(holderEuler.x, verticalMin, verticalMax);
            holderEuler.y = 0;
            holderEuler.z = 0;
            targetHolderRotation = Quaternion.Euler(holderEuler);
        }

        // Trava rota��o pra vertical se tiver teto
        //if (Mouse.current.rightButton.isPressed)
        //{
        //    // Rota��o horizontal do pivot (em torno do Y)
        //    float yaw = lookInput.x * rotationSpeed * Time.deltaTime;
        //    targetPivotRotation *= Quaternion.Euler(0, yaw, 0);

        //    // Simula rota��o vertical
        //    float pitch = -lookInput.y * rotationSpeed * Time.deltaTime;
        //    Quaternion verticalRotation = Quaternion.Euler(pitch, 0, 0);
        //    Quaternion simulatedHolderRotation = targetHolderRotation * verticalRotation;

        //    // Ajusta �ngulo e clampa
        //    Vector3 simEuler = simulatedHolderRotation.eulerAngles;
        //    simEuler.x = (simEuler.x > 180) ? simEuler.x - 360 : simEuler.x;
        //    simEuler.x = Mathf.Clamp(simEuler.x, verticalMin, verticalMax);
        //    simulatedHolderRotation = Quaternion.Euler(simEuler);

        //    // Simula posi��o da c�mera com nova rota��o
        //    Vector3 simCamLocalPos = new Vector3(0, Mathf.Lerp(2f, cameraHeight, currentDistance / maxZoom), -currentDistance);
        //    Vector3 simulatedWorldPos = holder.parent.TransformPoint(simulatedHolderRotation * simCamLocalPos);

        //    // Raycast: se n�o bate em nada, permite a rota��o
        //    if (!Physics.Linecast(pivot.position, simulatedWorldPos, collisionMask))
        //    {
        //        targetHolderRotation = simulatedHolderRotation;
        //    }
        //}

        // Suaviza a rota��o do pivot e do holder
        pivot.rotation = Quaternion.Slerp(pivot.rotation, targetPivotRotation, Time.deltaTime * 10f);
        holder.localRotation = Quaternion.Slerp(holder.localRotation, targetHolderRotation, Time.deltaTime * 10f);

        // Aplica o input de zoom (scroll do mouse)
        distance -= zoomInput * zoomSpeed;
        distance = Mathf.Clamp(distance, minZoom, maxZoom);

        // Suaviza a transi��o entre o valor atual e o desejado
        currentDistance = Mathf.Lerp(currentDistance, distance, Time.deltaTime * 3f);

        // Calcula uma altura proporcional � dist�ncia (evita clipe no ch�o)
        float adjustedHeight = Mathf.Lerp(2f, cameraHeight, currentDistance / maxZoom);

        // Posi��o desejada da c�mera, relativa ao holder
        Vector3 desiredCamLocalPos = new Vector3(0, adjustedHeight, -currentDistance);

        // Converte essa posi��o para o mundo
        Vector3 desiredWorldPos = cam.parent.TransformPoint(desiredCamLocalPos);

        // Posi��o de origem do Raycast (pivot ou player)
        Vector3 origin = pivot.position;

        // Raycast para detectar colis�o entre o pivot e a posi��o desejada da c�mera
        RaycastHit hit;
        if (Physics.Linecast(origin, desiredWorldPos, out hit, collisionMask))
        {
            // Se houver colis�o, calcula a posi��o segura da c�mera
            Debug.DrawLine(origin, hit.point, Color.red); // Linha vermelha pra colis�o

            Vector3 hitPoint = hit.point;
            Vector3 direction = (origin - hitPoint).normalized;
            float safeDistance = 0.3f;

            // Posi��o final segura com leve recuo
            Vector3 safePosition = hitPoint + direction * safeDistance;

            // Suaviza a movimenta��o at� a posi��o segura
            cam.position = Vector3.Lerp(cam.position, safePosition, Time.deltaTime * 3f);
        }
        else
        {
            Debug.DrawLine(origin, desiredWorldPos, Color.green); // Linha verde quando livre

            // Suaviza o retorno � posi��o normal da c�mera
            Vector3 targetWorldPosition = cam.parent.TransformPoint(desiredCamLocalPos);
            cam.position = Vector3.Lerp(cam.position, targetWorldPosition, Time.deltaTime * 3f);
        }

        //// Rota��o vertical do holder (em torno do X)
        //float pitch = -lookInput.y * rotationSpeed * Time.deltaTime;
        //Quaternion verticalRotation = Quaternion.Euler(pitch, 0, 0);
        //Quaternion testHolderRotation = targetHolderRotation * verticalRotation;

        //// Simula nova rota��o e calcula a posi��o da c�mera
        //Vector3 testEuler = testHolderRotation.eulerAngles;
        //testEuler.x = (testEuler.x > 180) ? testEuler.x - 360 : testEuler.x;
        //testEuler.x = Mathf.Clamp(testEuler.x, verticalMin, verticalMax);
        //testHolderRotation = Quaternion.Euler(testEuler);

        //// Simula a nova posi��o da c�mera com essa rota��o
        //Vector3 testCamLocalPos = new Vector3(0, Mathf.Lerp(2f, cameraHeight, currentDistance / maxZoom), -currentDistance);
        //Vector3 testWorldPos = holder.parent.TransformPoint(testHolderRotation * testCamLocalPos);

        //// Raycast para testar se essa posi��o ficaria obstru�da
        //if (!Physics.Linecast(pivot.position, testWorldPos, collisionMask))
        //{
        //    // Se n�o h� obstru��o, aplica a rota��o normalmente
        //    targetHolderRotation = testHolderRotation;
        //}
        //// sen�o: n�o aplica a rota��o vertical

    }
}

