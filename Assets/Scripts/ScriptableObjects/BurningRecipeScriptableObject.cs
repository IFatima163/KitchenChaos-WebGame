using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class BurningRecipeScriptableObject : ScriptableObject
{
    public KitchenObjectScriptableObject input; //what goes in to get chopped
    public KitchenObjectScriptableObject output; //which chopped version should come out
    public float burningTimerMax; 
} 
