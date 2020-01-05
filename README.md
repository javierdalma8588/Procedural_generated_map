# Procedural_generated_map

On this repository you can see the code I did to create a procedural generated map using Unity 5, it has some limitation as it only works with basic shapes of room as I used a 2d matrix to build it. 

Please take a look at the Room Scheme.PDF document to understand more about how the level generator works and what kind of mazes you could get.

grid.cs: this script it attached to an empty gameobject. Is the one that builds the maze it assigns the rooms and fills the maze matrix with the number on the rooms. After it builds the maze it close every exit that has no room connected.

BaseRoom.cs: is the script the goes attached to the room prefab. It has 4 booleans where you should assign the exits on the rooms, and it will automaticali fill the grid matrix. It also has a function that rotates the room and the inside matrix to chek if it fits on the maze.

NormalRoom.cs: this script also goes attached to the room, inside each room you might want to have different enemy encounters
so this script is incharge of picking 1 of the enemy encounters you put in the room.

Wall.cs: this is a script that has a function to fade walls, as we had a camera wit a 3/4 view the wall covered the player so if the player was covered by the wall. The wall will fade to almost invisible

Door.cs: finally the doorhave a script attached thar cover the players exit. the only wait to past trought is to destroy it so the player has to shoot to the door.

Hope it can help!
