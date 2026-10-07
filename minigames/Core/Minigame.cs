using System;
using UnityEngine;

/// <summary>
/// Schwierigkeit eines Minigames. Bestimmt die Notenverbesserung.
/// </summary>
public enum MinigameDifficulty
{
    Leicht,
    Mittel,
    Schwer,
}

/// <summary>
/// Notenverbesserung pro Schwierigkeit (in Notenstufen, z. B. 0.5 = eine halbe Note besser).
/// Nur HIER ändern, dann gilt es für alle Minigames gleich.
/// </summary>
public static class MinigameRewards
{
    public const float Leicht = 0.25f;
    public const float Mittel = 0.5f;
    public const float Schwer = 1.0f;

    public static float GetGradeImprovement(MinigameDifficulty difficulty)
    {
        switch (difficulty)
        {
            case MinigameDifficulty.Leicht: return Leicht;
            case MinigameDifficulty.Mittel: return Mittel;
            case MinigameDifficulty.Schwer: return Schwer;
            default: return 0f;
        }
    }
}

/// <summary>
/// Basisklasse für JEDES Minigame. Das Script auf dem Root-Objekt des Prefabs erbt davon.
/// Wenn die Aufgabe ganz geschafft ist, ruft das Minigame <see cref="Complete"/> auf.
/// Das Spiel hört auf <see cref="Completed"/> und liest dort die Notenverbesserung aus.
/// </summary>
public abstract class Minigame : MonoBehaviour
{
    [Header("Minigame Info")]
    [SerializeField] private string minigameName = "Neues Minigame";
    [SerializeField, TextArea] private string description;
    [SerializeField] private MinigameDifficulty difficulty = MinigameDifficulty.Leicht;

    public string MinigameName => minigameName;
    public string Description => description;
    public MinigameDifficulty Difficulty => difficulty;

    /// <summary>Notenverbesserung für dieses Minigame, abhängig von der Schwierigkeit.</summary>
    public float GradeImprovement => MinigameRewards.GetGradeImprovement(difficulty);

    /// <summary>
    /// Wird genau einmal ausgelöst, wenn die Aufgabe geschafft ist.
    /// Das Spiel hängt sich hier ein (für alle Minigames gleich).
    /// </summary>
    public event Action<Minigame> Completed;

    private bool _completed;

    /// <summary>
    /// Im Minigame aufrufen, sobald die Aufgabe VOLLSTÄNDIG geschafft ist. Sonst nie.
    /// Mehrfaches Aufrufen schadet nicht, es zählt nur der erste Aufruf.
    /// </summary>
    protected void Complete()
    {
        if (_completed) return;
        _completed = true;

        Completed?.Invoke(this);
    }
}
