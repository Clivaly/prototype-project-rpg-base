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
        // Centraliza o pivot no player no início
        pivot.position = target.position;

        // Garante que todas as rotações comecem zeradas (evita tilt ou torção indevida)
        pivot.rotation = Quaternion.identity;
        holder.localRotation = Quaternion.identity;

        // Define rotação vertical inicial (opcional)
        holder.localEulerAngles = new Vector3(30f, 0, 0); // visão "por cima"

        // Posição inicial da câmera
        cam.localPosition = new Vector3(0, 0, -distance);

        currentDistance = distance;

        targetPivotRotation = pivot.rotation;
        targetHolderRotation = holder.localRotation;

    }

    void LateUpdate()
    {
        // Atualiza o Pivot para sempre seguir o player
        pivot.position = Vector3.Lerp(pivot.position, target.position, Time.deltaTime * 10f);

        // Se o botão direito do mouse estiver pressionado, rotaciona a câmera
        if (Mouse.current.rightButton.isPressed)
        {
            // Rotação horizontal do pivot (em torno do Y)
            float yaw = lookInput.x * rotationSpeed * Time.deltaTime;
            targetPivotRotation *= Quaternion.Euler(0, yaw, 0);

            // Rotação vertical do holder (em torno do X)
            float pitch = -lookInput.y * rotationSpeed * Time.deltaTime;
            Quaternion verticalRotation = Quaternion.Euler(pitch, 0, 0);
            targetHolderRotation *= verticalRotation;

            // Clamp da rotação vertical para não passar dos limites
            Vector3 holderEuler = targetHolderRotation.eulerAngles;
            holderEuler.x = (holderEuler.x > 180) ? holderEuler.x - 360 : holderEuler.x;
            holderEuler.x = Mathf.Clamp(holderEuler.x, verticalMin, verticalMax);
            holderEuler.y = 0;
            holderEuler.z = 0;
            targetHolderRotation = Quaternion.Euler(holderEuler);
        }

        // Trava rotação pra vertical se tiver teto
        //if (Mouse.current.rightButton.isPressed)
        //{
        //    // Rotação horizontal do pivot (em torno do Y)
        //    float yaw = lookInput.x * rotationSpeed * Time.deltaTime;
        //    targetPivotRotation *= Quaternion.Euler(0, yaw, 0);

        //    // Simula rotação vertical
        //    float pitch = -lookInput.y * rotationSpeed * Time.deltaTime;
        //    Quaternion verticalRotation = Quaternion.Euler(pitch, 0, 0);
        //    Quaternion simulatedHolderRotation = targetHolderRotation * verticalRotation;

        //    // Ajusta ângulo e clampa
        //    Vector3 simEuler = simulatedHolderRotation.eulerAngles;
        //    simEuler.x = (simEuler.x > 180) ? simEuler.x - 360 : simEuler.x;
        //    simEuler.x = Mathf.Clamp(simEuler.x, verticalMin, verticalMax);
        //    simulatedHolderRotation = Quaternion.Euler(simEuler);

        //    // Simula posição da câmera com nova rotação
        //    Vector3 simCamLocalPos = new Vector3(0, Mathf.Lerp(2f, cameraHeight, currentDistance / maxZoom), -currentDistance);
        //    Vector3 simulatedWorldPos = holder.parent.TransformPoint(simulatedHolderRotation * simCamLocalPos);

        //    // Raycast: se não bate em nada, permite a rotação
        //    if (!Physics.Linecast(pivot.position, simulatedWorldPos, collisionMask))
        //    {
        //        targetHolderRotation = simulatedHolderRotation;
        //    }
        //}

        // Suaviza a rotação do pivot e do holder
        pivot.rotation = Quaternion.Slerp(pivot.rotation, targetPivotRotation, Time.deltaTime * 10f);
        holder.localRotation = Quaternion.Slerp(holder.localRotation, targetHolderRotation, Time.deltaTime * 10f);

        // Aplica o input de zoom (scroll do mouse)
        distance -= zoomInput * zoomSpeed;
        distance = Mathf.Clamp(distance, minZoom, maxZoom);

        // Suaviza a transição entre o valor atual e o desejado
        currentDistance = Mathf.Lerp(currentDistance, distance, Time.deltaTime * 3f);

        // Calcula uma altura proporcional à distância (evita clipe no chão)
        float adjustedHeight = Mathf.Lerp(2f, cameraHeight, currentDistance / maxZoom);

        // Posição desejada da câmera, relativa ao holder
        Vector3 desiredCamLocalPos = new Vector3(0, adjustedHeight, -currentDistance);

        // Converte essa posição para o mundo
        Vector3 desiredWorldPos = cam.parent.TransformPoint(desiredCamLocalPos);

        // Posição de origem do Raycast (pivot ou player)
        Vector3 origin = pivot.position;

        // Raycast para detectar colisão entre o pivot e a posição desejada da câmera
        RaycastHit hit;
        if (Physics.Linecast(origin, desiredWorldPos, out hit, collisionMask))
        {
            // Se houver colisão, calcula a posição segura da câmera
            Debug.DrawLine(origin, hit.point, Color.red); // Linha vermelha pra colisão

            Vector3 hitPoint = hit.point;
            Vector3 direction = (origin - hitPoint).normalized;
            float safeDistance = 0.3f;

            // Posição final segura com leve recuo
            Vector3 safePosition = hitPoint + direction * safeDistance;

            // Suaviza a movimentação até a posição segura
            cam.position = Vector3.Lerp(cam.position, safePosition, Time.deltaTime * 3f);
        }
        else
        {
            Debug.DrawLine(origin, desiredWorldPos, Color.green); // Linha verde quando livre

            // Suaviza o retorno à posição normal da câmera
            Vector3 targetWorldPosition = cam.parent.TransformPoint(desiredCamLocalPos);
            cam.position = Vector3.Lerp(cam.position, targetWorldPosition, Time.deltaTime * 3f);
        }

        //// Rotação vertical do holder (em torno do X)
        //float pitch = -lookInput.y * rotationSpeed * Time.deltaTime;
        //Quaternion verticalRotation = Quaternion.Euler(pitch, 0, 0);
        //Quaternion testHolderRotation = targetHolderRotation * verticalRotation;

        //// Simula nova rotação e calcula a posição da câmera
        //Vector3 testEuler = testHolderRotation.eulerAngles;
        //testEuler.x = (testEuler.x > 180) ? testEuler.x - 360 : testEuler.x;
        //testEuler.x = Mathf.Clamp(testEuler.x, verticalMin, verticalMax);
        //testHolderRotation = Quaternion.Euler(testEuler);

        //// Simula a nova posição da câmera com essa rotação
        //Vector3 testCamLocalPos = new Vector3(0, Mathf.Lerp(2f, cameraHeight, currentDistance / maxZoom), -currentDistance);
        //Vector3 testWorldPos = holder.parent.TransformPoint(testHolderRotation * testCamLocalPos);

        //// Raycast para testar se essa posição ficaria obstruída
        //if (!Physics.Linecast(pivot.position, testWorldPos, collisionMask))
        //{
        //    // Se não há obstrução, aplica a rotação normalmente
        //    targetHolderRotation = testHolderRotation;
        //}
        //// senão: não aplica a rotação vertical

    }
}

