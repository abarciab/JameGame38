using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] private List<RoomController> _rooms = new List<RoomController>();
    [SerializeField] private RoomController _bossRoom;
    [SerializeField] private Transform _dungeonParent;
    [SerializeField] private int _numRooms = 10;
    private List<Vector3> _unusedExits = new List<Vector3>();

    [ButtonMethod]
    private void ClearAndGenerate()
    {
        Clear();
        Generate();
    }

    [ButtonMethod]
    private void Clear()
    {
        for (int i = _dungeonParent.childCount - 1; i >= 0; i--) {
            if (Application.isPlaying) Destroy(_dungeonParent.GetChild(i).gameObject);
            else DestroyImmediate(_dungeonParent.GetChild(i).gameObject);
        }
        _unusedExits.Clear();
    }

    private void Generate()
    {
        _unusedExits.Add(_dungeonParent.position);
        for (int i = 0; i < _numRooms; i++) {
            PlaceRoom(_rooms[Random.Range(0, _rooms.Count)]);
        }
        PlaceRoom(_bossRoom);
    }

    private void PlaceRoom(RoomController toPlace)
    {
        if (_unusedExits.Count == 0) return;
        var pos = _unusedExits[0];
        _unusedExits.RemoveAt(0);
        var placedRoom = Instantiate(toPlace.gameObject, _dungeonParent);
        var room = placedRoom.GetComponent<RoomController>();
        placedRoom.transform.position = pos - room.Entrance.localPosition;
        _unusedExits.Add(room.Exit.position);
    }
}
