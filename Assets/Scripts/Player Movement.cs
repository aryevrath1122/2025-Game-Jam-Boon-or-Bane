using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] GameObject SheildPrefab;
    [SerializeField] GameObject BulletPrefab;
    private Rigidbody rb;
    private Vector2 moveInput;
    private bool isGrounded = true;

    public float moveSpeed;
    public float jumpForce;
    public float dashSpeed = 20f;
    public float superJumpMultiplier = 2f;
    public float stamina = 100f;
    private float maxStamina = 100f;
    private float staminaRegenRate = 10f;
    private float sheildCooldown = 3f;
    private float attackCooldown = 3f;
    private bool isDashing = false;

    private bool canUseDash = false;
    private bool canUseSuperJump = false;
    private bool canUseSheild = false;
    private bool canUseAttack = false;

    private float currentSheildCooldown = 0f;
    private float currentAttackCooldown = 0f;

    public PlayerInput playerInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        SheildPrefab.SetActive(false);

        if (playerInput == null)
            playerInput = GetComponent<PlayerInput>();

        // Assign different stats based on player index
        if (playerInput.playerIndex == 0) // Player 1
        {
            moveSpeed = 10f;
            jumpForce = 5f;
            canUseSuperJump = true; // Player 1 gets Super Jump
            canUseAttack = true; // Player 1 Gets to attack
        }
        else if (playerInput.playerIndex == 1) // Player 2
        {
            moveSpeed = 5f;
            jumpForce = 10f;
            canUseDash = true; // Player 2 gets Dash
            canUseSheild = true; // Player 2 gets a shield
        }

        // Subscribe to Input System events
        playerInput.actions["Move"].performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playerInput.actions["Move"].canceled += ctx => moveInput = Vector2.zero;
        playerInput.actions["Jump"].performed += ctx => Jump();
        playerInput.actions["Dash"].performed += ctx => UseDashOrSuperJump();
        playerInput.actions["Combat"].performed += ctx => UseSheildOrAttack();
    }

    void FixedUpdate()
    {
        if (!isDashing)
            Move();

        RegenerateStamina();
        UpdateCooldowns();
    }

    void Move()
    {
        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y) * moveSpeed;
        rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);
    }

    void Jump()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            isGrounded = false;
        }
    }

    void UseDashOrSuperJump()
    {
        if (canUseDash && stamina >= 40f) // Dash Mechanic for Player 2
        {
            StartCoroutine(Dash());
        }
        else if (canUseSuperJump && isGrounded && stamina >= 40f) // Super Jump for Player 1
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce * superJumpMultiplier, rb.velocity.z);
            stamina -= 40f;
            isGrounded = false;
        }
    }

    void UseSheildOrAttack()
    {
        if (canUseSheild && currentSheildCooldown <= 0f)
        {
            SpawnShield();
            currentSheildCooldown = sheildCooldown;
        }
        else if (canUseAttack && currentAttackCooldown <= 0f)
        {
            ShootBullet();
            currentAttackCooldown = attackCooldown;
        }
    }

    void SpawnShield()
    {
        GameObject shield = Instantiate(SheildPrefab, transform.position + transform.forward * 2f, transform.rotation);
        shield.SetActive(true);
        Shield shieldScript = shield.GetComponent<Shield>();
        if (shieldScript != null)
        {
            shieldScript.SetHealth(20);
        }
    }

    void ShootBullet()
    {
        GameObject bullet = Instantiate(BulletPrefab, transform.position + transform.forward, transform.rotation);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetDamage(10);
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;
        stamina -= 40f;

        float dashTime = 0.2f;
        float startTime = Time.time;
        Vector3 dashDirection = transform.forward * dashSpeed;

        while (Time.time < startTime + dashTime)
        {
            rb.velocity = new Vector3(dashDirection.x, rb.velocity.y, dashDirection.z);
            yield return null;
        }

        isDashing = false;
    }

    void RegenerateStamina()
    {
        if (stamina < maxStamina)
        {
            stamina += staminaRegenRate * Time.deltaTime;
            if (stamina > maxStamina)
                stamina = maxStamina;
        }
    }

    void UpdateCooldowns()
    {
        if (currentSheildCooldown > 0f)
        {
            currentSheildCooldown -= Time.deltaTime;
        }

        if (currentAttackCooldown > 0f)
        {
            currentAttackCooldown -= Time.deltaTime;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
