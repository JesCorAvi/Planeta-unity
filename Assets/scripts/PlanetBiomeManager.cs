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
    public Color colorBiome2 = new Color(1f, 0.6f, 0f); // Naranja
    public GameObject lootBiome2;

    [Header("Rocas")]
    public GameObject rockPrefab;
    public Color rockColor = Color.gray;
    public int numberOfRocks = 50;
    
    // Capa para que el raycast solo golpee el planeta (Opcional pero recomendado)
    public LayerMask planetLayer = ~0; 

    private Texture2D biomeMap;

    void Start()
    {
        GeneratePlanetTexture();
        // Forzamos actualización de colliders por si acaso
        Physics.SyncTransforms();
        GenerateRocks();
    }

    void GeneratePlanetTexture()
    {
        Renderer ren = GetComponent<Renderer>();
        biomeMap = new Texture2D(planetResolution, planetResolution);

        // --- CORRECCIÓN 1: QUITAR SUAVIZADO ---
        // FilterMode.Point hace que los píxeles sean duros. 
        // No habrá colores mezclados en los bordes, solo Verde o Naranja puro.
        biomeMap.filterMode = FilterMode.Point; 
        
        // WrapMode para que la esfera cierre bien la textura
        biomeMap.wrapModeU = TextureWrapMode.Repeat;
        biomeMap.wrapModeV = TextureWrapMode.Clamp;

        for (int y = 0; y < planetResolution; y++)
        {
            for (int x = 0; x < planetResolution; x++)
            {
                float xCoord = (float)x / planetResolution * noiseScale;
                float yCoord = (float)y / planetResolution * noiseScale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);

                // Asignamos color estricto (sin degradados)
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
        
        // Verificamos collider correcto
        if (GetComponent<MeshCollider>() == null)
        {
            Debug.LogError("¡Necesitas un MeshCollider en el planeta para que esto sea preciso!");
            // Intentamos añadirlo automáticamente si falta
            gameObject.AddComponent<MeshCollider>();
        }

        for (int i = 0; i < numberOfRocks; i++)
        {
            // 1. Calculamos POSICIÓN teórica
            Vector3 randomDir = Random.onUnitSphere;
            Vector3 spawnPos = transform.position + (randomDir * planetRadius);
            
            // --- CORRECCIÓN 2: LÓGICA ANTES DE INSTANCIAR ---
            // Decidimos el bioma ANTES de crear la roca para evitar que el raycast choque con ella.
            
            GameObject prefabToSpawn = lootBiome2; // Por defecto desierto
            
            // Lanzamos rayo DESDE LEJOS hacia el centro del planeta
            RaycastHit hit;
            Vector3 rayOrigin = spawnPos + (randomDir * 5.0f); // Empezamos 5 metros fuera
            
            if (Physics.Raycast(rayOrigin, -randomDir, out hit, 20f, planetLayer))
            {
                // Solo si golpeamos el planeta leemos la textura
                if (hit.collider.gameObject == gameObject)
                {
                    Vector2 uv = hit.textureCoord;
                    Color surfaceColor = biomeMap.GetPixelBilinear(uv.x, uv.y);
                    
                    // Decisión basada en qué color domina
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

            // 3. AHORA SÍ INSTANCIAMOS
            GameObject newRock = Instantiate(rockPrefab, spawnPos, Quaternion.identity);
            newRock.transform.up = randomDir;
            newRock.GetComponent<Renderer>().material.color = rockColor;

            // Asignamos gravedad
            GravityBody rockGravity = newRock.GetComponent<GravityBody>();
            if (rockGravity) rockGravity.attractor = myGravity;

            // Asignamos el loot que calculamos antes
            RockInteraction rockScript = newRock.GetComponent<RockInteraction>();
            if (rockScript == null) rockScript = newRock.AddComponent<RockInteraction>();
            
            rockScript.lootToSpawn = prefabToSpawn;
            
            // Debug Visual: Descomenta esto para ver si el color coincide con el suelo
            // newRock.GetComponent<Renderer>().material.color = (prefabToSpawn == lootBiome1) ? Color.green : Color.red;
        }
    }

    // Compara colores ignorando pequeñas diferencias de precisión
    bool IsColorSimilar(Color a, Color b)
    {
        float diff = Mathf.Abs(a.r - b.r) + Mathf.Abs(a.g - b.g) + Mathf.Abs(a.b - b.b);
        return diff < 0.1f; // Margen muy pequeño
    }
}