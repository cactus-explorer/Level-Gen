using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUps : MonoBehaviour
{
    PlayerShooting player; //Holds both crocodile

    public ArrayList powerUps = new ArrayList(); //Array list to hold th 


    // public float pickupMaxCountdown;
    public Transform[] spawnPoints; //Where the power up spawns
    public Transform[] bullets; //Position of power up
    public GameObject pickup;

    public Transform workstation;

    private void Awake()
    {
        player = FindObjectOfType<PlayerShooting>();
        SpawnWorkstation();
        SpawnPickUp();
    }

    private void Update()
    {
    }

    // Update is called once per frame


    void SpawnPickUp() //Spawn a random power up at a random spawnpoint 
    {
        float num = Random.value;
        if (num < .9f)
        {
            Transform pw = bullets[Random.Range(0, bullets.Length)];
            Transform _sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(pw, new Vector3(_sp.position.x, _sp.position.y, 0), Quaternion.identity);
        }
        /*Transform pw = powerUp[Random.Range(0, powerUp.Length)];
        Transform _sp = spawnPoints[Random.Range(0, spawnPoints.Length)];*/
    }

    void SpawnWorkstation()
    {
        float num = Random.value;
        if (num < .9f)
        {
            Transform _sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(workstation, new Vector3(_sp.position.x, _sp.position.y, 0), Quaternion.identity);
        }
        /*Transform pw = powerUp[Random.Range(0, powerUp.Length)];
        Transform _sp = spawnPoints[Random.Range(0, spawnPoints.Length)];*/
    }
}
