using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    #region Editor Params
    public bool move;
    public float moveSpeed;
    public float range;
    public bool LR;
    #endregion

    #region Params
    private MeshRenderer rend;
    private MaterialPropertyBlock propColor;
    public Color currentColor;
    public int id;
    #endregion

    #region Properties
    #endregion

    #region Events
    #endregion

    #region Methods
    private void Awake()
    {
        rend = GetComponent<MeshRenderer>();
        propColor = new MaterialPropertyBlock();
    }

    public void Initialize(int id, string tag, Vector3 pos, Vector3 scale, Color inputColor)
    {
        this.id = id;
        gameObject.tag = tag;
        transform.position = pos;
        transform.localScale = scale;
        currentColor = inputColor;
        move = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            FXManager.Instance.Spawn(0, transform.position, currentColor);
            MapManager.Instance.SetScaleZero(this.id);
            MapManager.Instance.Reclaim(this);
            MoreMountains.NiceVibrations.MMVibrationManager.VibrateLight();
            DataManager.Instance.currentScore++;
        }
    }

    public void InitMoveProperties(float moveSpeed, float range, bool LR = true)
    {
        this.moveSpeed = moveSpeed;
        this.range = range;
        this.LR = LR;
    }

    public void Move()
    {
        if (LR)
        {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
            if (transform.position.x >= range)
            {
                transform.position = new Vector3(-range, transform.position.y, transform.position.z);
            }
        }
        else
        {
            transform.Translate(-Vector3.right * moveSpeed * Time.deltaTime);
            if (transform.position.x <= -range)
            {
                transform.position = new Vector3(range, transform.position.y, transform.position.z);
            }
        }
    }

    private void Update()
    {
        if (move)
        {
            Move();
        }
    }

    #endregion
}
