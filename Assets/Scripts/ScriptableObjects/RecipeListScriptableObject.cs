using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu()] 
//commenting this out because we don't want multiple recipe lists
//it was a one time requirement
public class RecipeListScriptableObject : ScriptableObject
{
    public List<RecipeScriptableObject> recipeScriptableObjectList;
}
