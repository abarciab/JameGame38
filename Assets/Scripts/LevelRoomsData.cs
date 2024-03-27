using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelRoomsData
{
    public RoomController BossRoom;
    public List<RoomController> Rooms = new List<RoomController>();
}
