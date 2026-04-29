using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class InspectController : MonoBehaviour
{
    private PlayerController _player;
    private PlayerUIController _playerUI => PlayerUIController.Instance;
    private GameManager GameManager => GameManager.Instance;

    [SerializeField] private Transform holdPos;
    [SerializeField] private Camera inspectCamera;
    [SerializeField] private float pickUpRange = 2f; //how far the player can pickup the object from
    [SerializeField] private float rotationSensitivity = 1f; //how fast/slow the object is rotated in relation to mouse movement
    [SerializeField] private float zoomSensitivity = 3f; //how fast/slow the object is rotated in relation to mouse movement
    [SerializeField] private float zoomSmoothness = 3f;
    [SerializeField] private LayerMask HoldLayer; //layer index
    [SerializeField] private float minZoomRange = 0.5f;
    private float _currentZoom;
    public float currentZoom
    {
        get => _currentZoom;
        private set
        {
            _currentZoom = Mathf.Clamp(value, minZoomRange, pickUpRange);
        }
    }

    private GameObject _heldObj; //object which we pick up
    public GameObject HeldObj
    {
        get => _heldObj;
        set
        {
            _heldObj = value;

            _isRotating = false;
            if (_heldObj == null)
            {
                _player.Input.onLook -= RotateObject;
                _player.Input.onZoom -= ZoomObject;
                _player.Input.onRotate -= ToggleRotation;
                _player.EnableMovement();
                _playerUI.EnableCrosshair();
                _playerUI.DisableInspectUI();

                if (!isRotating)
                {
                    _player.LockCursor();
                }
            }
            else
            {
                _player.Input.onRotate += ToggleRotation;
                _player.Input.onZoom += ZoomObject;
                _player.DisableMovement();
                _playerUI.DisableCrosshair();
                _playerUI.EnableInspectUI();
                
                ToggleRotation(_isRotating);
            }
        }
    } //object which we pick up
    private Rigidbody heldObjRb; //object which we pick up
    private bool canDrop;
    private bool _isRotating = false;
    public bool isRotating
    {
        get => _isRotating;
        set
        {
            if (_isRotating != value)
            {
                _isRotating = value;
                ToggleRotation(_isRotating);
            }
        }
    }
    private Vector3 originalRot;
    private Vector3 originalPos;

    //Reference to script which includes mouse movement of player (looking around)
    //we want to disable the player looking around when rotating the object
    //example below 
    //MouseLookScript mouseLookScript;
    void Awake()
    {
        _player = GetComponent<PlayerController>();
        currentZoom = pickUpRange;
        _playerUI?.SetInspectCamera(inspectCamera);
    }

    void Update()
    {
        if (InputSystem.actions.FindAction("Interact").WasPressedThisFrame()) //change E to whichever key you want to press to pick up
        {
            if (HeldObj == null) //if currently not holding anything
            {
                //perform raycast to check if player is looking at object within pickuprange
                if (Physics.Raycast(_player.CameraTransform.position, _player.CameraTransform.TransformDirection(Vector3.forward), out RaycastHit hit, pickUpRange))
                {
                    //make sure pickup tag is attached
                    if (hit.transform.gameObject.CompareTag("canPickUp"))
                    {
                        //pass in object hit into the PickUpObject function
                        PickUpObject(hit.transform.gameObject);
                    }
                }
            }
            else
            {
                // if(canDrop == true)
                // {
                DropObject();
                // }
            }
        }

        if (HeldObj != null) //if player is holding object
        {
            LockObject();
        }
    }

    void PickUpObject(GameObject pickUpObj)
    {
        if (pickUpObj.TryGetComponent(out heldObjRb)) //make sure the object has a RigidBody
        {
            HeldObj = pickUpObj; //assign heldObj to the object that was hit by the raycast (no longer == null)

            // Save the original position and rotation
            originalPos = HeldObj.transform.position;
            originalRot = HeldObj.transform.localEulerAngles;

            heldObjRb.isKinematic = true;
            HeldObj.layer = GameManager.GetMaskLayers(HoldLayer.value); //change the object layer to the holdLayer
            HeldObj.transform.eulerAngles = holdPos.eulerAngles;
            HeldObj.transform.RotateAround(holdPos.position, -holdPos.up, 180);

            //make sure object doesnt collide with player, it can cause weird bugs
            Physics.IgnoreCollision(HeldObj.GetComponent<Collider>(), _player.GetComponent<Collider>(), true);
        }
    }
    void DropObject()
    {
        heldObjRb.isKinematic = false;
        HeldObj.transform.parent = null; //unparent object
        HeldObj.layer = 0; //object assigned back to default layer
        HeldObj.transform.position = originalPos;
        heldObjRb.transform.localEulerAngles = originalRot;

        originalPos = Vector3.zero;
        originalRot = Vector3.zero;
        HeldObj.transform.SetParent(null);

        //re-enable collision with player
        Physics.IgnoreCollision(HeldObj.GetComponent<Collider>(), _player.GetComponent<Collider>(), false);
        _player.EnableMovement();

        HeldObj = null; //undefine game object
    }

    void LockObject()
    {
        //keep object position the same as the holdPosition position
        Vector3 holdLocalPos = holdPos.transform.localPosition;
        holdPos.transform.localPosition = Vector3.Lerp(holdLocalPos, new Vector3(holdLocalPos.x, holdLocalPos.y, currentZoom), zoomSmoothness * Time.deltaTime);
        HeldObj.transform.position = holdPos.transform.position;
    }

    void RotateObject()
    {
        if (HeldObj == null) return;
        float XaxisRotation = _player.Input.LookInputVector.x * rotationSensitivity;
        float YaxisRotation = _player.Input.LookInputVector.y * rotationSensitivity;
        // rotate around the hold position using the hold transform axes

        HeldObj.transform.RotateAround(holdPos.position, -holdPos.up, XaxisRotation);
        HeldObj.transform.RotateAround(holdPos.position, holdPos.right, YaxisRotation);
    }

    void ToggleRotation()
    {
        isRotating = !isRotating;
    }

    void ToggleRotation(bool active)
    {
        if (active)
        {
            _player.LockCursor();
            _player.Input.onLook += RotateObject;
        }
        else
        {
            _player.UnlockCursor();
            _player.Input.onLook -= RotateObject;
        }
    }

    void ZoomObject()
    {
        currentZoom -= _player.Input.ZoomInputVector.y * zoomSensitivity;
    }

}
