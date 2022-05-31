using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class irradiatedScreen : MonoBehaviour
{


    [SerializeField] private playerAttributes playerAttributes;

    [SerializeField] private GameObject display;

    private void FixedUpdate() {
        if(playerAttributes.getIsIrradiated()){
            print("isIrradiated");
            display.SetActive(true);
        }else{
            display.SetActive(false);
        }
    }
}
