using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private GameObject SheildPrefab;
    [SerializeField] private GameObject BulletPrefab;
    [SerializeField] public Transform Checkpoint;
    
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
    private bool isFucked = false;
    private GameObject levitatedObject;
    

    public PlayerInput playerInput;

    // Invisible material reference
    [SerializeField] private Material invisibleMaterial;
    private Material originalMaterial;

    // Health and health regeneration variables
    public float health = 100f;
    private float maxHealth = 100f;
    private float healthRegenRate = 2f;  // Passive health regeneration rate per second

    // Store the last movement direction
    private Vector3 lastMoveDirection = Vector3.forward;

    //Sliders For UI
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider staminaSlider;


    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        SheildPrefab.SetActive(false);

        if (playerInput == null)
            playerInput = GetComponent<PlayerInput>();

        // Store the original material of the player
        originalMaterial = GetComponent<Renderer>().material;

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
        playerInput.actions["HealthRegen"].performed += ctx => UseStaminaToRegenerateHealth();
    }
    void Start()
    {
        // Initialize sliders with max values
        if (healthSlider != null) healthSlider.maxValue = maxHealth;
        if (staminaSlider != null) staminaSlider.maxValue = maxStamina;
        UpdateUI();

    }
    void FixedUpdate()
    
    {
        if (!isDashing)
            Move();

        RegenerateStamina();
        UpdateCooldowns();


    }
    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (healthSlider != null) healthSlider.value = health;
        if (staminaSlider != null) staminaSlider.value = stamina;
    }
    void Move()
    {
        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y) * moveSpeed;
        rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);

        // Store the movement direction
        if (moveInput.magnitude > 0)
        {
            lastMoveDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        }
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

    IEnumerator Dash()
    {
        isDashing = true;
        stamina -= 40f;

        float dashTime = 0.2f;
        float startTime = Time.time;

        // Use the last move direction or forward direction if no movement
        Vector3 dashDirection = lastMoveDirection != Vector3.zero ? lastMoveDirection : transform.forward;
        dashDirection *= dashSpeed;

        while (Time.time < startTime + dashTime)
        {
            rb.velocity = new Vector3(dashDirection.x, rb.velocity.y, dashDirection.z);
            yield return null;
        }

        isDashing = false;
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
        SheildPrefab.SetActive(true);
    }

    void ShootBullet()
    {
        GameObject bullet = Instantiate(BulletPrefab, transform.position - transform.right, transform.rotation);
    }

    void UseSpecialAbility()
    {
        if (canUseInvisibility && invisibilityCooldown <= 0f)
        {
            StartCoroutine(ActivateInvisibility());
            Debug.Log("Invisibility On");
        }
        else if (canUseLevitate && levitateCooldown <= 0f)
        {
            StartCoroutine(ActivateLevitate());
            Debug.Log("Levitation On");
        }
    }

    IEnumerator ActivateInvisibility()
    {
        isInvisible = true;
        invisibilityCooldown = 10f;

        // Change material to invisible
        GetComponent<Renderer>().material = invisibleMaterial;

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

        // Restore original material
        GetComponent<Renderer>().material = originalMaterial;
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

        // Detect objects within a 3-unit sphere
        Collider[] colliders = Physics.OverlapSphere(transform.position, 3f);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Levitatable Object"))
            {
                levitatedObject = collider.gameObject;
                Rigidbody rb = levitatedObject.GetComponent<Rigidbody>();
                break; // Lift only the first found object
            }
        }

        while (isLevitating && levitatedObject != null)
        {
            // Smoothly move the object above the player
            levitatedObject.transform.position = Vector3.Lerp(
            levitatedObject.transform.position,
                transform.position + Vector3.up * 2f, // Keep object floating above player
                Time.deltaTime * 5f
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
            Rigidbody rb = levitatedObject.GetComponent<Rigidbody>();
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

    // Method to use stamina to regenerate health when pressing the left shoulder button
    void UseStaminaToRegenerateHealth()
    {
        if (stamina >= 20f)  // Example: Use 20 stamina to regenerate 10 health
        {
            health += 10f;
            if (health > maxHealth) health = maxHealth;

            stamina -= 20f;
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
        if (collision.gameObject.CompareTag("Enemy Bullet"))
        {
            health = health - 20;
            if (health <= 0) Destroy(gameObject);
        }
        if (collision.collider.CompareTag("Death Area"))
        {
            isFucked = true;
            this.gameObject.SetActive(false);
            transform.position = Checkpoint.position;
            if (isFucked == true)
            {
                Debug.Log("Running");
                this.gameObject.SetActive(true);
                isFucked = false;
            }
        }
    }
}
