# AmongHTL Minigames

Sammlung der Minigames (Spieleraufgaben) für **Among HTL** — das Among-Us-artige Spiel der HTL Grieskirchen. Jede Aufgabe/jedes Minigame wird hier als eigenständiges Unity-Paket abgelegt und später ins Hauptspiel integriert.

## Wie du dein Minigame beisteuerst

1. **Fork** dieses Repo oder frag nach Schreibrechten (Issue aufmachen oder direkt melden).
2. Leg unter `minigames/` einen **eigenen Ordner** mit deinem Minigame-Namen an, z.B.:
   ```
   minigames/
     wiring-task/
     reaktortest/
     stundenplan-sortieren/
   ```
3. In deinem Ordner:
   - Dein **Unity-Package** (`.unitypackage`) oder exportierte Prefabs samt Abhängigkeiten (Scripts, Materials, Sprites).
   - Eine kurze `README.md` in deinem Ordner mit:
     - Kurzbeschreibung der Aufgabe (was macht der Spieler?)
     - Steuerung / Input
     - Erfolgsbedingung (wann ist die Aufgabe "geschafft"?)
     - Bekannte Einschränkungen / Abhängigkeiten (z.B. benötigte Unity-Version, Packages)
4. **Pull Request** aufmachen — wird von uns geprüft und ins Hauptspiel übernommen.

## Konventionen

- Ordnernamen: `kebab-case`, keine Leerzeichen/Sonderzeichen.
- Unity-Version im Minigame-README angeben.
- Keine großen Binärdateien (Videos, unkomprimierte Assets) direkt committen — wenn nötig, komprimieren oder Git LFS verwenden.
- Prefabs sollten möglichst unabhängig von Szenen-spezifischen Referenzen sein, damit sie sich sauber ins Hauptprojekt importieren lassen.

## Struktur

```
minigames/
  <dein-minigame-name>/
    README.md
    <unity package / prefabs / scripts>
```

## Status

Aktiv im Aufbau im Rahmen der Diplomarbeit "Among HTL" (HTL Grieskirchen).
