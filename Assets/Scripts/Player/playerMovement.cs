using UnityEngine;
using System.Collections;
using System;

public class playerMovement : MonoBehaviour
{
   
   private Rigidbody2D body;
   private Animator anim;
   private BoxCollider2D boxCollider;
   private playerAttributes playerAttributes;
   [SerializeField] private float baseSpeed;
   [SerializeField] private float dashStaminaCost;
   [SerializeField] private int ladderClimbSpeed;
   private bool canDash = true;
   private float timeSinceLastDash;
   private bool isDashing;
   private bool isTeleporting;
   private bool isAttacking;

   private bool dead = false;

   private bool onLadder = false;
   private bool isClimbingLadder = false;
   private float dashInputDirectionX;
   private float dashInputDirectionY;

   private int walkDirection;

   private Collider2D Ladder = null;

   private bool move = false;

   private float dashTime;
   [SerializeField] private int dashSpeed = 15;

   private int ladderInputDirection;
   private bool climbLadderBool;
   private float horizontalInput;
   private float verticalInput;





   private void Awake(){
       //Grab references from Rigidbody and Animator
       body = GetComponent<Rigidbody2D>();
       body.freezeRotation = true;
       anim = GetComponent<Animator>();
       boxCollider = GetComponent<BoxCollider2D>();
       playerAttributes = GetComponent<playerAttributes>();
       Application.targetFrameRate = 120;
       //Physics2D.IgnoreLayerCollision(9,10,true);
   }

   private void Update(){
       //Local Variables
       timeSinceLastDash += Time.deltaTime;
       horizontalInput = Input.GetAxisRaw("Horizontal");
       verticalInput = Input.GetAxisRaw("Vertical");


        dead = playerAttributes.getDead();
        if(dead){
            //print("char is dead");
        }
        getIsAttacking();

       if(isAttacking){
           body.velocity = new Vector2(0,0);
       }
        if(!onLadder){
           isClimbingLadder = false;
       }


        //Check to see if you're attacking
    
        if(!isDashing && !isAttacking && !dead && !isClimbingLadder && !isTeleporting){
            move = true;
        }
        else if(dead){
            body.velocity = new Vector2(0,0);
        }
        else{
            //print(isDashing + " is dashing");
            //print(isAttacking + " is attacking");
            //print(body.velocity);
            body.velocity = new Vector2(body.velocity.x, body.velocity.y);
        }
       

        if(Input.GetKeyDown(KeyCode.G) && !dead){
            takeDamage();
        }
    

       //Create dash 
       if(timeSinceLastDash >= .4 && !dead){
           canDash = true;
       }

       if(Input.GetKeyDown(KeyCode.X) && canDash && !isAttacking && !dead && !isClimbingLadder && !isTeleporting){
           if(!playerAttributes.getNegativeStam()){
               playerAttributes.remStamina(dashStaminaCost);
                StartCoroutine(Dash());
           }
       }


       //Ladder Climb
       if(onLadder && !dead){
           if(Input.GetAxis("Vertical") != 0){
               if(!isClimbingLadder){
                   if(Input.GetAxis("Vertical") > 0){
                        anim.SetTrigger("startClimb");
                   }else{
                        anim.SetTrigger("startClimb");
                   }
               }
               if(body.position.x != Ladder.gameObject.transform.position.x){
                   Vector2 ladderCenter = new Vector2(Ladder.gameObject.transform.position.x, body.position.y);
                    body.MovePosition(ladderCenter);
                    //print("moving positions");
               }
               ladderClimb();
           }
       }


        

        //Animations
        //Handle Walk Directional Animations
        //Walk Direction:
        //1 = Up
        //-1 = Down
        //0 = Side

        if(horizontalInput == 0 && verticalInput > 0){
            walkDirection = 1;
            if(!anim.GetCurrentAnimatorStateInfo(0).IsName("PlayerSideAttack")){
                transform.localScale = new Vector2(1,1);
            }
            //print("changing to UP VIEW");
        }else if(horizontalInput == 0 && verticalInput < 0){
            walkDirection = -1;
            if(!anim.GetCurrentAnimatorStateInfo(0).IsName("PlayerSideAttack")){
                transform.localScale = new Vector2(1,1);
            }
            //print("changing to DOWN VIEW");
        }else if(horizontalInput != 0 && verticalInput == 0){
            walkDirection = 0;
            if(!dead && !isTeleporting){
                //Flips player sprite around when turning left or turning right
                if(horizontalInput > 0.01f && !isDashing && !isAttacking){
                    transform.localScale = new Vector2(1,1);
                }
                else if(horizontalInput < -0.01f && !isDashing && !isAttacking){
                    transform.localScale = new Vector2(-1, 1);
                }
            }
            //print("CHANGING TO SIDE VIEW");
        }

        //Movement


        anim.SetInteger("WalkDirection", walkDirection);
        anim.SetBool("Walking", (horizontalInput != 0 || verticalInput != 0));
        //anim.SetBool("onLadder", isClimbingLadder);
        //anim.SetBool("isTeleporting", isTeleporting);
   }

    private void FixedUpdate() {
       if(move){
            Vector2 moveVect = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            moveVect = Vector3.ClampMagnitude(moveVect, 1f) * baseSpeed * Time.deltaTime;
            //print(moveVect + "moveVect");
            body.MovePosition(body.position + moveVect);
            move = false;
        }
        if(isDashing){
            Vector2 moveVect = new Vector3(dashInputDirectionX, dashInputDirectionY);
            moveVect = Vector3.ClampMagnitude(moveVect, 1f) * dashSpeed * Time.deltaTime;
            //print(moveVect + "moveVect");
            body.MovePosition(body.position + moveVect);            
        }
        if(isTeleporting){
            body.velocity = new Vector2(0, body.velocity.y);
        }
        if(climbLadderBool){
            //print(inputDirection);
            body.velocity = new Vector2(0, ladderInputDirection * ladderClimbSpeed);
            climbLadderBool = false;
        }
   }

    public float getVerticalInput(){
        return verticalInput;
    }

    public float getHorizontalInput(){
        return horizontalInput;
    }
   private IEnumerator Dash(){
       float startTime = Time.time;
       dashTime = .22f;
       float immunityTime = .12f;
       anim.SetTrigger("dash");
       canDash = false;
       dashInputDirectionX = Input.GetAxisRaw("Horizontal");
       dashInputDirectionY = Input.GetAxisRaw("Vertical");
       if(dashInputDirectionX == 0 && dashInputDirectionY == 0){
           if(walkDirection == 0){
                if(transform.localScale.x == 1){
                    dashInputDirectionX = 1;
                }else{
                    dashInputDirectionX = -1;
                }
           }
           else if(walkDirection == -1){
               dashInputDirectionY = -1;
           }else if(walkDirection == 1){
               dashInputDirectionY = 1;
           }
       }
       isDashing = true;
       //print("ignoring collision true");
       Physics2D.IgnoreLayerCollision(9,10, true);
       while(Time.time < startTime + dashTime){
           if(Time.time >= startTime + immunityTime){
               //print("ignoring collision false");
               Physics2D.IgnoreLayerCollision(9,10, false);
           }
           if(Physics2D.IsTouchingLayers(boxCollider, 10)){
               print("touching layers");
               Physics2D.IgnoreLayerCollision(9,10,true);
           }
           else{
               print("not touching");
           }
           yield return null;
       }
        isDashing = false;
        timeSinceLastDash = 0;
   }


    public bool canAttack(){
        return !isDashing && !isAttacking;
    }


    public void takeDamage(){
        playerAttributes.remHealth(20);
    }

    public bool getIsAttacking(){
        if((anim.GetCurrentAnimatorStateInfo(0).IsName("PlayerSideAttack") || 
        anim.GetCurrentAnimatorStateInfo(0).IsName("PlayerUpAttack") ||
        anim.GetCurrentAnimatorStateInfo(0).IsName("PlayerDownAttack") ||
        anim.GetCurrentAnimatorStateInfo(0).IsName("Special") ||
        anim.GetCurrentAnimatorStateInfo(0).IsName("Drink")) && !anim.IsInTransition(0)){
           isAttacking = true;
           //print("is Attacking");
       }else{
           isAttacking = false;
       }
       return isAttacking;
    }

    public Animator getAnim(){
        return anim;
    }

    public Rigidbody2D getBody(){
        return body;
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if(collision.gameObject.tag == "Ladder"){
            onLadder = true;
            Ladder = collision;
            //print(onLadder);
            //print(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision){
        if(collision.gameObject.tag == "Ladder"){
            onLadder = false;
            //print(onLadder);
            Ladder = null;
            //print(Ladder);
        }
    }

    private void ladderClimb(){
        isClimbingLadder = true;

        climbLadderBool = true;
        ladderInputDirection = 0;
        if(Input.GetKey(KeyCode.UpArrow)){
            ladderInputDirection = 1;
            //print(inputDirection);
        }
        if(Input.GetKey(KeyCode.DownArrow)){
            ladderInputDirection = -1;
            //print(inputDirection);
        }


        anim.SetInteger("ladderDirection", ladderInputDirection);
    }

    public void setisTeleporting(bool TP){
        isTeleporting = TP;
    }

    public bool getisClimbingLadder(){
        return isClimbingLadder;
    }
    



}
