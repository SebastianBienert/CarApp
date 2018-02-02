#include <LiquidCrystal.h> //Dołączenie bilbioteki
LiquidCrystal lcd(2, 3, 4, 8, 11, 12); //Informacja o podłączeniu nowego wyświetlacza

int izqA = 5;
int izqB = 6;
int derA = 9;
int derB = 10;
int vel = 127; // Predkosc silnikow (0 - 255)
int data = 12; // inicjowanie sygnalem stopu
bool updateLCD = false;

void serialFlush() 
{
  while (Serial.available() > 0) {
    char t = Serial.read();
  }
}

void setup() {
  Serial.begin(9600);
  pinMode(derA, OUTPUT);
  pinMode(derB, OUTPUT);
  pinMode(izqA, OUTPUT);
  pinMode(izqB, OUTPUT);

  lcd.begin(16, 2); //Deklaracja typu
  lcd.setCursor(0, 0); //Ustawienie kursora
  lcd.print("XDDDDDDDDDDD"); //Wyświetlenie tekstu
  lcd.setCursor(0, 1); //Ustawienie kursora
  lcd.print("     !!!!!!     "); //Wyświetlenie tekstu
}

void ControlMovements()
{
   if (data == 14) { // Forward
    analogWrite(derB, 0);
    analogWrite(izqB, 0);
    analogWrite(derA, vel);
    analogWrite(izqA, vel);
  }
  if (data == 11) { // right
    analogWrite(derB, vel);
    analogWrite(izqB, 0);
    analogWrite(derA, 0);
    analogWrite(izqA, vel);
  }
  if (data == 12) { // Stop
    analogWrite(derB, 0);
    analogWrite(izqB, 0);
    analogWrite(derA, 0);
    analogWrite(izqA, 0);
  }
  if (data == 13) { // left
    analogWrite(derB, 0);
    analogWrite(izqB, vel);
    analogWrite(izqA, 0);
    analogWrite(derA, vel);
  }

  if (data == 10) { // Reverse
    analogWrite(derA, 0);
    analogWrite(izqA, 0);
    analogWrite(derB, vel);
    analogWrite(izqB, vel);
  }
}

void UpdateLCDText(String text)
{
      Serial.println("Updating..");
  String temp;
  if(text.length() > 16)
  {
    temp = text.substring(0,16);
    lcd.clear();
    lcd.print(temp);
    lcd.setCursor(0, 1); 
    temp = text.substring(16, text.length());
    lcd.print(temp);
  }
  else
  {
    lcd.clear();
    lcd.print(text);
  }
  
}

String ReadStringFromStream()
{
  String result ="";
  char t;
  while(Serial.available() > 0)
  {
    t = Serial.read();
    result += t;
  }
  Serial.println(result);
  return result;
}

void loop()
{
  if (Serial.available() > 0)
  { 
     Serial.println("1 charakter");
     if(Serial.peek() == 9)
     {
        Serial.read();
        vel = Serial.read();
        UpdateLCDText("Predkosc: " + String(vel) );
     }

     
    if(Serial.available() > 1 && Serial.peek() > 20 )
    {
      delay(100);
      data = 'c';
      Serial.println("cosiedzieje");
      UpdateLCDText(ReadStringFromStream());
    }
    data = Serial.read();
  }
    ControlMovements();
    delay(10);
    //Serial.println(data);

}





