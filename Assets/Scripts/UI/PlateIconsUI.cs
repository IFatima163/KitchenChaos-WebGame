using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateIconsUI : MonoBehaviour
{
    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private Transform iconTemplate;

    private void Awake()
    {
        iconTemplate.gameObject.SetActive(false); //start game as icons disabled
    }

    private void Start()
    {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
    }

    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        //destroying previous cycle icons
        foreach (Transform child in transform)
        {
            if (child == iconTemplate) continue; //skip destroying the iconTemplate as that is also a child
            Destroy(child.gameObject);
        }

        //spawning icons
        foreach (KitchenObjectScriptableObject kitchenObjectScriptableObject in plateKitchenObject.GetKitchenObjectScriptableObjectList())
        {
            Transform iconTransform = Instantiate(iconTemplate, transform); //using transform to generate icon as child
            iconTransform.gameObject.SetActive(true);
            iconTransform.GetComponent<PlateIconsSingleUI>().SetKitchenObjectScriptableObject(kitchenObjectScriptableObject);
        }
    }
}
