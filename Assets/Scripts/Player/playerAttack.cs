using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerAttack : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float attackCooldown;
    [SerializeField] private float attackCooldown2;
    private Animator anim;
    private playerMovement playerMovement;
    private playerAttributes playerAttributes;
    private float attackStaminaCost = 25;
    private float specialStaminaCost = 35;

    [SerializeField] private float LSAttackStaminaCost;
    [SerializeField] private float LSSpecialStaminaCost;


    private float cooldownTimer = 10000;

    private float weaponDamage = 40;



    void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<playerMovement>();
        playerAttributes = GetComponent<playerAttributes>();
    }

    // Update is called once per frame
    void Update()
    {
        cooldownTimer += Time.deltaTime;
        if(Input.GetKeyDown(KeyCode.C) && cooldownTimer > attackCooldown && playerMovement.canAttack()){
            if(!playerAttributes.getNegativeStam()){
                //print("AttackStaminaCost: " + attackStaminaCost);
                //print("Player Stamina: " + playerAttributes.getStamina());
                Attack();
                playerAttributes.remStamina(attackStaminaCost);
                //print("Player Stamina After: " + playerAttributes.getStamina());
            }
        }

        if(Input.GetKeyDown(KeyCode.V) && cooldownTimer > attackCooldown && playerMovement.canAttack()){
            if(!playerAttributes.getNegativeStam()){
                specialAttack();
                playerAttributes.remStamina(specialStaminaCost);
            }

        }
    }

    private void Attack(){
        anim.SetTrigger("attack");
        cooldownTimer = 0;

        //print("attack() in playerAttack");

    }

    private void specialAttack(){
        anim.SetTrigger("special");
        cooldownTimer = 0;
    }

    public float getWeaponDamage(){
        return weaponDamage;
    }

}
