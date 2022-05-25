using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    private List<Item> inventory = new List<Item>();
    private List<inventorySlot> inventorySlots = new List<inventorySlot>();


    [SerializeField] private GameObject inventoryDisplay;

    [SerializeField] private bool inventoryOpen = false;

    private GameObject slotPrefab;

    public struct Item{
        public string name {get; set; }
        public Sprite icon {get; set; }

        public Item(string name){
            this.name = name;
            this.icon = null;
        }
        //public void createItem(string name, Sprite icon){
            //this.name = name;
            //this.icon = icon;
        //}
    }

    public void Start(){
        slotPrefab = Resources.Load<GameObject>("Fabs/UI/Slot");
    }

    public void Update(){
        if(Input.GetKeyDown(KeyCode.Y)){
            giveTestKey();
        }

        if(!inventoryOpen){
            if(Input.GetKeyDown(KeyCode.I)){
                openInventory();
            }
        }else{
            if(Input.GetKeyDown(KeyCode.I)){
                closeInventory();
            }
        }
    }

    public void openInventory(){
        //print("in open");
        inventoryOpen = true;
        Time.timeScale = 0;
        inventoryDisplay.SetActive(true);
    }
    public void closeInventory(){
        //print("in close");
        inventoryOpen = false;
        Time.timeScale = 1;
        inventoryDisplay.SetActive(false);
    }

    public List<Item> getInventory(){
        return inventory;
    }

    public void addItem(string name){
        Item temp = new Item(name);
        inventory.Add(temp);
        drawInventory();
    }

    public int contains(string name){
        for(int i = 0; i < inventory.Count; i++){
            if(inventory[i].name == name){
                return i;
            }
        }
        return -1;
    }

    public bool remItem(string name){
        int index = contains(name);
        if(index != -1){ 
            inventory.RemoveAt(index);
            drawInventory();
            return true;
        }
        else{
            return false;
        }
    }

    private void giveTestKey(){
        addItem("Lower Sewer Key");
        //print("Key acquired");
    }

    private void resetInventory(){
        foreach(Transform childTransform in inventoryDisplay.transform){
            Destroy(childTransform.gameObject);
        }
        inventorySlots = new List<inventorySlot>();
    }

    private void drawInventory(){
        resetInventory();

        for(int i  = 0; i < inventory.Count; i++){
            createInventorySlot();
        }

        for(int i = 0; i < inventory.Count; i++){
            inventorySlots[i].drawSlot(inventory[i]);
        }

    }

    private void createInventorySlot(){
        GameObject newSlot = Instantiate(slotPrefab);
        newSlot.transform.SetParent(inventoryDisplay.transform, false);

        inventorySlot newSlotComponent = newSlot.GetComponent<inventorySlot>();
        newSlotComponent.clearSlot();

        inventorySlots.Add(newSlotComponent);
    }
}
