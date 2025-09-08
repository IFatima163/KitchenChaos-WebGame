using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CuttingCounter;

public class StoveCounter : BaseCounter, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }
    public enum State //making a state machine for the stove
    {
        Idle,
        Frying,
        Fried,
        Burned,
    }

    [SerializeField] private FryingRecipeScriptableObject[] fryingRecipeScriptableObjectArray;
    [SerializeField] private BurningRecipeScriptableObject[] burningRecipeScriptableObjectArray;

    private State state;
    private float fryingTimer;
    private FryingRecipeScriptableObject fryingRecipeScriptableObject;
    private float burningTimer;
    private BurningRecipeScriptableObject burningRecipeScriptableObject;
    
    private void Start()
    {
        state = State.Idle;
    }

    private void Update()
    {
        if (HasKitchenObject())
        {
            switch (state)
            {
                case State.Idle:
                    break;
                case State.Frying:
                    fryingTimer += Time.deltaTime;
                    
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = fryingTimer / fryingRecipeScriptableObject.fryingTimerMax
                    });

                    if (fryingTimer > fryingRecipeScriptableObject.fryingTimerMax)
                    {
                        //fried
                        GetKitchenObject().DestroySelf();

                        //spawn fried version
                        KitchenObject.SpawnKitchenObject(fryingRecipeScriptableObject.output, this);
                        
                        state = State.Fried;
                        burningTimer = 0f;
                        burningRecipeScriptableObject = GetBurningRecipeScriptableObjectWithInput(GetKitchenObject().GetKitchenObjectScriptableObject());
                    
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = state
                        });
                    }
                    break;
                case State.Fried:
                    burningTimer += Time.deltaTime;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = burningTimer / burningRecipeScriptableObject.burningTimerMax
                    });

                    if (burningTimer > burningRecipeScriptableObject.burningTimerMax)
                    {
                        //burned
                        GetKitchenObject().DestroySelf();

                        //spawn burned version
                        KitchenObject.SpawnKitchenObject(burningRecipeScriptableObject.output, this);
                        state = State.Burned;
                        
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = state
                        });

                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = 0f
                        });

                    }
                    break;
                case State.Burned:
                    break;
            }
        }
    }

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            //nothing on counter
            if (player.HasKitchenObject())
            {
                //player holding something -> only let player drop on frying board if sliced version available
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectScriptableObject()))
                {
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    //player drops something so state changes
                    fryingRecipeScriptableObject = GetFryingRecipeScriptableObjectWithInput(GetKitchenObject().GetKitchenObjectScriptableObject());     
                    state = State.Frying;
                    fryingTimer = 0f;

                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                    {
                        state = state
                    });

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = fryingTimer / fryingRecipeScriptableObject.fryingTimerMax
                    });
                }
            }
            else
            {
                //player carrying nothing 
            }
        }
        else
        {
            //kitchen object already on counter
            if (player.HasKitchenObject())
            {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    //item is a plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectScriptableObject()))
                    {
                        //add ingredient to plate
                        GetKitchenObject().DestroySelf();
                        state = State.Idle;

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = state
                        });
                        
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = 0f
                        });
                    }
                }
            }
            else
            {
                //player carrying nothing -> let player pick counter item
                GetKitchenObject().SetKitchenObjectParent(player);
                state = State.Idle;

                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                {
                    state = state
                });
                
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = 0f
                });
            }
        }
    }

    private bool HasRecipeWithInput (KitchenObjectScriptableObject inputKitchenObjectScriptableObject)
    {
        //checking if item about to be placed has a sliced version or not
        FryingRecipeScriptableObject fryingRecipeScriptableObject = GetFryingRecipeScriptableObjectWithInput(inputKitchenObjectScriptableObject);
        return fryingRecipeScriptableObject != null;
    }

    private KitchenObjectScriptableObject GetOutputForInput(KitchenObjectScriptableObject inputKitchenObjectScriptableObject)
    {
        //using item data from InteractAlternate method before destroying it 
        FryingRecipeScriptableObject fryingRecipeScriptableObject = GetFryingRecipeScriptableObjectWithInput(inputKitchenObjectScriptableObject);
        if (fryingRecipeScriptableObject != null)
        {
            return fryingRecipeScriptableObject.output;
        }
        else
        {
            return null;
        }
    }

    private FryingRecipeScriptableObject GetFryingRecipeScriptableObjectWithInput (KitchenObjectScriptableObject inputKitchenObjectScriptableObject)
    {
        //using item data from InteractAlternate method & calculating max required frying for each item 
        foreach (FryingRecipeScriptableObject fryingRecipeScriptableObject in fryingRecipeScriptableObjectArray)
        {
            if (fryingRecipeScriptableObject.input == inputKitchenObjectScriptableObject) //match item on counter to array to return fried version
            {
                return fryingRecipeScriptableObject;
            }
        }
        return null;
    }    

    private BurningRecipeScriptableObject GetBurningRecipeScriptableObjectWithInput (KitchenObjectScriptableObject inputKitchenObjectScriptableObject)
    {
        //using item data from InteractAlternate method & calculating max required burning for each item 
        foreach (BurningRecipeScriptableObject burningRecipeScriptableObject in burningRecipeScriptableObjectArray)
        {
            if (burningRecipeScriptableObject.input == inputKitchenObjectScriptableObject) //match item on counter to array to return burned version
            {
                return burningRecipeScriptableObject;
            }
        }
        return null;
    }

    public bool IsFried()
    {
        return state == State.Fried;
    }
}
