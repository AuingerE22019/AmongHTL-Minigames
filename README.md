# AmongHTL Minigames

Domain: amonghtl-dpa.swedencentral.cloudapp.azure.com

Port: 7777 (im Schulnetz 443)

Ein Minigame ist eine kleine Aufgabe, die der Spieler im Spiel in einem Fenster löst. Unity-Version: **6000.6.4f1**.

<!-- MINIGAMES-LISTE:START -->

## Status

Stand: Build 2026-11-18.4

🆕 **neu** = seit dem letzten Build (18.11.2026 13:00) dazugekommen, ✏️ **bearbeitet** = seitdem vom Autor oder von uns geändert (neue Fassung, neue Texte).

| Nr. | Typ | Minigame | Autor | Stand | Markierung |
|---|---|---|---|---|---|
| 1 | Schüler | PC zusammenbauen | Jonas Hattinger | im Spiel (seit Build 2026-10-20) |  |
| 2 | Schüler | Tafel löschen | L. Klausner | im Spiel (seit Build 2026-11-01) |  |
| 3 | Schüler | **Klassenraum aufräumen** | Florian Schlager | im Spiel (seit Build 2026-11-01) | ✏️ **bearbeitet** |
| 4 | Schüler | Beamer-Fernbedienung reparieren | Tobias Hofinger | im Spiel (seit Build 2026-11-01) |  |
| 5 | Schüler | Uhr reparieren | Samuel Krempl | im Spiel (seit Build 2026-11-18.2) |  |
| 6 | Schüler | **Deckenplatten reparieren** | Tobias Rebenda | im Spiel (seit Build 2026-11-18.4) | 🆕 **neu** |
| 7 | Schüler | Kabel sortieren | Anel Hrvacic | im Spiel (seit Build 2026-10-14) |  |
| 8 | Schüler | Switch konfiguriern | Christian Berec | im Spiel (seit Build 2026-10-20) |  |
| 9 | Schüler | Hausschuhe anziehen | Jonas Sternbauer | im Spiel (seit Build 2026-10-20) |  |
| 10 | Schüler | Müll trennen | Julian Allerstorfer | im Spiel (seit Build 2026-11-18) |  |
| 11 | Schüler | Schaltung verdrahten | David Hehenberger | im Spiel (seit Build 2026-11-18) |  |
| 12 | Schüler | Binärzahl bauen | Jakob Kaltenböck | im Spiel (seit Build 2026-10-14) |  |
| 1 | Lauser | PC sabotieren | Jonas Hattinger | im Spiel (seit Build 2026-11-01) |  |
| 2 | Lauser | Tafel beschmieren | L. Klausner | im Spiel (seit Build 2026-11-18) |  |
| 3 | Lauser | **Klassenraum verwüsten** | Florian Schlager | im Spiel (seit Build 2026-11-18.4) | 🆕 **neu** |
| 4 | Lauser | Beamer sabotieren | Tobias Hofinger | im Spiel (seit Build 2026-11-01) |  |
| 5 | Lauser | Uhren verstellen | Samuel Krempl | im Spiel (seit Build 2026-11-01) |  |
| 6 | Lauser | Deckenplatten beschädigen | Tobias Rebenda | im Spiel (seit Build 2026-11-01) |  |
| 7 | Lauser | Unterschrift fälschen | Jonas Doppelbauer | im Spiel (seit Build 2026-11-18) |  |
| 8 | Lauser | Handyregal entsorgen | Elias Redl | im Spiel (seit Build 2026-11-18.2) |  |

Die Nummern stehen im Spiel im Kasten und im Fenstertitel. Schüler (weißer Kasten) und Lauser (roter Kasten) zählen getrennt ab 1. Alle Änderungen stehen in [MINIGAMES-HISTORIE.md](MINIGAMES-HISTORIE.md). Fertige Spiel-Builds (Windows, macOS, Linux) gibt es unter [Releases](https://github.com/AuingerE22019/AmongHTL-Minigames/releases).

<!-- MINIGAMES-LISTE:END -->

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
