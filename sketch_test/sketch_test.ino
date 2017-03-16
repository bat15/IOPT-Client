#include <NewPing.h>
#include <ESP8266WiFi.h>

#define TRIGGER_PIN  2
#define ECHO_PIN     5
#define MAX_DISTANCE 200
#define xPin A0
#define buttonPin 5

int xPosition = 0;
int buttonState = 0;

//const char* ssid     = "iksnw";
//const char* password = "stubarevdenis";
//
//const char* host = "192.168.1.102";
//int port=1025;
const char* ssid     = "sleidor";
const char* password = "56a22b34i";

const char* host = "193.32.20.242";
int port=8080;

String url_led=String("/IOPT-Server-Prod-1/models/TestModel/TestObject/LED?user=iks");
String url_button=String("/IOPT-Server-Prod-1/models/TestModel/TestObject/ButtonState?user=iks");
String url_xcontroller=String("/IOPT-Server-Prod-1/models/TestModel/TestObject/XPosition?user=iks");

int value = 0;
unsigned long timestamp = 0;

NewPing sonar(TRIGGER_PIN, ECHO_PIN, MAX_DISTANCE);

void setup() {
  Serial.begin(115200);
  delay(10);
  pinMode(14, OUTPUT);
  Serial.print("Connecting to ");
  Serial.println(ssid);
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("");
  Serial.println("WiFi connected");
  Serial.println("IP address: ");
  Serial.println(WiFi.localIP());

  pinMode(xPin, INPUT);
  pinMode(buttonPin, INPUT_PULLUP); 
}
unsigned long times = 0;
void loop() {  
  if(millis() - times > 500)
  {
  xPosition = analogRead(xPin);
  buttonState = digitalRead(buttonPin);
  
  Serial.print("X: ");
  Serial.print(xPosition);
  Serial.print(" | Button: ");
  Serial.println(buttonState);
  String bs=buttonState==0?String("true"):String("false");
  String json= String("{\"value\":\"") +xPosition+"\"}";
  sendData(json,url_xcontroller);
//{"name":"XPosition","type":9,"value":"0"}
  json= String("{\"value\":\"") +bs+"\"}";
  sendData(json,url_button);
  getData();
  times=millis();
  }
}

void getData()
{
  //Serial.print("connecting to ");
  //Serial.println(host);
  WiFiClient client;
  if (!client.connect(host, port)) {
    Serial.println("connection failed");
    return;
  }
  
  Serial.print("Requesting URL: ");
  Serial.println(url_led);

  // This will send the request to the server
  client.print(String("GET ") + url_led + " HTTP/1.1\r\n" +
               "Host: " + host + "\r\n" +
               "Connection: close\r\n\r\n");
  unsigned long timeout = millis();
  while (client.available() == 0) {
    if (millis() - timeout > 5000) {
      Serial.println(">>> Client Timeout !");
      client.stop();
      return;
    }
  }
  //{"Scripts":[{"Id":"Script1","Name":"Script1","Value":"JS"}],"Id":"Sostoyanie-okna","Name":"аЁаОббаОбаНаИаЕ аОаКаНаА","Value":"true","Type":3}
  while (client.available()) {
    String line = client.readStringUntil('\r');
     //Serial.println(line);
    line = line.substring(line.indexOf("value"));
    //line = line.substring(10);
    //line = line.substring(0, line.indexOf('"'));
    line.toLowerCase();
     //Serial.println(line);
    if (line.indexOf("true")>=0)digitalWrite(14, HIGH);
    if (line.indexOf("false")>=0)digitalWrite(14, LOW);
    //if (line.length() > 3)
      //Serial.println(line);
  }
  //Serial.println();
  // Serial.println("closing connection");
}

void sendData(String json, String url)
{
  WiFiClient client;
  if (!client.connect(host, port)) {
    Serial.println("connection failed");
    return;
  }
  
  //Serial.print("Requesting URL: ");
  //Serial.println(url);
//
//Serial.print(String("POST ") + url + " HTTP/1.1\r\n" +
//               "Host: " + host + "\r\n" +
//               "Content-Type: application/json\r\n"+
//               "Content-Length: "+json.length()+
//               +"\r\n\r\n"+json+
//                "\r\n\r\n");
  client.print(String("POST ") + url + " HTTP/1.1\r\n" +
               "Host: " + host + "\r\n" +
               "Content-Type: application/json\r\n"+
               "Content-Length: "+json.length()+
               +"\r\n\r\n"+json+
                "\r\n\r\n");
  unsigned long timeout = millis();
  while (client.available() == 0) {
    if (millis() - timeout > 5000) {
      Serial.println(">>> Client Timeout !");
      client.stop();
      return;
    }
  }

//    while (client.available()) {
//    String line = client.readStringUntil('\r');
//      Serial.println(line);
//  }
  //{"Scripts":[{"Id":"Script1","Name":"Script1","Value":"JS"}],"Id":"Sostoyanie-okna","Name":"аЁаОббаОбаНаИаЕ аОаКаНаА","Value":"true","Type":3}
}
