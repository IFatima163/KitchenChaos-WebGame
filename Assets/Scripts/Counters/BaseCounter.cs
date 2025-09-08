using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCounter : MonoBehaviour, IKitchenObjectParent
{
    public static event EventHandler OnAnyObjectPlacedHere;

    public static void ResetStaticData()
    {
        OnAnyObjectPlacedHere = null;
    }

    [SerializeField] private Transform counterTopPoint;
    
    private KitchenObject kitchenObject;

    public virtual void Interact(Player player) //virtual = abstract but simpler
    {
       Debug.LogError("BaseCounter.Interact();");
    }

    public virtual void InteractAlternate(Player player) //virtual = abstract but simpler
    {
       //Debug.LogError("BaseCounter.InteractAlternate();");
    }
        
    public Transform GetKitchenObjectFollowTransform() //making the item imagery to change to new parent after command
    {
        return counterTopPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject; //changing parent counters

        if (kitchenObject != null)
        {
            OnAnyObjectPlacedHere?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void clearKitchenObject()
    {
        kitchenObject = null; //updating prev parent counter after new parent assigned
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null; //if there is an item on the counter
    }
}
