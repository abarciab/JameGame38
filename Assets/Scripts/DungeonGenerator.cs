using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] private List<LevelRoomsData> _levels;
    [SerializeField] private Transform _dungeonParent;
    [SerializeField] private int _numRooms = 10;
    private List<Vector3> _unusedExits = new List<Vector3>();
    private int _currentLevel;

    private void Start()
    {
        ClearAndGenerate();
    }

    [ButtonMethod]
    private void ClearAndGenerate()
    {
        Clear();
        _unusedExits.Add(_dungeonParent.position);
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
        _currentLevel = 0;
    }

    private void Generate()
    {
        for (int i = 0; i < _numRooms; i++) {
            var rooms = _levels[_currentLevel].Rooms;
            PlaceRoom(rooms[Random.Range(0, rooms.Count)]);
        }
        PlaceRoom(_levels[_currentLevel].BossRoom);
        _currentLevel += 1;
        if (_levels.Count > _currentLevel) Generate();
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
