using UnityEngine;
using System.Collections;
using System;

public class playerMovement : MonoBehaviour
{
   
   private Rigidbody2D body;
   private Animator anim;
   private BoxCollider2D boxCollider;
   private playerAttributes playerAttributes;
   [SerializeField] private LayerMask groundLayer;
   [SerializeField] private LayerMask wallLayer;
   [SerializeField] private float baseSpeed;
   [SerializeField] private float dashStaminaCost;
   [SerializeField] private int jumpSpeed;
   [SerializeField] private int ladderClimbSpeed;
   private float weaponSpeed;
   private bool canDoubleJump = true;
   private bool canDash = true;
   private float timeSinceLastJump;
   private float timeSinceLastDash;
   private bool isDashing;
   private bool isTeleporting;
   private bool isAttacking;
   private float jumpDuration;
   private float currentWep = 1;
   private float nextWep = 1;

   private bool dead = false;

   private bool onLadder = false;
   private bool isClimbingLadder = false;
   private float dashInputDirection;


   //Current Wep:
   //1 - Longsword (Default)
   //2 - Greatsword 
   //3 - Dagger
   private float currentJumpLength;
   [SerializeField] protected AnimatorOverrideController GSOverride;
   [SerializeField] protected AnimatorOverrideController LSOverride;
   [SerializeField] protected AnimatorOverrideController DGOverride;

   private Collider2D Ladder = null;

   private bool move = false;

   private float dashTime;
   [SerializeField] private int dashSpeed = 15;

   private bool jump = false;
   private bool fastFall = false;

   private int ladderInputDirection;
   private bool climbLadderBool;





   private void Awake(){
       //Grab references from Rigidbody and Animator
       body = GetComponent<Rigidbody2D>();
       body.freezeRotation = true;
       anim = GetComponent<Animator>();
       boxCollider = GetComponent<BoxCollider2D>();
       playerAttributes = GetComponent<playerAttributes>();
       weaponSpeed = baseSpeed;
       Application.targetFrameRate = 120;
   }

   private void Update(){
       //Local Variables
       timeSinceLastJump += Time.deltaTime;
       timeSinceLastDash += Time.deltaTime;
       currentJumpLength += Time.deltaTime;
       float horizontalInput = Input.GetAxis("Horizontal");


        dead = playerAttributes.getDead();
        if(dead){
            //print("char is dead");
        }
        getIsAttacking();

       if(isAttacking){
           body.velocity = new Vector2(0,0);
       }
        if(!onLadder || isGrounded()){
           isClimbingLadder = false;
       }

       if(canAttack() && !dead){
           if(Input.GetKeyDown(KeyCode.Alpha1)){
               nextWep = 1;
           }
           if(Input.GetKeyDown(KeyCode.Alpha2)){
                nextWep = 2;
           }
           if(Input.GetKeyDown(KeyCode.Alpha3)){
               nextWep = 3;
           }
       }

       if(nextWep != currentWep && !dead){
           weaponSwap();
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
    

        //Create regular jump
       if(Input.GetKeyDown(KeyCode.Space) && isGrounded() && !isAttacking && !dead && !isClimbingLadder && !isTeleporting){
           currentJumpLength = 0;
           StartCoroutine(Jump());
       }

       //Check for double jump reset and isClimbing
       if(isGrounded()){
           canDoubleJump = true;
           isClimbingLadder = false;
           body.bodyType = RigidbodyType2D.Dynamic;
       }
       if(isClimbingLadder){
           canDoubleJump = true;
           canDash = true;
       }

        //Create double jump
       if(!isGrounded() && !dead && !isClimbingLadder && !isTeleporting){
           if(Input.GetKeyDown(KeyCode.Space) && canDoubleJump){
               StartCoroutine(Jump());
               canDoubleJump = false;
           }
       }

       //Create dash 
       if(timeSinceLastDash >= .4 && isGrounded() && !dead){
           canDash = true;
       }

       if(Input.GetKeyDown(KeyCode.X) && canDash && !isAttacking && !dead && !isClimbingLadder && !isTeleporting){
           if(!playerAttributes.getNegativeStam()){
               playerAttributes.remStamina(dashStaminaCost);
                StartCoroutine(Dash());
           }
       }

       //FastFall
       if(!isGrounded() && !dead && !isClimbingLadder && !isTeleporting){
           if(Input.GetKeyDown(KeyCode.DownArrow)){
               fastFall = true;
           }
       }


       //Ladder Climb
       if(onLadder && !dead){
           if(Input.GetAxis("Vertical") != 0){
               if(!isClimbingLadder){
                   if(isGrounded() && Input.GetAxis("Vertical") > 0){
                        anim.SetTrigger("startClimb");
                   }else if(!isGrounded()){
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


        if(!dead && !isTeleporting){
            //Flips player sprite around when turning left or turning right
            if(horizontalInput > 0.01f && isDashing == false && !isAttacking){
                transform.localScale = new Vector2(1,1);
            }
            else if(horizontalInput < -0.01f && isDashing == false && !isAttacking){
                transform.localScale = new Vector2(-1, 1);
            }
        }

        if(dead){
            StopCoroutine(Jump());
            StopCoroutine(Dash());
        }
       //Animations
        anim.SetBool("walk", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded());
        anim.SetBool("onLadder", isClimbingLadder);
        anim.SetBool("isTeleporting", isTeleporting);
   }

    private void FixedUpdate() {
        if(move){
            body.velocity = new Vector2(Input.GetAxis("Horizontal") * weaponSpeed, body.velocity.y);
            move = false;
        }
        if(isDashing){
            body.velocity = new Vector2(dashInputDirection, 0).normalized * dashSpeed;
        }
        if(isTeleporting){
            body.velocity = new Vector2(0, body.velocity.y);
        }
        if(jump){
            body.velocity = new Vector2(body.velocity.x, jumpSpeed);
        }
        if(fastFall){
            const int fallSpeed = -15;
            body.velocity  = new Vector2(body.velocity.x, fallSpeed);
            fastFall = false;
        }
        if(climbLadderBool){
            //print(inputDirection);
            if(ladderInputDirection == 0){
                body.bodyType = RigidbodyType2D.Kinematic;
            }
            else{
                body.bodyType = RigidbodyType2D.Dynamic;
            }

            body.velocity = new Vector2(0, ladderInputDirection * ladderClimbSpeed);
            climbLadderBool = false;
        }
       
   }

   private IEnumerator Jump(){
       //int jumpSpeed = 8;
       float startTime = Time.time;
       float jumpTime = .25f;
       jump = true;
       anim.SetTrigger("jump");
        while(Time.time < startTime + jumpTime){
            yield return null;
        }
        jump = false;
       timeSinceLastJump = 0;
   }

   private void doubleJump(){
       //int jumpSpeed = 11;
       body.velocity = new Vector2(body.velocity.x, jumpSpeed);
        anim.SetTrigger("jump");
   }

   private IEnumerator Dash(){
       float startTime = Time.time;
       dashTime = .22f;
       float immunityTime = .12f;
       anim.SetTrigger("dash");
       canDash = false;
       dashInputDirection = Input.GetAxis("Horizontal");
       if(dashInputDirection == 0){
           if(transform.localScale.x == 1){
               dashInputDirection = 1;
           }else{
               dashInputDirection = -1;
           }
       }
       if(Mathf.Sign(dashInputDirection) == 1){
           transform.localScale = new Vector2(1,1);
       }
       else{
           transform.localScale = new Vector2(-1,1);
       }
       isDashing = true;
       print("ignoring collision true");
       Physics2D.IgnoreLayerCollision(9,10, true);
       while(Time.time < startTime + dashTime){
           if(Time.time >= startTime + immunityTime){
               print("ignoring collision false");
               Physics2D.IgnoreLayerCollision(9,10, false);
           }
           yield return null;
       }
        isDashing = false;
        timeSinceLastDash = 0;
   }

   public bool isGrounded(){
       //Commented out raycastHit: old method, gave trouble when swapping sprites, needed to be hardcoded in (hence 2).
       //RaycastHit2D raycastHit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, Vector2.down, 2, groundLayer);
       RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
       //print("isGrounded: " + (raycastHit.collider != null));
       return raycastHit.collider != null;
   }

   private bool onWall(){
       RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
       //print("onWall: " + (raycastHit.collider != null));
        return raycastHit.collider != null;
   }

   public void weaponSwap(){
       if(nextWep == 1){
           anim.runtimeAnimatorController = LSOverride;
           currentWep = nextWep;
           weaponSpeed = baseSpeed;
           //print("Weapon swapped to: Longsword");
           return;
       }
       if(nextWep == 2){
           anim.runtimeAnimatorController = GSOverride;
           currentWep = nextWep;
           weaponSpeed = baseSpeed - 2;
           //print("Weapon swapped to: Greatsword");
           return;
       }
       if(nextWep == 3){
           anim.runtimeAnimatorController = DGOverride;
           currentWep = nextWep;
           weaponSpeed = baseSpeed + 2;
           //print("Weapon swapped to: Dagger");
           return;
       }

       return;

   }


    public bool canAttack(){
        return !isDashing && isGrounded() && !isAttacking;
    }



    public float getWeapon(){
        return currentWep;
    }

    public void takeDamage(){
        playerAttributes.remHealth(20);
    }

    public bool getIsAttacking(){
        if((anim.GetCurrentAnimatorStateInfo(0).IsName("horizontalAttack") || 
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
            body.bodyType = RigidbodyType2D.Dynamic;
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
