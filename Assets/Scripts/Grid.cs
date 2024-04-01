using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// Class to handle the room creation
/// </summary>
public class Grid : MonoBehaviour
{
    //Every element from the rooms
    [Space]
    [Header("Level Prefabs")]
    private BaseRoom _roomClone;
    public StartRoom StartRoom;
    public BossRoom BossRoom;

    //variables that manage the maze
    [Space]
    [Header("Rooms Placed")]
    private const int ROOM_SIZE = 3;
    private int _roomsPlaced;
    private int[,] _dungeon;
    public float BuildDelay;
    public int WantedRooms;

    //variables of the rooms
    [Space]
    [Header("List and Arrays of Rooms")]
    private Queue<GridPos> _roomPosQueue;
    private List<BaseRoom> _placedRooms = new List<BaseRoom>();
    public NormalRoom[] Rooms;

    /// <summary>
    /// We create the Grid and
    /// </summary>
    /// <returns></returns>
    private IEnumerator Start()
    {
        //Get the seed of the level
        //Random.InitState (1);

        _dungeon = new int[WantedRooms * ROOM_SIZE, WantedRooms * ROOM_SIZE];
        for (int i = 0; i < WantedRooms * ROOM_SIZE; i++)
        {
            for (int j = 0; j < WantedRooms * ROOM_SIZE; j++)
            {
                _dungeon[i, j] = -1;
            }
        }

        _roomPosQueue = new Queue<GridPos>();

        int startX = WantedRooms / 2;
        int startY = WantedRooms / 2;

        _roomPosQueue.Enqueue(new GridPos(startX * ROOM_SIZE, startY * ROOM_SIZE));

        _roomsPlaced = 0;

        //We add new room positions to queue
        while (_roomPosQueue.Count != 0)
        {
            GridPos pos = _roomPosQueue.Dequeue();

            int randomIndex = Random.Range(0, Rooms.Length - 1);
            PlaceRoom(randomIndex, pos.x, pos.y);


            if (BuildDelay > 0)
            {
                yield return new WaitForSeconds(BuildDelay);
            }
            if (_roomsPlaced >= WantedRooms)
            {
                yield break;
            }
        }
    }

    /// <summary>
    /// Instantiates the room and accommodates it on the grid
    /// </summary>
    /// <param name="roomIndex"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void PlaceRoom(int roomIndex, int x, int y)
    {
        _roomClone = null;
        if (_roomsPlaced == 0)
        {
            _roomClone = Instantiate(StartRoom);
        }
        else
        {
            //We instantiate a roomClone. This might be destroyed later (not efficient but keeping it for now)
            if (_roomsPlaced < WantedRooms - 1)
            {
                _roomClone = Instantiate(Rooms[roomIndex]);
            }
            else if (_roomsPlaced == WantedRooms - 1)
            {
                Debug.Log ("I Placed the boss room");
                _roomClone = Instantiate(BossRoom);
            }
        }

        //For safety, we break out of the array when all rooms (with all rotations) were checked. This should never happen though!
        int breakPoint = 0;
        int rotations = 0;

        //We try puzzling the room in
        while (!CanPlaceRoom(_roomClone, x, y))
        {
            //We rotate the room 4 times
            rotations++;
            _roomClone.Rotate();
            if (rotations >= 4)
            {
                //If the room doesn't fit after 4 rotations, we pick the next room
                rotations = 0;
                roomIndex++;
                if (roomIndex >= Rooms.Length)
                {
                    roomIndex = 0;
                }
                //We destroy the old room, since it didn't fit
                Destroy(_roomClone.gameObject);
                if (breakPoint > Rooms.Length)
                {
                    roomIndex = 4;
                    //CDebug.Log ("Unity would have crashed...");
                    return;
                }
                _roomClone = Instantiate(Rooms[roomIndex]).GetComponent<BaseRoom>();
                breakPoint++;
            }
        }

        //At this point we have found a fitting room, and we can enter the data in our grid
        for (int i = 0; i < ROOM_SIZE; i++)
        {
            for (int j = 0; j < ROOM_SIZE; j++)
            {
                _dungeon[x + i, y + j] = _roomClone.grid[i, j];
            }
        }

        //We place the room at the correct position
        _roomClone.transform.position = new Vector3((x - WantedRooms * .5f) * 54, 0, (y - WantedRooms * .5f) * 54);

        //TopDoor
        CheckAddRoom(x, y + 1, x - 1, y + 1, x - 3, y);
        //LeftDoor
        CheckAddRoom(x + 1, y, x + 1, y - 1, x, y - 3);
        //BottomDoor
        CheckAddRoom(x + 2, y + 1, x + 3, y + 1, x + 3, y);
        //RightDoor
        CheckAddRoom(x + 1, y + 2, x + 1, y + 3, x, y + 3);

        _placedRooms.Add(_roomClone);
        _roomsPlaced++;

        if (WantedRooms == _roomsPlaced)
        {
            ScalateRooms();
        }
    }

    /// <summary>
    /// Check if the room position is fine
    /// </summary>
    /// <param name="roomDoorPosX"></param>
    /// <param name="roomDoorPosY"></param>
    /// <param name="roomConnectPosX"></param>
    /// <param name="roomConnectPosY"></param>
    /// <param name="newPosX"></param>
    /// <param name="newPosY"></param>
    private void CheckAddRoom(int roomDoorPosX, int roomDoorPosY, int roomConnectPosX, int roomConnectPosY, int newPosX, int newPosY)
    {
        //If this side has a door and the dungeon is still empty
        if (_dungeon[roomDoorPosX, roomDoorPosY] == 1 && _dungeon[roomConnectPosX, roomConnectPosY] == -1)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    //We mark this grid as 'to be built upon'
                    _dungeon[newPosX + i, newPosY + j] = 2;
                }
            }
            _roomPosQueue.Enqueue(new GridPos(newPosX, newPosY));
        }
    }

    /// <summary>
    /// Check if is it possible to have a room here that will connect to the other rooms already created
    /// </summary>
    /// <param name="room"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private bool CanPlaceRoom(BaseRoom room, int x, int y)
    {
        //If any side does not fit, we can't place
        if (!CheckSide(room, 0, 1, x - 1, y + 1))
            return false;
        if (!CheckSide(room, 1, 0, x + 1, y - 1))
            return false;
        if (!CheckSide(room, 2, 1, x + 3, y + 1))
            return false;
        if (!CheckSide(room, 1, 2, x + 1, y + 3))
            return false;

        return true;
    }

    /// <summary>
    /// Check every side of the room to check connections
    /// </summary>
    /// <param name="room"></param>
    /// <param name="roomDoorPosX"></param>
    /// <param name="roomDoorPosY"></param>
    /// <param name="roomConnectPosX"></param>
    /// <param name="roomConnectPosY"></param>
    /// <returns></returns>
    private bool CheckSide(BaseRoom room, int roomDoorPosX, int roomDoorPosY, int roomConnectPosX, int roomConnectPosY)
    {
        //If the connectingpart is a -1, it always fits
        float connectingPart = _dungeon[roomConnectPosX, roomConnectPosY];
        if (connectingPart == -1 || connectingPart == 2)
            return true;
        //If the connectingpart is the same as the door (or not door), it also fits 
        if (room.grid[roomDoorPosX, roomDoorPosY] == connectingPart)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Just a nice effect to see rooms scalating from small to the actual size
    /// </summary>
    private void ScalateRooms()
    {
        ///Make sure to change this variable
        int roomsFinalSize = 16;
        Vector3 finalScale = new Vector3(roomsFinalSize, roomsFinalSize, roomsFinalSize);
        foreach (BaseRoom room in _placedRooms)
        {
            room.gameObject.transform.DOScale(finalScale, 2f);
        }
    }
}
