# catanconsole
<img width="1140" alt="image" src="https://github.com/user-attachments/assets/9b2c3896-3424-4bdd-bb4b-7f8ea50372ff" />

To learn how to play the game, i made a video tutorial:
https://youtu.be/bxgUWUcmfVs

## Terminal
This game needs a color terminal with Unicode support:

### works
- iTerm2 (MacOS)
- PowerShell (Windows)
- VS Code terminal (MacOS, Windows, Linux)

### doesn't work
- CMD (Windows)
- CMDer (Windows)
- Terminal (MacOS)

Please download [VS code](https://code.visualstudio.com) and play in the built-in terminal if you encounter graphical problems.

## Provided save games
Playing a game takes a long time. Therefore we have provided 6 interesting saved games. We recommend starting from one of these save games and not starting a new game everytime you want to test or play.

## One important point about the game
During the beginning phase of the game each player gets to place two villages and two roads. It is not possible to
take any other action during this phase such as loading or saving or rolling the dice. Only quitting to main menu is possible then.

## Compiling the game yourself
If you want to compile the game by yourself, we recommend compiling from a terminal by going the the catan-console folder and entering `dotnet run`. 
You might need to install some NuGet packages. These are: 
```
dotnet add package Pastel --version 4.1.0
```

and 
```
dotnet add package Newtonsoft.Json --version 13.0.2
```

enter these lines in the terminal while in the folder catan-console to install.

Trying to compile from from Microsoft Visual Studio results in problems with the folder structure.

## Window size
The game needs a large enough terminal window to run, please make your terminal window bigger if the game gives an error.

## Differences between the original game and our game
There are some small differences in rules between the original game and our game. For a casual player the difference will not be noticeable.

2. The part at the start where players use dicerolls to determine who starts is not implemented.
3. Trading between players is not possible. You can only trade with the bank.
4. Opponents' settlements don't break the longest road in our version.
5. In the original version there is a maximum of 5 villages, 4 cities and 15 roads per player. Our game does not have a limit.
6. In our game, when a seven is rolled, the player loses half their resources randomly, in the original game a player can choose which resources he loses.
8. In the original game you can only play 1 development card during your turn. Our game has no such restriction.
9. In the original game the development cards can run out, (never happens in practice), in our game if that happens a new deck is made so you never run out.


