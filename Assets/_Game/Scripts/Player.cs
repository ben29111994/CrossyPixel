using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    #region Editor Params
    public float speedHori;
    public float moveVertDuration;
    #endregion

    #region Params
    MapManager mapManager;
    bool back = true;
    bool isCrossing = false;
    float rangeX;
    float endPoint;
    #endregion

    #region Properties
    #endregion

    #region Events
    #endregion

    #region Methods

    private void Start()
    {
        mapManager = MapManager.Instance;
    }

    public void Initialize()
    {
        transform.position = new Vector3(0f, 0f, mapManager.mapsParent.position.z - (mapManager.scaleValue + mapManager.spaceBetweenTiles * 0.25f));
        transform.localScale = new Vector3(mapManager.scaleValue, 1f, mapManager.scaleValue);
        rangeX = (mapManager.mapWidth - mapManager.scaleValue) * 0.5f;
        isCrossing = false;
        endPoint = (mapManager.mapHeight + mapManager.scaleValue) * mapManager.mapsPerLevel - (mapManager.scaleValue + mapManager.spaceBetweenTiles * 0.5f);
    }

    private void Update()
    {
        if (GameManager.Instance.currentState == GameState.INGAME)
        {
#if UNITY_EDITOR
            if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0))
            {
                if (!isCrossing)
                {
                    MoveVertical();
                }
            }
            if (!isCrossing)
            {
                MoveHorizontal();
            }
#elif UNITY_IOS || UNITY_ANDROID
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (!IsPointerOverUIObject() && (touch.phase == TouchPhase.Began))
                {
                    if (!isCrossing)
                    {
                        MoveVertical();
                    }
                }
            }
            if (!isCrossing)
            {
                MoveHorizontal();
            }
#endif
        }
    }

    public void MoveHorizontal()
    {
        var temp = transform.position;
        if (back == true)
        {
            temp.x += speedHori * Time.deltaTime;
            transform.position = temp;
            if (transform.position.x >= rangeX)
            {
                back = false;
            }
        }
        if (back == false)
        {
            temp.x -= speedHori * Time.deltaTime;
            transform.position = temp;
            if (transform.position.x <= -rangeX)
            {
                back = true;
            }
        }
    }

    public void MoveVertical()
    {
        Vector3 tempPos = transform.position;
        Vector3 target = new Vector3(tempPos.x, tempPos.y, tempPos.z + mapManager.mapHeight + mapManager.scaleValue);
        StartCoroutine(C_MoveTowards(transform, target, moveVertDuration));
    }

    IEnumerator C_MoveTowards(Transform objectToMove, Vector3 toPosition, float duration)
    {
        isCrossing = true;
        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            Vector3 currentPos = objectToMove.position;
            float time = Vector3.Distance(currentPos, toPosition) / (duration - counter) * Time.deltaTime;
            objectToMove.position = Vector3.MoveTowards(currentPos, toPosition, time);
            yield return null;
        }
        isCrossing = false;

        if (transform.position.z >= endPoint)
        {
            GameManager.Instance.WinGame();
        }
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
#if UNITY_EDITOR
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
#elif UNITY_IOS || UNITY_ANDROID
        eventDataCurrentPosition.position = new Vector2(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
#endif
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            gameObject.SetActive(false);
            GameManager.Instance.LoseGame();
        }
    }

    #endregion

}
