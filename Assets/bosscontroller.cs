using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bosscontroller : MonoBehaviour
{
    public float speed;
    public bool vertical;
    public float changeTime = 3.0f;

    Rigidbody2D rigidbody2D;
    float timer;
    int direction = 1;
    bool broken = true;

    Animator animator;
    public ParticleSystem smokeEffect;
    private RubyController rubyController;

    //bot stuff for final
    public int health = 4;
    public AudioSource botaudio;
    public AudioClip splosion;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        timer = changeTime;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        //remember ! inverse the test, so if broken is true !broken will be false and return won’t be executed.
        if (!broken)
        {
            return;
        }

        //health stuff for bot, it gets slower if the bot has been hit
        if (health == 0)
        {
            Destroy(gameObject);
        }
        if (health == 1)
        {
            speed = 2;
        }
        if (health == 2)
        {
            speed = 3;
        }
        if (health == 3)
        {
            speed = 4;
        }


        timer -= Time.deltaTime;

        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }
    }

    void FixedUpdate()
    {
        //remember ! inverse the test, so if broken is true !broken will be false and return won’t be executed.
        if (!broken)
        {
            return;
        }


        Vector2 position = rigidbody2D.position;

        if (vertical)
        {
            position.y = position.y + Time.deltaTime * speed * direction;
            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", direction);
        }
        else
        {
            position.x = position.x + Time.deltaTime * speed * direction;
            animator.SetFloat("Move X", direction);
            animator.SetFloat("Move Y", 0);
        }

        rigidbody2D.MovePosition(position);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //checks if hit by bullet  
        if (collision.collider.tag == "bullet")
        {
            //Debug.Log ("enemy hit");
            health--;
            botaudio.PlayOneShot(splosion);
        }

        RubyController player = collision.gameObject.GetComponent<RubyController>();

        if (player != null)
        {
            player.ChangeHealth(-2);
        }
    }
}
