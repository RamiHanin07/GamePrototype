using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerAttributes : MonoBehaviour
{

    
    private float HEALTH_MAX = 100;
    private float STAMINA_MAX = 100;
    private float health;
    private float stamina;
    private Animator anim;
    private bool dead = false;

    private bool negativeStam = false;

    [SerializeField] private healingItem healingItem;
    [SerializeField] private GameOver GameOver;
    [SerializeField] playerMovement playerMovement;

    [SerializeField] Vector2 SpawnPoint;
    private bool gameIsPaused = false;






    void Awake()
    {
        anim = GetComponent<Animator>();
        health = HEALTH_MAX;
        stamina = STAMINA_MAX;
        
    }

    // FixedUpdate is called once every .02 seconds;
    void FixedUpdate()
    {
        
        if((anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || 
            anim.GetCurrentAnimatorStateInfo(0).IsName("Walk") ) && !anim.IsInTransition(0)){
            addStamina(.7f);   
        }

        
    }

    void Update(){

        if(Input.GetKeyDown(KeyCode.T)){
            //rest();
        }

        if(getHealth() <= 0){
            StartCoroutine(death());
        }

        if(Input.GetKeyDown(KeyCode.Escape)){
            gameIsPaused = !gameIsPaused;
            pauseGame();
        }
    }

    public float getHealth(){
        return health;
    }

    public float getStamina(){
        return stamina;
    }

    public float getHealthMax(){
        return HEALTH_MAX;
    }

    public float getStaminaMax(){
        return STAMINA_MAX;
    }

    public void addStamina(float tempStam){
        stamina += tempStam;
        if(stamina > STAMINA_MAX){
            stamina = STAMINA_MAX;
            negativeStam = false;
        }
        return;
    }

    public void addHealth(float tempHealth){
        health += tempHealth;
        if(health > HEALTH_MAX){
            health = HEALTH_MAX;
        }
        return;
    }

    public void remStamina(float tempStam){
        stamina -= tempStam;
        //print(stamina);
        if(stamina < 0){
            negativeStam = true;
        }
        if(stamina < -50){
            stamina = -50;
        }

        return;
    }

    public void remHealth(float tempHealth){
        health -= tempHealth;
        if(health < 0){
            health = 0;
        }
        return;

    }

    public bool getNegativeStam(){
        return negativeStam;
    }

    public void rest(Vector2 point){
        addStamina(200);
        healingItem.rest();
        addHealth(200);
        setSpawnPoint(point);
        anim.SetTrigger("drink");
    }


    public IEnumerator death(){
        dead = true;
        //print("ded");
        yield return StartCoroutine(GameOver.deathScreen());
        dead = false;
        anim.SetTrigger("drink");
        playerMovement.getBody().position = SpawnPoint;
        addHealth(200);
    }


    public bool getDead(){
        return dead;
    }

    public void setSpawnPoint(Vector2 point){
        SpawnPoint = point;
    }


    public void pauseGame(){
        if(gameIsPaused){
            Time.timeScale = 0;
        }else{
            Time.timeScale = 1;
        }
    }



 



}
