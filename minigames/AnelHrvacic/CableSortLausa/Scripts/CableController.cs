using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Handles drag-and-drop on an individual cable element.
/// Implements Unity's EventSystem interfaces for mouse/touch support.
/// Designed for UI overlay (Canvas/RectTransform).
/// </summary>
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class CableController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Settings")]
    [SerializeField] private float returnDuration = 0.25f;

    [Header("Data")]
    [SerializeField] private CableType cableType;
    [SerializeField] private Sprite hdmiSprite;
    [SerializeField] private Sprite usbCSprite;
    [SerializeField] private Sprite usbASprite;
    [SerializeField] private Sprite lanSprite;

    private RectTransform rectTransform;
    private CableSortingManager manager;
    private Image cableImage;
    private RectTransform canvasRect;
    private Vector3 startPosition;
    private bool isReturning;

    public CableType CableType => cableType;
    public Vector3 StartPosition => startPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        manager = UnityEngine.Object.FindAnyObjectByType<CableSortingManager>();
        cableImage = GetComponent<Image>();
        canvasRect = FindCanvasRectTransform();
    }

    private void Start()
    {
        startPosition = rectTransform.position;
    }

    /// <summary>
    /// Sets the cable type and updates its visual appearance.
    /// </summary>
    public void SetType(CableType type)
    {
        cableType = type;
        UpdateVisuals();
    }

    /// <summary>
    /// Returns the current cable type.
    /// </summary>
    public CableType GetCableType()
    {
        return cableType;
    }

    private void UpdateVisuals()
    {
        if (cableImage != null)
        {
            switch (cableType)
            {
                case CableType.HDMI: cableImage.sprite = hdmiSprite; break;
                case CableType.USBC: cableImage.sprite = usbCSprite; break;
                case CableType.USBA: cableImage.sprite = usbASprite; break;
                case CableType.LAN: cableImage.sprite = lanSprite; break;
            }
            cableImage.preserveAspect = true;
        }
    }

    public void SetSprites(Sprite hdmi, Sprite usbC, Sprite usbA, Sprite lan)
    {
        hdmiSprite = hdmi;
        usbCSprite = usbC;
        usbASprite = usbA;
        lanSprite = lan;
    }

    /// <summary>
    /// Called when the player starts dragging the cable.
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isReturning || manager == null) return;

        startPosition = rectTransform.position;
        rectTransform.localScale = new Vector3(1.1f, 1.1f, 1f);
    }

    /// <summary>
    /// Called as the player drags the cable. Updates position to follow the pointer.
    /// Uses Canvas space for proper UI drag operations.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (isReturning) return;

        if (canvasRect != null)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                canvasRect, eventData.position, null, out Vector3 worldPoint);
            rectTransform.position = worldPoint;
        }
        else
        {
            // Fallback: no Canvas found — position follows screen point directly.
            // This only works correctly if the Canvas is ScreenSpaceOverlay at identity.
            rectTransform.position = eventData.position;
        }
    }

    /// <summary>
    /// Walks up the hierarchy to find the Canvas RectTransform.
    /// </summary>
    private RectTransform FindCanvasRectTransform()
    {
        Transform t = transform;
        while (t != null)
        {
            Canvas c = t.GetComponent<Canvas>();
            if (c != null)
                return t.GetComponent<RectTransform>();
            t = t.parent;
        }
        return null;
    }

    /// <summary>
    /// Called when the player releases the cable.
    /// If dropped on a valid bin, the manager handles it;
    /// if dropped inside the spawn area, updates start position;
    /// otherwise returns to start position.
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (isReturning) return;

        rectTransform.localScale = Vector3.one;

        if (manager == null)
        {
            ReturnToStart();
            return;
        }

        // Let the manager determine if the drop was on a valid bin
        if (manager.TryDropCable(gameObject, eventData))
        {
            // Drop was handled correctly
            return;
        }

        // Check if dropped inside spawn/table area
        if (manager.IsInSpawnArea(rectTransform.position))
        {
            // Cable stays at its new position on the table
            startPosition = rectTransform.position;
            return;
        }

        // No valid bin and outside spawn area - return cable to start position
        ReturnToStart();
    }

    /// <summary>
    /// Public entry point to trigger the return-to-start animation.
    /// </summary>
    public void ReturnToStart()
    {
        if (isReturning) return;
        StartCoroutine(ReturnToStartCoroutine());
    }

    /// <summary>
    /// Smoothly animates the cable back to its starting position with cubic ease-out.
    /// </summary>
    private System.Collections.IEnumerator ReturnToStartCoroutine()
    {
        isReturning = true;
        Vector3 start = rectTransform.position;
        Vector3 end = startPosition;
        float elapsed = 0f;

        while (elapsed < returnDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / returnDuration);
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            rectTransform.position = Vector3.Lerp(start, end, eased);
            yield return null;
        }

        rectTransform.position = end;
        isReturning = false;
    }
}
