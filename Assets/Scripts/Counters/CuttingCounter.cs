using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{
    public static event EventHandler OnAnyCut; //to avoid listening to each cut on each counter and listen to whole class 

    //new because we using same method for different purposes in trash counter as well as base counter class- new = don't override
    new public static void ResetStaticData()
    {
        OnAnyCut = null;
    }

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler OnCut;

    [SerializeField] private CuttingRecipeScriptableObject[] cuttingRecipeScriptableObjectArray;

    private int cuttingProgress;

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            //nothing on counter
            if (player.HasKitchenObject())
            {
                //player holding something -> only let player drop on cutting board if sliced version available
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectScriptableObject()))
                {
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    cuttingProgress = 0;

                    CuttingRecipeScriptableObject cuttingRecipeScriptableObject = GetCuttingRecipeScriptableObjectWithInput(GetKitchenObject().GetKitchenObjectScriptableObject());

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = (float)cuttingProgress / cuttingRecipeScriptableObject.cuttingProgressMax //converting to float b/c int div by int would lose decimal values
                    });
                }
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
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    //item is a plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectScriptableObject()))
                    {
                        //add ingredient to plate
                        GetKitchenObject().DestroySelf();
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

    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectScriptableObject()))
        {
            //there is an item on counter && that item can be cut into a sliced version : then 
            //destroy existing item then spawn cut version of same item
            
            cuttingProgress++;
            OnCut?.Invoke(this, EventArgs.Empty); //starting cutting animation
            OnAnyCut?.Invoke(this, EventArgs.Empty); //sound
            
            //take ref of item before destroying it
            CuttingRecipeScriptableObject cuttingRecipeScriptableObject = GetCuttingRecipeScriptableObjectWithInput(GetKitchenObject().GetKitchenObjectScriptableObject());
           
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = (float)cuttingProgress / cuttingRecipeScriptableObject.cuttingProgressMax //converting to float b/c int div by int would lose decimal values
            });
                   
            if(cuttingProgress >= cuttingRecipeScriptableObject.cuttingProgressMax)
            { 
                KitchenObjectScriptableObject outputKitchenObjectScriptableObject = GetOutputForInput(GetKitchenObject().GetKitchenObjectScriptableObject());
                //destroy item
                GetKitchenObject().DestroySelf();
                KitchenObject.SpawnKitchenObject(outputKitchenObjectScriptableObject, this);
            }
        }
    }

    private bool HasRecipeWithInput (KitchenObjectScriptableObject inputKitchenObjectScriptableObject)
    {
        //checking if item about to be placed has a sliced version or not
        CuttingRecipeScriptableObject cuttingRecipeScriptableObject = GetCuttingRecipeScriptableObjectWithInput(inputKitchenObjectScriptableObject);
        return cuttingRecipeScriptableObject != null;
    }

    private KitchenObjectScriptableObject GetOutputForInput(KitchenObjectScriptableObject inputKitchenObjectScriptableObject)
    {
        //using item data from InteractAlternate method before destroying it 
        CuttingRecipeScriptableObject cuttingRecipeScriptableObject = GetCuttingRecipeScriptableObjectWithInput(inputKitchenObjectScriptableObject);
        if (cuttingRecipeScriptableObject != null)
        {
            return cuttingRecipeScriptableObject.output;
        }
        else
        {
            return null;
        }
    }

    private CuttingRecipeScriptableObject GetCuttingRecipeScriptableObjectWithInput (KitchenObjectScriptableObject inputKitchenObjectScriptableObject)
    {
        //using item data from InteractAlternate method & calculating max required cuts for each item 
        foreach (CuttingRecipeScriptableObject cuttingRecipeScriptableObject in cuttingRecipeScriptableObjectArray)
        {
            if (cuttingRecipeScriptableObject.input == inputKitchenObjectScriptableObject) //match item on counter to array to return cut version
            {
                return cuttingRecipeScriptableObject;
            }
        }
        return null;
    }    
}
