using UnityEngine;
using System.Collections;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class grid : MonoBehaviour
{
    //Every elemt from the rooms
    [Space]
    [Header("Level Prefabs")]
    public StartRoom startRoom;
    private BaseRoom roomClone;
    public BossRoom bossRoom;

    //variables that manage the maze
    [Space]
    [Header("Rooms Placed")]
    public int nRoomsPlaced = 0;
    public float buildDelay;
    public float startPosition;
    public int nRooms;
    public const int ROOM_SIZE = 3;
    public int[,] dungeon;

    //variables of the rooms
    [Space]
    [Header("List and Arrays of Rooms")]
    public NormalRoom[] rooms;
    private Queue<GridPos> roomPosQueue;
    public List<BaseRoom> placedRooms = new List<BaseRoom>();

    IEnumerator Start()
    {
        //Get the seed of the level
        //Random.InitState (1);

        dungeon = new int[nRooms * ROOM_SIZE, nRooms * ROOM_SIZE];
        for (int i = 0; i < nRooms * ROOM_SIZE; i++)
        {
            for (int j = 0; j < nRooms * ROOM_SIZE; j++)
            {
                dungeon[i, j] = -1;
            }
        }

        roomPosQueue = new Queue<GridPos>();

        int startX = nRooms / 2;
        int startY = nRooms / 2;

        roomPosQueue.Enqueue(new GridPos(startX * ROOM_SIZE, startY * ROOM_SIZE));

        nRoomsPlaced = 0;

        //We add new room positions to queue
        while (roomPosQueue.Count != 0)
        {
            GridPos pos = roomPosQueue.Dequeue();

            int randomIndex = Random.Range(0, rooms.Length - 1);
            PlaceRoom(randomIndex, pos.x, pos.y);


            if (buildDelay > 0)
            {
                yield return new WaitForSeconds(buildDelay);
            }
            if (nRoomsPlaced >= nRooms)
            {
                yield break;
            }
        }
    }

    void PlaceRoom(int roomIndex, int x, int y)
    {
        roomClone = null;
        if (nRoomsPlaced == 0)
        {
            roomClone = Instantiate(startRoom);
        }
        else
        {
            //We instantiate a roomClone. This might be destroyed later (not efficient but keeping it for now)
            if (nRoomsPlaced < nRooms - 1)
            {
                roomClone = Instantiate(rooms[roomIndex]);
            }
            else if (nRoomsPlaced == nRooms - 1)
            {
                Debug.Log ("I Placed the boss room");
                roomClone = Instantiate(bossRoom);
            }
        }

        //For safety, we break out of the array when all rooms (with all rotations) were checked. This should never happen though!
        int breakPoint = 0;
        int rotations = 0;

        //We try puzzling the room in
        while (!CanPlaceRoom(roomClone, x, y))
        {
            //We rotate the room 4 times
            rotations++;
            roomClone.Rotate();
            if (rotations >= 4)
            {
                //If the room doesn't fit after 4 rotations, we pick the next room
                rotations = 0;
                roomIndex++;
                if (roomIndex >= rooms.Length)
                {
                    roomIndex = 0;
                }
                //We destroy the old room, since it didn't fit
                Destroy(roomClone.gameObject);
                if (breakPoint > rooms.Length)
                {
                    roomIndex = 4;
                    //CDebug.Log ("Unity would have crashed...");
                    return;
                }
                roomClone = Instantiate(rooms[roomIndex]).GetComponent<BaseRoom>();
                breakPoint++;
            }
        }

        //At this point we have found a fitting room, and we can enter the data in our grid
        for (int i = 0; i < ROOM_SIZE; i++)
        {
            for (int j = 0; j < ROOM_SIZE; j++)
            {
                dungeon[x + i, y + j] = roomClone.grid[i, j];
            }
        }

        //We place the room at the correct position
        roomClone.transform.position = new Vector3((x - nRooms * .5f) * 54, 0, (y - nRooms * .5f) * 54);

        //TopDoor
        CheckAddRoom(x, y + 1, x - 1, y + 1, x - 3, y);
        //LeftDoor
        CheckAddRoom(x + 1, y, x + 1, y - 1, x, y - 3);
        //BottomDoor
        CheckAddRoom(x + 2, y + 1, x + 3, y + 1, x + 3, y);
        //RightDoor
        CheckAddRoom(x + 1, y + 2, x + 1, y + 3, x, y + 3);

        placedRooms.Add(roomClone);
        nRoomsPlaced++;

        if (nRooms == nRoomsPlaced)
        {
            MakeItCool();
        }
    }

    void CheckAddRoom(int roomDoorPosX, int roomDoorPosY, int roomConnectPosX, int roomConnectPosY, int newPosX, int newPosY)
    {
        //If this side has a door and the dungeon is still empty
        if (dungeon[roomDoorPosX, roomDoorPosY] == 1 && dungeon[roomConnectPosX, roomConnectPosY] == -1)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    //We mark this grid as 'to be built upon'
                    dungeon[newPosX + i, newPosY + j] = 2;
                }
            }
            roomPosQueue.Enqueue(new GridPos(newPosX, newPosY));
        }
    }

    bool CanPlaceRoom(BaseRoom room, int x, int y)
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

    bool CheckSide(BaseRoom room, int roomDoorPosX, int roomDoorPosY, int roomConnectPosX, int roomConnectPosY)
    {
        //If the connectingpart is a -1, it always fits
        float connectingPart = dungeon[roomConnectPosX, roomConnectPosY];
        if (connectingPart == -1 || connectingPart == 2)
            return true;
        //If the connectingpart is the same as the door (or not door), it also fits 
        if (room.grid[roomDoorPosX, roomDoorPosY] == connectingPart)
        {
            return true;
        }
        return false;
    }

    void MakeItCool()
    {
        int roomsFinalSize = 32;
        Vector3 finalScale = new Vector3(roomsFinalSize, roomsFinalSize, roomsFinalSize);
        foreach (BaseRoom room in placedRooms)
        {
            room.gameObject.transform.DOScale(finalScale, 2f);
        }
    }
}

public class GridPos
{

    public GridPos(int _x, int _y)
    {
        x = _x;
        y = _y;
    }

    public int x;
    public int y;
}
