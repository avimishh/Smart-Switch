#define BUFFER_SIZE 256

#include <ESP8266WiFi.h>
#include <WiFiClient.h>
#include <ESP8266WebServer.h>
#include <WebSocketClient.h> // from https://github.com/morrissinger/ESP8266-Websocket
#include <FS.h>   // Include the SPIFFS library

ESP8266WebServer server(80);    // Create a webserver object that listens for HTTP request on port 80

bool shouldBlink = false; // indicates whether the status led should blink
unsigned long blinked = 0; // the time from the last blink of the status led [ms]
const int statusLed = D1;

const int touchSensor = D6;
bool touched = false; // for loop logic
unsigned long touchedAt; // the time (from the moment the program started running) the sensor was touched [ms]
const int pressDuration = 5000; // time from the moment a user touches the touch sensor until its action is activated [ms]

const int mvPerAmp = 66;

char serverIP[] = "192.168.1.20";
const int webSocketsPort = 8181;
WebSocketClient webSocketClient;
WiFiClient client; // Use WiFiClient class to create TCP connections
bool readyToConnectToWebsocketsServer = false;

unsigned long handshakeSuccessAt = 0;
bool duringConnectionTrial = false;
const int waitBeforeTryingAgain = 15000;

bool readyToSendSamples = false;
long lastSample = 0;
const int sampleEvery = 60000; // ms

const int load = D2; // the electrical device switch

char owner[BUFFER_SIZE]; // SmartSwitch username of the device owner
String ownerString;
const String ownerFilePath = "/owner.txt";

void setup() {
  Serial.begin(115200);         // Start the Serial communication to send messages to the computer

  pinMode(statusLed, OUTPUT);

  pinMode(D5, OUTPUT); // VCC for the touch sensor
  pinMode(touchSensor, INPUT);

  pinMode(load, OUTPUT);

  pinMode(A0, INPUT);
  digitalWrite(D5, HIGH); // VCC for the touch sensor

  Serial.println("\nBegin");

  SPIFFS.begin();                           // Start the SPI Flash Files System

  int waiting;
  for (waiting = 0; WiFi.status() != WL_CONNECTED; ++waiting) {
    delay(500);
    Serial.print(".");
    if (waiting > 20) {
      WiFi.mode(WIFI_OFF);
      break;
    }
  }

  Serial.println();

  if (waiting > 20) Serial.println("not connected");
  else {
    Serial.println("Connected");
    Serial.print("IP address: ");
    Serial.println(WiFi.localIP());
    readyToConnectToWebsocketsServer = SPIFFS.exists(ownerFilePath);
    if (readyToConnectToWebsocketsServer) {
      // read owner username
      File ownerTxt = SPIFFS.open(ownerFilePath, "r");
      ownerString = ownerTxt.readStringUntil('\n');
      ownerString.trim();
      ownerTxt.close();
      Serial.print("read: ");
      Serial.println(ownerString);
      connectToWebSocketsServer();
    }
  }

  server.onNotFound([]() {                              // If the client requests any URI
    if (!handleFileRead(server.uri()))                  // send it if it exists
      server.send(404, "text/plain", "404: Not Found"); // otherwise, respond with a 404 (Not Found) error
  });

  server.on("/", HTTP_POST, handlePost);

  server.begin();                           // Actually start the server
  Serial.println("HTTP server started");
}

void loop() {
  server.handleClient();
  handleWebSocketsLoop();
  if (shouldBlink && (millis() - blinked) >= 1000) {
    blinked = millis();
    digitalWrite(statusLed, !digitalRead(statusLed));
  }

  if (digitalRead(touchSensor)) {
    Serial.println(millis());
    if (touched && (millis() - touchedAt) >= pressDuration) {
      startAP();
      touched = false;
    }
    else if (!touched) {
      touched = true;
      touchedAt = millis();
      digitalWrite(statusLed, HIGH);
    }
  } else {
    touched = false;
    if (!shouldBlink) digitalWrite(statusLed, LOW);
  }
  //Serial.println("L123");
  if (duringConnectionTrial && (millis() - handshakeSuccessAt) >= waitBeforeTryingAgain) connectToWebSocketsServer();

  delay(0);
  //Serial.println(String(getCurrent(), 3) + " [A]");
}

void turnLoad(String state) {
  if (state == "on") digitalWrite(load, HIGH);
  else if (state == "off") digitalWrite(load, LOW);
}

double getVoltage() {
  return 220; // [V]
}

double getCurrent() {
  if (!digitalRead(load)) return 0;
  
  double Voltage = 0;
  double VRMS = 0;
  double AmpsRMS = 0;

  int readValue;             //value read from the sensor
  int maxValue = 0;          // store max value here
  int minValue = 1024;          // store min value here

  uint32_t start_time = millis();
  while ((millis() - start_time) < 1000) //sample for 1 Sec
  {
    readValue = analogRead(A0);

    // see if you have a new maxValue
    if (readValue > maxValue)
    {
      /*record the maximum sensor value*/
      maxValue = readValue;
    }
    if (readValue < minValue)
    {
      /*record the minimum sensor value*/
      minValue = readValue;
    }
  }
  

  // Subtract min from max
  Voltage = ((maxValue - minValue - 5.6) * 3.3) / 1024.0; // adjusting value because of wrong measurments

  VRMS = (Voltage / 2.0) * 0.707;
  AmpsRMS = (VRMS * 1000) / mvPerAmp;

  Serial.println("sent " + String(AmpsRMS < 0 ? 0 : AmpsRMS));
  return AmpsRMS < 0 ? 0 : AmpsRMS;
}

void startAP() {
  WiFi.softAP("SmartSwitch " + WiFi.macAddress());
  WiFi.mode(WIFI_AP_STA);
  shouldBlink = true;
}

bool connectToWebSocketsServer() {
  if (!readyToConnectToWebsocketsServer) {
    return false;
  }

  if (client.connect(serverIP, webSocketsPort)) {
    Serial.println("Connected websockets client");
  } else {
    Serial.println("Connection failed. (websockets)");
    readyToSendSamples = false;
    return false;
  }

  // Handshake with the server
  webSocketClient.path = "/";
  webSocketClient.host = serverIP;
  if (webSocketClient.handshake(client)) {
    Serial.println("Handshake successful (websockets)");
    handshakeSuccessAt = millis();
    duringConnectionTrial = true;
  } else {
    Serial.println("Handshake failed. (websockets)");
    return false;
  }

  return true;
}

bool handleWebSocketsLoop() {
  if (client.connected()) {
    String data;
    webSocketClient.getData(data);

    if (data.length() > 0) {
      duringConnectionTrial = false;
      Serial.print("Received data: ");
      Serial.println(data);
      if (data == "turn-load-on") turnLoad("on");
      else if (data == "turn-load-off") turnLoad("off");
      else if (data == "who-are-you") {
        webSocketClient.sendData("i-am " + WiFi.macAddress() + " " + ownerString);
        Serial.println("Sending data: i-am " + WiFi.macAddress() + " " + ownerString);
      }
      else if (data == "are-you-on") {
        webSocketClient.sendData(digitalRead(load) ? "on yes" : "on no");
        Serial.print("Sending data: ");
        Serial.println(digitalRead(load) ? "on yes" : "on no");
        readyToSendSamples = true; // after the server knows who the owner is and if the device is on we can send samples
      }
    }

    if (readyToSendSamples && millis() - lastSample > sampleEvery) {
      lastSample = millis();
      webSocketClient.sendData("sample " + String(getVoltage(), 2) + " " + String(getCurrent(), 3));
    }

  } else {
    return connectToWebSocketsServer();
  }

  return true;
}

String getContentType(String filename) { // convert the file extension to the MIME type
  if (filename.endsWith(".html")) return "text/html";
  else if (filename.endsWith(".css")) return "text/css";
  else if (filename.endsWith(".js")) return "application/javascript";
  else if (filename.endsWith(".ico")) return "image/x-icon";
  return "text/plain";
}

bool handleFileRead(String path) { // send the right file to the client (if it exists)
  Serial.println("handleFileRead: " + path);
  if (path.endsWith("/")) path += "index.html";         // If a folder is requested, send the index file
  String contentType = getContentType(path);            // Get the MIME type
  if (SPIFFS.exists(path)) {                            // If the file exists
    File file = SPIFFS.open(path, "r");                 // Open it
    size_t sent = server.streamFile(file, contentType); // And send it to the client
    file.close();                                       // Then close the file again
    return true;
  }
  Serial.println("\tFile Not Found");
  return false;                                         // If the file doesn't exist, return false
}

void handlePost() {
  server.sendHeader("Access-Control-Allow-Origin", "*");

  // change load state
  if (server.hasArg("turn-load")) {
    turnLoad(server.arg("turn-load"));
    Serial.print("recieved: ");
    Serial.println(server.arg("turn-load"));
    server.send(200, "text/plain", "ok");
  }

  // query server for available wifi
  if (server.hasArg("give-wifi-networks")) {
    readyToConnectToWebsocketsServer = false;
    Serial.println("giving wifi networks (scanning)");
    int networksAmount = WiFi.scanNetworks();
    String str = "[\"";
    for (int i = 0; i < networksAmount; ++i) {
      str += "{\\\"name\\\":\\\"" + WiFi.SSID(i) + "\\\"";
      str += ",";
      str += "\\\"strength\\\":";
      str += WiFi.RSSI(i);
      str += ",";
      str += "\\\"hasEncryption\\\":";
      str += (WiFi.encryptionType(i) != ENC_TYPE_NONE);
      str += "}\"";
      if (i != networksAmount - 1) str += ",\"";
    }
    str += "]";
    server.send(200, "application/json", str);
    Serial.print("Sending to ");
    Serial.println(server.client().remoteIP());
  }

  // tell the device to connect to a given wifi
  if (server.hasArg("connect-to-network")) {
    WiFi.mode(WIFI_AP_STA);
    Serial.print("received ssid: ");
    Serial.println(server.arg("ssid"));

    digitalWrite(statusLed, HIGH);

    if (server.hasArg("pass")) {
      Serial.print("received pass: ");
      Serial.println(server.arg("pass"));
      WiFi.begin(server.arg("ssid"), server.arg("pass"));
    } else {
      WiFi.begin(server.arg("ssid"));
    }
    Serial.println("Connecting");

    for (int waiting = 0; WiFi.status() != WL_CONNECTED; ++waiting) {
      delay(500);
      Serial.print(".");
      if (waiting > 20) return;
    }

    shouldBlink = false;
    digitalWrite(statusLed, LOW);

    Serial.println("");
    Serial.println("Connected");
    Serial.print("IP address: ");
    Serial.println(WiFi.localIP());
  }

  // checking if the server is connected to wifi
  if (server.hasArg("are-you-connected")) server.send(200, "text/plain", (WiFi.status() == WL_CONNECTED) ? "yes" : "no");

  if (server.hasArg("username-given")) {
    // username is server.arg("username")
    Serial.println("received username: " + server.arg("username"));
    ownerString = server.arg("username");
    WiFi.mode(WIFI_STA);
    readyToConnectToWebsocketsServer = true;
    File ownerTxt = SPIFFS.open(ownerFilePath, "w+");
    ownerTxt.println(ownerString);
    ownerTxt.close();
  }

  if (server.hasArg("give-load-state")) {
    server.send(200, "text/plain", digitalRead(load) ? "on" : "off");
  }
}
