using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()] //creates an option in project folder to add
public class KitchenObjectScriptableObject : ScriptableObject
{
    public Transform prefab;
    public Sprite sprite;
    public string objectName;
}
