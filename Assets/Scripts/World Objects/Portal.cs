using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Portal: MonoBehaviour
{

    private bool triggerActive = false;

    [SerializeField] private TMP_Text UI_text;

    [SerializeField] private GameObject UI_text_gameObject;

    [SerializeField] playerMovement playerMovement;


    [SerializeField] Portal partnerPortalUp = null;
    [SerializeField] Portal partnerPortalDown = null;

    [SerializeField] private bool isActive = false;

    private Sprite portalOn;
    private SpriteRenderer spriteRenderer;

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

        if(isActive && triggerActive && playerMovement.isGrounded() && partnerPortalUp != null && partnerPortalDown != null){
            if(partnerPortalUp.getIsActive() && Input.GetKeyDown(KeyCode.UpArrow)){
                UI_text_gameObject.SetActive(false);
                usePortalUp();
            }
            if(partnerPortalDown.getIsActive() && Input.GetKeyDown(KeyCode.DownArrow)){
                UI_text_gameObject.SetActive(false);
                usePortalDown();
            }
        }
        else if(isActive && triggerActive && Input.GetKeyDown(KeyCode.UpArrow) && playerMovement.isGrounded()){
            if(partnerPortalUp != null){
                 if(partnerPortalUp.getIsActive()){
                    //print("partner is active");
                    UI_text_gameObject.SetActive(false);
                    usePortalUp();
                }
                else{
                    //print("partner is not active");
                }
            }
        }
        else if(isActive && triggerActive && Input.GetKeyDown(KeyCode.DownArrow) && playerMovement.isGrounded()){
            if(partnerPortalDown != null){
                if(partnerPortalDown.getIsActive()){
                    //print("partner is active");
                    UI_text_gameObject.SetActive(false);
                    usePortalDown();
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

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            triggerActive = true;
            //print(triggerActive);
            UI_text_gameObject.SetActive(true); 
        }
        else{
            //print(other);
        }
    }

    private void OnTriggerExit2D(Collider2D other){
        if(other.CompareTag("Player")){
            //print("leaving trigger");
            triggerActive = false;
            UI_text_gameObject.SetActive(false);
        }
    }

    private void usePortalUp(){
        //print("using portal");
        playerMovement.getBody().position = partnerPortalUp.transform.position;
        playerMovement.getBody().velocity = new Vector2(0,0);
    }

    private void usePortalDown(){
        //print("using portal");
        playerMovement.getBody().position = partnerPortalDown.transform.position;
        playerMovement.getBody().velocity = new Vector2(0,0);
    }

    public bool getIsActive(){
        return isActive;
    }

    public void activate(){
        isActive = true;
        spriteRenderer.sprite = portalOn;
        //print(isActive);
        //print("activating");
    }
        
}
