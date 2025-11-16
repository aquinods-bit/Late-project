using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerHealth : MonoBehaviour
{
    public GameObject deathScreen;
    public GameObject player;

    public float health = 100f;



    void Start()
    {
        deathScreen.SetActive(false);
    }



    void Update()
    {

        if(health <= 0)
        {
            player.GetComponent<FPSController>().enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            deathScreen.SetActive(true);
        }

        if (health > 100)
        {
            health = 100;
        }
        
    }
}
