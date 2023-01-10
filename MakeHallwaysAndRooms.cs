using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MakeHallwaysAndRooms : MonoBehaviour
{
    [Header("Don't mess with this stuff")]
    public GameObject basicRoom;
    public GameObject bossRoom;
    public LayerMask roomLayer;
    public string makeType;
    public bool firstRoom;
    
    [Header("Mess with this stuff")]
    public int startNumOfRooms;
    Transform thisExitPoint;
    Transform roomEntry;
    Vector3 spawnPoint;
    Vector3 myRotation;
    Collider2D[] collidersFound;
    int attempts;
    int roomEntryUsed;
    bool valid;
    bool goodInTheLongTerm;
    GameObject room;

    // Siderooms variables
    GameObject[] roomArray;
    int chooseRoom;
    public bool sideRoomsEnabled = false;
    public int branches;
    public int branchLength;
    bool sideRoomValid;

    // Start is called before the first frame update
    void Start()
    {
        // This only kicks in with a room created in the editor (with the right configs in the script)
        MakeNumRooms(startNumOfRooms);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MakeSideRooms()
    {
        bool foundBossRoom;
        // For each branching path that isn't the main path: pick a random room and make a small path from it.
        for (int i = 0; i < branches; i++)
        {
            sideRoomValid = false;
            while (!sideRoomValid)
            {
                roomArray = GameObject.FindGameObjectsWithTag("Room");
                foundBossRoom = true;
                // Don't make rooms off of the boss room
                while (foundBossRoom)
                {
                    chooseRoom = UnityEngine.Random.Range(1, roomArray.Length);
                    foundBossRoom = roomArray[chooseRoom].name.Contains("Boss Room");
                }
                // Use room that was randomly picked to start the mini side path
                sideRoomValid = roomArray[chooseRoom].GetComponent<MakeHallwaysAndRooms>().MakeNumRooms(branchLength + 1, true, "hallway");
            }
        }
    }

    public bool MakeNumRooms(int numRooms, bool sideRoom = false, string typeToMake = null)
    {   
        Debug.Log(numRooms + makeType);
        // A couple of these must be instantiated in the method itself, otherwise it BREAKS. I'm too lazy to figure out which of
        // these three it is, so all shall remain
        GameObject newRoom = null;
        goodInTheLongTerm = false;
        GameObject roomCover = null;

        // Only used for side paths, specify room type from function using this
        if (typeToMake != null) 
        {
            makeType = typeToMake;
        }
        // Make it so the level generation always ends on a room instead of a hallway
        if (numRooms > 1 || makeType == "room")
        {
            attempts = 0;
            // Make sure room doesn't paint itself into a corner
            while (!goodInTheLongTerm)
            {
                // Make sure room doesn't intersect with any other room 
                while (!valid)
                {
                    // If it's the last room on the main path
                    if (numRooms == 1 && !sideRoom)
                    {
                        room = bossRoom;
                    } else {
                        room = basicRoom;
                    }

                    // Make sure room prefab is at default position
                    room.transform.rotation = Quaternion.identity;

                    // If making modifying a room layout that had to be redone (due to painting itself into a corner
                    // delete the old one and the door cover it opened so you can start fresh
                    if (newRoom)
                    {
                        roomCover.SetActive(true);
                        Destroy(newRoom);
                    }
                    // If room generation fails maxAttempt amount of times, it's probably painted itself into a corner
                    if (attempts > 5) {
                        return false;
                    }
                    // Pick random exit/entrance of room to make room/hallway from
                    roomEntryUsed = UnityEngine.Random.Range(0,5);

                    // If making a hallway, build the hallway off of the room (which determines which position)
                    if (makeType == "hallway")
                    {
                        thisExitPoint = this.gameObject.transform.GetChild(0).GetChild(roomEntryUsed); // Get choose room exit to start hallway at
                        roomEntry = room.transform.GetChild(0).GetChild(0);
                    } else { // If making a room, build it off of the hallway
                        thisExitPoint = this.gameObject.transform.GetChild(0).GetChild(1); // Get exit point of hallway
                        roomEntry = room.transform.GetChild(0).GetChild(roomEntryUsed);
                    }
                    
                    // Get final room rotation
                    myRotation = new Vector3(0, 0,thisExitPoint.transform.eulerAngles.z + 180 - roomEntry.transform.eulerAngles.z);

                    // Rotate room to see how far the exit would be offset, and use that to help find the spawnpoint of the new room
                    room.transform.eulerAngles = myRotation;
                    // Find how offset the room would be from the exit
                    spawnPoint = thisExitPoint.position - roomEntry.position;
                    // Place on correct layer
                    spawnPoint = new Vector3(spawnPoint.x, spawnPoint.y, 1);
                    // Check to see if room/hallway would be built on top of another
                    collidersFound = Physics2D.OverlapBoxAll(spawnPoint, room.transform.localScale, myRotation.z, roomLayer);
                    
                    // If you found an overlap
                    if (collidersFound.Length > 0)
                    {
                        // It's ok if the overlap is the hallway/room you're building off of
                        if (collidersFound.Length == 1 && collidersFound[0] == this.gameObject.GetComponent<Collider2D>())
                        {
                            valid = true;
                        // But if your overlapping something preexisting, that's no good
                        } else {
                            valid = false;
                            attempts++;
                        }
                    } else {
                        valid = true;
                    }
                }
                // Make the room/hallway
                newRoom = Instantiate(room, spawnPoint, Quaternion.Euler(myRotation));
                
                // Make sure the prefab is back to its normal rotation
                room.transform.rotation = Quaternion.identity;

                // Recursive room generation
                // Only make it so that rooms subtract from the rooms left count, and no thallways
                if (makeType == "hallway")
                {
                    roomCover = this.gameObject.transform.GetChild(1).GetChild(roomEntryUsed).gameObject;

                    if (numRooms > 0)
                    {
                        // See if the final room doesn't get painted into a corner
                        goodInTheLongTerm = newRoom.GetComponent<MakeHallwaysAndRooms>().MakeNumRooms(numRooms - 1, sideRoom);
                    } else {
                        // If you've reached the end and haven't painted yourself into a corner:
                        // it's a plan that's good in the long term!
                        goodInTheLongTerm = true;
                    }
                } else {
                    roomCover = newRoom.transform.GetChild(1).GetChild(roomEntryUsed).gameObject;
                    
                    // See if the final room doesn't get painted into a corner
                    goodInTheLongTerm = newRoom.GetComponent<MakeHallwaysAndRooms>().MakeNumRooms(numRooms, sideRoom);
                }
                
                // Open up the roomcover to the new hallway/room created
                roomCover.SetActive(false);

                // If you're here that means you couldn't find a valid place to put a room
                // Go back a room/hallway and try again
                attempts++;
                valid = false;
            }
        }

        // Once the main path has been created have the first room run the side room generation script
        if (firstRoom && sideRoomsEnabled)
            MakeSideRooms();

        // If you're here, that means that you found out the final room was succesfully prevented from
        // being painted into a corner!
        return true;
    }
}
