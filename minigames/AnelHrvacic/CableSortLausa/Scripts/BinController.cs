using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents a single bin that accepts a specific cable type.
/// Handles visual feedback when a cable is dragged over it.
/// </summary>
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class BinController : MonoBehaviour
{
    [Header("Bin Settings")]
    [SerializeField] private CableType acceptedCableType;

    [SerializeField] private Sprite portSprite;

    /// <summary>
    /// Gets the accepted cable type for this bin.
    /// </summary>
    public CableType AcceptedCableType => acceptedCableType;

    private void Awake()
    {
        UpdateVisuals();
    }

    /// <summary>
    /// Sets the accepted cable type programmatically (for Editor setup).
    /// </summary>
    public void SetAcceptedCableType(CableType type)
    {
        acceptedCableType = type;
    }

    /// <summary>
    /// Sets the port image for this bin.
    /// </summary>
    public void SetPortSprite(Sprite sprite)
    {
        portSprite = sprite;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        Image image = GetComponent<Image>();
        image.sprite = portSprite;
        image.preserveAspect = true;
    }
}
