using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{
    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded; //to visualize added ingredient
    public class OnIngredientAddedEventArgs : EventArgs 
    {
        public KitchenObjectScriptableObject kitchenObjectScriptableObject;
    }


    //list of items that are valid to be put on the plate
    [SerializeField] private List<KitchenObjectScriptableObject> validKitchenObjectScriptableObjectList;
    private List<KitchenObjectScriptableObject> kitchenObjectScriptableObjectList;

    private void Awake()
    {
        kitchenObjectScriptableObjectList = new List<KitchenObjectScriptableObject>();
    }

    public bool TryAddIngredient (KitchenObjectScriptableObject kitchenObjectScriptableObject)
    {
        if (!validKitchenObjectScriptableObjectList.Contains(kitchenObjectScriptableObject))
        {
            //not a valid ingredient
            return false;
        }
        if(kitchenObjectScriptableObjectList.Contains(kitchenObjectScriptableObject))
        {
            //already has item of the type
            return false; 
        }
        else
        {
            kitchenObjectScriptableObjectList.Add(kitchenObjectScriptableObject);
            
            OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs
            {
                kitchenObjectScriptableObject = kitchenObjectScriptableObject
            });

            return true;
        }
    }

    public List<KitchenObjectScriptableObject> GetKitchenObjectScriptableObjectList()
    {
        return kitchenObjectScriptableObjectList;
    }
}
