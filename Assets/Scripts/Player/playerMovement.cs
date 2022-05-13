using UnityEngine;
using System.Collections;

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
   //Current Wep:
   //1 - Longsword (Default)
   //2 - Greatsword 
   //3 - Dagger

   private float currentJumpLength;

   [SerializeField] protected AnimatorOverrideController GSOverride;
   [SerializeField] protected AnimatorOverrideController LSOverride;

   [SerializeField] protected AnimatorOverrideController DGOverride;




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


        getIsAttacking();

       if(isAttacking){
           body.velocity = new Vector2(0,0);
       }

       if(canAttack()){
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

       if(nextWep != currentWep){
           weaponSwap();
       }

        

        //Check to see if you're attacking
    
        if(!isDashing && !isAttacking ){

            body.velocity = new Vector2(Input.GetAxis("Horizontal") * weaponSpeed, body.velocity.y);
        }
        else{
            //print(isDashing + " is dashing");
            //print(isAttacking + " is attacking");
            //print(body.velocity);
            body.velocity = new Vector2(body.velocity.x, body.velocity.y);
        }
       

        if(Input.GetKeyDown(KeyCode.G)){
            takeDamage();
        }
    

        //Create regular jump
       if(Input.GetKeyDown(KeyCode.Space) && isGrounded() && !isAttacking){
           currentJumpLength = 0;

           Jump();
       }

       //Check for double jump reset
       if(isGrounded()){
           canDoubleJump = true;
       }
        //Create double jump
       if(!onWall() && !isGrounded() && timeSinceLastJump >= .25f){
           if(Input.GetKeyDown(KeyCode.Space) && canDoubleJump){
               doubleJump();
               canDoubleJump = false;
           }
       }

       //Create dash 
       if(timeSinceLastDash >= .4 && isGrounded()){
           canDash = true;
       }

       if(Input.GetKeyDown(KeyCode.X) && canDash && !isAttacking){
           if(!playerAttributes.getNegativeStam()){
               playerAttributes.remStamina(dashStaminaCost);
                StartCoroutine(Dash());
           }
       }

       //FastFall
       if(isGrounded() == false){
           if(Input.GetKeyDown(KeyCode.DownArrow)){
               fastFall();
           }
       }


        //Flips player sprite around when turning left or turning right
       if(horizontalInput > 0.01f && isDashing == false && !isAttacking){
           transform.localScale = new Vector2(1,1);
       }
       else if(horizontalInput < -0.01f && isDashing == false && !isAttacking){
           transform.localScale = new Vector2(-1, 1);
       }



       //Animations
        anim.SetBool("walk", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded());
        //anim.SetBool("dash", isDashing);

      


   }

   private void Jump(){
       const int jumpSpeed = 11;
       body.velocity = new Vector2(body.velocity.x, jumpSpeed);
        anim.SetTrigger("jump");
       timeSinceLastJump = 0;
   }

   private void doubleJump(){
       const int jumpSpeed = 11;
       body.velocity = new Vector2(body.velocity.x, jumpSpeed);
        anim.SetTrigger("jump");
   }

   private void fastFall(){
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

   private bool isGrounded(){
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
           print("Weapon swapped to: Longsword");
           return;
       }
       if(nextWep == 2){
           anim.runtimeAnimatorController = GSOverride;
           currentWep = nextWep;
           weaponSpeed = baseSpeed - 2;
           print("Weapon swapped to: Greatsword");
           return;
       }
       if(nextWep == 3){
           anim.runtimeAnimatorController = DGOverride;
           currentWep = nextWep;
           weaponSpeed = baseSpeed + 2;
           print("Weapon swapped to: Dagger");
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
        if((anim.GetCurrentAnimatorStateInfo(0).IsName("horizontalAttack") || anim.GetCurrentAnimatorStateInfo(0).IsName("Special")) && !anim.IsInTransition(0)){
           isAttacking = true;
           //print("is Attacking");
       }else{
           isAttacking = false;
       }
       return isAttacking;
    }

    



}
