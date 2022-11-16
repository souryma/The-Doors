using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject room;
    private void OnTriggerEnter(Collider other)
    {
        room.GetComponent<Room>().DoorHasBeenTouched();
    }
}
