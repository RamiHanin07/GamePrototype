using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class giantRat : MonoBehaviour
{

    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float distanceFromPlayer;
    private GameObject[] playerArray;
    private GameObject player;

    private bool moveTowards = false;
    [SerializeField] private float attackRange = 1;

    [SerializeField] private float moveSpeed;

    [SerializeField] private float attackCooldown = 3;

    [SerializeField] private float endLagNum = 1;

    private bool delayBool = false;

    private Vector2 playerXPos;

    private float step;

    private float cooldownTimer = 10000;

    private bool triggerActive = false;

    private bool follow = false;

    private bool reset = false;
    private bool moveAway = false;

    private bool endLagBool = false;

    private Vector3 originalPosition;


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
            //print(player);
        }

    }

    void Awake(){
        originalPosition = transform.position;
        //print(originalPosition + " originalPosition");
    }

    void OnEnable(){
        transform.position = originalPosition;
        reset = false;
        follow = false;
        cooldownTimer = 0;
        delayBool = false;
        triggerActive = false;
        moveTowards = false;
        distanceFromPlayer = 0;
    }

    void onDisable(){
    }

    // Update is called once per frame
    void Update()
    {
        //print(transform.position + "transform.position in update");
        cooldownTimer += Time.deltaTime;
        //Check if player is within "agro range"
        if(triggerActive){
            follow = true;
            //print("following");
        }
        if(follow && !endLagBool){
            //Check if player is within range for attack
            distanceFromPlayer = Vector2.Distance(transform.position, player.transform.position);
            //Check if player is out of range to reset agro.
            if(distanceFromPlayer > 20){
                follow = false;
                enemyAttributes.resetHealth();
            }
            if(!delayBool && !enemyAttributes.getTakenDamage() && !getIsAttacking() && distanceFromPlayer >= attackRange){
                moveTowards = true;
                
            }
            if(!delayBool && !enemyAttributes.getTakenDamage() && !getIsAttacking() && distanceFromPlayer <= attackRange - .5){
                moveAway = true;
            }
            if(cooldownTimer > attackCooldown && distanceFromPlayer <= attackRange){
                int randCheck = Random.Range(1,3);
                if(randCheck == 2){
                    attack();
                }
                else{
                    print(randCheck);
                    print("failed");
                    cooldownTimer = Random.Range(0,2);
                }
            }
        }
        else{
            if(transform.position != originalPosition){
                reset = true;
            }
        }

        anim.SetBool("walking", moveTowards);
        
    }

    void FixedUpdate(){
        step = Time.deltaTime * moveSpeed;
        playerXPos = new Vector2(player.transform.position.x, transform.position.y);
        if(moveTowards){
            //print("moving towards");
            if(transform.position.x < player.transform.position.x){
                transform.localScale = new Vector2(-1,1);
            }
            else{
                transform.localScale = new Vector2(1, 1);
            }
            //print(step + " step giantRat");
            Vector2 move = Vector2.MoveTowards(transform.position, playerXPos, step);
            transform.position = move;
            //print(transform.position + " in move");
            moveTowards = false;
        }
        if(reset){
            //print("resetting fixedUpdate");
            if(transform.position.x < originalPosition.x){
                transform.localScale = new Vector2(-1,1);
            }
            else{
                transform.localScale = new Vector2(1, 1);
            }
            Vector2 move = Vector2.MoveTowards(transform.position, originalPosition, step);
            transform.position = move;
            reset = false;
        }
        if(moveAway){
            Vector2 move = Vector2.MoveTowards(transform.position, playerXPos, -step / 2);
            transform.position = move;
            if(transform.position.x < player.transform.position.x){
                transform.localScale = new Vector2(-1,1);
            }
            else{
                transform.localScale = new Vector2(1, 1);
            }
            moveAway = false;
        }
    }


    public void setAgroEnter() {
        triggerActive = true;
        //print("agro'd");
        if(!follow){
            cooldownTimer = 0;
        }
        
    }

    public void setAgroExit(){
        triggerActive = false;
    }

    private void attack(){
        if(transform.position.x < player.transform.position.x){
                transform.localScale = new Vector2(-1,1);
            }
        else{
            transform.localScale = new Vector2(1, 1);
        }
        anim.SetTrigger("attack");
        //StartCoroutine(endLag());
    }

    private IEnumerator endLag(){
        float startTime = Time.time;
        endLagBool = true;
        while(Time.time < startTime + endLagNum){
            yield return !getIsAttacking();
        }
        endLagBool = false;
        cooldownTimer = 0;
    }

    private IEnumerator hitstun(){
        float startTime = Time.time;
        float cooldownTime = .2f;
        while(Time.time < startTime + cooldownTime){
            yield return null;
        }
        enemyAttributes.setTakenDamage(false);
    }

    private bool getIsAttacking(){
        if(anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") && !anim.IsInTransition(0)){
            return true;
        }else{
            return false;
        }
    }

}
