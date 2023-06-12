using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IndirectInstancing))]
public class MapManager : MonoBehaviour
{
    #region CONST
    #endregion

    #region Editor Params
    [Header("EDITOR PARAMS")]
    public Tile tilePrefab;
    public Map mapPrefab;
    public List<Texture2D> mapTextures;
    public int mapsPerLevel;
    public Transform mapsParent;
    public Transform tilesPoolTransform;
    public float spaceBetweenTiles;
    public float mapWidth;
    #endregion

    #region Params
    private static MapManager instance;
    IndirectInstancing indirectInstancing;
    List<Tile> poolTilesRecycle = new List<Tile>();

    [Header("PARAMS")]
    public List<Tile> listTiles;
    public Vector4[] posTiles;
    public Vector4[] scaleTiles;
    public Vector4[] colorTiles;
    public List<Texture2D> randMaps = new List<Texture2D>();
    public List<bool> moveMaps = new List<bool>();
    public float mapHeight;
    public float scaleValue;
    Vector3 scale;
    int id;
    #endregion

    #region Properties
    public static MapManager Instance { get => instance; set => instance = value; }
    #endregion

    #region Events
    #endregion

    #region Methods

    private void Awake()
    {
        instance = this;
        indirectInstancing = GetComponent<IndirectInstancing>();
    }

    private void Update()
    {
        if (mapsParent.hasChanged)
        {
            InitPostionBuffer();
            mapsParent.hasChanged = false;
        }
    }

    void AdaptMapWidth()
    {
        float cameraAspect = Camera.main.aspect;
        if (cameraAspect <= 0.465f) //9:19.5
        {
            mapWidth = 10f;
        }
        else
        if (cameraAspect <= 0.474f) //9:19
        {
            mapWidth = 10f;
        }
        else
        if (cameraAspect <= 0.5f)   //9:18
        {
            mapWidth = 10f;
        }
        else
        if (cameraAspect <= 0.5625f)//9:16
        {
            mapWidth = 12f;
        }
        else
        if (cameraAspect <= 0.75f)  //3:4
        {
            mapWidth = 16f;
        }
    }

    void RandomMaps()
    {
        UpdateMapsPerLevel();
        randMaps.Clear();
        moveMaps.Clear();
        mapTextures.Shuffle();
        for (int i = 0; i < mapsPerLevel; i++)
        {
            randMaps.Add(mapTextures[i]);
            if (i % 2 == 0)
            {
                moveMaps.Add(false);
            }
            else
            {
                moveMaps.Add(true);
            }
        }
    }

    public void UpdateMapsPerLevel()
    {
        mapsPerLevel = 5 + (int)(DataManager.Instance.currentLevel / 5);
        mapsPerLevel = (int)Mathf.Clamp(mapsPerLevel, 5, 15);
    }

    [NaughtyAttributes.Button]
    public void SpawnLevel()
    {
        AdaptMapWidth();
        ReclaimAllTiles();
        RandomMaps();
        for (int i = 0; i < randMaps.Count; i++)
        {
            Map map = Instantiate(mapPrefab);
            map.Initialize(
                new Vector3(0f, 0f, i * (mapHeight + scaleValue)),
                mapsParent,
                moveMaps[i],
                Random.value < 0.5f ? true : false,
                Random.Range(1f, 2f)
            );

            GenerateMap(randMaps[i], map.transform);
            indirectInstancing.UpdateMesh(indirectInstancing.instanceMesh);
            InitBuffers();
            map.InitTiles();
        }
    }

    [NaughtyAttributes.Button]
    public void RespawnLevel()
    {
        AdaptMapWidth();
        ReclaimAllTiles();
        for (int i = 0; i < randMaps.Count; i++)
        {
            Map map = Instantiate(mapPrefab);
            map.Initialize(
                    new Vector3(0f, 0f, i * (mapHeight + scaleValue)),
                    mapsParent,
                    moveMaps[i],
                    Random.value < 0.5f ? true : false,
                    Random.Range(1f, 2f)
                );

            GenerateMap(randMaps[i], map.transform);
            indirectInstancing.UpdateMesh(indirectInstancing.instanceMesh);
            InitBuffers();
            map.InitTiles();
        }
    }

    [NaughtyAttributes.Button]
    public void InitBuffers()
    {
        int count = listTiles.Count;
        posTiles = new Vector4[count];
        colorTiles = new Vector4[count];
        scaleTiles = new Vector4[count];
        for (int i = 0; i < count; i++)
        {
            Tile tile = listTiles[i];
            posTiles[i] = tile.transform.position;
            colorTiles[i] = tile.currentColor;
            scaleTiles[i] = scale;
        }
        if (indirectInstancing != null)
        {
            indirectInstancing.UpdateBuffers();
        }
    }

    public void InitPostionBuffer()
    {
        int count = listTiles.Count;
        posTiles = new Vector4[count];
        for (int i = 0; i < count; i++)
        {
            Tile tile = listTiles[i];
            posTiles[i] = tile.transform.position;
        }
        if (indirectInstancing != null)
        {
            indirectInstancing.UpdateBuffers();
        }
    }

    public void GenerateMap(Texture2D texture, Transform parent)
    {
        float textureRatio = (float)texture.width / (float)texture.height;
        mapHeight = mapWidth / textureRatio;
        float ratio = mapWidth / (float)texture.width;
        scaleValue = ratio * (1f - spaceBetweenTiles);
        this.scale = new Vector3(scaleValue, 0.5f, scaleValue);

        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                GenerateTile(texture, x, y, ratio, spaceBetweenTiles, parent);
            }
        }
    }

    void GenerateTile(Texture2D texture, int x, int y, float ratio, float spaceBetweenTiles, Transform parent)
    {
        Color pixelColor = texture.GetPixel(x, y);
        Vector3 pos = new Vector3((float)x * ratio - mapWidth * 0.5f + scale.x * 0.5f, 0f, (float)y * ratio + parent.position.z);
        Tile instance;

        if (pixelColor.a == 0)
        {
            return;
        }

        int lastIndex = poolTilesRecycle.Count - 1;
        if (lastIndex >= 0)
        {
            instance = poolTilesRecycle[lastIndex];
            poolTilesRecycle.RemoveAt(lastIndex);
            instance.gameObject.SetActive(true);
        }
        else
        {
            instance = Instantiate(tilePrefab);
        }

        string tempTag = pixelColor == Color.white ? "Tile" : "Obstacle";

        instance.transform.SetParent(parent, false);
        instance.Initialize(id, tempTag, pos, scale, pixelColor);
        listTiles.Add(instance);
        id++;
    }

    public void SetScaleZero(int id)
    {
        scaleTiles[id] = new Vector4(0f, 0f, 0f);
        indirectInstancing.UpdateBuffers();
    }

    public void SetScaleNormal(int id)
    {
        scaleTiles[id] = new Vector4(scale.x, scale.y, scale.z);
        indirectInstancing.UpdateBuffers();
    }

    public void HideTile(Tile tile)
    {
        poolTilesRecycle.Add(tile);
    }

    public void Reclaim(Tile tile)
    {
        poolTilesRecycle.Add(tile);
        tile.gameObject.SetActive(false);
        tile.transform.SetParent(tilesPoolTransform);
    }

    [NaughtyAttributes.Button]
    public void ReclaimAllTiles()
    {
        poolTilesRecycle.Clear();
        foreach (Tile tile in listTiles)
        {
            Reclaim(tile);
        }
        listTiles.Clear();
        foreach (Transform map in mapsParent)
        {
            Destroy(map.gameObject);
        }
        id = 0;
    }

    #endregion

    #region DEBUG
    #endregion
}
