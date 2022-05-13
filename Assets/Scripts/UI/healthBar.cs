using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class healthBar : MonoBehaviour
{
    [SerializeField] private playerAttributes playerAttributes;

    [SerializeField] private Image barImage;
    void Awake(){

    }

    void FixedUpdate(){
        updateHealthBar();
    }

    public void updateHealthBar(){
        float currentHealth = playerAttributes.getHealth();
        if(currentHealth < 0){
            currentHealth= 0;
        }
        barImage.fillAmount = currentHealth / playerAttributes.getHealthMax();
    }

}
