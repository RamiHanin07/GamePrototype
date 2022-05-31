using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class corruptionMiasmaTest : MonoBehaviour
{

    [SerializeField] private playerAttributes playerAttributes;

    private bool triggerActive = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate(){
        if(triggerActive){
            //print("inUpdate");
            playerAttributes.addCorruption(.2f);
            print(playerAttributes.getCorruption());
        }

    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            triggerActive = true;
            //print("triggerActive");
            playerAttributes.setInCorruption(true);
        }
        else{
        }
    }

    private void OnTriggerExit2D(Collider2D other){
        if(other.CompareTag("Player")){
            triggerActive = false;
            playerAttributes.setInCorruption(false);
        }
    }
}
