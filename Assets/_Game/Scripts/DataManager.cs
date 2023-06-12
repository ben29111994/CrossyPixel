using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    #region CONST
    public const string LEVEL = "LEVEL";
    public const string SCORE = "SCORE";
    #endregion

    #region Editor Params
    #endregion

    #region Params
    private static DataManager instance;
    public int currentLevel;
    public int currentScore;
    #endregion

    #region Properties
    public static DataManager Instance { get => instance; private set => instance = value; }
    #endregion

    #region Events
    #endregion

    #region Methods

    private void Awake()
    {
        instance = this;
        LoadData();
    }

    public void LoadData()
    {
        currentLevel = PlayerPrefs.GetInt(LEVEL);
        currentScore = PlayerPrefs.GetInt(SCORE);
    }

    public void SaveData()
    {
        PlayerPrefs.SetInt(LEVEL, currentLevel);
        PlayerPrefs.SetInt(SCORE, currentScore);
    }

    #endregion
}
