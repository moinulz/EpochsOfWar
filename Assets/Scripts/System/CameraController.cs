using UnityEngine;

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
        // WASD movement (no scrolling)
        Vector3 movement = Vector3.zero;
        
        if (Input.GetKey(KeyCode.W))
            movement += Vector3.forward;
        if (Input.GetKey(KeyCode.S))
            movement += Vector3.back;
        if (Input.GetKey(KeyCode.A))
            movement += Vector3.left;
        if (Input.GetKey(KeyCode.D))
            movement += Vector3.right;
        
        // Apply movement
        if (movement != Vector3.zero)
        {
            movement = movement.normalized * moveSpeed * Time.deltaTime;
            transform.Translate(movement, Space.World);
        }
    }

    void HandleMouseInput()
    {
        // Mouse scroll for zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            ZoomCamera(-scroll * zoomSpeed);
        }
        
        // Mouse drag for panning (middle mouse or right mouse)
        if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)) // Right or middle mouse
        {
            lastTouchPosition = Input.mousePosition;
            isDragging = true;
        }
        
        if (Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))
        {
            isDragging = false;
        }
        
        if (isDragging && (Input.GetMouseButton(1) || Input.GetMouseButton(2)))
        {
            Vector3 deltaPosition = Input.mousePosition - lastTouchPosition;
            PanCamera(deltaPosition);
            lastTouchPosition = Input.mousePosition;
        }
    }

    void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            // Single touch for panning
            Touch touch = Input.GetTouch(0);
            
            if (touch.phase == TouchPhase.Began)
            {
                lastTouchPosition = touch.position;
                isDragging = true;
                isPinching = false;
            }
            else if (touch.phase == TouchPhase.Moved && isDragging && !isPinching)
            {
                Vector3 deltaPosition = (Vector3)touch.position - lastTouchPosition;
                PanCamera(deltaPosition * touchSensitivity);
                lastTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
            }
        }
        else if (Input.touchCount == 2)
        {
            // Two finger pinch for zoom
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);
            
            float currentPinchDistance = Vector2.Distance(touch1.position, touch2.position);
            
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
    
    // Public methods for UI buttons (mobile-friendly)
    public void MoveUp() { transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.World); }
    public void MoveDown() { transform.Translate(Vector3.back * moveSpeed * Time.deltaTime, Space.World); }
    public void MoveLeft() { transform.Translate(Vector3.left * moveSpeed * Time.deltaTime, Space.World); }
    public void MoveRight() { transform.Translate(Vector3.right * moveSpeed * Time.deltaTime, Space.World); }
    public void ZoomIn() { ZoomCamera(-1f); }
    public void ZoomOut() { ZoomCamera(1f); }
}
