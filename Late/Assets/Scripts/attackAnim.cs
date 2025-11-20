using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attackAnim : MonoBehaviour
{
    [SerializeField] private Animator myAnimatorController;
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            myAnimatorController.SetBool("hitAttack", true);
        }


    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            myAnimatorController.SetBool("hitAttack", false);
        }

        
    }
}
