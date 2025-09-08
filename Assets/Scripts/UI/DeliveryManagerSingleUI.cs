using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryManagerSingleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipeNameText;
    [SerializeField] private Transform iconContainer;
    [SerializeField] private Transform iconTemplate;
    
    private void Awake()
    {
        iconTemplate.gameObject.SetActive(false);
    }

    public void SetRecipeScriptableObject(RecipeScriptableObject recipeScriptableObject)
    {
        recipeNameText.text = recipeScriptableObject.recipeName;

        foreach (Transform child in iconContainer)
        {
            if (child == iconTemplate) continue;
            Destroy(child.gameObject); //cleanup
        }

        foreach (KitchenObjectScriptableObject kitchenObjectScriptableObject in recipeScriptableObject.kitchenObjectScriptableObjectList)
        {
            Transform iconTransform = Instantiate(iconTemplate, iconContainer);
            iconTransform.gameObject.SetActive(true);
            iconTransform.GetComponent<Image>().sprite = kitchenObjectScriptableObject.sprite;
        }
    }
} 
