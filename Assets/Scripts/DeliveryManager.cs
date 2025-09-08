using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess; //sound
    public event EventHandler OnRecipeFailed; //sound
    
    public static DeliveryManager Instance {get; private set;}

    [SerializeField] private RecipeListScriptableObject recipeListScriptableObject;
    
    private List<RecipeScriptableObject> waitingRecipeScriptableObjectList;
    private float spawnRecipeTimer;
    private float spawnRecipeTimerMax = 4f;
    private int waitingRecipeMax = 4; //to not backlog orders
    private int successfulRecipesAmount;

    private void Awake()
    {
        Instance = this;

        waitingRecipeScriptableObjectList = new List<RecipeScriptableObject>();
    }

    private void Update()
    {
        spawnRecipeTimer -= Time.deltaTime;
        if(spawnRecipeTimer <= 0f)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if (GameManager.Instance.IsGamePlaying() && waitingRecipeScriptableObjectList.Count < waitingRecipeMax)
            {
                RecipeScriptableObject waitingRecipeScriptableObject = recipeListScriptableObject.recipeScriptableObjectList[UnityEngine.Random.Range(0, recipeListScriptableObject.recipeScriptableObjectList.Count)];
                
                waitingRecipeScriptableObjectList.Add(waitingRecipeScriptableObject);

                OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public void DeliveryRecipe (PlateKitchenObject plateKitchenObject) //order will be on a plate
    {
        //cycling through orders in waiting and comparing them with plate
        for(int i = 0; i < waitingRecipeScriptableObjectList.Count; i++)
        {
            RecipeScriptableObject waitingRecipeScriptableObject = waitingRecipeScriptableObjectList[i];

            if (waitingRecipeScriptableObject.kitchenObjectScriptableObjectList.Count == plateKitchenObject.GetKitchenObjectScriptableObjectList().Count)
            {
                //has same number of ingredients
                bool plateCotentMatchesRecipe = true;

                foreach (KitchenObjectScriptableObject recipeKitchenObjectScriptableObject in waitingRecipeScriptableObject.kitchenObjectScriptableObjectList)
                {
                    //cycling through all ingredients in recipe
                    bool ingredientFound = false;
                    foreach (KitchenObjectScriptableObject plateKitchenObjectScriptableObject in plateKitchenObject.GetKitchenObjectScriptableObjectList())
                    {
                        //cycling through all ingredients in plate
                        if (plateKitchenObjectScriptableObject == recipeKitchenObjectScriptableObject)
                        {
                            //ingredient match!
                            ingredientFound = true;
                            break;
                        }
                    }
                    if (!ingredientFound)
                    {
                        //ingredient of this recipe not found on the plate
                        plateCotentMatchesRecipe = false;
                    }
                }
                if (plateCotentMatchesRecipe)
                {
                    successfulRecipesAmount++;

                    waitingRecipeScriptableObjectList.RemoveAt (i); //if delivered item valid- remove from order waiting
                    
                    OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
                    OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
                    return;
                }
            }
        }
        //no matches found
        //player did not deliver correct recipe  
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }

    public List<RecipeScriptableObject> GetWaitingRecipeScriptableObjectList()
    {
        return waitingRecipeScriptableObjectList;
    }

    public int GetSuccessfulRecipesAmount()
    {
        return successfulRecipesAmount;
    }
}