using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Input System")]
    [SerializeField] InputActionReference moveAction;

    [Header("Refs")]
    [SerializeField] Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float acceleration = 60f;
    [SerializeField] float deceleration = 80f;

    Vector2 moveInput;
    Vector2 desiredVelocity;
    Vector2 velSmoothRef;

    void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void OnEnable()
    {
        if (moveAction != null)
        {
            moveAction.action.Enable();
        }
    }

    void OnDisable()
    {
        if (moveAction != null)
        {
            moveAction.action.Disable();
        }
    }

    void Update()
    {
        if (moveAction == null)
        {
            moveInput = Vector2.zero;
            return;
        }

        moveInput = moveAction.action.ReadValue<Vector2>();

        if (moveInput.sqrMagnitude > 1f)
        {
            moveInput = moveInput.normalized;
        }
    }

    void FixedUpdate()
    {
        desiredVelocity = moveInput * moveSpeed;

        float accel = (moveInput.sqrMagnitude > 0f) ? acceleration : deceleration;

        Vector2 newVel = Vector2.SmoothDamp(
            rb.linearVelocity,
            desiredVelocity,
            ref velSmoothRef,
            1f / Mathf.Max(accel, 0.0001f)
        );

        rb.linearVelocity = newVel;

    }
}
