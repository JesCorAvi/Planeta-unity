using UnityEngine;

public class PlanetBiomeManager : MonoBehaviour
{
    [Header("Configuración General")]
    public int planetResolution = 256; 
    public float noiseScale = 3f;      
    public float planetRadius = 10f;

    [Header("Bioma 1 (Bosque)")]
    public Color colorBiome1 = Color.green;
    public GameObject lootBiome1; // Prefab Loot Bosque

    [Header("Bioma 2 (Desierto)")]
    public Color colorBiome2 = new Color(1f, 0.6f, 0f); // Naranja
    public GameObject lootBiome2; // Prefab Loot Desierto

    [Header("Rocas")]
    public GameObject rockPrefab;
    public Color rockColor = Color.gray; // Color base de las rocas
    public int numberOfRocks = 50;

    void Start()
    {
        GeneratePlanetTexture();
        GenerateRocks();
    }

    void GeneratePlanetTexture()
    {
        Renderer ren = GetComponent<Renderer>();
        Texture2D texture = new Texture2D(planetResolution, planetResolution);
        
        for (int y = 0; y < planetResolution; y++)
        {
            for (int x = 0; x < planetResolution; x++)
            {
                float xCoord = (float)x / planetResolution * noiseScale;
                float yCoord = (float)y / planetResolution * noiseScale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                
                // Pintamos EL SUELO del planeta
                Color pixelColor = (sample < 0.5f) ? colorBiome1 : colorBiome2;
                texture.SetPixel(x, y, pixelColor);
            }
        }
        texture.Apply();
        ren.material.mainTexture = texture;
    }

 void GenerateRocks()
    {
        // NOTA: Como ahora las rocas son Kinematic, no necesitan GravityBody ni Attractor
        // pero lo dejamos por si acaso decides reactivar la física luego.
        GravityAttractor myGravity = GetComponent<GravityAttractor>();

        for (int i = 0; i < numberOfRocks; i++)
        {
            Vector3 randomDir = Random.onUnitSphere;
            
            // CAMBIO: Usamos exactamente el radio del planeta.
            // Si tu radio es 50 (porque escala es 100), pon aquí 50 exactos.
            // Si la roca se queda medio enterrada, súmale un poquito (ej: planetRadius + 1f)
            Vector3 spawnPos = transform.position + (randomDir * planetRadius); 

            GameObject newRock = Instantiate(rockPrefab, spawnPos, Quaternion.identity);
            
            // Alinear perfectamente con el centro del planeta
            newRock.transform.up = randomDir; 

            // Configurar Script
            RockScript rockScript = newRock.GetComponent<RockScript>();
            // Si se había perdido el script por el error "Missing", esto evitará que el juego pete
            if (rockScript == null) rockScript = newRock.AddComponent<RockScript>();

            // Asignar Color y Gravedad (aunque sea Kinematic)
            newRock.GetComponent<Renderer>().material.color = rockColor;
            GravityBody rockGravity = newRock.GetComponent<GravityBody>();
            if (rockGravity) rockGravity.attractor = myGravity;

            // --- LÓGICA DE BIOMA ---
            float u = 0.5f + (Mathf.Atan2(randomDir.z, randomDir.x) / (2f * Mathf.PI));
            float v = 0.5f - (Mathf.Asin(randomDir.y) / Mathf.PI);
            float noiseValue = Mathf.PerlinNoise(u * noiseScale, v * noiseScale);

            if (noiseValue < 0.5f)
            {
                rockScript.lootToSpawn = lootBiome1;
            }
            else
            {
                rockScript.lootToSpawn = lootBiome2;
            }
        }
    }
}