# 🧮 Calculator
#### I made a simple calculator in C# as a challenge. A big lesson was learned when I attempted this: "solve the problem on paper first." Of course I've made that mistake a few times since, but I think I've got it down by now.
- A simple calculator with a satisfying clicking sound.
- Hitting the '=' button does the previous operation on the current number.
- Accuracy is pretty good (doubles) but it doesn't have any logic regarding significant figures so please don't use it for anything important.

## ✅ Possible Improvements
- Implement logic for significant figues
- I think now I would probably come up with a different architecture for this, it feels a bit raw

## 🧰 Tech Stack
- [Raylib-cs](https://github.com/raylib-cs/raylib-cs)
- .NET 6

## 🚀 Getting Started
```bash
git clone git@github.com:holypeachy/calculator.git
cd calculator
dotnet run
```
If raylib is missing add the package using:
```bash
dotnet add package Raylib-cs
```

## 📤 Output Example:
<img width="482" height="832" alt="image" src="https://github.com/user-attachments/assets/d05deac3-9999-4676-bda5-7a42138c1186" />
