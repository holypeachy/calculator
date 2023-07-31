using System;
using System.Numerics;
using Raylib_cs;

namespace Calculator
{
    enum State
    {
        init,
        op_selected,
        result,
        cannot_divide_zero
    }

    class Program
    {
        const int ViewportX = 480; // 480
        const int ViewportY = 800; // 800

        static float circleSize = 45;

        static float quarterX = ViewportX / 4 - 8;
        static float fifthY = ViewportY * 0.6f / 5;

        static float col1 = quarterX / 2 + 16;
        static float col2 = quarterX + quarterX / 2 + 16;
        static float col3 = quarterX * 2 + quarterX / 2 + 16;
        static float col4 = quarterX * 3 + quarterX / 2 + 16;
        static float colEquals = (quarterX * 2 + quarterX / 2 + 16) - circleSize;

        static float row1 = ViewportY * 0.4f;
        static float row2 = ViewportY * 0.4f + fifthY;
        static float row3 = ViewportY * 0.4f + fifthY * 2;
        static float row4 = ViewportY * 0.4f + fifthY * 3;
        static float row5 = ViewportY * 0.4f + fifthY * 4;
        static float rowEquals = (ViewportY * 0.4f + fifthY * 4) - circleSize;

        static Color colorSpecial = new Color(212, 123, 83, 255);
        static Color colorNumber = new Color(83, 95, 124, 255);
        static Color colorOperator = new Color(175, 95, 255, 255);

        static Color colorSpecialSelected = new Color(162, 73, 33, 255);
        static Color colorNumberSelected = new Color(33, 45, 74, 255);
        static Color colorOperatorSelected = new Color(125, 35, 205, 255);

        static Color colorDisplay = new Color(52, 73, 94, 255);
        static Color colorHistory = new Color(102, 123, 134, 255);

        static Color buttonFontColor = Color.WHITE;

        static Button[] Buttons = {
                new RoundButton("C", col1, row1, circleSize, colorSpecial, colorSpecialSelected),
                new RoundButton("+/-", col2, row1, circleSize, colorSpecial, colorSpecialSelected),
                new RoundButton("%", col3, row1, circleSize, colorSpecial, colorSpecialSelected),
                new RoundButton("/", col4, row1, circleSize, colorOperator, colorOperatorSelected),
                new RoundButton("1", col1, row2, circleSize, colorNumber, colorNumberSelected),
                new RoundButton("2", col2, row2, circleSize, colorNumber, colorNumberSelected),
                new RoundButton("3", col3, row2, circleSize, colorNumber, colorNumberSelected),
                new RoundButton("X", col4, row2, circleSize, colorOperator, colorOperatorSelected),
                new RoundButton("4", col1, row3, circleSize, colorNumber, colorNumberSelected),
                new RoundButton("5", col2, row3, circleSize, colorNumber, colorNumberSelected),
                new RoundButton("6", col3, row3, circleSize, colorNumber, colorNumberSelected),
                new RoundButton("+", col4, row3, circleSize, colorOperator, colorOperatorSelected),
                new RoundButton("7", col1, row4, circleSize, colorNumber, colorNumberSelected),
                new RoundButton("8", col2, row4, circleSize, colorNumber, colorNumberSelected),
                new RoundButton("9", col3, row4, circleSize, colorNumber, colorNumberSelected),
                new RoundButton("-", col4, row4, circleSize, colorOperator, colorOperatorSelected),
                new RoundButton(".", col1, row5, circleSize, colorNumber, colorNumberSelected),
                new RoundButton("0", col2, row5, circleSize, colorNumber, colorNumberSelected),
                new RoundedRectangleButton("=", colEquals, rowEquals, circleSize * 4 + circleSize / 2, circleSize * 2, 1, colorNumber, colorNumberSelected),
    };


        static float buttonFontSize = 50;
        static Font buttonFont = Raylib.LoadFontEx("assets/Inter-Black.ttf", (int)buttonFontSize, null, 0);
        static float buttonLabelSpacing = 0f;

        static Vector2 vect2Temp;
        static Vector2 rectCenterTemp;

        static float displayFontSize = 62f;
        static Vector2 displayPos = new Vector2(0 + 32, ViewportY * 0.2f);
        static Font displayFont = Raylib.LoadFontEx("assets/Inter-Black.ttf", (int)displayFontSize, null, 0);

        static float hDisplayFontSize = 20f;
        static Vector2 hDisplayPos = new Vector2(0 + 32, ViewportY * 0.18f);
        static Font hDisplayFont = Raylib.LoadFontEx("assets/Inter-Black.ttf", (int)hDisplayFontSize, null, 0);

        static string displayText = "0";
        static string histDisplayText = "History";
        static string currentOp = "";

        static State currentState = State.init;
        static bool isFirstCharacter = true;

        static float result = 0;
        static float first = 0;
        static float second = 0;

        static Sound click = Raylib.LoadSound("assets/click.wav");


        public static void Main()
        {
            // Config
            Raylib.SetConfigFlags(ConfigFlags.FLAG_MSAA_4X_HINT);
            // Raylib.SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE);

            // Init
            Raylib.InitWindow(ViewportX, ViewportY, "Calculator");
            Raylib.InitAudioDevice();
            Raylib.SetWindowIcon(Raylib.LoadImage("assets/icon.png"));

            Raylib.SetTargetFPS(144);


            while (!Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.WHITE);

                DrawButtons();
                DrawDisplay();
                CheckClicks();

                Raylib.EndDrawing();
            }
            Raylib.CloseWindow();
            
        }

        static void DrawButtons()
        {
            foreach (Button button in Buttons)
            {
                if (button is RoundButton)
                {
                    Raylib.DrawCircle((int)button.Position.X, (int)button.Position.Y, ((RoundButton)button).Radius, button.IsSelected() ? button.SelectedColor : button.Color);
                    DrawCircleLabel(button.Label, button.Position, buttonFontColor);
                }
                else if (button is RoundedRectangleButton)
                {
                    Raylib.DrawRectangleRounded(button.Rectangle, ((RoundedRectangleButton)button).Roundness, 10, button.IsSelected() ? button.SelectedColor : button.Color);
                    DrawRectLabel(button.Label, button.Position, button.Size, buttonFontColor);
                }
                else
                {
                    Raylib.DrawRectangle((int)button.Position.X, (int)button.Position.Y, (int)button.Size.X, (int)button.Size.Y, button.IsSelected() ? button.SelectedColor : button.Color);
                    DrawRectLabel(button.Label, button.Position, button.Size, buttonFontColor);
                }
            }
        }

        static void DrawCircleLabel(string label, Vector2 position, Color color)
        {
            vect2Temp = Raylib.MeasureTextEx(buttonFont, label, buttonFontSize, buttonLabelSpacing);
            vect2Temp = new Vector2(vect2Temp.X / 2, vect2Temp.Y / 2);
            Raylib.DrawTextEx(buttonFont, label, new Vector2(position.X - vect2Temp.X, position.Y - vect2Temp.Y), buttonFontSize, buttonLabelSpacing, color);
        }

        static void DrawRectLabel(string label, Vector2 position, Vector2 size, Color color)
        {
            vect2Temp = Raylib.MeasureTextEx(buttonFont, label, buttonFontSize, buttonLabelSpacing);
            vect2Temp = new Vector2(vect2Temp.X / 2, vect2Temp.Y / 2);
            rectCenterTemp = new Vector2(size.X / 2, size.Y / 2);
            Raylib.DrawTextEx(buttonFont, label, new Vector2(position.X + (rectCenterTemp.X - vect2Temp.X), position.Y + (rectCenterTemp.Y - vect2Temp.Y)), buttonFontSize, buttonLabelSpacing, color);
        }

        static void CheckClicks()
        {
            foreach (Button button in Buttons)
            {
                if (button.IsClickedOn())
                {
                    // Console.WriteLine(button.Label + " clicked");
                    Logic(button.Label);
                    Raylib.PlaySound(click);
                }
            }
        }

        static void DrawDisplay()
        {
            Raylib.DrawTextEx(displayFont, displayText, displayPos, displayFontSize, 1, colorDisplay);
            Raylib.DrawTextEx(hDisplayFont, histDisplayText, hDisplayPos, hDisplayFontSize, 1, colorHistory);
        }

        static void Logic(string button){
            switch (currentState)
            {
                case State.init:
                    Initial(button);
                    break;
                case State.op_selected:
                    OpSelected(button);
                    break;
                case State.result:
                    Result(button);
                    break;
                case State.cannot_divide_zero:
                    CannotDivideZero(button);
                    break;
                default:
                    break;
            }
        }

        static void Initial(string button){
            switch (button)
            {
                case "C":
                    displayText = "0";
                    isFirstCharacter = true;
                    break;
                case "+/-":
                    if(displayText != "0" && !IsNegative()){
                        displayText = "-" + displayText;
                    }
                    else if(IsNegative()){
                        displayText = displayText.Remove(0, 1);
                    }
                    break;
                case "%":
                    displayText = (float.Parse(displayText) / 100).ToString();
                    break;
                case "/":
                    currentOp = "/";
                    isFirstCharacter = true;
                    first = float.Parse(displayText);
                    histDisplayText = displayText + " / ";
                    currentState = State.op_selected;
                    break;
                case "X":
                    currentOp = "X";
                    isFirstCharacter = true;
                    first = float.Parse(displayText);
                    histDisplayText = displayText + " X ";
                    currentState = State.op_selected;
                    break;
                case "+":
                    currentOp = "+";
                    isFirstCharacter = true;
                    first = float.Parse(displayText);
                    histDisplayText = displayText + " + ";
                    currentState = State.op_selected;
                    break;
                case "-":
                    currentOp = "-";
                    isFirstCharacter = true;
                    first = float.Parse(displayText);
                    histDisplayText = displayText + " - ";
                    currentState = State.op_selected;
                    break;
                case "=":
                    first = float.Parse(displayText);
                    result = first;
                    histDisplayText = displayText + " = ";
                    isFirstCharacter = true;
                    currentState = State.result;
                    break;
                case ".":
                    if(!displayText.Contains(".")){
                        displayText += ".";
                        isFirstCharacter = false;
                    }
                    break;
                case "1":
                    if(isFirstCharacter || displayText == "0"){
                        displayText = "1";
                        isFirstCharacter = false;
                    }
                    else if(!HasReachedCharLimit())
                    {
                        displayText += "1";
                    }
                    break;
                case "2":
                    if (isFirstCharacter || displayText == "0")
                    {
                        displayText = "2";
                        isFirstCharacter = false;
                    }
                    else if (!HasReachedCharLimit())
                    {
                        displayText += "2";
                    }
                    break;
                case "3":
                    if (isFirstCharacter || displayText == "0")
                    {
                        displayText = "3";
                        isFirstCharacter = false;
                    }
                    else if (!HasReachedCharLimit())
                    {
                        displayText += "3";
                    }
                    break;
                case "4":
                    if (isFirstCharacter || displayText == "0")
                    {
                        displayText = "4";
                        isFirstCharacter = false;
                    }
                    else if (!HasReachedCharLimit())
                    {
                        displayText += "4";
                    }
                    break;
                case "5":
                    if (isFirstCharacter || displayText == "0")
                    {
                        displayText = "5";
                        isFirstCharacter = false;
                    }
                    else if (!HasReachedCharLimit())
                    {
                        displayText += "5";
                    }
                    break;
                case "6":
                    if (isFirstCharacter || displayText == "0")
                    {
                        displayText = "6";
                        isFirstCharacter = false;
                    }
                    else if (!HasReachedCharLimit())
                    {
                        displayText += "6";
                    }
                    break;
                case "7":
                    if (isFirstCharacter || displayText == "0")
                    {
                        displayText = "7";
                        isFirstCharacter = false;
                    }
                    else if (!HasReachedCharLimit())
                    {
                        displayText += "7";
                    }
                    break;
                case "8":
                    if (isFirstCharacter || displayText == "0")
                    {
                        displayText = "8";
                        isFirstCharacter = false;
                    }
                    else if (!HasReachedCharLimit())
                    {
                        displayText += "8";
                    }
                    break;
                case "9":
                    if (isFirstCharacter || displayText == "0")
                    {
                        displayText = "9";
                        isFirstCharacter = false;
                    }
                    else if (!HasReachedCharLimit())
                    {
                        displayText += "9";
                    }
                    break;
                case "0":
                    if (isFirstCharacter)
                    {
                        displayText = "0";
                        isFirstCharacter = false;
                    }
                    else if (!HasReachedCharLimit())
                    {
                        displayText += "0";
                    }
                    break;
                default:
                    break;
            }
        }
        
        static void OpSelected(string button){
            switch (button)
            {
                case "C":
                    if (displayText != "0")
                    {
                        displayText = "0";
                        isFirstCharacter = true;
                    }
                    else if (displayText == "0")
                    {
                        histDisplayText = "History";
                        displayText = "0";
                        isFirstCharacter = true;
                        currentOp = "";
                        currentState = State.init;
                    }
                    break;
                case "+/-":
                    if (displayText != "0" && !IsNegative())
                    {
                        displayText = "-" + displayText;
                    }
                    else if (!IsNegative())
                    {
                        displayText = displayText.Remove(0, 1);
                    }
                    break;
                case "%":
                    second = float.Parse(displayText);
                    second /= 100f;
                    histDisplayText = first + " " + currentOp + " " + second;
                    displayText = second.ToString();
                    break;
                case "/":
                    if(isFirstCharacter)
                    {
                        histDisplayText = histDisplayText.Replace(currentOp, "/");
                        currentOp = "/";
                    }
                    else{
                        second = float.Parse(displayText);
                        if(second == 0){
                            histDisplayText = first + " " + currentOp + " " + second + " = ";
                            displayText = "Error";
                            currentState = State.cannot_divide_zero;
                        }
                        else{
                            Calculate();
                            currentOp = "/";
                            histDisplayText = result + " " + currentOp;
                            displayText = result.ToString();
                            first = result;
                            isFirstCharacter = true;
                        }
                    }
                    break;
                case "X":
                    if (isFirstCharacter)
                    {
                        histDisplayText = histDisplayText.Replace(currentOp, "X");
                        currentOp = "X";
                    }
                    else
                    {
                        second = float.Parse(displayText);
                        Calculate();
                        currentOp = "X";
                        histDisplayText = result + " " + currentOp;
                        displayText = result.ToString();
                        first = result;
                        isFirstCharacter = true;
                    }
                    break;
                case "+":
                    if (isFirstCharacter)
                    {
                        histDisplayText = histDisplayText.Replace(currentOp, "+");
                        currentOp = "+";
                    }
                    else
                    {
                        second = float.Parse(displayText);
                        Calculate();
                        currentOp = "+";
                        histDisplayText = result + " " + currentOp;
                        displayText = result.ToString();
                        first = result;
                        isFirstCharacter = true;
                    }
                    break;
                case "-":
                    if (isFirstCharacter)
                    {
                        histDisplayText = histDisplayText.Replace(currentOp, "-");
                        currentOp = "-";
                    }
                    else
                    {
                        second = float.Parse(displayText);
                        Calculate();
                        currentOp = "-";
                        histDisplayText = result + " " + currentOp;
                        displayText = result.ToString();
                        first = result;
                        isFirstCharacter = true;
                    }
                    break;
                case "=":
                    second = float.Parse(displayText);
                    if(second == 0 && currentOp == "/"){
                        histDisplayText = first + " " + currentOp + " " + second + " = ";
                        displayText = "Error";
                        currentState = State.cannot_divide_zero;
                    }
                    else{
                        Calculate();
                        histDisplayText = first + " " + currentOp + " " + second + " = ";
                        displayText = result.ToString();
                        currentState = State.result;
                    }
                    break;
                case ".":
                    if(isFirstCharacter){
                        displayText = "0.";
                        isFirstCharacter = false;
                    }
                    else if (!displayText.Contains("."))
                    {
                        displayText += ".";
                    }
                    break;
                case "1":
                    if (isFirstCharacter || displayText == "0")
                    {
                        displayText = "1";
                        isFirstCharacter = false;
                    }
                    else if (!HasReachedCharLimit())
                    {
                        displayText += "1";
                    }
                    break;
                case "2":
                    if (isFirstCharacter || displayText == "0")
                    {
                        displayText = "2";
                        isFirstCharacter = false;
                    }
                    else if (!HasReachedCharLimit())
                    {
                        displayText += "2";
                    }
                    break;
                case "3":
                    if (isFirstCharacter || displayText == "0")
                    {
                        displayText = "3";
                        isFirstCharacter = false;
                    }
                    else if (!HasReachedCharLimit())
                    {
                        displayText += "3";
                    }
                    break;
                case "4":
                    if (isFirstCharacter || displayText == "0")
                    {
                        displayText = "4";
                        isFirstCharacter = false;
                    }
                    else if (!HasReachedCharLimit())
                    {
                        displayText += "4";
                    }
                    break;
                case "5":
                    if (isFirstCharacter || displayText == "0")
                    {
                        displayText = "5";
                        isFirstCharacter = false;
                    }
                    else if (!HasReachedCharLimit())
                    {
                        displayText += "5";
                    }
                    break;
                case "6":
                    if (isFirstCharacter || displayText == "0")
                    {
                        displayText = "6";
                        isFirstCharacter = false;
                    }
                    else if (!HasReachedCharLimit())
                    {
                        displayText += "6";
                    }
                    break;
                case "7":
                    if (isFirstCharacter || displayText == "0")
                    {
                        displayText = "7";
                        isFirstCharacter = false;
                    }
                    else if (!HasReachedCharLimit())
                    {
                        displayText += "7";
                    }
                    break;
                case "8":
                    if (isFirstCharacter || displayText == "0")
                    {
                        displayText = "8";
                        isFirstCharacter = false;
                    }
                    else if (!HasReachedCharLimit())
                    {
                        displayText += "8";
                    }
                    break;
                case "9":
                    if (isFirstCharacter || displayText == "0")
                    {
                        displayText = "9";
                        isFirstCharacter = false;
                    }
                    else if (!HasReachedCharLimit())
                    {
                        displayText += "9";
                    }
                    break;
                case "0":
                    if (isFirstCharacter && displayText == "0")
                    {
                        isFirstCharacter = false;
                    }
                    else if (isFirstCharacter){
                        displayText = "0";
                        isFirstCharacter = false;
                    }
                    else if (!HasReachedCharLimit())
                    {
                        displayText += "0";
                    }
                    break;
                default:
                    break;
            }
        }
        
        static void Result(string button){
            switch (button)
            {
                case "C":
                    histDisplayText = "History";
                    displayText = "0";
                    isFirstCharacter = true;
                    currentOp = "";
                    currentState = State.init;
                    break;
                case "+/-":
                    histDisplayText = "History";
                    displayText = result.ToString();
                    if(IsNegative()){
                        displayText = displayText.Remove(0, 1);
                    }
                    else{
                        displayText = "-" + displayText;
                    }
                    isFirstCharacter = true;
                    currentOp = "";
                    currentState = State.init;
                    break;
                case "%":
                    histDisplayText = "History";
                    first = result;
                    first /= 100;
                    displayText = first.ToString();
                    isFirstCharacter = true;
                    currentOp = "";
                    currentState = State.init;
                    break;
                case "/":
                    first = result;
                    histDisplayText = first + " / ";
                    currentOp = "/";
                    displayText = result.ToString();
                    isFirstCharacter = true;
                    currentState = State.op_selected;
                    break;
                case "X":
                    first = result;
                    histDisplayText = first + " X ";
                    currentOp = "X";
                    displayText = result.ToString();
                    isFirstCharacter = true;
                    currentState = State.op_selected;
                    break;
                case "+":
                    first = result;
                    histDisplayText = first + " + ";
                    currentOp = "+";
                    displayText = result.ToString();
                    isFirstCharacter = true;
                    currentState = State.op_selected;
                    break;
                case "-":
                    first = result;
                    histDisplayText = first + " - ";
                    currentOp = "-";
                    displayText = result.ToString();
                    isFirstCharacter = true;
                    currentState = State.op_selected;
                    break;
                case "=":
                    if(currentOp != ""){
                        first = result;
                        histDisplayText = first + " " + currentOp + " " + second + " = ";
                        Calculate();
                        displayText = result.ToString();
                    }
                    break;
                case ".":
                    histDisplayText = "History";
                    displayText = "0.";
                    isFirstCharacter = false;
                    currentOp = "";
                    currentState = State.init;
                    break;
                case "1":
                    histDisplayText = "History";
                    displayText = "1";
                    isFirstCharacter = false;
                    currentOp = "";
                    currentState = State.init;
                    break;
                case "2":
                    histDisplayText = "History";
                    displayText = "2";
                    isFirstCharacter = false;
                    currentOp = "";
                    currentState = State.init;
                    break;
                case "3":
                    histDisplayText = "History";
                    displayText = "3";
                    isFirstCharacter = false;
                    currentOp = "";
                    currentState = State.init;
                    break;
                case "4":
                    histDisplayText = "History";
                    displayText = "4";
                    isFirstCharacter = false;
                    currentOp = "";
                    currentState = State.init;
                    break;
                case "5":
                    histDisplayText = "History";
                    displayText = "5";
                    isFirstCharacter = false;
                    currentOp = "";
                    currentState = State.init;
                    break;
                case "6":
                    histDisplayText = "History";
                    displayText = "6";
                    isFirstCharacter = false;
                    currentOp = "";
                    currentState = State.init;
                    break;
                case "7":
                    histDisplayText = "History";
                    displayText = "7";
                    isFirstCharacter = false;
                    currentOp = "";
                    currentState = State.init;
                    break;
                case "8":
                    histDisplayText = "History";
                    displayText = "8";
                    isFirstCharacter = false;
                    currentOp = "";
                    currentState = State.init;
                    break;
                case "9":
                    histDisplayText = "History";
                    displayText = "9";
                    isFirstCharacter = false;
                    currentOp = "";
                    currentState = State.init;
                    break;
                case "0":
                    histDisplayText = "History";
                    displayText = "0";
                    isFirstCharacter = false;
                    currentOp = "";
                    currentState = State.init;
                    break;
                default:
                    break;
            }
        }

        static void CannotDivideZero(string button){
            switch (button)
            {
                case "C":
                    histDisplayText = "History";
                    displayText = "0";
                    isFirstCharacter = true;
                    currentOp = "";
                    currentState = State.init;
                    break;
                case "+/-":
                    break;
                case "%":
                    break;
                case "/":
                    break;
                case "X":
                    break;
                case "+":
                    break;
                case "-":
                    break;
                case "=":
                    break;
                case ".":
                    break;
                case "1":
                    histDisplayText = "History";
                    displayText = "1";
                    isFirstCharacter = false;
                    currentOp = "";
                    currentState = State.init;
                    break;
                case "2":
                    histDisplayText = "History";
                    displayText = "2";
                    isFirstCharacter = false;
                    currentOp = "";
                    currentState = State.init;
                    break;
                case "3":
                    histDisplayText = "History";
                    displayText = "3";
                    isFirstCharacter = false;
                    currentOp = "";
                    currentState = State.init;
                    break;
                case "4":
                    histDisplayText = "History";
                    displayText = "4";
                    isFirstCharacter = false;
                    currentOp = "";
                    currentState = State.init;
                    break;
                case "5":
                    histDisplayText = "History";
                    displayText = "5";
                    isFirstCharacter = false;
                    currentOp = "";
                    currentState = State.init;
                    break;
                case "6":
                    histDisplayText = "History";
                    displayText = "6";
                    isFirstCharacter = false;
                    currentOp = "";
                    currentState = State.init;
                    break;
                case "7":
                    histDisplayText = "History";
                    displayText = "7";
                    isFirstCharacter = false;
                    currentOp = "";
                    currentState = State.init;
                    break;
                case "8":
                    histDisplayText = "History";
                    displayText = "8";
                    isFirstCharacter = false;
                    currentOp = "";
                    currentState = State.init;
                    break;
                case "9":
                    histDisplayText = "History";
                    displayText = "9";
                    isFirstCharacter = false;
                    currentOp = "";
                    currentState = State.init;
                    break;
                case "0":
                    histDisplayText = "History";
                    displayText = "0";
                    isFirstCharacter = false;
                    currentOp = "";
                    currentState = State.init;
                    break;
                default:
                    break;
            }
        }

        static void Calculate(){
            switch (currentOp)
            {
                case "/":
                    result = first / second;
                    break;
                case "X":
                    result = first * second;
                    break;
                case "+":
                    result = first + second;
                    break;
                case "-":
                    result = first - second;
                    break;
                default:
                    break;
            }
        }

        static bool HasReachedCharLimit(){
            if(displayText.Length > 9){
                return true;
            }
            else{
                return false;
            }
        }
    
        static bool IsNegative(){
            if(displayText[0] == '-'){
                return true;
            }
            else{
                return false;
            }
        }

    }

    class Button
    {
        public Rectangle Rectangle { get; private set; }
        public Vector2 Position { get; private set; }
        public Vector2 Size { get; private set; }
        public Color Color { get; private set; }
        public Color SelectedColor { get; private set; }
        public string Label { get; private set; }

        public Button(string label, float x, float y, float width, float height, Color color, Color selectedColor)
        {
            this.Label = label;

            this.Position = new Vector2(x, y);
            this.Size = new Vector2(width, height);

            this.Color = color;
            this.SelectedColor = selectedColor;

            this.Rectangle = new Rectangle(Position.X, Position.Y, Size.X, Size.Y);
        }

        public virtual bool IsClickedOn()
        {
            return (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), this.Rectangle) && Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT));
        }

        public virtual bool IsSelected(){
            return (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), this.Rectangle) && Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT));
        }
    }

    class RoundButton : Button
    {
        public float Radius { get; private set; }

        public RoundButton(string id, float x, float y, float radius, Color color, Color selectedColor) : base(id, x, y, 0, 0, color, selectedColor)
        {
            this.Radius = radius;
        }

        public override bool IsClickedOn()
        {
            return (Raylib.CheckCollisionPointCircle(Raylib.GetMousePosition(), Position, Radius) && Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT));
        }

        public override bool IsSelected()
        {
            return (Raylib.CheckCollisionPointCircle(Raylib.GetMousePosition(), Position, Radius) && Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT));
        }
    }

    class RoundedRectangleButton : Button
    {
        public float Roundness { get; set; }

        public RoundedRectangleButton(string label, float x, float y, float width, float height, float roundness, Color color, Color selectedColor) : base(label, x, y, width, height, color, selectedColor)
        {
            this.Roundness = roundness;
        }

        public override bool IsClickedOn()
        {
            return base.IsClickedOn();
        }
        public override bool IsSelected()
        {
            return base.IsSelected();
        }
    }
}
