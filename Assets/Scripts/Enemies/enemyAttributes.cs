using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyAttributes : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private float HEALTH_MAX;

    private Animator anim;

    private Rigidbody2D body;
    private float health;

    private bool takenDamage = false;

    private bool hasCollide = false;

    private float collideTimer = .5f;
    private float startTime;
    private float staggerTimer = .25f;
    [SerializeField] private float attackDamage;

    void Awake()
    {   
        health = HEALTH_MAX;
        anim = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
    }

    void Start(){
        health = HEALTH_MAX;
        Physics2D.IgnoreLayerCollision(10,10, true);
    }

    void OnEnable(){
        health = HEALTH_MAX;
    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0){
            //Death animation in the future
            StartCoroutine(death());
        }

        if(Time.time > startTime + collideTimer){
            hasCollide = false;
        }
        if(Time.time > startTime + staggerTimer){
            takenDamage = false;
            //print("taken damage off");
        }
    }

    public void remHealth(float tempHealth){
        health -= tempHealth;
        if(health < 0){
            health = 0;
        }
        takenDamage = true;
        print(takenDamage);
        print(health + " health");
    }

    public void resetHealth(){
        health = HEALTH_MAX;
    }

    public bool getTakenDamage(){
        return takenDamage;
    }

    public void setTakenDamage(bool temp){
        takenDamage = temp;
    }

    public void setHasCollide(bool temp){
        hasCollide = temp;
        startTime = Time.time;

    }

    public bool getHasCollide(){
        return hasCollide;
    }

    private IEnumerator death(){
        float startTime = Time.time;
        float deathTime = 1;

        transform.position = new Vector2(-500,-500);
        //print("moved");
        anim.Rebind();
        while(Time.time < startTime + deathTime)
        {
            //print("waiting");
            yield return null;
        }

        this.gameObject.SetActive(false);
        
    }

    public float getAttackDamage(){
        return attackDamage;
    }

}
