# Battleship Game
A simple code kata to implement the battleship game. The idea here was to implement the state tracker part of the game that includes the following functionalities:

* Create a board for the user
* Add battleships to the board
* Take attacks from the opponent
* Check the status of the board (to see if user lost the game)


## Quick Start:

1. Create a board:
```csharp
var boardWidth = 10;
var boardHeight = 10;
var battleshipBoardTracker = new BoardStateTracker(boardWidth, boardHeight);
```

2. Create a battleship:
```csharp
var shipFactory = new ShipFactory();
var oneDimensionShip = new OneDimensionShip() {Orientation = ShipOrientation.Horizontal, StartPosition = new Position(2,3), Length = 3};
var ship = shipFactory.CreateShip(oneDimensionShip);
```

2. Add the battleship to the board:

```csharp
battleshipBoardTracker.AddShip(ship);
```

3. Take attack from the opponent and report the "Miss" or "Hit" result
```csharp
var attackResult = battleshipBoardTracker.TakeAttack(new Position(2,5));
Console.WriteLine($"Attack Result: {attackResult}")
```

4. Check the current status of the board ("Empty" or "ShipsAvailable" or "AllShipsDestroyed")
```csharp
var boardStatus = battleshipBoardTracker.GetBoardStatus();
Console.WriteLine($"Current Board Status : {boardStatus}")
```