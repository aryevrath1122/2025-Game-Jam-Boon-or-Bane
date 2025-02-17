using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    // Reference to PlayerStats for shared health and stamina
    private bool isGrounded = true;
    private bool isDashing = false;

    public float moveSpeed;
    public float jumpForce;
    public float dashSpeed = 20f;
    public float superJumpMultiplier = 2f;
    private bool canUseDash = false;
    private bool canUseSuperJump = false;

    private bool isInvisible = false;
    private bool isLevitating = false;
    private GameObject levitatedObject;

    // Abilities cooldowns
    private float currentSheildCooldown = 0f;
    private float currentAttackCooldown = 0f;
    private float invisibilityCooldown = 0f;
    private float levitateCooldown = 0f;

    public PlayerInput playerInput;

    // References to the shield and bullet prefabs
    [SerializeField] private GameObject SheildPrefab;
    [SerializeField] private GameObject BulletPrefab;

    [SerializeField] private Material OriginalMaterial;
    [SerializeField] private Material InvisibleMaterial;

    // Ability flags
    private bool canUseSheild = false;
    private bool canUseAttack = false;
    private bool canUseInvisibility = false;
    private bool canUseLevitate = false;

    // Health/Stamina Slider references
    public Slider healthSlider;
    public Slider staminaSlider;

    void Awake()
    {
        // Set the sliders to the PlayerStats instance
        PlayerStats.Instance.healthSlider = healthSlider;
        PlayerStats.Instance.staminaSlider = staminaSlider;

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
        playerInput.actions["Move"].performed += ctx => Move();
        playerInput.actions["Jump"].performed += ctx => Jump();
        playerInput.actions["Dash"].performed += ctx => UseDashOrSuperJump();
        playerInput.actions["Combat"].performed += ctx => UseSheildOrAttack();
        playerInput.actions["Ability"].performed += ctx => UseSpecialAbility();
        playerInput.actions["HealthRegen"].performed += ctx => UseStaminaToRegenerateHealth();
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
        Vector3 moveDirection = new Vector3(playerInput.actions["Move"].ReadValue<Vector2>().x, 0f, playerInput.actions["Move"].ReadValue<Vector2>().y) * moveSpeed;
        GetComponent<Rigidbody>().velocity = new Vector3(moveDirection.x, GetComponent<Rigidbody>().velocity.y, moveDirection.z);
    }

    void Jump()
    {
        if (isGrounded)
        {
            GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, jumpForce, GetComponent<Rigidbody>().velocity.z);
            isGrounded = false;
        }
    }

    void UseDashOrSuperJump()
    {
        if (canUseDash && PlayerStats.Instance.stamina >= 40f)
        {
            StartCoroutine(Dash());
        }
        else if (canUseSuperJump && isGrounded && PlayerStats.Instance.stamina >= 40f)
        {
            GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, jumpForce * superJumpMultiplier, GetComponent<Rigidbody>().velocity.z);
            PlayerStats.Instance.ModifyStamina(-40f);  // Decrease shared stamina
            isGrounded = false;
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;
        PlayerStats.Instance.ModifyStamina(-40f);  // Decrease shared stamina

        float dashTime = 0.2f;
        float startTime = Time.time;
        Vector3 dashDirection = transform.forward * dashSpeed;

        while (Time.time < startTime + dashTime)
        {
            GetComponent<Rigidbody>().velocity = new Vector3(dashDirection.x, GetComponent<Rigidbody>().velocity.y, dashDirection.z);
            yield return null;
        }

        isDashing = false;
    }

    void UseSheildOrAttack()
    {
        if (canUseSheild && currentSheildCooldown <= 0f)
        {
            SpawnShield();
            currentSheildCooldown = 3f; // Shield cooldown time
        }
        else if (canUseAttack && currentAttackCooldown <= 0f)
        {
            ShootBullet();
            currentAttackCooldown = 3f; // Bullet attack cooldown time
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

        // Change material to invisible
        GetComponent<Renderer>().material = InvisibleMaterial;

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
        GetComponent<Renderer>().material = OriginalMaterial;
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
        if (PlayerStats.Instance.stamina < PlayerStats.Instance.maxStamina)
        {
            PlayerStats.Instance.ModifyStamina(10f * Time.deltaTime); // Regenerate stamina over time
        }
    }

    void UseStaminaToRegenerateHealth()
    {
        if (PlayerStats.Instance.stamina >= 20f)  // Use 20 stamina to regenerate 10 health
        {
            PlayerStats.Instance.ModifyHealth(10f);
            PlayerStats.Instance.ModifyStamina(-20f);
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
            PlayerStats.Instance.ModifyHealth(-20f);  // Shared health damage
            if (PlayerStats.Instance.health <= 0) Destroy(gameObject);  // Player death
        }
    }
}
