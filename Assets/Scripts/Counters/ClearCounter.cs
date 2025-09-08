using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField] private KitchenObjectScriptableObject kitchenObjectScriptableObject;
        
    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            //nothing on counter
            if (player.HasKitchenObject())
            {
                //player holding something
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
            else
            {
                //player carrying nothing 
            }
        }
        else
        {
            //kitchen object already on counter
            if (player.HasKitchenObject())
            {
                //player is also carrying something
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    //item is a plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectScriptableObject()))
                    {
                        //add ingredient to plate
                        GetKitchenObject().DestroySelf();
                    }
                }
                else
                {
                    //player isn't carrying a plate but something else
                    if (GetKitchenObject().TryGetPlate(out plateKitchenObject))
                    {
                        //counter has plate
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectScriptableObject()))
                        {
                            player.GetKitchenObject().DestroySelf();
                        }
                    }
                }
            }
            else
            {
                //player carrying nothing -> let player pick counter item
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }
}
