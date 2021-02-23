# dungeon_game
A fun C# project I made during my 1st semester of college to practice the language and general OOP concepts.
It's a simple rouge-like game with a randomly generated world and a simple console UI.
Some information about the inner workings of the game is in the code itself as comments.
If anyone stumbles across this repository - all feedback is welcome.

For some reason the game runs slower than it used to - maybe because I switched from Windows 8 to Windows 10.
Now the screen visibly flashes everytime anything happens - earlier it used to happen so quick that you couldn't notice anything changing.
If you actually compile it and run it let me know how smooth it runs on your machine.
Also I focused more on making a good project than on making a fun game so it might be a little difficult and unforgiving 
(by a little I mean a lot which, although was the idea from the very beggining, can be frustrating)

# Screenshots
![screenshot1](https://github.com/BDemut/dungeon_game/blob/master/1.JPG?raw=true)

![screenshot2](https://github.com/BDemut/dungeon_game/blob/master/2.JPG?raw=true)

![screenshot3](https://github.com/BDemut/dungeon_game/blob/master/3.JPG?raw=true)

![screenshot4](https://github.com/BDemut/dungeon_game/blob/master/4.JPG?raw=true)

# That said for anyone that decides to play it:
General:
- You have to get through 3 levels of randomly generated dungeons to get to the boss
- You gain money along the way which you can spend in stores
- You can carry up to 2 pieces of equipment at any time (one for each hand, no infinite pockets, sorry).
- By killing enemies you gain experience and level up - with each level you can spend 2 points upgrading your stats

Fighting:
- A - a normal "physical" attack
- S - you use a spell - a spell has random effects which have a higher chance of happening the higher your magic skill is
  (effects are: set the enemy ablaze, stun the enemy, steal some of the enemy's life)
- F - try to escape

Stats:
- Strength - the more you have the more damage your normal attacks do
- Toughness - the more you have the less damage you take
- Magic - the more you have the better chance you spells do something
- Speed - the more you have the better chance of success when escaping a fight

Map:
- 'x' - the player
- ' ' - a room, treasure room or special room
- '/' - unexplored area
- '0' - a wall
- 'S' - level start
- 'E' - level end
- 'B' - boss room
- '$' - a store
