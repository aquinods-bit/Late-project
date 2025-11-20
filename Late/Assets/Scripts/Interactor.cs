using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

interface IInteractable
{
    bool Interact();
}

public class Interactor : MonoBehaviour
{
    // LayerMask for interactables (set in Inspector if needed)
    public LayerMask interactableLayer;
    // Reference to the interactable popup UI (assign in Inspector)
    public GameObject InteractablePopUp;
    public AudioClip PickUp;
    public Transform InteractionPoint;
    public float InteractionRange = 3f;

    void Start()
    {
        // Try to find the popup if it wasn't assigned in the Inspector
        if (InteractablePopUp == null)
            InteractablePopUp = GameObject.Find("InteractablePopUp");

        // If InteractionPoint isn't set, fall back to this object's transform
        if (InteractionPoint == null)
            InteractionPoint = this.transform;

        HidePopup();
    }

    void Update()
    {
        // Proximity check for interactables
        // Use the provided layer mask when set; otherwise search all layers
        Collider[] nearby;
        if (interactableLayer.value != 0)
            nearby = Physics.OverlapSphere(InteractionPoint.position, InteractionRange, interactableLayer);
        else
            nearby = Physics.OverlapSphere(InteractionPoint.position, InteractionRange);
        IInteractable closestInteractable = null;
        float closestDistance = float.MaxValue;
        GameObject closestObject = null;
        foreach (var col in nearby)
        {
            if (col.TryGetComponent(out IInteractable interactableObj))
            {
                float dist = Vector3.Distance(InteractionPoint.position, col.transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestInteractable = interactableObj;
                    closestObject = col.gameObject;
                }
            }
        }
        // Only show popup if close to a pickable object
        if (closestInteractable != null && closestDistance <= InteractionRange)
        {
            if (InteractablePopUp != null)
            {
                InteractablePopUp.SetActive(true);
                var text = InteractablePopUp.GetComponentInChildren<Text>(true);
                if (text != null) text.gameObject.SetActive(true);
                Debug.Log($"[Interactor] Popup shown for {closestObject?.name}");
            }

            // Interact on key press
            if (Input.GetKeyDown(KeyCode.E))
            {
                bool pickedUp = false;
                try
                {
                    pickedUp = closestInteractable.Interact();
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"[Interactor] Exception during Interact(): {ex}");
                }

                // Also treat the object being deactivated as a successful pickup
                if (!pickedUp && closestObject != null && !closestObject.activeInHierarchy)
                {
                    pickedUp = true;
                }

                if (pickedUp)
                {
                    if (PickUp != null)
                        AudioSource.PlayClipAtPoint(PickUp, transform.position);
                    HidePopup(); // Hide popup after pickup
                    Debug.Log("[Interactor] Popup hidden after pickup");
                }
            }
        }
        else
        {
            if (InteractablePopUp != null && InteractablePopUp.activeSelf)
            {
                HidePopup();
                Debug.Log("[Interactor] Popup hidden (no interactable found)");
            }
        }
    }

    void HidePopup()
    {
        if (InteractablePopUp != null)
        {
            InteractablePopUp.SetActive(false);
        }
    }
}

// public class Interactor : MonoBehaviour, IInteractable{
//     public void Interact(){
//         Debug.Log("Interacted with " + gameObject.name);
// }
// Needed for all interactable objects