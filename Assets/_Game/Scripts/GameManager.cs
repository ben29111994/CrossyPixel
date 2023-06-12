using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    NULL, MENU, INGAME, WINGAME, LOSEGAME
}

public class GameManager : MonoBehaviour
{
    #region Editor Params
    public Player player;
    #endregion

    #region Params
    private static GameManager instance;
    public GameState currentState;
    #endregion

    #region Properties
    public static GameManager Instance { get => instance; private set => instance = value; }
    #endregion

    #region Events
    public static event System.Action<GameState> OnStateChanged;
    #endregion

    #region Methods

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Application.targetFrameRate = 300;
        StartGame();
    }

    public void StartGame()
    {
        ChangeState(GameState.MENU);
        MapManager.Instance.SpawnLevel();
        player.gameObject.SetActive(true);
        player.Initialize();
    }

    public void PlayGame()
    {
        ChangeState(GameState.INGAME);
        MapManager.Instance.SpawnLevel();
        player.gameObject.SetActive(true);
        player.Initialize();
    }

    public void RestartGame()
    {
        ChangeState(GameState.INGAME);
        MapManager.Instance.RespawnLevel();
        player.gameObject.SetActive(true);
        player.Initialize();
    }

    public void WinGame()
    {
        ChangeState(GameState.WINGAME);
        DataManager.Instance.currentLevel++;
        DataManager.Instance.SaveData();
    }

    public void LoseGame()
    {
        ChangeState(GameState.LOSEGAME);
    }

    public void ChangeState(GameState state)
    {
        currentState = state;
        if (OnStateChanged != null)
        {
            OnStateChanged(state);
        }
    }

    #endregion
}
