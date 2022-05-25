using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class inventorySlot : MonoBehaviour
{
    public TextMeshProUGUI labelText;
    // Start is called before the first frame update
    void Start(){
        labelText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void clearSlot(){
        labelText.enabled = false;
    }

    public void drawSlot(Inventory.Item item){
        labelText.enabled = true;
        labelText.text = item.name;
    }
}
