using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class corruptionBar : MonoBehaviour
{
    [SerializeField] private playerAttributes playerAttributes;

    [SerializeField] private Image barImage;

    private Color baseColor;
    private Color corruptedColor = new Color(243,2,166,255);
    void Awake(){
        baseColor = barImage.color;
    }

    void FixedUpdate(){
        updateCorruptionBar();
        if(playerAttributes.getIsIrradiated() == true){

            barImage.color = corruptedColor;
        }
        else{
            barImage.color = baseColor;
        }
    }

    public void updateCorruptionBar(){
        //print("in stam");
        //print("Player Percentage: " + playerAttributes.getStamina() / playerAttributes.getStaminaMax());
        float currentCorruption = playerAttributes.getCorruption();
        if(currentCorruption < 0){
            currentCorruption = 0;
        }
        barImage.fillAmount = currentCorruption / playerAttributes.getCorruptionMax();
    }
}
