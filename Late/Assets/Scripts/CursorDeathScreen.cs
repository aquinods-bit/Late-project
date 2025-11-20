using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorDeathScreen : MonoBehaviour
{
    void Start()
            {
                // Make the cursor visible
                Cursor.visible = true;

                // Unlock the cursor so it can move freely
                Cursor.lockState = CursorLockMode.None;
            }
}
