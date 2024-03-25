using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] private List<RoomController> _rooms = new List<RoomController>();

    [ButtonMethod]
    private void GenerateDungeon()
    {
    }
}
