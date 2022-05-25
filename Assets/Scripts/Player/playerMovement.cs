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
   private bool isAttacking;
   private float jumpDuration;
   private float currentWep = 1;
   private float nextWep = 1;

   private bool dead = false;

   private bool onLadder = false;
   private bool isClimbingLadder = false;


   //Current Wep:
   //1 - Longsword (Default)
   //2 - Greatsword 
   //3 - Dagger
   private float currentJumpLength;
   [SerializeField] protected AnimatorOverrideController GSOverride;
   [SerializeField] protected AnimatorOverrideController LSOverride;
   [SerializeField] protected AnimatorOverrideController DGOverride;

   private Collider2D Ladder = null;




   private void Awake(){
       //Grab references from Rigidbody and Animator
       body = GetComponent<Rigidbody2D>();
       body.freezeRotation = true;
       anim = GetComponent<Animator>();
       boxCollider = GetComponent<BoxCollider2D>();
       playerAttributes = GetComponent<playerAttributes>();
       weaponSpeed = baseSpeed;
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
    
        if(!isDashing && !isAttacking && !dead && !isClimbingLadder){

            body.velocity = new Vector2(Input.GetAxis("Horizontal") * weaponSpeed, body.velocity.y);
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
       if(Input.GetKeyDown(KeyCode.Space) && isGrounded() && !isAttacking && !dead && !isClimbingLadder){
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
       if(!onWall() && !isGrounded() && !dead && !isClimbingLadder){
           if(Input.GetKeyDown(KeyCode.Space) && canDoubleJump){
               StartCoroutine(Jump());
               canDoubleJump = false;
           }
       }

       //Create dash 
       if(timeSinceLastDash >= .4 && isGrounded() && !dead){
           canDash = true;
       }

       if(Input.GetKeyDown(KeyCode.X) && canDash && !isAttacking && !dead && !isClimbingLadder){
           if(!playerAttributes.getNegativeStam()){
               playerAttributes.remStamina(dashStaminaCost);
                StartCoroutine(Dash());
           }
       }

       //FastFall
       if(!isGrounded() && !dead && !isClimbingLadder){
           if(Input.GetKeyDown(KeyCode.DownArrow)){
               fastFall();
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


        if(!dead){
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
        //anim.SetBool("dash", isDashing);

      


   }

   private IEnumerator Jump(){
       //int jumpSpeed = 8;
       float startTime = Time.time;
       float jumpTime = .25f;
       anim.SetTrigger("jump");
        while(Time.time < startTime + jumpTime){
            body.velocity = new Vector2(body.velocity.x, jumpSpeed);
            yield return null;
        }
       timeSinceLastJump = 0;
   }

   private void doubleJump(){
       //int jumpSpeed = 11;
       body.velocity = new Vector2(body.velocity.x, jumpSpeed);
        anim.SetTrigger("jump");
   }

   private void fastFall(){
       //print("fastFalling");
       const int fallSpeed = -15;
       body.velocity  = new Vector2(body.velocity.x, fallSpeed);
   }




   private IEnumerator Dash(){
       const int dashSpeed = 30;
       float startTime = Time.time;
       float dashTime = .22f;
       anim.SetTrigger("dash");
       canDash = false;
       float inputDirection = Input.GetAxis("Horizontal");
       if(inputDirection == 0){
           if(transform.localScale.x == 1){
               inputDirection = 1;
           }else{
               inputDirection = -1;
           }
       }
       isDashing = true;
       if(Mathf.Sign(inputDirection) == 1){
           transform.localScale = new Vector2(1,1);
       }
       else{
           transform.localScale = new Vector2(-1,1);
       }
       body.velocity = new Vector2(inputDirection, 0).normalized * dashSpeed;
       while(Time.time < startTime + dashTime){
           body.velocity = new Vector2(body.velocity.x, 0);
           yield return null;
       }

        body.velocity = new Vector2(inputDirection * weaponSpeed, 0);
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
        return !isDashing && !onWall() && isGrounded() && !isAttacking;
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
        int inputDirection = 0;
        if(Input.GetKey(KeyCode.UpArrow)){
            inputDirection = 1;
            //print(inputDirection);
        }
        if(Input.GetKey(KeyCode.DownArrow)){
            inputDirection = -1;
            //print(inputDirection);
        }

        //print(inputDirection);
        if(inputDirection == 0){
            body.bodyType = RigidbodyType2D.Kinematic;
        }
        else{
            body.bodyType = RigidbodyType2D.Dynamic;
        }

        body.velocity = new Vector2(0, inputDirection * ladderClimbSpeed);

        anim.SetInteger("ladderDirection", inputDirection);
    }
    



}
