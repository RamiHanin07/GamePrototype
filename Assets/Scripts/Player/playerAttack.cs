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
    private float attackStaminaCost;
    private float specialStaminaCost;

    [SerializeField] private float LSAttackStaminaCost;
    [SerializeField] private float LSSpecialStaminaCost;

    private double currentWeapon;

    private float cooldownTimer = 10000;


    void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<playerMovement>();
        playerAttributes = GetComponent<playerAttributes>();
        currentWeapon = playerMovement.getWeapon();

        if(currentWeapon == 1){
            attackStaminaCost = LSAttackStaminaCost;
            specialStaminaCost = LSSpecialStaminaCost;
        }else if(currentWeapon == 2){
            attackStaminaCost = 25;
            specialStaminaCost = 35;
        }else if(currentWeapon == 3){
            attackStaminaCost = 5;
            specialStaminaCost = 25;
        }
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

        print("attack() in playerAttack");

    }

    private void Attack2(){
        anim.SetTrigger("attack2");
        cooldownTimer = 0;
    }

    private void specialAttack(){
        anim.SetTrigger("special");
        cooldownTimer = 0;
    }
}
