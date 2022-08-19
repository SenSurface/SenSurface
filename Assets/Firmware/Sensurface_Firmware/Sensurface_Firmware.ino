// Uduino settings
//#include <Uduino_Wifi.h>
//Uduino_Wifi uduino("boardU"); // Declare and name your object


#include <Uduino.h>
Uduino uduino("U"); // Declare and name your object



#include <Muca.h>
Muca muca;
#define MAX_CALIBRATION_STEPS 5
short currentCalibrationStep = 0;
unsigned int calibrationGrid[NUM_RX * NUM_TX];

int numRx = 12;
int numTx = 12;


#include <FastLED.h>
#define NUM_LEDS 1
#define DATA_PIN 0
CRGB leds[NUM_LEDS];



void setup()
{
  FastLED.addLeds<WS2811, DATA_PIN, GRB>(leds, NUM_LEDS);  // GRB ordering is assumed
  FastLED.setBrightness(30);

  Serial.begin(115200);

  // delay(100);


  //  delay(100);
  // uduino.setPort(4223);

  // uduino.setConnectionTries(5);
  //if (uduino.connectWifi("PoleDeVinci_DVIC", "8PfURsp!dvic")) {
  /* if (uduino.connectWifi("SSID", "password")) {


     //  leds[0] = CRGB::Green;
     // FastLED.show();
     //delay(200);
    } else {
     //leds[0] = CRGB::Red;
     //FastLED.show();
     //delay(200);
    }
    // uduino.connectWifi("BIBOUBOX", "Melaniebibou");
    //uduino.connectWifi("SSID", "password");
    uduino.useSerial(true);
  */
  uduino.addCommand("c", calib);
  uduino.addCommand("r", softReset);
  uduino.addCommand("g", gain);
  uduino.addCommand("reso", setResolution);



  muca.skipLine(LINE_TX, (const short[]) {
    0, 1, 2, 3, 4, 5, 6, 7, 8
  }, 9);

  /*
     muca.skipLine(LINE_RX, (const short[]) {
      13, 14, 15, 16, 17, 18, 19, 20, 21
    }, 9);
  */
  muca.init(false);
  muca.useRawData(true); // If you use the raw data, the interrupt is not working
  currentCalibrationStep = 0;
}

void calib() {
  currentCalibrationStep = 0;
}

void softReset() {
 // ESP.restart();
}

void gain() {
  char * firstParameter = uduino.getParameter(0);
  int isGain = uduino.charToInt(firstParameter);

  muca.setGain(isGain);
  currentCalibrationStep = 0;
}


void setResolution() {
  char * firstParameter = uduino.getParameter(0);
  numRx = uduino.charToInt(firstParameter);

  char * secondParameter = uduino.getParameter(1);
  numTx = uduino.charToInt(secondParameter);
}

void loop()
{
  uduino.update();

  if (uduino.isConnected()) {

    leds[0] = CRGB::Blue;
    FastLED.show();
    // delay(100);

    GetCalibratedData();

  } else {

    leds[0] = CRGB::Red;
    FastLED.show();

  }
  uduino.delay(10);
}



void GetCalibratedData() {
  if (muca.updated()) {

    if (currentCalibrationStep < MAX_CALIBRATION_STEPS) {
      CalibrationStep();
      return;
    }
    int i = 0;
    char c[numRx * numTx * 2 + 1];

    for (int colY = numTx - 1; colY >= 0 ; colY--) { // col ( x )
      for (int rowX = 0; rowX < numRx; rowX++) { // rowX ( y )
        // int index = ( y * 12) +  x ;

        c[i] = 1;

        int index = ( ( colY ) * numRx ) + ( rowX );
        //    index = ( rowX * numTx ) +  colY;

        //   Serial.print(index);
        //   Serial.print(" ");
        //    index = i;

        index +=  ( 12 * 9 ); // skipped lines

        // int index = ( rowX * numTx ) +  (colY );//  + ( 12 * 9 ) ;

        int val = max(1, (int)muca.grid[index] - (int)calibrationGrid[index] + 20);
  
        if (val <= 1) val = 1;
        if (val >=  127) val = 127;
        if (val == 10) val = 9;
        if (val == 13) val = 12;

        c[i] = char(val);
        i++;
      }
    }
    c[i] = '\0';
    // Serial.println(i);

    // Serial.println(c);
    //    Serial.println();
    // uduino.println(i);
    uduino.println(c);
    // Serial.println();
    //  uduino.delay(1);

  } // End Muca Updated
}



void CalibrationStep() {
  Serial.println("Calib");
  for (int i = 0; i < NUM_RX * NUM_TX ; ++i) {
    if (currentCalibrationStep == 0) calibrationGrid[i] = muca.grid[i]; // Copy array
    else calibrationGrid[i] = (calibrationGrid[i] + muca.grid[i]) / 2 ; // Get average
  }
  currentCalibrationStep++;
}
