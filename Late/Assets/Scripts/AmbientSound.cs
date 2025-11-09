using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSound : MonoBehaviour
{

    public Collider Area;
    public GameObject Player;

    // Update is called once per frame
    void Update()
    {
        // Locate closest point on the the collider to the player.
        Vector3 closestPoint = Area.ClosestPoint(Player.transform.position);

        // set position to the clostest point to the player.
        transform.position = closestPoint;
    }
}
