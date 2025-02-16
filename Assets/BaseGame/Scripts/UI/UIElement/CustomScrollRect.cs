using System;
using TW.Utility.CustomType;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomScrollRect : UIBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler,
    IDragHandler, ICanvasElement, ILayoutElement, ILayoutGroup, IPointerDownHandler, IPointerUpHandler
{
    [field: SerializeField] private RectTransform Viewport { get; set; }
    [field: SerializeField] private RectTransform Content { get; set; }
    [field: SerializeField] private RectTransform ResizeContent { get; set; }
    [field: SerializeField] private bool Horizontal { get; set; } = true;

    [field: SerializeField] private bool Vertical { get; set; } = true;

    [field: SerializeField] private float Elasticity { get; set; } = 0.1f;

    [field: SerializeField] private bool Inertia { get; set; } = true;

    [field: SerializeField] private float DecelerationRate { get; set; } = 0.135f;

    [field: SerializeField] private float CurrentScale { get; set; } = 1.0f;
#if UNITY_EDITOR
    [field: SerializeField] private float ScrollSensitivity { get; set; } = 1.0f;
#endif

    [field: SerializeField] private FloatRange ScaleRange {get; set;}
    [field: SerializeField] private Vector2 DefaultContentSize {get; set;}
    private Vector2 m_PointerStartLocalCursor = Vector2.zero;
    private Vector2 m_ContentStartPosition = Vector2.zero;

    private RectTransform m_ViewRect;

    private RectTransform ViewRect
    {
        get
        {
            if (m_ViewRect == null)
                m_ViewRect = Viewport;
            if (m_ViewRect == null)
                m_ViewRect = (RectTransform)transform;
            return m_ViewRect;
        }
    }

    private Bounds m_ContentBounds;
    private Bounds m_ViewBounds;

    private Vector2 m_Velocity;

    private bool m_Dragging;

    private Vector2 m_PrevPosition = Vector2.zero;
    private Bounds m_PrevContentBounds;
    private Bounds m_PrevViewBounds;
    [NonSerialized] private bool m_HasRebuiltLayout = false;

    [NonSerialized] private RectTransform m_Rect;

    private RectTransform RectTransform
    {
        get
        {
            if (m_Rect == null)
                m_Rect = GetComponent<RectTransform>();
            return m_Rect;
        }
    }

    // field is never assigned warning
#pragma warning disable 649
    private DrivenRectTransformTracker m_Tracker;
#pragma warning restore 649
    
    private bool m_IsZooming = false;
    private float m_InitialTouchDistance;
    protected CustomScrollRect()
    {
    }

    public virtual void Rebuild(CanvasUpdate executing)
    {
        if (executing != CanvasUpdate.PostLayout) return;
        UpdateBounds();
        UpdatePrevData();

        m_HasRebuiltLayout = true;
    }

    public virtual void LayoutComplete()
    {
    }

    public virtual void GraphicUpdateComplete()
    {
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
        SetDirty();
    }

    protected override void OnDisable()
    {
        CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);
        m_Dragging = false;
        m_HasRebuiltLayout = false;
        m_Tracker.Clear();
        m_Velocity = Vector2.zero;
        LayoutRebuilder.MarkLayoutForRebuild(RectTransform);
        base.OnDisable();
    }

    public override bool IsActive()
    {
        return base.IsActive() && Content != null;
    }

    private void EnsureLayoutHasRebuilt()
    {
        if (!m_HasRebuiltLayout && !CanvasUpdateRegistry.IsRebuildingLayout())
            Canvas.ForceUpdateCanvases();
    }

    public void StopMovement()
    {
        m_Velocity = Vector2.zero;
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        m_Velocity = Vector2.zero;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (!IsActive())
            return;

        UpdateBounds();

        m_PointerStartLocalCursor = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(ViewRect, eventData.position,
            eventData.pressEventCamera, out m_PointerStartLocalCursor);
        m_ContentStartPosition = Content.anchoredPosition;
        m_Dragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        m_Dragging = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!m_Dragging)
            return;

        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (!IsActive())
            return;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(ViewRect, eventData.position,
                eventData.pressEventCamera, out Vector2 localCursor))
            return;

        UpdateBounds();

        Vector2 pointerDelta = localCursor - m_PointerStartLocalCursor;
        Vector2 position = m_ContentStartPosition + pointerDelta;

        // Offset to get content into place in the view.
        Vector2 offset = CalculateOffset(position - Content.anchoredPosition);
        position += offset;
        if (offset.x != 0)
            position.x -= RubberDelta(offset.x, m_ViewBounds.size.x);
        if (offset.y != 0)
            position.y -= RubberDelta(offset.y, m_ViewBounds.size.y);

        SetContentAnchoredPosition(position);
    }

    private void SetContentAnchoredPosition(Vector2 position)
    {
        if (!Horizontal)
            position.x = Content.anchoredPosition.x;
        if (!Vertical)
            position.y = Content.anchoredPosition.y;

        if (position != Content.anchoredPosition)
        {
            Content.anchoredPosition = position;
            UpdateBounds();
        }
    }

    private void LateUpdate()
    {
        if (!Content) return;

        EnsureLayoutHasRebuilt();
        UpdateBounds();
        float deltaTime = Time.unscaledDeltaTime;
        Vector2 offset = CalculateOffset(Vector2.zero);

        // Skip processing if deltaTime is invalid (0 or less) as it will cause inaccurate velocity calculations and a divide by zero error.
        if (deltaTime > 0.0f)
        {
            if (!m_Dragging && (offset != Vector2.zero || m_Velocity != Vector2.zero))
            {
                Vector2 position = Content.anchoredPosition;
                for (int axis = 0; axis < 2; axis++)
                {
                    // Apply spring physics if movement is elastic and content has an offset from the view.
                    if (offset[axis] != 0)
                    {
                        float speed = m_Velocity[axis];
                        float smoothTime = Elasticity;
                        position[axis] = Mathf.SmoothDamp(Content.anchoredPosition[axis],
                            Content.anchoredPosition[axis] + offset[axis], ref speed, smoothTime, Mathf.Infinity,
                            deltaTime);
                        if (Mathf.Abs(speed) < 1)
                            speed = 0;
                        m_Velocity[axis] = speed;
                    }
                    // Else move content according to velocity with deceleration applied.
                    else if (Inertia)
                    {
                        m_Velocity[axis] *= Mathf.Pow(DecelerationRate, deltaTime);
                        if (Mathf.Abs(m_Velocity[axis]) < 1)
                            m_Velocity[axis] = 0;
                        position[axis] += m_Velocity[axis] * deltaTime;
                    }
                    // If we have neither elaticity or friction, there shouldn't be any velocity.
                    else
                    {
                        m_Velocity[axis] = 0;
                    }
                }

                SetContentAnchoredPosition(position);
            }

            if (m_Dragging && Inertia)
            {
                Vector3 newVelocity = (Content.anchoredPosition - m_PrevPosition) / deltaTime;
                m_Velocity = Vector3.Lerp(m_Velocity, newVelocity, deltaTime * 10);
            }
        }

        if (m_ViewBounds != m_PrevViewBounds || m_ContentBounds != m_PrevContentBounds ||
            Content.anchoredPosition != m_PrevPosition)
        {
            UpdatePrevData();
        }
        
        if (m_IsZooming && Input.touchCount == 2)
        {
            float currentTouchDistance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
            ChangeResizeContent(CurrentScale + (currentTouchDistance - m_InitialTouchDistance) * 0.0005f);
        }
#if UNITY_EDITOR
        if (Input.mouseScrollDelta.y != 0)
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(ViewRect, Input.mousePosition)) return;
            if (Mathf.Abs(Input.mouseScrollDelta.y) > 1) return;
            ChangeResizeContent(CurrentScale + Input.mouseScrollDelta.y * ScrollSensitivity);
        }
#endif
        
    }

    public void ChangeResizeContent(float scale)
    {
        CurrentScale = Mathf.Clamp(scale, ScaleRange.m_Min, ScaleRange.m_Max);
        ResizeContent.localScale = Vector3.one * CurrentScale;
            
        Vector2 newContentSize = new Vector2(
            Mathf.Max(ResizeContent.sizeDelta.x * CurrentScale, DefaultContentSize.x),
            Mathf.Max(ResizeContent.sizeDelta.y * CurrentScale, DefaultContentSize.y));
        Content.sizeDelta = newContentSize;
    }
    public void ChangeResizeContent(Vector2 size)
    {
        ResizeContent.sizeDelta = size;
            
        Vector2 newContentSize = new Vector2(
            Mathf.Max(ResizeContent.sizeDelta.x * CurrentScale, DefaultContentSize.x),
            Mathf.Max(ResizeContent.sizeDelta.y * CurrentScale, DefaultContentSize.y));
        Content.sizeDelta = newContentSize;
    }
    private void UpdatePrevData()
    {
        m_PrevPosition = Content == null ? Vector2.zero : Content.anchoredPosition;
        m_PrevViewBounds = m_ViewBounds;
        m_PrevContentBounds = m_ContentBounds;
    }

    public Vector2 NormalizedPosition
    {
        get => new(HorizontalNormalizedPosition, VerticalNormalizedPosition);
        set
        {
            SetNormalizedPosition(value.x, 0);
            SetNormalizedPosition(value.y, 1);
        }
    }
    public float HorizontalNormalizedPosition
    {
        get
        {
            UpdateBounds();
            if ((m_ContentBounds.size.x <= m_ViewBounds.size.x) ||
                Mathf.Approximately(m_ContentBounds.size.x, m_ViewBounds.size.x))
                return (m_ViewBounds.min.x > m_ContentBounds.min.x) ? 1 : 0;
            return (m_ViewBounds.min.x - m_ContentBounds.min.x) / (m_ContentBounds.size.x - m_ViewBounds.size.x);
        }
        set => SetNormalizedPosition(value, 0);
    }
    public float VerticalNormalizedPosition
    {
        get
        {
            UpdateBounds();
            if ((m_ContentBounds.size.y <= m_ViewBounds.size.y) ||
                Mathf.Approximately(m_ContentBounds.size.y, m_ViewBounds.size.y))
                return (m_ViewBounds.min.y > m_ContentBounds.min.y) ? 1 : 0;

            return (m_ViewBounds.min.y - m_ContentBounds.min.y) / (m_ContentBounds.size.y - m_ViewBounds.size.y);
        }
        set => SetNormalizedPosition(value, 1);
    }

    protected virtual void SetNormalizedPosition(float value, int axis)
    {
        EnsureLayoutHasRebuilt();
        UpdateBounds();
        float hiddenLength = m_ContentBounds.size[axis] - m_ViewBounds.size[axis];
        float contentBoundsMinPosition = m_ViewBounds.min[axis] - value * hiddenLength;
        float newAnchoredPosition =
            Content.anchoredPosition[axis] + contentBoundsMinPosition - m_ContentBounds.min[axis];

        Vector3 anchoredPosition = Content.anchoredPosition;
        if (Mathf.Abs(anchoredPosition[axis] - newAnchoredPosition) > 0.01f)
        {
            anchoredPosition[axis] = newAnchoredPosition;
            Content.anchoredPosition = anchoredPosition;
            m_Velocity[axis] = 0;
            UpdateBounds();
        }
    }

    private static float RubberDelta(float overStretching, float viewSize)
    {
        return (1 - (1 / ((Mathf.Abs(overStretching) * 0.55f / viewSize) + 1))) * viewSize * Mathf.Sign(overStretching);
    }

    protected override void OnRectTransformDimensionsChange()
    {
        SetDirty();
    }

    public virtual void CalculateLayoutInputHorizontal()
    {
    }

    public virtual void CalculateLayoutInputVertical()
    {
    }

    public virtual float minWidth => -1;
    public virtual float preferredWidth => -1;
    public virtual float flexibleWidth => -1;
    public virtual float minHeight => -1;
    public virtual float preferredHeight => -1;
    public virtual float flexibleHeight => -1;
    public virtual int layoutPriority => -1;

    public virtual void SetLayoutHorizontal()
    {
        m_Tracker.Clear();
    }

    public virtual void SetLayoutVertical()
    {
        m_ViewBounds = new Bounds(ViewRect.rect.center, ViewRect.rect.size);
        m_ContentBounds = GetBounds();
    }

    private void UpdateBounds()
    {
        m_ViewBounds = new Bounds(ViewRect.rect.center, ViewRect.rect.size);
        m_ContentBounds = GetBounds();

        if (Content == null) return;

        Vector3 contentSize = m_ContentBounds.size;
        Vector3 contentPos = m_ContentBounds.center;
        Vector2 contentPivot = Content.pivot;
        AdjustBounds(ref m_ViewBounds, ref contentPivot, ref contentSize, ref contentPos);
        m_ContentBounds.size = contentSize;
        m_ContentBounds.center = contentPos;
    }

    private static void AdjustBounds(ref Bounds viewBounds, ref Vector2 contentPivot, ref Vector3 contentSize,
        ref Vector3 contentPos)
    {
        Vector3 excess = viewBounds.size - contentSize;
        if (excess.x > 0)
        {
            contentPos.x -= excess.x * (contentPivot.x - 0.5f);
            contentSize.x = viewBounds.size.x;
        }

        if (excess.y > 0)
        {
            contentPos.y -= excess.y * (contentPivot.y - 0.5f);
            contentSize.y = viewBounds.size.y;
        }
    }

    private readonly Vector3[] m_Corners = new Vector3[4];

    private Bounds GetBounds()
    {
        if (Content == null) return new Bounds();
        Content.GetWorldCorners(m_Corners);
        var viewWorldToLocalMatrix = ViewRect.worldToLocalMatrix;
        return InternalGetBounds(m_Corners, ref viewWorldToLocalMatrix);
    }

    private static Bounds InternalGetBounds(Vector3[] corners, ref Matrix4x4 viewWorldToLocalMatrix)
    {
        var vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        var vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        for (int j = 0; j < 4; j++)
        {
            Vector3 v = viewWorldToLocalMatrix.MultiplyPoint3x4(corners[j]);
            vMin = Vector3.Min(v, vMin);
            vMax = Vector3.Max(v, vMax);
        }

        var bounds = new Bounds(vMin, Vector3.zero);
        bounds.Encapsulate(vMax);
        return bounds;
    }

    private Vector2 CalculateOffset(Vector2 delta)
    {
        return InternalCalculateOffset(ref m_ViewBounds, ref m_ContentBounds, Horizontal, Vertical, ref delta);
    }

    private static Vector2 InternalCalculateOffset(ref Bounds viewBounds, ref Bounds contentBounds, bool horizontal,
        bool vertical, ref Vector2 delta)
    {
        Vector2 offset = Vector2.zero;
        Vector2 min = contentBounds.min;
        Vector2 max = contentBounds.max;
        if (horizontal)
        {
            min.x += delta.x;
            max.x += delta.x;

            float maxOffset = viewBounds.max.x - max.x;
            float minOffset = viewBounds.min.x - min.x;

            if (minOffset < -0.001f)
                offset.x = minOffset;
            else if (maxOffset > 0.001f)
                offset.x = maxOffset;
        }

        if (vertical)
        {
            min.y += delta.y;
            max.y += delta.y;

            float maxOffset = viewBounds.max.y - max.y;
            float minOffset = viewBounds.min.y - min.y;

            if (maxOffset > 0.001f)
                offset.y = maxOffset;
            else if (minOffset < -0.001f)
                offset.y = minOffset;
        }

        return offset;
    }

    private void SetDirty()
    {
        if (!IsActive())
            return;

        LayoutRebuilder.MarkLayoutForRebuild(RectTransform);
    }

    private void SetDirtyCaching()
    {
        if (!IsActive())
            return;

        CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
        LayoutRebuilder.MarkLayoutForRebuild(RectTransform);

        m_ViewRect = null;
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        SetDirtyCaching();
    }
#endif
    public void OnPointerDown(PointerEventData eventData)
    {
        if (Input.touchCount == 2)
        {
            m_IsZooming = true;
            m_InitialTouchDistance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (Input.touchCount < 2)
        {
            m_IsZooming = false;
        }
    }
}