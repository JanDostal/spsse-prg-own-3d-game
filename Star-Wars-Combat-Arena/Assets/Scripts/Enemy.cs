using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using TMPro;

public class Enemy : MonoBehaviour
{
    private readonly float lookRadius = 800f;

    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private GameObject player;
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

    private int health;
    private int stamina;

    private float timeOfLastStaminaRegen;
    private float timeToStopBlocking;

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

    public void DisableIsBodyHit()
    {
        animator.SetBool("isBodyHit", false);
    }

    public void DisableIsLightsaberHit()
    {
        animator.SetBool("isLightsaberHit", false);
    }

    public void DisableIsAttacking()
    {
        animator.SetInteger("attackType", 0);
    }

    private void OnCollisionEnter(Collision other) 
    {
        if (GetIsBlockingStatus() == false && GetIsBodyHitStatus() == false && player.GetComponent<Player>().GetIsBodyHitStatus() == false &&
            GetIsDeadStatus() == false && player.GetComponent<Player>().GetIsDeadStatus() == false &&
            player.GetComponent<Player>().GetIsAttackingStatus() == true && player.GetComponent<Player>().GetIsPlayerTurnedTowardsToEnemy() == true &&
            other.gameObject.CompareTag("Player") == true) 
        {
            ReceiveDamage();
        }
        else if (GetIsBlockingStatus() == true && GetIsLightsaberHitStatus() == false &&
                 player.GetComponent<Player>().GetIsAttackingStatus() == true && player.GetComponent<Player>().GetIsPlayerTurnedTowardsToEnemy() == true &&
                 other.gameObject.CompareTag("Player") == true) 
        {
            LoseStamina();
        }
    }

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");
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

            if (durationAfterLastStaminaRegen > 6f && stamina < currentGameLevelData.EnemyConfigurationData.StaminaMaxLimit) 
            {
                RegenStamina();
            }

            SwitchBetweenWalkingIdleAnimation();

            if (GetIsBlockingStatus() == true && Time.time >= timeToStopBlocking && GetIsLightsaberHitStatus() == false) 
            {
                StopBlocking();
            }

            float distanceFromPlayer = Vector3.Distance(player.transform.position, transform.position);

            if (distanceFromPlayer <= lookRadius) 
            {
                if (player.GetComponent<Player>().GetIsBodyHitStatus() == false && player.GetComponent<Player>().GetIsDeadStatus() == false && 
                    GetIsBodyHitStatus() == false && GetIsDeadStatus() == false &&
                    GetIsBlockingStatus() == false && GetIsLightsaberHitStatus() == false && 
                    distanceFromPlayer > navMeshAgent.stoppingDistance) 
                {
                    navMeshAgent.SetDestination(player.transform.position);
                }

                if (distanceFromPlayer <= navMeshAgent.stoppingDistance) 
                {
                    TurnTowardsToPlayer();

                    float playerCurrentStaminaInPercents = player.GetComponent<Player>().Stamina / 
                        ((float) currentGameLevelData.PlayerConfigurationData.StaminaMaxLimit / 100);

                    float playerCurrentHealthInPercents = player.GetComponent<Player>().Health / 
                        ((float)currentGameLevelData.PlayerConfigurationData.HealthMaxLimit / 100);

                    float enemyCurrentHealthInPercents = health / ((float) currentGameLevelData.EnemyConfigurationData.HealthMaxLimit / 100);

                    if ((playerCurrentStaminaInPercents <= 10 || playerCurrentHealthInPercents <= 15 || 
                        enemyCurrentHealthInPercents <= 20) && player.GetComponent<Player>().GetIsPlayerTurnedTowardsToEnemy() == true)
                    {
                        Attack();
                    }
                    else
                    {
                        int chosenReaction = Random.Range(0, 2);
                        switch (chosenReaction)
                        {
                            case 0:
                                Attack();
                                break;
                            case 1:
                                StartBlocking();
                                break;
                        }
                    }
                } 
            }
        }
    }

    private void InitializeLightsaber(GameLevelData currentGameLevelData) 
    {
        Lightsaber = GameObject.FindWithTag("EnemyLightsaber").GetComponent<Lightsaber>();
        Lightsaber.InitializeBladeColor(currentGameLevelData.EnemyConfigurationData);
    }

    private void InitializeStaminaAndHealth(GameLevelData currentGameLevelData, UI ui)
    {
        stamina = currentGameLevelData.EnemyConfigurationData.StaminaMaxLimit;
        health = currentGameLevelData.EnemyConfigurationData.HealthMaxLimit;

        ui.EnemyHealthBar.GetComponent<HealthBar>().SetMaxHealth(currentGameLevelData.EnemyConfigurationData.HealthMaxLimit);
        ui.EnemyStaminaBar.GetComponent<StaminaBar>().SetMaxStamina(currentGameLevelData.EnemyConfigurationData.StaminaMaxLimit);
    }

    private void SwitchBetweenWalkingIdleAnimation() 
    {
        if (animator.GetBool("isWalking") == false && navMeshAgent.velocity.magnitude > 0)
        {
            animator.SetBool("isWalking", true);
        }
        else if (animator.GetBool("isWalking") == true && navMeshAgent.velocity.magnitude <= 0)
        {
            animator.SetBool("isWalking", false);
        }
    }

    private void StopBlocking() 
    {
        Lightsaber.DoLightsaberSound(Block);
        animator.SetBool("isBlocking", false);
    }

    private void TurnTowardsToPlayer()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void RegenStamina() 
    {
        stamina += currentGameLevelData.EnemyConfigurationData.StaminaRegenSpeed;

        timeOfLastStaminaRegen = Time.time;

        if (stamina > currentGameLevelData.EnemyConfigurationData.StaminaMaxLimit)
        {
            stamina = currentGameLevelData.EnemyConfigurationData.StaminaMaxLimit;
        }

        ui.EnemyStaminaBar.GetComponent<StaminaBar>().SetStamina(stamina);
    }

    private void ReceiveDamage() 
    {
        health -= currentGameLevelData.PlayerConfigurationData.LightsaberDamage;

        if (health <= 0)
        {
            health = 0;
            ui.EnemyHealthBar.GetComponent<HealthBar>().SetHealth(health);

            Lightsaber.DoLightsaberSound(Death);
            animator.SetBool("isDead", true);
        }
        else 
        {
            ui.EnemyHealthBar.GetComponent<HealthBar>().SetHealth(health);

            Lightsaber.DoLightsaberSound(BodyHit);
            animator.SetBool("isBodyHit", true);
        }
    }

    private void LoseStamina() 
    {
        stamina -= currentGameLevelData.PlayerConfigurationData.StaminaPenaltyForAttack;

        if (stamina <= 0)
        {
            stamina = 0;
            ui.EnemyStaminaBar.GetComponent<StaminaBar>().SetStamina(stamina);

            Lightsaber.DoLightsaberSound(BlockBreak);
            animator.SetBool("isBlocking", false);
            animator.SetBool("isLightsaberHit", true);
        }
        else
        {
            ui.EnemyStaminaBar.GetComponent<StaminaBar>().SetStamina(stamina);

            Lightsaber.DoLightsaberSound(LightsaberClash);
            animator.SetBool("isLightsaberHit", true);
        }
    }

    private void StartBlocking() 
    {
        if (GetIsBlockingStatus() == false && stamina > 0 && 
            GetIsBodyHitStatus() == false && GetIsDeadStatus() == false && 
            GetIsAttackingStatus() == false && GetIsLightsaberHitStatus() == false) 
        {
            float blockingDuration = Random.Range(3, 6);
            timeToStopBlocking = Time.time + blockingDuration;

            Lightsaber.DoLightsaberSound(Block);       
            animator.SetBool("isBlocking", true);
        }
    }

    private void Attack() 
    {
        if (GetIsAttackingStatus() == false && stamina >= currentGameLevelData.EnemyConfigurationData.StaminaPenaltyForAttack && 
            player.GetComponent<Player>().GetIsBodyHitStatus() == false && player.GetComponent<Player>().GetIsDeadStatus() == false &&
            GetIsBodyHitStatus() == false && GetIsDeadStatus() == false &&
            GetIsBlockingStatus() == false && player.GetComponent<Player>().GetIsPlayerTurnedTowardsToEnemy() == true) 
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
            
            stamina -= currentGameLevelData.EnemyConfigurationData.StaminaPenaltyForAttack;

            if (stamina < 0) 
            {
                stamina = 0;
            }

            ui.EnemyStaminaBar.GetComponent<StaminaBar>().SetStamina(stamina);
        }
    }
}
