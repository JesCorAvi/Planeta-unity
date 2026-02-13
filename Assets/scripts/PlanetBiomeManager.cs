using UnityEngine;
using System.Collections.Generic;

public class PlanetBiomeManager : MonoBehaviour
{
    [Header("Configuración General")]
    [SerializeField] private int planetResolution = 256;
    [SerializeField] private float noiseScale = 3f;
    [SerializeField] private float planetRadius = 10f;
    [SerializeField] private LayerMask planetLayer;
    [Range(0f, 0.2f)]
    [SerializeField] private float biomeEdgeThreshold = 0.05f;

    [Header("Bioma 1 (Bosque/Verde)")]
    [SerializeField] private Color colorBiome1 = Color.green;
    [SerializeField] private GameObject lootBiome1;
    [SerializeField] private GameObject[] treesBiome1;

    [Header("Bioma 2 (Desierto/Rojo)")]
    [SerializeField] private Color colorBiome2 = new Color(1f, 0.6f, 0f);
    [SerializeField] private GameObject lootBiome2;
    [SerializeField] private GameObject[] treesBiome2;

    [Header("Rocas Interactivas")]
    [SerializeField] private GameObject[] rockPrefabs;
    [SerializeField] private int numberOfRocks = 50;

    [Header("Vegetación (Árboles)")]
    [SerializeField] private int numberOfTrees = 30;

    private Texture2D biomeMap;
    private GravityAttractor myGravity;

    void Start()
    {
        myGravity = GetComponent<GravityAttractor>();

        if (GetComponent<MeshCollider>() == null)
        {
            gameObject.AddComponent<MeshCollider>();
        }

        GeneratePlanetTexture();
        Physics.SyncTransforms();

        GenerateRocks();
        GenerateTrees();
    }

    void GeneratePlanetTexture()
    {
        Renderer ren = GetComponent<Renderer>();
        biomeMap = new Texture2D(planetResolution, planetResolution);
        biomeMap.filterMode = FilterMode.Point;
        biomeMap.wrapMode = TextureWrapMode.Clamp;

        for (int y = 0; y < planetResolution; y++)
        {
            for (int x = 0; x < planetResolution; x++)
            {
                float xCoord = (float)x / planetResolution * noiseScale;
                float yCoord = (float)y / planetResolution * noiseScale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);

                Color pixelColor = (sample < 0.5f) ? colorBiome1 : colorBiome2;
                biomeMap.SetPixel(x, y, pixelColor);
            }
        }
        biomeMap.Apply();
        ren.material.mainTexture = biomeMap;
    }

    void GenerateRocks()
    {
        if (rockPrefabs == null || rockPrefabs.Length == 0) return;

        for (int i = 0; i < numberOfRocks; i++)
        {
            SpawnData spawnData = GetSpawnPointOnSurface();

            if (spawnData.hitFound)
            {
                GameObject selectedRockPrefab = rockPrefabs[Random.Range(0, rockPrefabs.Length)];
                GameObject newRock = Instantiate(selectedRockPrefab, spawnData.position, Quaternion.identity);

                AlignToPlanet(newRock);
                AttachGravity(newRock);

                RockInteraction rockScript = newRock.GetComponent<RockInteraction>();
                if (rockScript == null) rockScript = newRock.AddComponent<RockInteraction>();

                if (spawnData.isBiome1)
                {
                    rockScript.LootToSpawn = lootBiome1;
                }
                else
                {
                    rockScript.LootToSpawn = lootBiome2;
                }

                newRock.transform.parent = transform;
            }
        }
    }

    void GenerateTrees()
    {
        for (int i = 0; i < numberOfTrees; i++)
        {
            SpawnData spawnData = GetSpawnPointOnSurface();

            if (spawnData.hitFound)
            {
                GameObject treeToSpawn = null;

                if (spawnData.isBiome1)
                {
                    if (treesBiome1 != null && treesBiome1.Length > 0)
                        treeToSpawn = treesBiome1[Random.Range(0, treesBiome1.Length)];
                }
                else
                {
                    if (treesBiome2 != null && treesBiome2.Length > 0)
                        treeToSpawn = treesBiome2[Random.Range(0, treesBiome2.Length)];
                }

                if (treeToSpawn != null)
                {
                    GameObject newTree = Instantiate(treeToSpawn, spawnData.position, Quaternion.identity);

                    AlignToPlanet(newTree);
                    AttachGravity(newTree);

                    float randomScale = Random.Range(0.8f, 1.2f);
                    newTree.transform.localScale *= randomScale;

                    newTree.transform.parent = transform;
                }
            }
        }
    }

    struct SpawnData
    {
        public bool hitFound;
        public Vector3 position;
        public bool isBiome1;
    }

    SpawnData GetSpawnPointOnSurface()
    {
        SpawnData data = new SpawnData();
        data.hitFound = false;

        int maxAttempts = 10;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 randomDir = Random.onUnitSphere;
            Vector3 rayOrigin = transform.position + (randomDir * (planetRadius + 5f));
            Vector3 rayDirection = (transform.position - rayOrigin).normalized;

            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, rayDirection, out hit, 20f, planetLayer))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    Vector2 uv = hit.textureCoord;

                    float noiseVal = Mathf.PerlinNoise(uv.x * noiseScale, uv.y * noiseScale);

                    if (Mathf.Abs(noiseVal - 0.5f) < biomeEdgeThreshold)
                    {
                        continue;
                    }

                    data.hitFound = true;
                    data.position = hit.point;
                    data.isBiome1 = noiseVal < 0.5f;

                    return data;
                }
            }
        }

        return data;
    }

    void AlignToPlanet(GameObject obj)
    {
        Vector3 direction = (obj.transform.position - transform.position).normalized;
        obj.transform.up = direction;
    }

    void AttachGravity(GameObject obj)
    {
        GravityBody gb = obj.GetComponent<GravityBody>();
        if (gb == null) gb = obj.AddComponent<GravityBody>();
        gb.Attractor = myGravity;
    }
}