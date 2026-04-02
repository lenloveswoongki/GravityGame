using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject PlayerPos;

    [Header("Debug")]
    public PlayableCat PlayerPrefab;
    public GameManager GameManagerDebug;
    private void Start()
    {
        try
        {
            GameManager.Instance.SpawnPlayer(PlayerPos);
        }
        catch (NullReferenceException)
        {
            Instantiate(GameManagerDebug).SpawnPlayer(PlayerPos);
        }
    }
}
