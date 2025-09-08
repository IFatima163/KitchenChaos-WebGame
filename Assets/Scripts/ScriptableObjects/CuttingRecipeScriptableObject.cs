using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class CuttingRecipeScriptableObject : ScriptableObject
{
    public KitchenObjectScriptableObject input; //what goes in to get chopped
    public KitchenObjectScriptableObject output; //which chopped version should come out
    public int cuttingProgressMax; 
} 
