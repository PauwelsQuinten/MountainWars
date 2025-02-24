using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;
    private float speed = 8f;
    private float jumpingPower = 16f;
    private bool isFacingRight = true;
    private EventInstance _groundInstance;
    private int _surfaceType = 0;
    private bool isGrounded = false;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask groundLayer;

    private void Start()
    {
        _surfaceType = FMODAudioManager.instance.SurfaceType;
        _groundInstance = RuntimeManager.CreateInstance(FMODAudioManager.instance.Ground);
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && GetIsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
            _groundInstance.setParameterByName("SurfaceType", _surfaceType, false);
            _groundInstance.start();
        }

        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }

        Flip();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
    }

    private bool GetIsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, LayerMask.GetMask("Grass", "Sand", "Water"));
        if (hit.collider != null)
        {
            string layerName = LayerMask.LayerToName(hit.collider.gameObject.layer);
            if (layerName == "Grass")
            {
                _surfaceType = 0;
            }
            else if (layerName == "Sand")
            {
                _surfaceType = 1;
            }
            else if (layerName == "Water")
            {
                _surfaceType = 2;
            }
            Debug.Log("Standing on: " + layerName);
            return true;
        }
        return false;
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
}