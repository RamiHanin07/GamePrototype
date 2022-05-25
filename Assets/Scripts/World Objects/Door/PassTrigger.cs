using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassTrigger : MonoBehaviour
{

    private Door parent;

    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent.GetComponent<Door>();
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D aCol) {
        //parent.OnTriggerEnter2D(Collider2D, aCol);
    }
}
