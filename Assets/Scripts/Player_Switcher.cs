using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Switcher : MonoBehaviour
{
    [SerializeField] private GameObject[] playerPrefabs;
    [SerializeField] private GameObject currentlySelectedPrefab;
    [SerializeField] private PlayerInputManager PIM;

    public void playerSwitch()
    {
        if (currentlySelectedPrefab == playerPrefabs[0])
        {
            currentlySelectedPrefab = playerPrefabs[1];
            PIM.playerPrefab = currentlySelectedPrefab;
        }
        else if (currentlySelectedPrefab == playerPrefabs[1])
        {
            Debug.Log("Cannot join. Max players have already joined");

        }

    }
}
