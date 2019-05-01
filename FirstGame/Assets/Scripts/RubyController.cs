using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;
    
    public int maxHealth = 5;
    public float timeInvincible = 2.0f;
    
    int currentHealth;
    public int health { get { return currentHealth; }}
    
    bool isInvincible;
    float invincibleTimer;
    
    Rigidbody2D rigidbody2d;
    Animator animator;
    public ParticleSystem healingEffect;
    public ParticleSystem hitEffect;
    public ParticleSystem InvincibilityEffect;

    public AudioClip throwCog;
    public AudioClip hitSound;

    Vector2 lookDirection = new Vector2(1,0);

    public GameObject projectilePrefab;
    
    // Start is called before the first frame update
    AudioSource audioSource;

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        audioSource= GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);
                
        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }
                
        animator.SetFloat("Move X", lookDirection.x);
        animator.SetFloat("Move Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);
        
        Vector2 position = rigidbody2d.position;
                
        position = position + move * speed * Time.deltaTime;
                
        rigidbody2d.MovePosition(position);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }
        else{
            InvincibilityEffect.Stop();
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            Launch();
            this.PlaySound(throwCog);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayableCharacter character = hit.collider.GetComponent<NonPlayableCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                }  
            }
        }
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;
            
            isInvincible = true;
            invincibleTimer = timeInvincible;
            hitEffect.Play();
            InvincibilityEffect.Play();
            this.PlaySound(hitSound);
        }
        else{
            healingEffect.Play();
        }
        
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");
    }
}