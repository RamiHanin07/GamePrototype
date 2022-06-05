using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Portal: MonoBehaviour
{

    private bool triggerActive = false;

    private float step;

    [SerializeField] private TMP_Text UI_text;

    [SerializeField] private GameObject UI_text_gameObject;

    [SerializeField] playerMovement playerMovement;

    [SerializeField] playerAttributes playerAttributes;


    [SerializeField] Portal partnerPortalUp = null;
    [SerializeField] Portal partnerPortalDown = null;

    [SerializeField] private bool isActive = true;

    private bool usePortalDownBool = false;
    private bool usePortalUpBool = false;

    private Sprite portalPlayerSprite;

    private Sprite portalOn;
    private SpriteRenderer spriteRenderer;

    private bool isTeleporting = false;

    // Update is called once per frame

    void Awake(){
        spriteRenderer = GetComponent<SpriteRenderer>();
        portalOn = Resources.Load<Sprite>("World Blocks/Portal/Portal");
        UI_text.text = "Press 'E' to Activate";
        if(isActive){
            activate();
        }
        //print(portalOn);
    }
    void Update()
    {   
        //Update UI button prompt text
        if(isActive){
            if(partnerPortalUp != null && partnerPortalDown != null){
                if(partnerPortalDown.getIsActive() && partnerPortalUp.getIsActive()){
                    UI_text.text = "Press 'Up Arrow' or 'Down Arrow' to activate";
                }
                else if(partnerPortalDown.getIsActive()){
                    UI_text.text = "Press 'Down Arrow' to activate";
                }
                else if(partnerPortalUp.getIsActive()){
                    UI_text.text = "Press 'Up Arrow' to activate";
                }
                else{
                    UI_text.text = "Activate partner portal to access";
                }
            }
            else if(partnerPortalDown != null){
                if(partnerPortalDown.getIsActive()){
                    UI_text.text = "Press 'Down Arrow' to activate";
                }else{
                    UI_text.text = "Activate partner portal to access";
                }
            }
            else if(partnerPortalUp != null){
                if(partnerPortalUp.getIsActive()){
                    UI_text.text = "Press 'Up Arrow' to activate";
                }else{
                    UI_text.text = "Activate partner portal to access";
                }
            }
        }

        if(triggerActive && !isActive){
            UI_text_gameObject.SetActive(true);
            if(Input.GetKeyDown(KeyCode.E)){
                activate();
            }
        }

        if(isActive && triggerActive && partnerPortalUp != null && partnerPortalDown != null && !isTeleporting){
            if(partnerPortalUp.getIsActive() && Input.GetKeyDown(KeyCode.UpArrow)){
                UI_text_gameObject.SetActive(false);
                StartCoroutine(usePortalUp());
            }
            if(partnerPortalDown.getIsActive() && Input.GetKeyDown(KeyCode.DownArrow)){
                UI_text_gameObject.SetActive(false);
                StartCoroutine(usePortalDown());
            }
        }
        else if(isActive && triggerActive && Input.GetKeyDown(KeyCode.UpArrow) && !isTeleporting){
            if(partnerPortalUp != null){
                 if(partnerPortalUp.getIsActive()){
                    //print("partner is active");
                    UI_text_gameObject.SetActive(false);
                    StartCoroutine(usePortalUp());
                }
                else{
                    //print("partner is not active");
                }
            }
        }
        else if(isActive && triggerActive && Input.GetKeyDown(KeyCode.DownArrow) && !isTeleporting){
            if(partnerPortalDown != null){
                if(partnerPortalDown.getIsActive()){
                    //print("partner is active");
                    UI_text_gameObject.SetActive(false);
                    StartCoroutine(usePortalDown());
                }
                else{
                    //print("partner is not active");
                }
            }
        }
        else{
            //print("isActive: " + isActive);
            //print("triggerActive: " + triggerActive);
        }

    }

    private void FixedUpdate() {
        step = Time.deltaTime * 15;
        if(usePortalDownBool){
            if(!playerMovement.getBody().isKinematic){
                playerMovement.getBody().isKinematic = true;
            }
            playerMovement.getBody().transform.position = Vector2.MoveTowards(playerMovement.getBody().transform.position, partnerPortalDown.transform.position, step);
        }
        if(usePortalUpBool){
            if(!playerMovement.getBody().isKinematic){
                playerMovement.getBody().isKinematic = true;
            }
            playerMovement.getBody().transform.position = Vector2.MoveTowards(playerMovement.getBody().transform.position, partnerPortalUp.transform.position, step);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            triggerActive = true;
            UI_text_gameObject.SetActive(true); 
        }
        else{
        }
    }

    private void OnTriggerExit2D(Collider2D other){
        if(other.CompareTag("Player")){
            triggerActive = false;
            UI_text_gameObject.SetActive(false);
        }
    }

    private IEnumerator usePortalUp(){
        playerMovement.getBody().isKinematic = true;
        isTeleporting = true;
        playerMovement.setisTeleporting(isTeleporting);
        usePortalUpBool = true;
        playerMovement.getBody().transform.position = transform.position;
        playerAttributes.setInCorruption(true);
        while(playerMovement.getBody().transform.position != partnerPortalUp.transform.position){
            playerAttributes.addCorruption(.2f);
            yield return null;
        }
        playerAttributes.setInCorruption(false);
        usePortalUpBool = false;
        playerMovement.getBody().isKinematic = false;
        isTeleporting = false;
        playerMovement.setisTeleporting(isTeleporting);
        
    }

    private IEnumerator usePortalDown(){
        playerMovement.getBody().isKinematic = true;
        isTeleporting = true;
        playerMovement.setisTeleporting(isTeleporting);
        playerMovement.getBody().transform.position = transform.position;
        usePortalDownBool = true;
        playerAttributes.setInCorruption(true);
        while(playerMovement.getBody().transform.position != partnerPortalDown.transform.position){
            playerAttributes.addCorruption(.2f);
            yield return null;
        }
        playerAttributes.setInCorruption(false);
        usePortalDownBool = false;
        playerMovement.getBody().isKinematic = false;
        isTeleporting = false;
        playerMovement.setisTeleporting(isTeleporting);
    }


    public bool getIsActive(){
        return isActive;
    }

    public void activate(){
        isActive = true;
        spriteRenderer.sprite = portalOn;
    }

    public bool getIsTeleporting(){
        return isTeleporting;
    }
        
}
