using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Fountain : MonoBehaviour
{

    [SerializeField] private playerAttributes playerAttributes;

    [SerializeField] private bool triggerActive = false;

    [SerializeField] public GameObject text;
    // Update is called once per frame
    void Update()
    {   
        if(triggerActive && Input.GetKeyDown(KeyCode.E)){
            useFountain();
            text.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            triggerActive = true;
            //print(triggerActive);
            text.SetActive(true); 
        }
        else{
            //print(other);
        }
    }

    private void OnTriggerExit2D(Collider2D other){
        if(other.CompareTag("Player")){
            triggerActive = false;
            text.SetActive(false);
        }
    }

    private void useFountain(){
        //print("resting");
        playerAttributes.rest((Vector2) transform.position);
    }
        
}
