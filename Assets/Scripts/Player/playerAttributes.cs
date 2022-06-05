using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerAttributes : MonoBehaviour
{

    
    private float HEALTH_MAX = 100;
    private float STAMINA_MAX = 100;
    private float CORRUPTION_MAX = 100;
    private float health;
    private float stamina;
    private float corruption;
    private Animator anim;
    private bool dead = false;

    private bool negativeStam = false;
    private bool irradiated = false;

    private bool hasCollide = false;

    [SerializeField] private healingItem healingItem;
    [SerializeField] private GameOver GameOver;
    [SerializeField] playerMovement playerMovement;

    [SerializeField] Vector2 SpawnPoint;
    private bool gameIsPaused = false;

    private bool inCorruption = false;

    private bool weaponDamage;

    private float collideTimer = 1;
    private float startTime = 0;
    private GameObject[] enemyList;






    void Awake()
    {
        anim = GetComponent<Animator>();
        health = HEALTH_MAX;
        stamina = STAMINA_MAX;
        corruption = 0;
        if(enemyList == null){
            enemyList = GameObject.FindGameObjectsWithTag("Enemy");
        }else{
            print("else");
        }
        
        
    }

    // FixedUpdate is called once every .02 seconds;
    void FixedUpdate()
    {
        
        if(checkStaminaGain()){
            addStamina(.7f);   
        }

        if(irradiated){
            remHealth(.2f);
        }

        if(!inCorruption){
            if(irradiated){
                remCorruption(.3f);
            }
            else{
                remCorruption(.4f);
            }
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

        if(Time.time > startTime + collideTimer){
            hasCollide = false;
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

    public float getCorruption(){
        return corruption;
    }
    public float getCorruptionMax(){
        return CORRUPTION_MAX;
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

    public void addCorruption(float tempCorruption){
        //print("adding corruption");
        corruption += tempCorruption;
        if(corruption > CORRUPTION_MAX){
            irradiated = true;
        }
        if(corruption > CORRUPTION_MAX + 50){
            corruption = CORRUPTION_MAX;
        }
    }

    public void remCorruption(float tempCorruption){
        corruption -= tempCorruption;
        if(corruption < 0){
            corruption = 0;
        }
        if(corruption == 0){
            irradiated = false;
        }
    }

    public void remStamina(float tempStam){
        stamina -= tempStam;
        //print(stamina);
        if(stamina < 0){
            negativeStam = true;
            //print("negative stamina");
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

    public bool getIsIrradiated(){
        return irradiated;
    }

    public void rest(Vector2 point){
        addStamina(200);
        healingItem.rest();
        addHealth(200);
        setSpawnPoint(point);
        anim.SetTrigger("drink");
        remCorruption(200);
        irradiated = false;
        addCorruption(20);
        
    }


    public IEnumerator death(){
        dead = true;
        //print("ded");
        resetEnemies();
        yield return StartCoroutine(GameOver.deathScreen());
        dead = false;
        anim.SetTrigger("drink");
        playerMovement.getBody().position = SpawnPoint;
        addHealth(200);
        remCorruption(200);
        irradiated = false;
        
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

    public void setInCorruption(bool corrupt){
        inCorruption = corrupt;
    }

    public bool getInCorruption(){
        return inCorruption;
    }

    public bool getHasCollide(){
        return hasCollide;
    }

    public void setHasCollide(bool temp){
        hasCollide = temp;
        if(hasCollide){
            startTime = Time.time;
        }
    }

    private void resetEnemies(){
        for(int i = 0; i < enemyList.Length; i++){
            print(enemyList[i]);
            StartCoroutine(resetEnemy(enemyList[i]));
        }
    }

    private IEnumerator resetEnemy(GameObject enemy){
        float startTime = Time.time;
        float deathTime = 1;

        enemy.transform.position = new Vector2(-500,-500);
        //print("moved");
        
        enemy.GetComponent<Animator>().Rebind();
        while(Time.time < startTime + deathTime)
        {
            //print("waiting");
            yield return null;
        }

        enemy.SetActive(false);
        enemy.SetActive(true);
        
    }

    private bool checkStaminaGain(){
        if(playerMovement.getisClimbingLadder() || 
        anim.GetCurrentAnimatorStateInfo(0).IsName("Idle Up") || 
        anim.GetCurrentAnimatorStateInfo(0).IsName("Idle Side") ||
        anim.GetCurrentAnimatorStateInfo(0).IsName("Idle Down") ||
        anim.GetCurrentAnimatorStateInfo(0).IsName("Walk Up") || 
        anim.GetCurrentAnimatorStateInfo(0).IsName("Walk Side") ||
        anim.GetCurrentAnimatorStateInfo(0).IsName("Walk Down") && 
        !anim.IsInTransition(0)){
            return true;
        }else{
            return false;
        }


    }


 



}
