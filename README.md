# Etage6

## Installation
1. git clone https://github.com/HTW-Berlin-CAVE/Etage6.git
2. git clone https://github.com/HTW-Berlin-CAVE/cave-package.git
3. Ordner cave-package-development/Packages/de.htw.cave in Etage6/Etage6/Packages kopieren

## Steuerung
Bewegung:
* Linker Joycon Stick: Bewegen
* Rechter Joycon Stick: Rotation

Ball werfen (beide Controller):
* vordere Schultertaste: Laserpointer aktivieren
* hintere Schultertaste: Ball werfen

## Troubleshooting
1. Unity sagt, dass viele Scripts nicht geladen werden können. 
   
   A: Manchmal hat Unity Probleme damit .dll Dateien und/oder die Packages richtig zu laden, es hilft Unity neu zu starten.
   
2. Die Joycons funktionieren nicht richtig, ich kann mich zwar rotieren aber nicht bewegen. 

   A: Mouse Keyboard User Control deaktivieren. Es kann vorkommen, dass dieses Script den Wert des Joycon User Control Scripts überschreibt.
   
## Wichtige Scripts
### ShootController.cs (im de.htw.cave GameObject)
Regelt die Tasten des Joycons, die zum Werfen der Bälle genutzt werden. 
Beinhaltet ebenfalls die Berechnung der Stärke und Richtung, in die die Bälle geworfen werden.
### Enemy.cs (in den jeweiligen Enemies)
Beinhaltet die "KI" der verschiedenen Gegnertypen
### DoorManager.cs (im Tueren-GameObject - Model/Waende/Tueren)
Ist eine zentrale Stelle zum Einstellen verschiedener Parameter der Türen:
* Dauer in der sich die Türen öffnen
* Dauer, die die Türen offen bleiben
* Referenz auf den Spieler (zum Berechnen, ob der Spieler so nah ist, dass sich die Tür öffnen soll)
* Sounds, die beim Öffnen und Schließen der Türen abgespielt werden sollen.
### DoorController.cs
Beinhaltet die Logik, die prüft wie weit der Spieler weg ist, die die Türen öffnet und wieder schließt.
