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


# How to play
# How to play

[Rules](https://www.catan.com/sites/default/files/2021-06/catan_base_rules_2020_200707.pdf)

[Youtube video I made](https://youtu.be/bxgUWUcmfVs)



## Starting phase

In the starting phase you can only place villages and roads. Saving and loading games is not possible. Each turn place a village and then a road. You don't need to end your turn in this phase.

Place a village, city or road, add the location number as argument

- `village 34`
- `road 07`

## Main phase

1. Start a turn by rolling the dice `roll`

   * Resources are distributed automatically.

   * If you roll a 7 you need to replace the robber. Inside the hexagons on the map new numbers will show enclosed in square brackets like this `[14]`. Type the number of the location where you want to place the robber and press Enter.

Then do any of the following things in any order.

* Place a village, city or road, add the location number as argument

  - `village 34`

  - `road 07`

  - `city 23`

* Buy or play development cards

  * `buycard`

  * `playcard kngt`   play a Knight card

  * `playcard rodb`   play a Roadbuilding card

  * `playcard vict`   play a victorypoint card

  * `playcard yeop`   play a Year of Plenty card

  * `playcard mono`   play a Monopoly card

* Trade with the bank. The rate depends on the harbors that you have.

  * `trade wool iron`  means you give 2/3/4 wool to the bank and receive 1 iron

  * `trade grain lumber`  give grain to bank and receive lumber

* Save the game
  * `save`   Then in the dialog enter the name of your save file.
* Load a game
  * `load`   Then in the dialog type the exact name of the game you want to load.

Finally end your turn:

* `end`

Now let the next player take place behind the computer.

## UI
<img width="1058" alt="Screenshot 2025-02-03 at 17 32 40" src="https://github.com/user-attachments/assets/684f413f-864a-4ebf-b845-de863f412970" />

