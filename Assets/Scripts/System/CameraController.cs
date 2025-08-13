using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 20f; // Increased for larger world
    public float zoomSpeed = 5f;
    public float minZoom = 10f; // Adjusted for larger scale
    public float maxZoom = 100f; // Much higher max zoom
    
    [Header("Mobile Touch Controls")]
    public bool enableTouchControls = true;
    public float touchSensitivity = 2f;
    public float pinchSensitivity = 1f;
    
    private Camera cam;
    private Vector3 lastTouchPosition;
    private bool isDragging = false;
    
    // Touch handling
    private Touch[] touches;
    private float lastPinchDistance = 0f;
    private bool isPinching = false;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
            cam = Camera.main;
    }

    void Update()
    {
        HandleKeyboardInput();
        HandleMouseInput();
        
        if (enableTouchControls)
            HandleTouchInput();
    }

    void HandleKeyboardInput()
    {
        // WASD movement using new Input System
        Vector3 movement = Vector3.zero;
        
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
                movement += Vector3.forward;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
                movement += Vector3.back;
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                movement += Vector3.left;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                movement += Vector3.right;
        }
        
        // Apply movement
        if (movement != Vector3.zero)
        {
            movement = movement.normalized * moveSpeed * Time.deltaTime;
            transform.Translate(movement, Space.World);
        }
    }

    void HandleMouseInput()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null) return;
        
        // Mouse scroll for zoom
        Vector2 scroll = mouse.scroll.ReadValue();
        if (scroll.y != 0)
        {
            ZoomCamera(-scroll.y * 0.001f * zoomSpeed); // Adjusted for new input system scaling
        }
        
        // Mouse drag for panning (middle mouse or right mouse)
        if (mouse.rightButton.wasPressedThisFrame || mouse.middleButton.wasPressedThisFrame)
        {
            lastTouchPosition = mouse.position.ReadValue();
            isDragging = true;
        }
        
        if (mouse.rightButton.wasReleasedThisFrame || mouse.middleButton.wasReleasedThisFrame)
        {
            isDragging = false;
        }
        
        if (isDragging && (mouse.rightButton.isPressed || mouse.middleButton.isPressed))
        {
            Vector3 currentMousePos = mouse.position.ReadValue();
            Vector3 deltaPosition = currentMousePos - lastTouchPosition;
            PanCamera(deltaPosition);
            lastTouchPosition = currentMousePos;
        }
    }

    void HandleTouchInput()
    {
        Touchscreen touchscreen = Touchscreen.current;
        if (touchscreen == null) return;
        
        var touches = touchscreen.touches;
        int activeTouchCount = 0;
        
        // Count active touches
        for (int i = 0; i < touches.Count; i++)
        {
            if (touches[i].isInProgress)
                activeTouchCount++;
        }
        
        if (activeTouchCount == 1)
        {
            // Single touch for panning
            var touch = touches[0];
            
            if (touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
            {
                lastTouchPosition = touch.position.ReadValue();
                isDragging = true;
                isPinching = false;
            }
            else if (touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Moved && isDragging && !isPinching)
            {
                Vector3 currentPos = touch.position.ReadValue();
                Vector3 deltaPosition = currentPos - lastTouchPosition;
                PanCamera(deltaPosition * touchSensitivity);
                lastTouchPosition = currentPos;
            }
            else if (touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Ended || 
                     touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Canceled)
            {
                isDragging = false;
            }
        }
        else if (activeTouchCount == 2)
        {
            // Two finger pinch for zoom
            var touch1 = touches[0];
            var touch2 = touches[1];
            
            Vector2 touch1Pos = touch1.position.ReadValue();
            Vector2 touch2Pos = touch2.position.ReadValue();
            float currentPinchDistance = Vector2.Distance(touch1Pos, touch2Pos);
            
            if (!isPinching)
            {
                lastPinchDistance = currentPinchDistance;
                isPinching = true;
                isDragging = false;
            }
            else
            {
                float deltaDistance = currentPinchDistance - lastPinchDistance;
                ZoomCamera(-deltaDistance * pinchSensitivity * 0.01f);
                lastPinchDistance = currentPinchDistance;
            }
        }
        else
        {
            isPinching = false;
            isDragging = false;
        }
    }

    void PanCamera(Vector3 deltaPosition)
    {
        // Convert screen movement to world movement
        Vector3 worldDelta = cam.ScreenToWorldPoint(new Vector3(deltaPosition.x, deltaPosition.y, cam.nearClipPlane));
        Vector3 worldStart = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector3 movement = worldStart - worldDelta;
        
        // Apply movement (only X and Z for top-down style)
        movement.y = 0;
        transform.Translate(movement, Space.World);
    }

    void ZoomCamera(float zoomAmount)
    {
        if (cam.orthographic)
        {
            cam.orthographicSize += zoomAmount;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
        else
        {
            Vector3 pos = transform.position;
            pos.y += zoomAmount;
            pos.y = Mathf.Clamp(pos.y, minZoom, maxZoom);
            transform.position = pos;
        }
    }
}
