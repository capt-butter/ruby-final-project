using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;

    public int maxHealth = 5;

    public int score = 0;

    public GameObject projectilePrefab;
    public GameObject hitfx;
    public GameObject healthfx;

    public AudioClip throwSound;
    public AudioClip hitSound;

    public int health { get { return currentHealth; } }
    int currentHealth;

    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);

    AudioSource audioSource;
    public AudioSource musicSource;

    public int ammo = 4;
    public Text ammotxt;
    public Text scoretxt;
    public GameObject losemsg;
    public GameObject nxtlvl;

    public AudioClip win;
    public AudioClip lose;
    public bool lost = false;
    public GameObject winmsg;
    public bool lvl2 = false;

    public int elec;
    public AudioClip fixedelec;
    public AudioClip dialog;
    bool generator = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;

        audioSource = GetComponent<AudioSource>();
        ammotxt.text = "ammo: 4";
        scoretxt.text = "Score: " + score.ToString();
        losemsg.SetActive(false);
        nxtlvl.SetActive(false);
        winmsg.SetActive(false);


    }

    // Update is called once per frame
    void Update()
    {
        //method first checks if timer is over limit
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Launch();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    //new thing for displaying new level txt
                    if (score == 4 && lvl2 == false)
                    {
                        audioSource.PlayOneShot(dialog);
                        character.Displaylvl2();
                        StartCoroutine(ExampleCoroutine());
                    }
                    else
                    {
                        audioSource.PlayOneShot(dialog);
                        character.DisplayDialog();
                    }
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (lost == true)
            {
                if (lvl2 == true)
                {
                    SceneManager.LoadScene("lvl2");
                }
                else
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);

    }

    public void ChangeHealth(int amount)
    {

        if (amount < 0)
        {
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;
            GameObject hit = Instantiate(hitfx, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
            animator.SetTrigger("Hit");
            PlaySound(hitSound);
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
        //gameover
        if (currentHealth == 0)
        {
            //changes audio
            musicSource.Stop();
            musicSource.clip = lose;
            musicSource.Play();
            //text display code
            losemsg.SetActive(true);
            lost = true;
            speed = 0.0f;

        }
        GameObject health = Instantiate(healthfx, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
    }

    void Launch()
    {
        if (ammo > 0)
        {
            GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

            Projectile projectile = projectileObject.GetComponent<Projectile>();
            projectile.Launch(lookDirection, 300);

            animator.SetTrigger("Launch");

            PlaySound(throwSound);
            ammo = ammo - 1;
            ammotxt.text = "ammo: " + ammo.ToString();
        }
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "ammo")
        {
            ammo = ammo + 4;
            ammotxt.text = "ammo: " + ammo.ToString();
            Destroy(collision.collider.gameObject);
        }
        if (collision.collider.tag == "health")
        {
            GameObject health = Instantiate(healthfx, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        }
        if (collision.collider.tag == "electricity")
        {
            elec++;
            Debug.Log("fixed a thing");
            audioSource.PlayOneShot(fixedelec);
            if (elec >= 2)
            {
                generator = true;
                if (score == 4)
                {
                    winmsg.SetActive(true);
                    lost = true;
                    musicSource.Stop();
                    musicSource.clip = win;
                    musicSource.Play();
                }
            }
        }

    }
    public void scoreupdate()
    {
        score = score + 1;
        scoretxt.text = "Score: " + score.ToString();
        if (score == 4)
        {
            if (lvl2 == false)
            {
                nxtlvl.SetActive(true);
                StartCoroutine(Coroutine2());
            }
            else
            {
                if (generator == true)
                {
                    winmsg.SetActive(true);
                    lost = true;
                    musicSource.Stop();
                    musicSource.clip = win;
                    musicSource.Play();
                }
            }
        }
    }


    IEnumerator ExampleCoroutine()
    {
        yield return new WaitForSecondsRealtime(2);
        SceneManager.LoadScene("lvl2");
    }
    IEnumerator Coroutine2()
    {
        yield return new WaitForSecondsRealtime(3);
        nxtlvl.SetActive(false);
    }
}