using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{

    private bool isActive = false;

    public IEnumerator deathScreen(){
        gameObject.SetActive(true); 
        isActive = true;
        float startTime = Time.time;
        float endTime = 1;

        while(Time.time < startTime + endTime){
            yield return null;
        }

        gameObject.SetActive(false);
    }

    public bool getActive(){
        return isActive;
    }
}
