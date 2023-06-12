using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region Editor Params
    public float moveSpeed;
    public bool reverseDirection;
    public float rangeX;
    public float startTime;
    public bool directionLR;
    #endregion

    #region Params

    #endregion

    #region Properties
    #endregion

    #region Events
    #endregion

    #region Methods

    private void Update()
    {
        var temp = transform.position;
        if (directionLR == true)
        {
            temp.x += moveSpeed * Time.deltaTime;
            transform.position = temp;
            if (transform.position.x >= rangeX)
            {
                directionLR = false;
            }
        }
        if (directionLR == false)
        {
            temp.x -= moveSpeed * Time.deltaTime;
            transform.position = temp;
            if (transform.position.x <= -rangeX)
            {
                directionLR = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Tile") || other.gameObject.CompareTag("Obstacle"))
        {
            Tile temp = other.gameObject.GetComponent<Tile>();
            MapManager.Instance.SetScaleZero(temp.id);
            MapManager.Instance.HideTile(temp);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Tile") || other.gameObject.CompareTag("Obstacle"))
        {
            Tile temp = other.gameObject.GetComponent<Tile>();
            MapManager.Instance.SetScaleNormal(temp.id);
        }
    }

    #endregion
}
