using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour
{
    #region CONST
    #endregion

    #region EDITOR PARAMS
    public GameObject[] prefabs;
    public Transform FXParent;
    #endregion

    #region PARAMS
    private static FXManager instance;
    List<GameObject> poolBreakPS = new List<GameObject>();
    #endregion

    #region PROPERTIES
    public static FXManager Instance { get => instance; private set => instance = value; }
    #endregion

    #region EVENTS
    #endregion

    #region METHODS

    private void Awake()
    {
        instance = this;
    }

    public GameObject Get(int id)
    {
        GameObject instance;
        int lastIndex = poolBreakPS.Count - 1;
        if (lastIndex >= 0)
        {
            instance = poolBreakPS[lastIndex];
            instance.gameObject.SetActive(true);
            poolBreakPS.RemoveAt(lastIndex);
        }
        else
        {
            instance = Instantiate(prefabs[id]);
        }
        return instance;
    }

    public void Spawn(int id, Vector3 pos, Color color)
    {
        GameObject temp = Get(id);
        temp.transform.SetParent(FXParent);
        temp.transform.position = pos;
        SetColor(temp, color);
        ReturnToPool(temp);
    }

    public void SetColor(GameObject go, Color color)
    {
        ParticleSystem ps = go.GetComponent<ParticleSystem>();
        if (ps == null)
            return;
        ParticleSystem.MainModule main = ps.main;
        main.startColor = color;
    }

    void ReturnToPool(GameObject go)
    {
        StartCoroutine(C_ReturnToPool(go));
    }

    IEnumerator C_ReturnToPool(GameObject go)
    {
        yield return new WaitForSeconds(1f);
        Reclaim(go);
    }

    public void Reclaim(GameObject goToRecycle)
    {
        poolBreakPS.Add(goToRecycle);
        goToRecycle.SetActive(false);
    }

    #endregion
}
