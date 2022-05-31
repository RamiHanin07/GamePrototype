using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class agro_radius : MonoBehaviour
{
    private corruptedFighter parent;

    void Start(){
        parent = transform.parent.GetComponent<corruptedFighter>();
    }
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            //print("agro'd other player");
            parent.setAgroEnter();
        }
    }

    private void OnTriggerExit2D(Collider2D other){
        if(other.CompareTag("Player")){
            parent.setAgroExit();
        }
    }
}
