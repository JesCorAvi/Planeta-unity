using UnityEngine;
using System.Collections.Generic;

public class PlanetBiomeManager : MonoBehaviour
{
    [Header("Configuración General")]
    public int planetResolution = 256;
    public float noiseScale = 3f;
    public float planetRadius = 10f;
    public LayerMask planetLayer = ~0;

    [Header("Bioma 1 (Bosque/Verde)")]
    public Color colorBiome1 = Color.green;
    public GameObject lootBiome1;
    public GameObject[] treesBiome1; // Arrastra aquí: Fir_Tree, Oak_Tree, etc.

    [Header("Bioma 2 (Desierto/Rojo)")]
    public Color colorBiome2 = new Color(1f, 0.6f, 0f);
    public GameObject lootBiome2;
    public GameObject[] treesBiome2; // Arrastra aquí: Palm_Tree, Poplar_Tree, etc.

    [Header("Rocas Interactivas")]
    public GameObject[] rockPrefabs; // Arrastra aquí tus prefabs SM_Rocks_01, SM_Rocks_02...
    public int numberOfRocks = 50;

    [Header("Vegetación (Árboles)")]
    public int numberOfTrees = 30; // Cantidad de árboles a generar

    private Texture2D biomeMap;
    private GravityAttractor myGravity;

    void Start()
    {
        myGravity = GetComponent<GravityAttractor>();
        if (GetComponent<MeshCollider>() == null)
        {
            Debug.LogError("¡Necesitas un MeshCollider en el planeta para que esto sea preciso!");
            gameObject.AddComponent<MeshCollider>();
        }

        GeneratePlanetTexture();

        // Es importante sincronizar las físicas antes de lanzar rayos
        Physics.SyncTransforms();

        GenerateRocks(); // Genera las rocas interactivas
        GenerateTrees(); // Genera los árboles decorativos
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

    // Genera las rocas usando los assets de JC_StylizedRocks
    void GenerateRocks()
    {
        if (rockPrefabs == null || rockPrefabs.Length == 0)
        {
            Debug.LogWarning("No has asignado prefabs de rocas en el inspector.");
            return;
        }

        for (int i = 0; i < numberOfRocks; i++)
        {
            Vector3 spawnPos = GetRandomPositionOnSurface();
            Quaternion rotation = Quaternion.identity;

            // 1. Elegir un modelo de roca aleatorio del array
            GameObject selectedRockPrefab = rockPrefabs[Random.Range(0, rockPrefabs.Length)];

            GameObject newRock = Instantiate(selectedRockPrefab, spawnPos, rotation);

            // 2. Alinear con la gravedad
            AlignToPlanet(newRock);

            // 3. Configurar lógica interna (Loot según bioma)
            BiomeData biomeData = GetBiomeDataAtPosition(spawnPos);

            RockInteraction rockScript = newRock.GetComponent<RockInteraction>();
            if (rockScript == null) rockScript = newRock.AddComponent<RockInteraction>();

            // Asignar el loot correspondiente al color del suelo
            if (biomeData.isBiome1)
                rockScript.lootToSpawn = lootBiome1;
            else
                rockScript.lootToSpawn = lootBiome2;

            // 4. Asegurar gravedad
            AttachGravity(newRock);
        }
    }

    // Nueva función para generar árboles según el bioma
    void GenerateTrees()
    {
        for (int i = 0; i < numberOfTrees; i++)
        {
            Vector3 spawnPos = GetRandomPositionOnSurface();

            // Verificamos en qué bioma estamos
            BiomeData biomeData = GetBiomeDataAtPosition(spawnPos);
            GameObject treeToSpawn = null;

            if (biomeData.isBiome1)
            {
                // Estamos en Bioma 1 (Bosque)
                if (treesBiome1 != null && treesBiome1.Length > 0)
                {
                    treeToSpawn = treesBiome1[Random.Range(0, treesBiome1.Length)];
                }
            }
            else
            {
                // Estamos en Bioma 2 (Desierto)
                if (treesBiome2 != null && treesBiome2.Length > 0)
                {
                    treeToSpawn = treesBiome2[Random.Range(0, treesBiome2.Length)];
                }
            }

            if (treeToSpawn != null)
            {
                GameObject newTree = Instantiate(treeToSpawn, spawnPos, Quaternion.identity);
                AlignToPlanet(newTree);
                AttachGravity(newTree);

                // Opcional: Variación de tamaño para que se vea más natural
                float randomScale = Random.Range(0.8f, 1.2f);
                newTree.transform.localScale *= randomScale;
            }
        }
    }

    // --- Funciones Auxiliares ---

    Vector3 GetRandomPositionOnSurface()
    {
        Vector3 randomDir = Random.onUnitSphere;
        return transform.position + (randomDir * planetRadius);
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
        gb.attractor = myGravity;
    }

    // Estructura simple para devolver datos del bioma
    struct BiomeData
    {
        public bool isBiome1; // True = Bioma 1, False = Bioma 2
        public Color colorDetected;
    }

    BiomeData GetBiomeDataAtPosition(Vector3 pos)
    {
        BiomeData data = new BiomeData();
        data.isBiome1 = false; // Por defecto bioma 2

        RaycastHit hit;
        Vector3 dirToCenter = (transform.position - pos).normalized;
        Vector3 rayOrigin = pos - (dirToCenter * 5.0f); // Retrocedemos un poco para lanzar el rayo hacia el planeta

        if (Physics.Raycast(rayOrigin, dirToCenter, out hit, 20f, planetLayer))
        {
            if (hit.collider.gameObject == gameObject)
            {
                Vector2 uv = hit.textureCoord;
                Color surfaceColor = biomeMap.GetPixelBilinear(uv.x, uv.y);
                data.colorDetected = surfaceColor;

                // Comparamos si el color se parece al del Bioma 1
                if (IsColorSimilar(surfaceColor, colorBiome1))
                {
                    data.isBiome1 = true;
                }
            }
        }
        return data;
    }

    bool IsColorSimilar(Color a, Color b)
    {
        float diff = Mathf.Abs(a.r - b.r) + Mathf.Abs(a.g - b.g) + Mathf.Abs(a.b - b.b);
        return diff < 0.2f; // Margen de error un poco más amplio
    }
}