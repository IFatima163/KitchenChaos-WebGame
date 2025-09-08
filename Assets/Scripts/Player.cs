using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent
{
    public static Player Instance { get; private set;} //a property- same as variable except skip the getter setter part

    public event EventHandler OnPickedSomething;
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
   
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }


    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;
    //layer mask ensures that a raycast hits objects within a certain layer
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;
        
    private bool isWalking;
    private Vector3 lastInteractDir; 
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject; //player holding items
   
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one Player instance"); //should not happen for singleton pattern as that destroys whole purpose
        }
        Instance = this; //setting instance
    }

    private void Start()
    {
        gameInput.OnInteractAction += gameInput_OnInteractAction;
        gameInput.OnInteractAlternateAction += gameInput_OnInteractAlternateAction;
    }

    private void gameInput_OnInteractAlternateAction(object sender, System.EventArgs e)
    {
        if (!GameManager.Instance.IsGamePlaying()) return; //not interact if game not in gamePlay mode

        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void gameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        if (!GameManager.Instance.IsGamePlaying()) return; //not interact if game not in gamePlay mode
        
        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }

    private void Update()
    {
        HandleMovement();
        HandleInteractions();
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    private void HandleInteractions()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if (moveDir != Vector3.zero)
        {
            lastInteractDir = moveDir; //to refer to the last moved direction after player stops moving
        }

        float interactDistance = 2f;
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask)) //Note 1 (B)
        {

            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter)) //same as GetComponent with null handling
            {
                //has base counter
                if (baseCounter != selectedCounter) //hitting aka selecting a counter to interact with
                {
                    SetSelectedCounter(baseCounter);
                }
            }
            else
            {
                SetSelectedCounter(null); //if rays hit something other than a counter
            }
        }
        else //if rays don't hit any counters or no longer hitting a counter
        {
            SetSelectedCounter(null);
        }
    }

    private void HandleMovement()
    {

        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        //converting 2D input from keyboard to 3D
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        //collision detection- Note 1
        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance); 

        //if player can't move- checking which of the all directions are still available to move to aka "hug the wall"
        if (!canMove)
        {
            //cannot move towards moveDir

            //attempt only x mvmnt
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = (moveDir.x < -.5f || moveDir.x > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            if (canMove)
            {
                //can move only on the X
                moveDir = moveDirX;
            }
            else
            {
                //cannot move only on the X

                //attempt only Z mvmnt
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = (moveDir.z < -.5f || moveDir.z > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

                if (canMove)
                {
                    //can only move on the Z
                    moveDir = moveDirZ;
                }
                else
                {
                    //cannot move in any direction
                }
            }
        }

        //if player can just simply keep moving straight
        if (canMove)
        {
            //deltaTime -to maintain the speed and not randomize it based on frame rate
            transform.position += moveDir * moveSpeed * Time.deltaTime; 
        }

        isWalking = moveDir != Vector3.zero;

        //turn player to face wherever it goes(forward) while transition into new position (Slerp):
        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }
        
    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = selectedCounter
        });        
    }

    public Transform GetKitchenObjectFollowTransform() //making the item imagery to change to new parent after command
    {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject; //changing parent counters

        if (kitchenObject != null)
        {
            OnPickedSomething?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void clearKitchenObject()
    {
        kitchenObject = null; //updating prev parent counter after new parent assigned
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null; //if there is an item on the counter
    }
}

/*
NOTE 1: RayCast & CapsuleCast

* checking if player is hitting something via emitting ray fom player to a distance
  equal to the player- player moves only if ray did not collide with anything in "playerRadius"      
* Ray cast emmits a ray from center of player body meaning if player misses an obkect by anything but its center- it'll pass
  and similarly, will collide an object ONLY IF that obj hits the center of the player. 
  This is why capsule casts are used for players as they consider the entire body.
* Ray cast requires player's current position, which direction player is moving in, and the distance you want to check.
* Capsule cast requires player's bottom position, head position, radius/ distance, direction, & distance moved.
* for raycast, code would have been: bool canMove = !Physics.Raycast(transform.position, moveDir, playerRadius);
* (B) Raycast normally doesn't return- for return you need to write OUT and give value (raycastHit) with type (RaycastHit)
  with the data of the type of collision among many other returnable things.

*/