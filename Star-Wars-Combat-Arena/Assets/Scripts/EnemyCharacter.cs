using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using TMPro;

public class EnemyCharacter : MonoBehaviour
{
    public float lookRadius = 10f;

    private Transform target;
    private NavMeshAgent agent;
    private Animator animator;


    public int damage;
    public int regenSpeed;
    private int health;
    private int stamina;
    public int attackPenalty;
    public int staminaLimit;
    public int healthLimit;
    private float lastRegen;

    private float blockingTime;
    private float timeToStop;

    private AudioSource lightsaber;
    public AudioClip block;
    public AudioClip attack1;
    public AudioClip attack2;
    public AudioClip attack3;
    public AudioClip attack4;
    public AudioClip death;
    public AudioClip clash;
    public AudioClip breakBlock;
    public AudioClip bodyHit;

    private HealthBar healthBar;
    private StaminaBar staminaBar;

    private GameObject player;
    private TMP_Text sceneHeader;


    private void OnCollisionEnter(Collision other) 
    {
        if (GetIsBlockingStatus() == false && GetDamagedStatus() == false && player.GetComponent<Character>().GetDamagedStatus() == false && GetDeadStatus() == false && 
            player.GetComponent<Character>().GetIsAttacking() == true && other.gameObject.tag == "Player"
            &&
            getIsPlayerFacingEnemyStatus() == true) 
        {
            ReceiveDamage();
        }
        else if (player.GetComponent<Character>().GetIsAttacking() == true && GetIsBlockingStatus() == true && GetIsBlockHitStatus() == false && 
            other.gameObject.tag == "Player" &&
            getIsPlayerFacingEnemyStatus() == true
        ) 
        {
            LoseStamina();
        }
    }

    public bool getIsPlayerFacingEnemyStatus()
    {
        Vector3 forward = player.transform.forward;
        Vector3 toOther = (transform.position - player.transform.position).normalized;

        return Vector3.Dot(forward, toOther) >= 0.8f ? true : false;
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        staminaBar = GameObject.FindWithTag("EnemyStaminaBar").GetComponent<StaminaBar>();
        healthBar = GameObject.FindWithTag("EnemyHealthBar").GetComponent<HealthBar>();
        stamina = staminaLimit;
        health = healthLimit;
        healthBar.SetMaxHealth(healthLimit);
        staminaBar.SetMaxStamina(staminaLimit);
        target = GameObject.FindWithTag("Player").transform;
        lightsaber = GameObject.FindWithTag("EnemyLightsaberSound").GetComponent<AudioSource>();
        player = GameObject.FindWithTag("Player");
        sceneHeader = GameObject.FindWithTag("SceneHeader").GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {

        if (PauseMenu.GameIsPaused == false) 
        {

             if (Time.time - lastRegen > 6f && stamina < staminaLimit) 
            {
                
                stamina += regenSpeed;
                lastRegen = Time.time;
                if (stamina > staminaLimit) 
                {
                    stamina = staminaLimit;
                }

                staminaBar.SetStamina(stamina);
            }

            if (agent.velocity.magnitude > 0 && animator.GetBool("isWalking") == false) 
            {

                animator.SetBool("isWalking", true);
            }
            else if (animator.GetBool("isWalking") == true && agent.velocity.magnitude <= 0)
            { 

                animator.SetBool("isWalking", false);
            }

            if (GetIsBlockingStatus() == true && Time.time >= timeToStop && GetIsBlockHitStatus() == false) 
            {
                lightsaber.Stop();
                lightsaber.loop = false;
                lightsaber.clip = block;
                lightsaber.Play();
                animator.SetBool("isBlocking", false);
            }

            float distance = Vector3.Distance(target.position, transform.position);

            if (distance <= lookRadius) 
            {
                if (player.GetComponent<Character>().GetDamagedStatus() == false && 
                player.GetComponent<Character>().GetDeadStatus() == false && GetDeadStatus() == false
                && GetDamagedStatus() == false && GetIsBlockingStatus() == false && GetIsBlockHitStatus() == false && distance > agent.stoppingDistance) 
                {
                    agent.SetDestination(target.position);
                }

                if (distance <= agent.stoppingDistance) 
                {
                    FaceTarget();


                    if (((float)player.GetComponent<Character>().GetCurrentStamina()
                    / ((float)player.GetComponent<Character>().staminaLimit / 100) <= 10 ||
                    (float)player.GetComponent<Character>().GetCurrentHealth()
                    / ((float)player.GetComponent<Character>().healthLimit / 100) <= 15 ||
                    (float)health / ((float)healthLimit / 100) <= 20) && getIsPlayerFacingEnemyStatus() == true)
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
                                    
                                
                                break;
                        }
                    }
                } 
            }
        }
    }

    public bool GetDamagedStatus() 
    {
        return animator.GetBool("isDamaged");
    }

    public bool GetDeadStatus() 
    {
        return animator.GetBool("isDead");
    }

    public bool GetIsBlockingStatus()
    {
        return animator.GetBool("isBlocking");
    }

    public bool GetIsBlockHitStatus()
    {
        return animator.GetBool("isBlockHit");
    }

    public bool GetIsAttacking() 
    {
        return animator.GetInteger("attackType") == 0 ? false : true;
    }

    public void DisableDamaged() 
    {
        animator.SetBool("isDamaged", false);
    }

    public void DisableBlockHit()
    {
        animator.SetBool("isBlockHit", false);
    }

    public void DisableAttacking () 
    {
        animator.SetInteger("attackType", 0);
    }

    private void ReceiveDamage () 
    {
      
 
            health -= player.GetComponent<Character>().damage;

        if (health <= 0) 
        {
            health = 0;
            healthBar.SetHealth(health);
            if (SceneManager.GetActiveScene().name == "ThirdLevel") 
            {
                sceneHeader.text = "Game over";
            }
            else 
            {
                sceneHeader.text = "You won";
            }
            lightsaber.Stop();
            lightsaber.loop = false;
            lightsaber.clip = death;
            lightsaber.Play();
            animator.SetBool("isDead", true);
        }
        else 
        {
            healthBar.SetHealth(health);
            lightsaber.Stop();
            lightsaber.loop = false;
            lightsaber.clip = bodyHit;
            lightsaber.Play();
            animator.SetBool("isDamaged", true);
        }
        
    }

    private void LoseStamina () 
    {
       

            stamina -= player.GetComponent<Character>().attackPenalty;
            if (stamina <= 0) 
            {
                stamina = 0;
                staminaBar.SetStamina(stamina);
                lightsaber.Stop();
                lightsaber.loop = false;
                lightsaber.clip = breakBlock;
                lightsaber.Play();
                animator.SetBool("isBlocking", false);
                animator.SetBool("isBlockHit", true);
            }
            else 
            {
                staminaBar.SetStamina(stamina);
                lightsaber.Stop();
                lightsaber.loop = false;
                lightsaber.clip = clash;
                lightsaber.Play();
                animator.SetBool("isBlockHit", true);
            }
        
    }

    private void FaceTarget() 
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3 (direction.x, 0, direction.z));

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void Block() 
    {
        if (GetIsBlockingStatus() == false && stamina > 0 && GetDamagedStatus() == false && GetDeadStatus() == false && GetIsAttacking() == false && GetIsBlockHitStatus() == false) 
        {
            


            blockingTime = (float) Random.Range(3, 6);
            timeToStop = Time.time + blockingTime;
            lightsaber.Stop();
            lightsaber.loop = false;
            lightsaber.clip = block;
            lightsaber.Play();       
            animator.SetBool("isBlocking", true);
        }
    }

    private void Attack () 
    {
        if (GetIsAttacking() == false && stamina >= attackPenalty && player.GetComponent<Character>().GetDamagedStatus() == false &&
        player.GetComponent<Character>().GetDeadStatus() == false &&
        GetDamagedStatus() == false &&
        GetDeadStatus() == false &&
        GetIsBlockingStatus() == false &&
            getIsPlayerFacingEnemyStatus() == true) 
        {


            int chosenAnimation = Random.Range(0, 4);
            switch (chosenAnimation) 
            {
                case 0:
                    lightsaber.Stop();
                    lightsaber.loop = false;
                    lightsaber.clip = attack3;
                    lightsaber.Play();
                    animator.SetInteger("attackType", 3);
                    break;
                case 1:
                    lightsaber.Stop();
                    lightsaber.loop = false;
                    lightsaber.clip = attack1;
                    lightsaber.Play();
                    animator.SetInteger("attackType", 1);
                    break;
                case 2:
                    lightsaber.Stop();
                    lightsaber.loop = false;
                    lightsaber.clip = attack4;
                    lightsaber.Play();
                    animator.SetInteger("attackType", 4);
                    break;
                case 3:
                    lightsaber.Stop();
                    lightsaber.loop = false;
                    lightsaber.clip = attack2;
                    lightsaber.Play();
                    animator.SetInteger("attackType", 2);
                    break;
            }
            
            stamina -= attackPenalty;
            if (stamina < 0) 
            {
                stamina = 0;
            }
            staminaBar.SetStamina(stamina);
        }
    }
}
