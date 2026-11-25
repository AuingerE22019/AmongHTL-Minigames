using UnityEngine;

/// <summary>
/// Defines the types of cables that can be sorted in the minigame.
/// Each type has a corresponding image for visual identification.
/// </summary>
public enum CableType
{
    HDMI,
    USBC,
    USBA,
    LAN
}

/// <summary>
/// Provides editor-friendly settings for each cable type.
/// </summary>
[CreateAssetMenu(fileName = "NewCableTypeSettings", menuName = "CableSorting/CableTypeSettings")]
public class CableTypeSettings : ScriptableObject
{
    public CableType type;
    public Sprite icon;
    public string label;
}

/// <summary>
/// Extension methods for CableType to provide labels.
/// </summary>
public static class CableTypeExtensions
{
    public static string ToLabel(this CableType type)
    {
        switch (type)
        {
            case CableType.HDMI:  return "HDMI";
            case CableType.USBC:  return "USB-C";
            case CableType.USBA:  return "USB-A";
            case CableType.LAN:   return "LAN";
            default:              return "Unknown";
        }
    }
}
