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
    private bool canUseInvisibility = false;
    private bool canUseLevitate = false;

    private float currentSheildCooldown = 0f;
    private float currentAttackCooldown = 0f;
    private float invisibilityCooldown = 0f;
    private float levitateCooldown = 0f;

    private bool isInvisible = false;
    private bool isLevitating = false;
    private GameObject levitatedObject;

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
            canUseSuperJump = true;
            canUseAttack = true;
            canUseInvisibility = true;
        }
        else if (playerInput.playerIndex == 1) // Player 2
        {
            moveSpeed = 5f;
            jumpForce = 10f;
            canUseDash = true;
            canUseSheild = true;
            canUseLevitate = true;
        }

        // Subscribe to Input System events
        playerInput.actions["Move"].performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playerInput.actions["Move"].canceled += ctx => moveInput = Vector2.zero;
        playerInput.actions["Jump"].performed += ctx => Jump();
        playerInput.actions["Dash"].performed += ctx => UseDashOrSuperJump();
        playerInput.actions["Combat"].performed += ctx => UseSheildOrAttack();
        playerInput.actions["Ability"].performed += ctx => UseSpecialAbility();
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
        if (canUseDash && stamina >= 40f)
        {
            StartCoroutine(Dash());
        }
        else if (canUseSuperJump && isGrounded && stamina >= 40f)
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
    }

    void ShootBullet()
    {
        GameObject bullet = Instantiate(BulletPrefab, transform.position + transform.forward, transform.rotation);
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

    void UseSpecialAbility()
    {
        if (canUseInvisibility && invisibilityCooldown <= 0f)
        {
            StartCoroutine(ActivateInvisibility());
        }
        else if (canUseLevitate && levitateCooldown <= 0f)
        {
            StartCoroutine(ActivateLevitate());
        }
    }

    IEnumerator ActivateInvisibility()
    {
        isInvisible = true;
        invisibilityCooldown = 10f;

        // Make player visually invisible
        GetComponent<Renderer>().enabled = false;

        // Disable collision with "Passable Object" tagged objects
        Collider[] passableObjects = FindObjectsOfType<Collider>();
        foreach (var obj in passableObjects)
        {
            if (obj.CompareTag("Passable Object"))
            {
                Physics.IgnoreCollision(GetComponent<Collider>(), obj, true);
            }
        }

        yield return new WaitForSeconds(5f); // Fixed duration

        // Restore normal visibility
        GetComponent<Renderer>().enabled = true;
        isInvisible = false;

        // Re-enable collision with "Passable Object" tagged objects
        foreach (var obj in passableObjects)
        {
            if (obj.CompareTag("Passable Object"))
            {
                Physics.IgnoreCollision(GetComponent<Collider>(), obj, false);
            }
        }
    }

    IEnumerator ActivateLevitate()
    {
        isLevitating = true;
        levitateCooldown = 10f;

        Collider[] colliders = Physics.OverlapSphere(transform.position, 3f);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Levitatable Object"))
            {
                levitatedObject = collider.gameObject;
                levitatedObject.GetComponent<Rigidbody>().useGravity = false;
                break; // Lift only one object
            }
        }

        while (isLevitating && levitatedObject != null)
        {
            // Keep the object floating above the player
            levitatedObject.transform.position = Vector3.Lerp(
                levitatedObject.transform.position,
                transform.position + Vector3.up * 2f,
                Time.deltaTime * 5f // Smooth movement
            );

            if (playerInput.actions["Ability"].WasReleasedThisFrame())
            {
                DropLevitatedObject();
                break;
            }

            yield return null;
        }

        DropLevitatedObject();
    }

    void DropLevitatedObject()
    {
        if (levitatedObject != null)
        {
            levitatedObject.GetComponent<Rigidbody>().useGravity = true;
            levitatedObject = null;
        }
        isLevitating = false;
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
        if (currentSheildCooldown > 0f) currentSheildCooldown -= Time.deltaTime;
        if (currentAttackCooldown > 0f) currentAttackCooldown -= Time.deltaTime;
        if (invisibilityCooldown > 0f) invisibilityCooldown -= Time.deltaTime;
        if (levitateCooldown > 0f) levitateCooldown -= Time.deltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
