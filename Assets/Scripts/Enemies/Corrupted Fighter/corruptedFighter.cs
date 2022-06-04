using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class corruptedFighter : MonoBehaviour
{
    // Start is called before the first frame update

    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float distanceFromPlayer;
    private GameObject[] playerArray;
    private GameObject player;

    private bool moveTowards = false;

    private bool attacking  = false;
    [SerializeField] private float attackRange = 1;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float chargeSpeed;

    [SerializeField] private float attackCooldown = 3;

    [SerializeField] private float endLagNum = 1;

    private bool delayBool = false;

    private Vector2 playerXPos;

    private float step;

    private float cooldownTimer = 10000;

    private bool triggerActive = false;

    private bool follow = false;

    private bool reset = false;

    private Vector3 originalPosition;
    private float chargeDirection;


    private enemyAttributes enemyAttributes;


    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        body.freezeRotation = true;
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        enemyAttributes = GetComponent<enemyAttributes>();
        if(player == null){
            //print("getting player");
            playerArray = GameObject.FindGameObjectsWithTag("Player");
            player = playerArray[0];
            print(player);
        }

    }

    void Awake(){
        originalPosition = transform.position;
    }

    void OnEnable(){
        transform.position = originalPosition;
        reset = false;
        follow = false;
        cooldownTimer = 0;
        delayBool = false;
        triggerActive = false;
        attacking = false;
        moveTowards = false;
        distanceFromPlayer = 0;
    }

    void onDisable(){
    }

    // Update is called once per frame
    void Update()
    {
        cooldownTimer += Time.deltaTime;
        //Check if player is within "agro range"
        if(triggerActive){
            follow = true;
        }
        if(follow){
            //Check if player is within range for attack
            distanceFromPlayer = Vector2.Distance(transform.position, player.transform.position);
            //Check if player is out of range to reset agro.
            if(distanceFromPlayer > 20){
                follow = false;
                //print("resetting");
                enemyAttributes.resetHealth();
            }
            //print(distanceFromPlayer + " distanceFromPlayer");
            if(!delayBool && !enemyAttributes.getTakenDamage()){
                moveTowards = true;
                if(cooldownTimer > attackCooldown && distanceFromPlayer < attackRange){
                    if(!attacking){
                        if(transform.position.x < player.transform.position.x){
                            chargeDirection = 1;
                            transform.localScale = new Vector2(1,1);
                        }
                        else{
                            chargeDirection = -1;
                            transform.localScale = new Vector2(-1, 1);
                        }
                        StartCoroutine(charge());
                    }
                }
            }
        }
        else{
            //print("else");
            if(transform.position != originalPosition){
                //print("moving home");
                //print(transform.position + " current Pos");
                //print(originalPosition + " home Pos");
                reset = true;
            }
        }

        anim.SetBool("walking", moveTowards);
        anim.SetBool("attack", attacking);
        
    }

    void FixedUpdate(){
        step = Time.deltaTime * moveSpeed;
        playerXPos = new Vector2(player.transform.position.x, transform.position.y);
        if(moveTowards){
            //print("moving towards");
            if(transform.position.x < player.transform.position.x){
                transform.localScale = new Vector2(1,1);
            }
            else{
                transform.localScale = new Vector2(-1, 1);
            }
            Vector2 move = Vector2.MoveTowards(transform.position, playerXPos, step);
            transform.position = move;
            print(step + " step corrupted Fighter");
            moveTowards = false;
        }
        if(reset){
            //print("resetting fixedUpdate");
            if(transform.position.x < originalPosition.x){
                transform.localScale = new Vector2(1,1);
            }
            else{
                transform.localScale = new Vector2(-1, 1);
            }
            Vector2 move = Vector2.MoveTowards(transform.position, originalPosition, step);
            transform.position = move;
            reset = false;
        }
        if(attacking){
            //print("charging");
            body.velocity = new Vector2(chargeDirection, 0).normalized * chargeSpeed;
        }else{
            body.velocity = new Vector2(0,0);
        }
    }


    public void setAgroEnter() {
        triggerActive = true;
        if(!follow){
            cooldownTimer = 0;
        }
        
    }

    public void setAgroExit(){
        triggerActive = false;
    }

    private IEnumerator charge(){
       float startTime = Time.time;
       float chargeTime = 1f;
       attacking = true;
       while(Time.time < startTime + chargeTime){
           yield return null;
       }
       delayBool = true;
       attacking = false;
       yield return StartCoroutine(endLag());
       delayBool = false;
       cooldownTimer = 0;
    }

    private IEnumerator endLag(){
        float startTime = Time.time;
        while(Time.time < startTime + endLagNum){
            yield return null;
        }
    }

    private IEnumerator hitstun(){
        float startTime = Time.time;
        float cooldownTime = .2f;
        while(Time.time < startTime + cooldownTime){
            yield return null;
        }
        enemyAttributes.setTakenDamage(false);
    }


}


