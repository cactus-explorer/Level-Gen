using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRoomStuff : MonoBehaviour
{
    GameObject[] normalRoomLayouts;
    GameObject[] bossRoomLayouts;
    GameObject[] roomLayouts;
    GameObject useRoomLayout;
    Vector3 newPosition;
    bool[] exitDoorState;
    Transform exitDoorsParent;
    Transform[] ts; 
    bool unlocked;
    GameObject enemySpawns;
    public GameObject exitStairs;
    bool bossRoom;
    

    // Start is called before the first frame update
    void Start()
    {
        normalRoomLayouts = Resources.LoadAll<GameObject>("Room Layouts");
        roomLayouts = normalRoomLayouts;
        bossRoom = false;
        if (this.transform.parent.gameObject.name.Contains("Boss Room"))
        {
            bossRoom = true;
            bossRoomLayouts = Resources.LoadAll<GameObject>("Boss Room Layouts");
            roomLayouts = bossRoomLayouts;
        }
        newPosition = new Vector3(this.transform.parent.position.x, this.transform.parent.position.y, 0);
        useRoomLayout = Instantiate(roomLayouts[Random.Range(0, roomLayouts.Length)], newPosition, this.transform.rotation);
        useRoomLayout.transform.parent = this.transform.parent;
        useRoomLayout.transform.position = newPosition;
        exitDoorsParent = this.transform.parent.GetChild(1);
        ts = exitDoorsParent.gameObject.GetComponentsInChildren<Transform>(true);
        exitDoorState = new bool[ts.Length];
        for (int i = 0; i < ts.Length; i++)
        {
            exitDoorState[i] = ts[i].gameObject.activeSelf;
        }
        unlocked = false;
        enemySpawns = this.transform.parent.GetChild(2).gameObject;

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (!unlocked && other.gameObject.name == "Player")
        {
            foreach (Transform i in ts)
            {
                i.gameObject.SetActive(true);
            }

            // Instantiate();
            enemySpawns.SetActive(true);
            // StartCoroutine(ExampleCoroutine());
        }
    }

    public void DisableDoors()
    {
        if (bossRoom)
        {
            GameObject stairs;

            bossRoom = true;
            stairs = Instantiate(exitStairs, this.transform.position, Quaternion.identity);
            stairs.transform.parent = this.transform.parent;
            foreach (Transform  child in transform.parent){
                // print(child.tag);
                if (child.name.Contains("Layout")){
                    child.gameObject.SetActive(false);
                }
            }
        }

        for (int i = 0; i < ts.Length; i++)
        {
            ts[i].gameObject.SetActive(exitDoorState[i]);
        }
        unlocked = true;
    }
}
