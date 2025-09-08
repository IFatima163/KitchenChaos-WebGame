using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlateIconsSingleUI : MonoBehaviour
{  
    [SerializeField] private Image image;

    public void SetKitchenObjectScriptableObject (KitchenObjectScriptableObject kitchenObjectScriptableObject)
    {
        image.sprite = kitchenObjectScriptableObject.sprite;
    }
}
