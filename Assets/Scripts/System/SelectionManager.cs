using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class SelectionManager : MonoBehaviour
{
    [Header("Selection Settings")]
    public LayerMask unitLayer = 1;
    public Material selectionMaterial;
    
    [Header("Selection Box")]
    public RectTransform selectionBox;
    public Canvas selectionCanvas;
    
    private List<Unit> selectedUnits = new List<Unit>();
    private Camera playerCamera;
    private Vector2 startMousePosition;
    private bool isDragging = false;
    
    void Start()
    {
        playerCamera = Camera.main;
        CreateSelectionBox();
    }
    
    void CreateSelectionBox()
    {
        if (selectionCanvas == null)
        {
            var canvasObj = new GameObject("SelectionCanvas");
            selectionCanvas = canvasObj.AddComponent<Canvas>();
            selectionCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            selectionCanvas.sortingOrder = 100;
        }
        
        if (selectionBox == null)
        {
            var boxObj = new GameObject("SelectionBox");
            boxObj.transform.SetParent(selectionCanvas.transform, false);
            selectionBox = boxObj.AddComponent<RectTransform>();
            var boxImage = boxObj.AddComponent<UnityEngine.UI.Image>();
            boxImage.color = new Color(0, 1, 0, 0.2f);
            boxObj.SetActive(false);
        }
    }
    
    void Update()
    {
        HandleSelectionInput();
        HandleMovementInput();
    }
    
    void HandleSelectionInput()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null) return;
        
        if (mouse.leftButton.wasPressedThisFrame)
        {
            startMousePosition = mouse.position.ReadValue();
            isDragging = true;
            selectionBox.gameObject.SetActive(true);
        }
        
        if (isDragging && mouse.leftButton.isPressed)
        {
            UpdateSelectionBox();
        }
        
        if (mouse.leftButton.wasReleasedThisFrame)
        {
            FinishSelection();
            isDragging = false;
            selectionBox.gameObject.SetActive(false);
        }
    }
    
    void UpdateSelectionBox()
    {
        Vector2 currentMousePosition = Mouse.current.position.ReadValue();
        Vector2 lowerLeft = new Vector2(
            Mathf.Min(startMousePosition.x, currentMousePosition.x),
            Mathf.Min(startMousePosition.y, currentMousePosition.y)
        );
        Vector2 upperRight = new Vector2(
            Mathf.Max(startMousePosition.x, currentMousePosition.x),
            Mathf.Max(startMousePosition.y, currentMousePosition.y)
        );
        
        selectionBox.position = lowerLeft;
        selectionBox.sizeDelta = upperRight - lowerLeft;
    }
    
    void FinishSelection()
    {
        Vector2 currentMousePosition = Mouse.current.position.ReadValue();
        
        ClearSelection();
        
        if (Vector2.Distance(startMousePosition, currentMousePosition) < 5f)
        {
            SelectSingleUnit();
        }
        else
        {
            SelectUnitsInBox();
        }
    }
    
    void SelectSingleUnit()
    {
        Ray ray = playerCamera.ScreenPointToRay(startMousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, unitLayer))
        {
            var unit = hit.collider.GetComponent<Unit>();
            if (unit != null)
            {
                AddToSelection(unit);
            }
        }
    }
    
    void SelectUnitsInBox()
    {
        var allUnits = FindObjectsByType<Unit>(FindObjectsSortMode.None);
        foreach (var unit in allUnits)
        {
            Vector3 screenPos = playerCamera.WorldToScreenPoint(unit.transform.position);
            if (IsPointInSelectionBox(screenPos))
            {
                AddToSelection(unit);
            }
        }
    }
    
    bool IsPointInSelectionBox(Vector3 screenPos)
    {
        Vector2 lowerLeft = new Vector2(
            Mathf.Min(startMousePosition.x, Mouse.current.position.ReadValue().x),
            Mathf.Min(startMousePosition.y, Mouse.current.position.ReadValue().y)
        );
        Vector2 upperRight = new Vector2(
            Mathf.Max(startMousePosition.x, Mouse.current.position.ReadValue().x),
            Mathf.Max(startMousePosition.y, Mouse.current.position.ReadValue().y)
        );
        
        return screenPos.x >= lowerLeft.x && screenPos.x <= upperRight.x &&
               screenPos.y >= lowerLeft.y && screenPos.y <= upperRight.y;
    }
    
    void AddToSelection(Unit unit)
    {
        if (!selectedUnits.Contains(unit))
        {
            selectedUnits.Add(unit);
            unit.SetSelected(true);
        }
    }
    
    void ClearSelection()
    {
        foreach (var unit in selectedUnits)
        {
            if (unit != null)
                unit.SetSelected(false);
        }
        selectedUnits.Clear();
    }
    
    void HandleMovementInput()
    {
        Mouse mouse = Mouse.current;
        if (mouse != null && mouse.rightButton.wasPressedThisFrame && selectedUnits.Count > 0)
        {
            Vector2 mousePosition = mouse.position.ReadValue();
            Ray ray = playerCamera.ScreenPointToRay(mousePosition);
            
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                MoveSelectedUnits(hit.point);
            }
        }
    }
    
    void MoveSelectedUnits(Vector3 targetPosition)
    {
        for (int i = 0; i < selectedUnits.Count; i++)
        {
            if (selectedUnits[i] != null)
            {
                Vector3 offset = new Vector3(
                    (i % 3 - 1) * 3f,
                    0,
                    (i / 3 - 1) * 3f
                );
                selectedUnits[i].MoveTo(targetPosition + offset);
            }
        }
    }
}
