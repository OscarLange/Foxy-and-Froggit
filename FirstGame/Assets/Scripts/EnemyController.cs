using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 3.0f;
    public bool vertical = true;
    public float changeTime = 3.0f;

    public bool broken = true;

    Rigidbody2D rigidbody2D;
    Animator animator;
    public ParticleSystem smokeEffect;

    float timer;
    int direction = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        timer = changeTime;
    }

    // Update is called once per frame
    void Update()
    {

        if(!broken){
            return;
        }

        timer -= Time.deltaTime;

        if (timer < 0)
        {
            direction = -direction;
            if(direction > 0){
                if(vertical){
                    vertical = false;
                }
                else
                {
                    vertical = true;
                }
            }
            timer = changeTime;
        }
        
        
        Vector2 position = rigidbody2D.position;

        if (vertical)
        {
            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", direction);
            position.y = position.y + Time.deltaTime * speed * direction;
        }
        else
        {
            animator.SetFloat("Move X", direction);
            animator.SetFloat("Move Y", 0);
            position.x = position.x + Time.deltaTime * speed * direction;
        }
 
        rigidbody2D.MovePosition(position);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        RubyController player = other.gameObject.GetComponent<RubyController>();

        if (player != null)
        {
            player.ChangeHealth(-1);
        }
    }


    public void Fix()
    {
        broken = false;
        rigidbody2D.simulated = false;

        animator.SetTrigger("Fixed");

        smokeEffect.Stop();
    }

}

