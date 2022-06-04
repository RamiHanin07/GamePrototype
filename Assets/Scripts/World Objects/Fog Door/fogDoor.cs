using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fogDoor : MonoBehaviour
{
    [SerializeField] private GameObject[] enemies;

    private Animator anim;

    void Start(){
        anim = GetComponent<Animator>();
    }

    void FixedUpdate(){
        StartCoroutine(OpenDoor());
    }

    private IEnumerator OpenDoor(){
        //print("checking open Door");
        bool allDead = true;
        for(int i = 0; i < enemies.Length; i++){
            if(enemies[i].activeSelf){
                allDead = false;
            }
        }

        if(allDead){
            anim.SetTrigger("open");
            yield return new WaitForSeconds(.5f);
            this.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(2);
    }


}
