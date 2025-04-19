using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    [Header("Refer�ncia")]
    private Animator animator;

    [Header("Configura��o")]
    public float speedSmoothTime = 0.1f; // Tempo para suavizar a velocidade

    private float currentSpeed;
    private float targetSpeed;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // Suaviza a transi��o de speed todo frame
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime / speedSmoothTime);
        animator.SetFloat("Speed", currentSpeed);
    }

    /// <summary>
    /// Recebe o valor de velocidade normalizado (0 a 1)
    /// </summary>
    public void UpdateAnimation(float speedNormalized)
    {
        targetSpeed = speedNormalized;
    }

    public void TriggerJump()
    {
        animator.SetTrigger("Jump");
    }
}


// Versao antiga
//using UnityEngine;

//public class PlayerAnimatorController : MonoBehaviour
//{
//    [Header("Refer�ncia")]
//    private Animator animator;

//    [Header("Configura��o")]
//    public float speedSmoothTime = 0.1f; // Suaviza��o da transi��o entre estados

//    private float currentSpeed;

//    void Awake()
//    {
//        animator = GetComponentInChildren<Animator>();
//    }

//    // Chamado pelo PlayerMovement
//    public void UpdateAnimation(float speedNormalized)
//    {
//        // Atualiza a vari�vel "Speed" do Animator com suaviza��o
//        currentSpeed = Mathf.Lerp(currentSpeed, speedNormalized, Time.deltaTime / speedSmoothTime);
//        animator.SetFloat("Speed", currentSpeed);
//    }

//    public void TriggerJump()
//    {
//        animator.SetTrigger("Jump");
//    }
//}
