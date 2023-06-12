using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    #region Editor Params
    public bool move;
    public bool LR;
    public float moveSpeed;
    #endregion

    #region Params
    public List<Tile> tiles = new List<Tile>();
    #endregion

    #region Properties
    #endregion

    #region Events
    #endregion

    #region Methods

    public void Initialize(Vector3 pos, Transform parent, bool move, bool LR, float moveSpeed = 1f)
    {
        transform.position = pos;
        transform.SetParent(parent);
        tiles.Clear();
        this.move = move;
        this.LR = LR;
        this.moveSpeed = moveSpeed;
    }

    public void InitTiles()
    {
        tiles.Clear();
        foreach (Transform item in transform)
        {
            Tile tile = item.GetComponent<Tile>();
            this.tiles.Add(tile);
            tile.InitMoveProperties(moveSpeed, MapManager.Instance.mapWidth * 0.5f, LR);
        }
        SetMoveTiles(move, LR);
    }

    public void SetMoveTiles(bool move, bool LR)
    {
        foreach (Tile tile in tiles)
        {
            tile.move = move;
            tile.LR = LR;
        }
    }

    private void Update()
    {
        if (move)
        {
            MapManager.Instance.InitPostionBuffer();
        }
    }

    #endregion
}
