# AmongHTL Minigames

Hier geben wir Minigames (Tasks) für **Among HTL** ab. Ein Minigame ist eine kleine Aufgabe, die der Spieler im Spiel in einem Fenster löst. Unity-Version: **6000.6.4f1**.

## Regeln

- Abgegeben wird ein **Unity-Paket (`.unitypackage`)**. Lose Dateien oder Ordner werden **nicht** eingebaut.
- Das Paket muss **vollständig** sein: Prefab, Scripts, Grafiken und Sounds. Alles, was dein Minigame braucht, muss drin sein (auch Dateien aus `Resources`-Ordnern).
- Ein Minigame ist **ein UI-Prefab**. Das Seitenverhältnis soll **ungefähr Querformat** sein (etwa 3:2). Die genaue Größe in Pixeln ist egal, das Spiel skaliert dein Minigame automatisch in das Fenster.
- Das Minigame ändert **nichts am restlichen Spiel**: keine eigene Kamera, kein eigenes `EventSystem`, keine Szene laden, kein `Time.timeScale`.
- Eingabe über **UI-Ereignisse** (`Button.onClick`, `IPointerClickHandler`, `IDragHandler` ...) oder das **neue Input System**. Das alte `Input.GetKey` / `Input.mousePosition` gibt im Spiel Fehler.
- **Eigener `namespace`** (z. B. `Minigames.DeinName`), damit sich Klassennamen nicht mit anderen Minigames überschneiden.
- **Man kann nicht verlieren.** Es gibt kein Game Over und keinen eigenen Schließen-Button.
- Das Minigame wird oft wiederholt: Bei jedem Start soll es **neu** sein (Zufall, nichts merken).
- Nur eigene oder frei verwendbare Bilder und Sounds.

Du musst **nicht** von `Minigame` erben. Schreib stattdessen in den Pull Request, **woran man erkennt, dass die Aufgabe geschafft ist** (dein Event oder deine Methode). Den Anschluss ans Spiel baut das Team. Der Ordner `minigames/Core` gehört dem Team, du brauchst ihn nicht.

Schwierigkeit: **Leicht, Mittel oder Schwer**. Je schwerer, desto mehr Notenverbesserung.

## Abgeben

1. Repo **forken** und klonen.
2. Ordner **`minigames/vorname-nachname`** anlegen (klein, ohne Umlaute und Leerzeichen).
3. In Unity auf deinen Ordner rechtsklicken, **Export Package** wählen und **nur dein Minigame** auswählen (keine Testszenen, keine Editor-Skripte, keine Vorlagen-Dateien). Die `.unitypackage`-Datei kommt in deinen Ordner.
4. Committen, pushen und einen **Pull Request** öffnen. Schreib dazu: Name, Beschreibung (ein Satz), Schwierigkeit und woran man erkennt, dass die Aufgabe geschafft ist.

## Checkliste

- [ ] Ordner `minigames/vorname-nachname` mit der `.unitypackage`
- [ ] Paket vollständig (Grafiken und Sounds drin, nichts fehlt)
- [ ] Seitenverhältnis ungefähr Querformat (etwa 3:2)
- [ ] Keine eigene Kamera, kein `EventSystem`, eigener `namespace`
- [ ] Nicht verlierbar, jeder Start ist neu
- [ ] Mehrmals getestet, keine Fehler in der Konsole
- [ ] Name, Beschreibung, Schwierigkeit und „Wann ist es geschafft?" im Pull-Request-Text

## Hosting & Joining

Um einen Server zu hosten oder beizutreten, gib im Hauptmenü die Domain "jakob.tobias.hofinga.at" mit dem Port 7777 ein.

Fertige Spiel-Builds (Windows, macOS, Linux) stehen unter **Releases**.

Fragen? Elias Auinger oder Jakob Kaltenböck.
