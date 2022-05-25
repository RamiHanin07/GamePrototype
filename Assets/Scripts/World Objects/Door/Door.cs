using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Door : MonoBehaviour
{

    private Animator anim;
    [SerializeField] private bool triggerActive = false;

    [SerializeField] public GameObject textLeft;
    [SerializeField] public GameObject textRight;

    [SerializeField] private Inventory inventory;
    [SerializeField] private bool isLocked = false;

    private GameObject text;

    private TMP_Text edit_text;

    [SerializeField] public GameObject UI;

    private Sprite doorOpen;
    private SpriteRenderer spriteRenderer;

    private BoxCollider2D setCollider;

    [SerializeField] private string key = "";
    
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        setCollider = GetComponent<BoxCollider2D>();

        if(transform.localScale.x == -1){
            text = textRight;
            text.transform.localScale = new Vector2(-1,1);
        }else{
            text = textLeft;
        }

        edit_text = text.GetComponent<TMP_Text>();
        if(key != ""){
            isLocked = true;
        }
    }

    // Update is called once per frame
    void Update()
    {   

        if(isLocked){
            if(inventory.contains(key) != -1){
                edit_text.text = "Press 'E' to Unlock";
            }else{
                edit_text.text = "Locked";
            }
        }
        else{
            edit_text.text = "Press 'E' to Open";
        }
        if(triggerActive && Input.GetKeyDown(KeyCode.E)){
            if(isLocked){
                if(inventory.contains(key) != -1){
                    unlock();
                }
            }else{
                StartCoroutine(setDoor());
                text.SetActive(false);
            }
            
        }

    }

    public void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            triggerActive = true;
            //print(triggerActive);
            text.SetActive(true); 
        }
        else{
            //print(other);
        }
    }

    public void OnTriggerExit2D(Collider2D other){
        if(other.CompareTag("Player")){
            triggerActive = false;
            text.SetActive(false);
        }
    }

    private IEnumerator openDoor(){
        //print("opening door");
        float startTime = Time.time;
        float endTime = 1;
        anim.SetTrigger("OpenDoor");
        while(Time.time < startTime + endTime){
            yield return null;
        }
    }
    private IEnumerator setDoor(){
        yield return (openDoor());
        spriteRenderer.sprite = doorOpen;
        const float ySize = 0.60194f;
        const float yOffset = 0.20348f;
        setCollider.size = new Vector2(setCollider.size.x, ySize);
        setCollider.offset = new Vector2(setCollider.offset.x, yOffset);
        UI.SetActive(false);
    }

    private void unlock(){
        isLocked = false;
    }
}


