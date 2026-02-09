using UnityEngine;

public class PlanetBiomeManager : MonoBehaviour
{
    [Header("Configuración General")]
    public int planetResolution = 256;
    public float noiseScale = 3f;
    public float planetRadius = 10f;

    [Header("Bioma 1 (Bosque)")]
    public Color colorBiome1 = Color.green;
    public GameObject lootBiome1;

    [Header("Bioma 2 (Desierto)")]
    public Color colorBiome2 = new Color(1f, 0.6f, 0f); 
    public GameObject lootBiome2;

    [Header("Rocas")]
    public GameObject rockPrefab;
    public Color rockColor = Color.gray;
    public int numberOfRocks = 50;
    
    public LayerMask planetLayer = ~0; 

    private Texture2D biomeMap;

    void Start()
    {
        GeneratePlanetTexture();
        Physics.SyncTransforms();
        GenerateRocks();
    }

    void GeneratePlanetTexture()
    {
        Renderer ren = GetComponent<Renderer>();
        biomeMap = new Texture2D(planetResolution, planetResolution);

        biomeMap.filterMode = FilterMode.Point; 
        
        biomeMap.wrapModeU = TextureWrapMode.Repeat;
        biomeMap.wrapModeV = TextureWrapMode.Clamp;

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
        GravityAttractor myGravity = GetComponent<GravityAttractor>();

        if (lootBiome1 == null || lootBiome2 == null) return;
        
        if (GetComponent<MeshCollider>() == null)
        {
            Debug.LogError("¡Necesitas un MeshCollider en el planeta para que esto sea preciso!");
            gameObject.AddComponent<MeshCollider>();
        }

        for (int i = 0; i < numberOfRocks; i++)
        {
            Vector3 randomDir = Random.onUnitSphere;
            Vector3 spawnPos = transform.position + (randomDir * planetRadius);
            
            
            GameObject prefabToSpawn = lootBiome2; 
            
            RaycastHit hit;
            Vector3 rayOrigin = spawnPos + (randomDir * 5.0f); 
            
            if (Physics.Raycast(rayOrigin, -randomDir, out hit, 20f, planetLayer))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    Vector2 uv = hit.textureCoord;
                    Color surfaceColor = biomeMap.GetPixelBilinear(uv.x, uv.y);
                    
                    if (IsColorSimilar(surfaceColor, colorBiome1))
                    {
                        prefabToSpawn = lootBiome1;
                    }
                    else
                    {
                        prefabToSpawn = lootBiome2;
                    }
                }
            }

            GameObject newRock = Instantiate(rockPrefab, spawnPos, Quaternion.identity);
            newRock.transform.up = randomDir;
            newRock.GetComponent<Renderer>().material.color = rockColor;

            GravityBody rockGravity = newRock.GetComponent<GravityBody>();
            if (rockGravity) rockGravity.attractor = myGravity;

            RockInteraction rockScript = newRock.GetComponent<RockInteraction>();
            if (rockScript == null) rockScript = newRock.AddComponent<RockInteraction>();
            
            rockScript.lootToSpawn = prefabToSpawn;

        }
    }

    bool IsColorSimilar(Color a, Color b)
    {
        float diff = Mathf.Abs(a.r - b.r) + Mathf.Abs(a.g - b.g) + Mathf.Abs(a.b - b.b);
        return diff < 0.1f; 
    }
}