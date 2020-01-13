# Procedural_generated_map

Catrina's Plight Procedural dungeon developed on Unity 2017 using C# by Javier Dalma.

On this repository you can see the code I did to create a procedural generated map using Unity 2017, it has some limitation as it only works with basic shapes of room as I used a 2d matrix to build it. 

Please take a look at the Room Scheme.PDF document to understand more about how the level generator works and what kind of mazes you could get.

There are also example images of how the dungeon could be.

-grid.cs: this script it attached to an empty gameobject. Is the one that builds the maze it assigns the rooms and fills the maze matrix with the number on the rooms. After it builds the maze it close every exit that has no room connected.

-BaseRoom.cs: is the script the goes attached to the room prefab. It has 4 booleans where you should assign the exits on the rooms, and it will automaticali fill the grid matrix. It also has a function that rotates the room and the inside matrix to chek if it fits on the maze.

-NormalRoom.cs: inherit from the clase BaseRoom this script also goes attached to the room

-BossRoom.cs: inherit from the clase BaseRoom this script also goes attached to the room this script also goes attached to the room that you would like the maze to finish.

-StartRoom.cs: inherit from the clase BaseRoom this script also goes attached to the room that you would like the maze to start.

More comments about the code and how it works are on every .cs file.

If you want to learn more about the game check the promo video:

https://www.youtube.com/watch?v=BGqxvSEvG6o
 
If you want the project you can get it on:

https://drive.google.com/open?id=1hEGV7icUGABBWUziNjoqxOsFm4T46cdI
