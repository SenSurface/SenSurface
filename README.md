# SenSurface

This is the software for SenSurface. You need to download the entire folder and open it with Unity 2020.3.25
The main scene is named Sensurface; it is the interface software. Do not compile it. It works in debug mode when you launch the scene.

## Setup

Once you open the unity software, you must upload the firmware software on your SenSurface boards. Here is the process :
1. Download *[Arduino IDE](https://www.arduino.cc/)*
2. Incorporate the following libraries :
  - FastLED
  - *[Muca](https://github.com/muca-board/Muca)*
  - *[Uduino](https://github.com/marcteys/Uduino)* 
3. Incorporate the board libraries of ESP8266 from Adafruit and use the WEMOS D1 mini board.
4. Launch the firmware code with Arduino. path : /Assets/Firmware/Sensurface_Firmware/Sensurface_Firmware.ino
5. Upload the firmware on the wemos D1 board. If you are using multiple interfaces, modify the name of your object between each board (one of the first lines of the firmware script).
6. Launch the unity scene. The surface will appear using serial connection.
7. For wifi connection, you need to modify the commented lines in the Arduino script setup function (to choose your wifi).
