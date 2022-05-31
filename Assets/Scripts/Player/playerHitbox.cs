using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerHitbox : MonoBehaviour
{
    private enemyAttributes enemyAttributes;

    private playerAttack playerAttack;
    private BoxCollider2D myHitbox;

    private void Start(){
        playerAttack = transform.parent.GetComponent<playerAttack>();
        myHitbox = GetComponent<BoxCollider2D>();
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Enemy")){
            print(other.name);
            enemyAttributes = other.GetComponent<enemyAttributes>();
            if(!enemyAttributes.getHasCollide()){
                print(playerAttack.getWeaponDamage() + " weaponDamage");
                enemyAttributes.remHealth(playerAttack.getWeaponDamage());
                enemyAttributes.setHasCollide(true);
                print(enemyAttributes.getHasCollide());
                print("hit enemy");
                //print(myHitbox.name);
                myHitbox.enabled = false;
            }
        }
        else{
            print("else");
        }
    }
}
