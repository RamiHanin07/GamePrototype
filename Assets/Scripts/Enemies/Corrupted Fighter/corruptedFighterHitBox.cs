using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class corruptedFighterHitBox : MonoBehaviour
{

    private playerAttributes playerAttributes;
    private enemyAttributes enemyAttributes;
    private BoxCollider2D myHitbox;
    void Start(){
        myHitbox = GetComponent<BoxCollider2D>();
        enemyAttributes = transform.parent.GetComponent<enemyAttributes>();
    }
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D other) {
        //print("colliding");
        if(other.CompareTag("Player")){
            if(playerAttributes == null){
                playerAttributes = other.GetComponent<playerAttributes>();
            }
            if(!playerAttributes.getHasCollide()){
                playerAttributes.setHasCollide(true);
                playerAttributes.remHealth(enemyAttributes.getAttackDamage());
                //print(myHitbox.name);
                //print("hit player");
            }
        }else{
            //print(other.name);
        }
    }
}
