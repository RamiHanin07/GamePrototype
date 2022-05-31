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

    private GameObject[] enemyList;
    // Update is called once per frame

    void Start(){
    }
    void Awake(){
        if(enemyList == null){
            enemyList = GameObject.FindGameObjectsWithTag("Enemy");
        }else{
            print("else");
        }
    }
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
        for(int i = 0; i < enemyList.Length; i++){
            print(enemyList[i]);
            StartCoroutine(death(enemyList[i]));
        }
    }

    private IEnumerator death(GameObject enemy){
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
        
}
