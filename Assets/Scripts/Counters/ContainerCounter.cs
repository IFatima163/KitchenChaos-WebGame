using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    public event EventHandler OnPlayerGrabbedObject; //to trigger animation

    [SerializeField] private KitchenObjectScriptableObject kitchenObjectScriptableObject;
   
    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            //making sure only 1 item is spawned on top of the counter & player can pick it
            KitchenObject.SpawnKitchenObject(kitchenObjectScriptableObject, player);
           
            OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
        }
    }
}
