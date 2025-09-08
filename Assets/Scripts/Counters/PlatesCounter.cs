using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateRemoved;

    //spawn a specific amount of plates after awhile
    [SerializeField] private KitchenObjectScriptableObject plateKitchenObjectScriptableObject;

    private float spawnPlateTimer;
    private float spawnPlateTimerMax = 4f;
    private int plateSpawnedAmount; 
    private int plateSpawnedAmountMax = 4;
    //since player can interact with only one kitchen object at a time-
    //we spawn the "visual" of the plates multiple time but only a single interactable kitchen obj is spawned!

    private void Update()
    {
        spawnPlateTimer += Time.deltaTime;
        if (spawnPlateTimer > spawnPlateTimerMax)
        {
            spawnPlateTimer = 0f; //resetting timer

            if (GameManager.Instance.IsGamePlaying() &&  plateSpawnedAmount < plateSpawnedAmountMax)
            {
                plateSpawnedAmount++;

                OnPlateSpawned?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            //player is empty handed
            if (plateSpawnedAmount > 0)
            {
                //there is atleast one plate here
                plateSpawnedAmount--; //player picks it

                KitchenObject.SpawnKitchenObject(plateKitchenObjectScriptableObject, player);
                OnPlateRemoved?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
