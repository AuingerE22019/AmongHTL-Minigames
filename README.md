# AmongHTL Minigames

Hier geben wir Minigames (Tasks) für **Among HTL** ab. Unity-Version: **6000.4.1f1**.

## Regeln

- Ein Minigame ist **ein UI-Prefab** mit einem Script, das von `Minigame` erbt (`minigames/Core/Minigame.cs`, nicht ändern).
- Passt in ein Fenster mit **600 × 400**.
- **Man kann nicht verlieren.** Es gibt kein Game Over und keinen eigenen Schließen-Button.
- Rufe **`Complete()`** nur auf, wenn die Aufgabe **ganz geschafft** ist.
- Das Minigame wird oft wiederholt: Bei jedem Start soll es **neu** sein (Zufall, nichts merken).
- Stell im Inspector **Name, Beschreibung und Schwierigkeit** ein.

Schwierigkeit: **Leicht, Mittel oder Schwer**. Je schwerer, desto mehr Notenverbesserung (steht in `Minigame.cs`).

## Beispiel

```csharp
public class KaffeeHolen : Minigame
{
    void Update()
    {
        if (AllesGeschafft()) Complete();
    }
}
```

## Abgeben

1. Repo **forken** und klonen.
2. Ordner **`minigames/vorname-nachname`** anlegen (klein, ohne Umlaute und Leerzeichen). Alle deine Prefabs kommen da hinein.
3. Committen, pushen und einen **Pull Request** öffnen. Schreib dazu Name und Schwierigkeit jedes Minigames.

## Checkliste

- [ ] Ordner `minigames/vorname-nachname`
- [ ] Script erbt von `Minigame`
- [ ] Nicht verlierbar, `Complete()` nur bei vollem Erfolg
- [ ] Jeder Start ist neu
- [ ] Name, Beschreibung, Schwierigkeit eingetragen
- [ ] Mehrmals getestet, keine Fehler in der Konsole
- [ ] Nur eigene oder frei verwendbare Bilder und Sounds

Fragen? Elias Auinger oder Jakob Kaltenböck.
