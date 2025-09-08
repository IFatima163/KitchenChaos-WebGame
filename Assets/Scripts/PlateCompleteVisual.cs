using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour
{
    [Serializable] //to make the struct customizable
    public struct KitchenObjectScriptableObject_GameObject
    {
        public KitchenObjectScriptableObject kitchenObjectScriptableObject;
        public GameObject gameObject;
    }

    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private List<KitchenObjectScriptableObject_GameObject> kitchenObjectScriptableObjectList;

    private void Start()
    {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
        
        //making plate empty by default:
        foreach (KitchenObjectScriptableObject_GameObject kitchenObjectScriptableObjectGameObject in kitchenObjectScriptableObjectList)
        {
            kitchenObjectScriptableObjectGameObject.gameObject.SetActive(false);
        }
    }

    private void PlateKitchenObject_OnIngredientAdded (object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
    {
        //now comparing recieved item through list to visualize
        foreach (KitchenObjectScriptableObject_GameObject kitchenObjectScriptableObjectGameObject in kitchenObjectScriptableObjectList)
        {
            if (kitchenObjectScriptableObjectGameObject.kitchenObjectScriptableObject == e.kitchenObjectScriptableObject)
            {
                kitchenObjectScriptableObjectGameObject.gameObject.SetActive(true);
            }
        }
    }
}
