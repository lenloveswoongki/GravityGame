
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GravityManager gravityManager;
    [SerializeField] private PlayerInputController _playerInputController;
    public PlayerInputController GetPlayerInputController {  get { return _playerInputController; } }

    [Header("Global References")]
    public LevelManager levelManager;
    public static GameManager Instance { get; private set; }
    public PlayableCat player { get; private set; }

    [Header("Prefabs")]
    public PlayableCat PlayerPrefab;
    private void Awake()
    {
        if (GameManager.Instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        _playerInputController.Init();
        gravityManager.Init();
    }
    private void Update()
    {
        _playerInputController.Update();
    }
    private void FixedUpdate()
    {
        _playerInputController.FixedUpdate();
    }

    public void SpawnPlayer(GameObject SpawnPos)
    {       
        player = Instantiate(PlayerPrefab.gameObject, SpawnPos.transform).GetComponent<PlayableCat>();
    }
    public void SetTimer(float duration, Action method)
    {
        StartCoroutine(SetTimer_Corroutine(duration, method));
    }

    private IEnumerator SetTimer_Corroutine(float duration, Action method)
    {
        yield return new WaitForSeconds(duration);
        method?.Invoke();
    }

    public void LoadLevel(string targetLevelName)
    {
        SceneManager.LoadScene(targetLevelName);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
