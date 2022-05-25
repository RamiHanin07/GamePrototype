using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class healingItem : MonoBehaviour
{
    [SerializeField] private playerAttributes playerAttributes;
    [SerializeField] private playerMovement playerMovement;

    [SerializeField] public TMP_Text txt;

    private Animator anim;

    int HEAL_MAX_CHARGES = 5;
    int HEAL_AMOUNT = 60;
    int healCharges;

    // Update is called once per frame

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R) && playerMovement.canAttack()){
            if(getHealCharges() > 0){
                useHeal();
            }
            else{
                print(getHealCharges());
            }
        }
        txt.text = healCharges.ToString();

    }
    

    private void useHeal(){
        playerMovement.getAnim().SetTrigger("drink");
        playerAttributes.addHealth(HEAL_AMOUNT);
        healCharges--;
    }

    public void addHealChargeMax(int add){
        HEAL_MAX_CHARGES += add;
    }

    public void rest(){
        healCharges = HEAL_MAX_CHARGES;
    }

    public int getHealCharges(){
        return healCharges;
    }
}
