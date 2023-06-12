using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Editor Params
    [Header("SCREENS")]
    public GameObject Menu;
    public GameObject InGame;
    public GameObject WinGame;
    public GameObject LoseGame;

    [Header("UI ELEMENTS")]
    public Button btnPlay;
    public Text txtLevelL;
    public Text txtLevelR;
    public Text txtScore;
    #endregion

    #region Params
    #endregion

    #region Properties
    #endregion

    #region Events
    #endregion

    #region Methods

    private void OnEnable()
    {
        RegisterEvents();
    }

    private void OnDisable()
    {
        UnregisterEvents();
    }

    private void RegisterEvents()
    {
        GameManager.OnStateChanged += OnGameStateChanged;
        btnPlay.onClick.AddListener(OnBtnPlayClicked);
    }

    private void UnregisterEvents()
    {
        GameManager.OnStateChanged -= OnGameStateChanged;
        btnPlay.onClick.RemoveListener(OnBtnPlayClicked);
    }

    public void OnBtnPlayClicked()
    {
        Menu.SetActive(false);
        GameManager.Instance.ChangeState(GameState.INGAME);
    }

    public void OnGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.MENU:
                OnMenu();
                break;
            case GameState.INGAME:
                OnInGame();
                break;
            case GameState.WINGAME:
                OnWinGame();
                break;
            case GameState.LOSEGAME:
                OnLoseGame();
                break;
            default:
                break;
        }
        UpdateLevel();
    }

    private void Update()
    {
        UpdateScore();
    }

    public void OnMenu()
    {
        Menu.SetActive(true);
        InGame.SetActive(true);
        WinGame.SetActive(false);
        LoseGame.SetActive(false);
    }
    public void OnInGame()
    {
        Menu.SetActive(false);
        InGame.SetActive(true);
        WinGame.SetActive(false);
        LoseGame.SetActive(false);
    }
    public void OnWinGame()
    {
        Menu.SetActive(false);
        WinGame.SetActive(true);
        LoseGame.SetActive(false);
    }
    public void OnLoseGame()
    {
        Menu.SetActive(false);
        WinGame.SetActive(false);
        LoseGame.SetActive(true);
    }

    public void UpdateLevel()
    {
        txtLevelL.text = DataManager.Instance.currentLevel.ToString();
        txtLevelR.text = (DataManager.Instance.currentLevel + 1).ToString();
    }

    public void UpdateScore()
    {
        txtScore.text = DataManager.Instance.currentScore.ToString();
    }

    #endregion
}
