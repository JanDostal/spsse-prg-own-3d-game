using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class Player : MonoBehaviour
{
    private Rigidbody body;
    private Transform mainCamera;
    private Animator animator;
    private GameObject enemy;
    private UI ui;
    private GameLevelData currentGameLevelData;

    public Lightsaber Lightsaber { get; private set; }

    [field: SerializeField]
    public AudioClip Block { get; private set; }

    [field: SerializeField]
    public AudioClip Attack1 { get; private set; }

    [field: SerializeField]
    public AudioClip Attack2 { get; private set; }

    [field: SerializeField]
    public AudioClip Attack3 { get; private set; }

    [field: SerializeField]
    public AudioClip Attack4 { get; private set; }

    [field: SerializeField]
    public AudioClip Death { get; private set; }

    [field: SerializeField]
    public AudioClip LightsaberClash { get; private set; }

    [field: SerializeField]
    public AudioClip BlockBreak { get; private set; }

    [field: SerializeField]
    public AudioClip BodyHit { get; private set; }


    public int Health { get; private set; }
    public int Stamina { get; private set; }

    private float timeOfLastStaminaRegen;
    private float turnSmoothVelocity;
    private Vector2 moveVector;

    public bool GetIsBodyHitStatus()
    {
        return animator.GetBool("isBodyHit");
    }

    public bool GetIsDeadStatus()
    {
        return animator.GetBool("isDead");
    }

    public bool GetIsBlockingStatus()
    {
        return animator.GetBool("isBlocking");
    }

    public bool GetIsLightsaberHitStatus()
    {
        return animator.GetBool("isLightsaberHit");
    }

    public bool GetIsAttackingStatus()
    {
        return animator.GetInteger("attackType") == 0 ? false : true;
    }

    public void DisableIsAttacking()
    {
        animator.SetInteger("attackType", 0);
    }

    public void DisableIsBodyHit()
    {
        animator.SetBool("isBodyHit", false);
    }

    public void DisableIsLightsaberHit()
    {
        animator.SetBool("isLightsaberHit", false);
    }

    public bool GetIsPlayerTurnedTowardsToEnemy()
    {
        Vector3 forward = transform.forward;
        Vector3 toEnemy = (enemy.transform.position - transform.position).normalized;

        return Vector3.Dot(forward, toEnemy) >= 0.8f ? true : false;
    }

    private void OnCollisionEnter(Collision other) 
    {
        if (GetIsBlockingStatus() == false && GetIsBodyHitStatus() == false && enemy.GetComponent<Enemy>().GetIsBodyHitStatus() == false &&
            GetIsDeadStatus() == false && enemy.GetComponent<Enemy>().GetIsDeadStatus() == false && 
            enemy.GetComponent<Enemy>().GetIsAttackingStatus() == true && other.gameObject.CompareTag("Enemy") == true) 
        {
            ReceiveDamage();
        }
        else if (GetIsBlockingStatus() == true && GetIsLightsaberHitStatus() == false && 
                 enemy.GetComponent<Enemy>().GetIsAttackingStatus() == true && other.gameObject.CompareTag("Enemy") == true) 
        {
            LoseStamina();
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        body = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        mainCamera = GameObject.FindWithTag("MainCamera").transform;
        enemy = GameObject.FindWithTag("Enemy");
        ui = GameObject.FindWithTag("UI").GetComponent<UI>();
        currentGameLevelData = GameLevelsManager.Instance.GetCurrentGameLevelData();

        InitializeLightsaber(currentGameLevelData);

        InitializeStaminaAndHealth(currentGameLevelData, ui);         
    }

    // Update is called once per frame
    private void Update() 
    {
        if (GameLevelsManager.Instance.IsLevelPaused == false) 
        {
            float durationAfterLastStaminaRegen = Time.time - timeOfLastStaminaRegen;

            if (durationAfterLastStaminaRegen > 6f && Stamina < currentGameLevelData.PlayerConfigurationData.StaminaMaxLimit) 
            {
                RegenStamina();
            }

            if (enemy.GetComponent<Enemy>().GetIsBodyHitStatus() == false && enemy.GetComponent<Enemy>().GetIsDeadStatus() == false &&
                GetIsBodyHitStatus() == false && GetIsDeadStatus() == false && 
                GetIsBlockingStatus() == false && GetIsLightsaberHitStatus() == false) 
            {
                Move();        
            }
        }
    }

    private void InitializeLightsaber(GameLevelData currentGameLevelData)
    {
        Lightsaber = GameObject.FindWithTag("PlayerLightsaber").GetComponent<Lightsaber>();
        Lightsaber.InitializeBladeColor(currentGameLevelData.PlayerConfigurationData);
    }

    private void InitializeStaminaAndHealth(GameLevelData currentGameLevelData, UI ui)
    {
        Stamina = currentGameLevelData.PlayerConfigurationData.StaminaMaxLimit;
        Health = currentGameLevelData.PlayerConfigurationData.HealthMaxLimit;

        ui.PlayerHealthBar.GetComponent<HealthBar>().SetMaxHealth(currentGameLevelData.PlayerConfigurationData.HealthMaxLimit);
        ui.PlayerStaminaBar.GetComponent<StaminaBar>().SetMaxStamina(currentGameLevelData.PlayerConfigurationData.StaminaMaxLimit);
    }

    private void Move()
    {
        float turnSmoothTime = 0.1f;

        float horizontal = moveVector.x;
        float vertical = moveVector.y;
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.4f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            body.velocity = new Vector3(moveDir.x, body.velocity.y, moveDir.z);
        }
    }

    private void RegenStamina() 
    {
        Stamina += currentGameLevelData.PlayerConfigurationData.StaminaRegenSpeed;

        timeOfLastStaminaRegen = Time.time;

        if (Stamina > currentGameLevelData.PlayerConfigurationData.StaminaMaxLimit)
        {
            Stamina = currentGameLevelData.PlayerConfigurationData.StaminaMaxLimit;
        }

        ui.PlayerStaminaBar.GetComponent<StaminaBar>().SetStamina(Stamina);
    }

    private void ReceiveDamage() 
    {
        Health -= currentGameLevelData.EnemyConfigurationData.LightsaberDamage;

        if (Health <= 0)
        {
            Health = 0;
            ui.PlayerHealthBar.GetComponent<HealthBar>().SetHealth(Health);

            Lightsaber.DoLightsaberSound(Death);
            animator.SetBool("isDead", true);
        }
        else
        {
            ui.PlayerHealthBar.GetComponent<HealthBar>().SetHealth(Health);

            Lightsaber.DoLightsaberSound(BodyHit);
            animator.SetBool("isBodyHit", true);
        }
    }

    private void LoseStamina() 
    {
        Stamina -= currentGameLevelData.EnemyConfigurationData.StaminaPenaltyForAttack;

        if (Stamina <= 0) 
        {
            Stamina = 0;
            ui.PlayerStaminaBar.GetComponent<StaminaBar>().SetStamina(Stamina);

            Lightsaber.DoLightsaberSound(BlockBreak);
            animator.SetBool("isBlocking", false);
            animator.SetBool("isLightsaberHit", true);
        }
        else 
        {
            ui.PlayerStaminaBar.GetComponent<StaminaBar>().SetStamina(Stamina);

            Lightsaber.DoLightsaberSound(LightsaberClash);
            animator.SetBool("isLightsaberHit", true);
        }

    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveVector = context.ReadValue<Vector2>();

        if (animator.GetBool("isWalking") == false && moveVector.magnitude > 0) 
        {
            animator.SetBool("isWalking", true);
        }
        else if (animator.GetBool("isWalking") == true && moveVector.magnitude <= 0)
        {
            animator.SetBool("isWalking", false);
        }
    }

    public void OnBlock(InputAction.CallbackContext context) 
    {
        if (context.performed == true && GetIsBlockingStatus() == false && Stamina > 0 && 
            GetIsBodyHitStatus() == false && GetIsDeadStatus() == false && 
            GetIsAttackingStatus() == false && GetIsLightsaberHitStatus() == false) 
        {
            Lightsaber.DoLightsaberSound(Block);
            animator.SetBool("isBlocking", true);
        }
    }

    public void OnReleaseBlock(InputAction.CallbackContext context) 
    {
        if (GetIsBlockingStatus() == true) 
        {
            animator.SetBool("isBlocking", false);

            if (GetIsLightsaberHitStatus() == false) 
            {
                Lightsaber.DoLightsaberSound(Block);
            }            
        }
    }

    public void OnAttack(InputAction.CallbackContext context) 
    {
        if (GetIsAttackingStatus() == false && Stamina >= currentGameLevelData.PlayerConfigurationData.StaminaPenaltyForAttack && 
            enemy.GetComponent<Enemy>().GetIsBodyHitStatus() == false && enemy.GetComponent<Enemy>().GetIsDeadStatus() == false && 
            GetIsBodyHitStatus() == false && GetIsDeadStatus() == false && 
            GetIsBlockingStatus() == false) 
        {
            int chosenAnimation = Random.Range(0, 4);

            switch (chosenAnimation) 
            {
                case 0:
                    Lightsaber.DoLightsaberSound(Attack3);
                    animator.SetInteger("attackType", 3);
                    break;
                case 1:
                    Lightsaber.DoLightsaberSound(Attack1);
                    animator.SetInteger("attackType", 1);
                    break;
                case 2:
                    Lightsaber.DoLightsaberSound(Attack4);
                    animator.SetInteger("attackType", 4);
                    break;
                case 3:
                    Lightsaber.DoLightsaberSound(Attack2);
                    animator.SetInteger("attackType", 2);
                    break;
            }
            
            Stamina -= currentGameLevelData.PlayerConfigurationData.StaminaPenaltyForAttack;

            if (Stamina < 0) 
            {
                Stamina = 0;
            }

            ui.PlayerStaminaBar.GetComponent<StaminaBar>().SetStamina(Stamina);
        }
    }
}
