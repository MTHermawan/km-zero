using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(MeshCollider))]
public class VirtualScreen : GraphicRaycaster
{
    [SerializeField] private Camera _screenCamera; // Reference to the camera responsible for rendering the virtual screen's rendertexture
    public Camera ScreenCamera
    {
        get => _screenCamera;
        private set
        {
            if (_screenCamera != value)
            {
                _screenCamera = value;
            }
        }
    }

    [SerializeField] private GraphicRaycaster _screenCaster; // Reference to the GraphicRaycaster of the canvas displayed on the virtual screen
    public GraphicRaycaster ScreenCaster
    {
        get => _screenCaster;
        private set
        {
            if (_screenCaster != value)
            {
                _screenCaster = value;
            }
        }
    }
    private MeshCollider col;

    protected override void Awake()
    {
        base.Awake();
        col = GetComponent<MeshCollider>();
    }

    // Called by Unity when a Raycaster should raycast because it extends BaseRaycaster.
    public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
    {
        Ray ray = eventCamera.ScreenPointToRay(eventData.position); // Mouse
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.transform == transform)
            {
                // Figure out where the pointer would be in the second camera based on texture position or RenderTexture.
                Vector3 virtualPos = new Vector3(hit.textureCoord.x, hit.textureCoord.y);
                virtualPos.x *= ScreenCamera.targetTexture.width;
                virtualPos.y *= ScreenCamera.targetTexture.height;

                eventData.position = virtualPos;

                ScreenCaster.Raycast(eventData, resultAppendList);
            }
        }
    }

    public void EnableHit()
    {
        if (col == null) return;
        col.enabled = true;
    }
    
    public void DisableHit()
    {
        if (col == null) return;
        col.enabled = false;
    }

    public void SetScreenCamera(Camera newScreenCamera)
    {
        ScreenCamera = newScreenCamera;
    }

    public void SetScreenCaster(GraphicRaycaster newScreenCaster)
    {
        ScreenCaster = newScreenCaster;
    }
}