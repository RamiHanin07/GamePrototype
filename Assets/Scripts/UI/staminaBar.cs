using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class staminaBar : MonoBehaviour
{
    [SerializeField] private playerAttributes playerAttributes;

    [SerializeField] private Image barImage;

    private Color baseColor;
    void Awake(){
        baseColor = barImage.color;
    }

    void FixedUpdate(){
        updateStaminaBar();
        if(playerAttributes.getNegativeStam() == true){
            barImage.color = Color.black;
        }
        else{
            barImage.color = baseColor;
        }
    }

    public void updateStaminaBar(){
        //print("in stam");
        //print("Player Percentage: " + playerAttributes.getStamina() / playerAttributes.getStaminaMax());
        float currentStam = playerAttributes.getStamina();
        if(currentStam < 0){
            currentStam = 0;
        }
        barImage.fillAmount = currentStam / playerAttributes.getStaminaMax();
    }
}
