using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[SelectionBase]
public class GridDrawer : MonoBehaviour
{
    [field: SerializeField] public bool IsShowGrid {get; private set;}
    [field: SerializeField] public RectTransform RectTransform {get; private set;}
    [field: SerializeField] public RectTransform ParentRectTransform {get; private set;}
    [field: SerializeField] public Vector2 Offset {get; private set;}
    private void Reset()
    {
        RectTransform = GetComponent<RectTransform>();
        ParentRectTransform = RectTransform.parent.GetComponent<RectTransform>();
        
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GridDrawer))]
public class GridDrawerEditor : Editor
{
    void OnSceneGUI()
    {
        DrawGrid();
        HandleSnapping();
    }

    private void DrawGrid()
    {
        GridDrawer drawer = (GridDrawer)target;
        if (!drawer.IsShowGrid) return; 
        if (drawer.RectTransform == null) return; 
        if (drawer.ParentRectTransform == null) return; 

        Vector2 size = drawer.RectTransform.sizeDelta;
        Vector2 position = drawer.RectTransform.position;
        Vector2 snappedPosition = new Vector2(
            Mathf.Round(position.x / position.x) * position.x,
            Mathf.Round(position.y / position.y) * position.y
        );
        Vector2 offset = drawer.Offset;
        for (int i = -2; i < 3; i++)
        {
            int distance = Mathf.Abs(i);
            for (int j = -2; j < 3; j++)
            {
                if (Mathf.Abs(2 - Mathf.Abs(j)) < distance) continue;
                Handles.DrawWireCube(snappedPosition + new Vector2(offset.x * i, offset.y * j), size);
            }
        }
    }
    private void HandleSnapping()
    {
        GridDrawer drawer = (GridDrawer)target;
        if (drawer.RectTransform == null || drawer.ParentRectTransform == null) return;

        // Start position
        Vector2 position = drawer.RectTransform.anchoredPosition;
        Vector2 offset = drawer.Offset;

        // Snap to grid
        Vector2 snappedPosition = new Vector2(
            Mathf.Round(position.x / offset.x) * offset.x,
            Mathf.Round(position.y / offset.y) * offset.y
        );

        // Apply snapped position if it changed
        if (position != snappedPosition)
        {
            Undo.RecordObject(drawer.RectTransform, "Snap to Grid");
            drawer.RectTransform.anchoredPosition = snappedPosition;
        }
    }
}
#endif