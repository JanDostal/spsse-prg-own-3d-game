using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class Character : MonoBehaviour
{

    private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;
    private Vector2 moveVector;

    private float timeToAppear = 6f;
    private float timeWhenDisappear;

    private Rigidbody body;
    private Animator animator;

    public int damage;
    public int regenSpeed;
    public int attackPenalty;
    private int health;
    private int stamina;
    public int staminaLimit;
    public int healthLimit;
    private float lastRegen;


    private bool isAttacked = false;
    private bool isBlocked = false;
    private bool isAttacking = false;
    private bool isBlocking = false;

    private Transform cam;

    private AudioSource lightsaber;
    public AudioClip block;
    public AudioClip attack1;
    public AudioClip attack2;
    public AudioClip attack3;
    public AudioClip attack4;
    public AudioClip bodyHit;
    public AudioClip breakBlock;
    public AudioClip clash;
    public AudioClip death;

    private HealthBar healthBar;
    private StaminaBar staminaBar;
    private TMP_Text sceneHeader;
    private GameObject enemy;

    private void OnCollisionEnter(Collision other) 
    {
        if (isBlocking == false && isAttacked == false && isBlocked == false && enemy.GetComponent<EnemyCharacter>().GetAttackingStatus() == true && 
        other.gameObject.tag == "Enemy") 
        {
            ReceiveDamage();
        }
        else if (enemy.GetComponent<EnemyCharacter>().GetAttackingStatus() == true &&
        other.gameObject.tag == "Enemy" && isBlocking == true && isBlocked == false && isAttacked == false) 
        {
            LoseStamina();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        body = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        staminaBar = GameObject.FindWithTag("StaminaBar").GetComponent<StaminaBar>();
        healthBar = GameObject.FindWithTag("HealthBar").GetComponent<HealthBar>();
        stamina = staminaLimit;
        health = healthLimit;
        healthBar.SetMaxHealth(healthLimit);
        staminaBar.SetMaxStamina(staminaLimit);
        cam = GameObject.FindWithTag("MainCamera").transform;
        enemy = GameObject.FindWithTag("Enemy");
        lightsaber = GameObject.FindWithTag("LightsaberSound").GetComponent<AudioSource>();
        sceneHeader = GameObject.FindWithTag("SceneHeader").GetComponent<TMP_Text>();
        switch(SceneManager.GetActiveScene().name) 
        {
            case "FirstLevel":
                sceneHeader.text = "Round 1";
                break;
            case "SecondLevel":
                sceneHeader.text = "Round 2";
                break;
            case "ThirdLevel": 
                sceneHeader.text = "Round 3";
                break;
        }
        timeWhenDisappear = Time.time + timeToAppear;           
    }


    // Update is called once per frame
    void Update() 
    {
        
        if ((sceneHeader.text == "Round 1" || sceneHeader.text == "Round 2"
         || sceneHeader.text == "Round 3") && Time.time >= timeWhenDisappear)
        {
            sceneHeader.text = "";
        }

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

            if (enemy.GetComponent<EnemyCharacter>().GetDamagedStatus() == false &&
            enemy.GetComponent<EnemyCharacter>().GetDeadStatus() == false &&
            GetDeadStatus() == false && GetDamagedStatus() == false && isBlocking == false) 
            {
                Move();        
            }
        }
    }

    public bool GetAttackedStatus () 
    {
        return isAttacked;
    }

    public bool GetBlockedStatus () 
    {
        return isBlocked;
    }

    public bool GetAttackingStatus () 
    {
        return isAttacking;
    }

    public void SetToNoAttacked () 
    {
        isAttacked = false;
    }

    public void SetToNoBlocked () 
    {
        isBlocked = false;
    }
    
    public bool GetDamagedStatus() 
    {
        return animator.GetBool("isDamaged");
    }

    public bool GetDeadStatus() 
    {
        return animator.GetBool("isDead");
    }

    public void SetToNoAttacking () 
    {
        isAttacking = false;
    }

    public void SetToNoBlocking () 
    {
        isBlocking = false;
    }

    public void ResetAttackType () 
    {
        animator.SetInteger("attackType", 0);
    }

    public void DisableDamaged() 
    {
        animator.SetBool("isDamaged", false);
    }

    public int GetCurrentStamina() 
    {
        return stamina;
    }

    public int GetCurrentHealth() 
    {
        return health;
    }

    private void ReceiveDamage () 
    {
        if (isAttacked == false) 
        {
            isAttacked = true;
            health -= enemy.GetComponent<EnemyCharacter>().damage;
            if (health <= 0) 
            {
                health = 0;
                healthBar.SetHealth(health);
                sceneHeader.text = "You lost";
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
    }

    private void LoseStamina () 
    {
        if (isBlocked == false) 
        {
isBlocked = true;
        stamina -= enemy.GetComponent<EnemyCharacter>().attackPenalty;
        if (stamina <= 0) 
        {
            stamina = 0;
            staminaBar.SetStamina(stamina);
            lightsaber.Stop();
            lightsaber.loop = false;
            lightsaber.clip = breakBlock;
            lightsaber.Play();
            animator.SetBool("isBlocking", false);
        }
        else 
        {
            staminaBar.SetStamina(stamina);
            lightsaber.Stop();
            lightsaber.loop = false;
            lightsaber.clip = clash;
            lightsaber.Play();
            isBlocked = false;
        }
        }
    }

    public void OnMove (InputAction.CallbackContext context) 
    {
        moveVector = context.ReadValue<Vector2>();
        if (moveVector.magnitude > 0) 
        {
           animator.SetBool("isWalking", true);
        }
        else 
        {
            animator.SetBool("isWalking", false);
        }
    }

    public void OnBlock (InputAction.CallbackContext context) 
    {
        if (context.performed == true && stamina > 0 && GetDamagedStatus() == false && GetDeadStatus() == false) 
        {
            isBlocking = true;
            lightsaber.Stop();
            lightsaber.loop = false;
            lightsaber.clip = block;
            lightsaber.Play();
            animator.SetBool("isBlocking", true);
        }
        
    }

    public void OnReleaseBlock (InputAction.CallbackContext context) 
    {
        if (isBlocking == true) 
        {
            lightsaber.Stop();
            lightsaber.loop = false;
            lightsaber.clip = block;
            lightsaber.Play();
            animator.SetBool("isBlocking", false);
        }
    }

    public void OnAttack (InputAction.CallbackContext context) 
    {
        if (isAttacking == false && stamina >= attackPenalty && enemy.GetComponent<EnemyCharacter>().GetDamagedStatus() == false
        && enemy.GetComponent<EnemyCharacter>().GetDeadStatus() == false && GetDamagedStatus() == false &&
        GetDeadStatus() == false && enemy.GetComponent<EnemyCharacter>().GetAttackedStatus() == false && 
        enemy.GetComponent<EnemyCharacter>().GetBlockedStatus() == false) 
        {
            isAttacking = true;
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


    private void Move () 
    {
        float horizontal = moveVector.x;
        float vertical = moveVector.y;
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.4f) {

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime );
            transform.rotation = Quaternion.Euler(0f, angle, 0f);


            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            body.velocity = new Vector3(moveDir.x, body.velocity.y, moveDir.z);
        }

    }
}
