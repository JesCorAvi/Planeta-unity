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
        GravityAttractor myGravity = GetComponent<GravityAttractor>();

        // PREVENCIÓN DE ERRORES:
        // Si no hay loot asignado, asignamos un cubo temporal para que no pete el juego
        if (lootBiome1 == null) { Debug.LogError("¡Falta asignar Loot Bioma 1!"); return; }
        if (lootBiome2 == null) { Debug.LogError("¡Falta asignar Loot Bioma 2!"); return; }

        for (int i = 0; i < numberOfRocks; i++)
        {
            // 1. Dirección aleatoria en el mundo
            Vector3 randomDir = Random.onUnitSphere;
            Vector3 spawnPos = transform.position + (randomDir * planetRadius);

            // 2. Instanciar
            GameObject newRock = Instantiate(rockPrefab, spawnPos, Quaternion.identity);
            newRock.transform.up = randomDir;

            // Configurar Scripts
            RockInteraction rockScript = newRock.GetComponent<RockInteraction>();
            if (rockScript == null) rockScript = newRock.AddComponent<RockInteraction>();
            
            newRock.GetComponent<Renderer>().material.color = rockColor;
            
            GravityBody rockGravity = newRock.GetComponent<GravityBody>();
            if (rockGravity) rockGravity.attractor = myGravity;

            // --- CORRECCIÓN CLAVE AQUÍ ---
            // Convertimos la dirección Mundial a Local para que la rotación del planeta no afecte
            Vector3 localDir = transform.InverseTransformDirection(randomDir);

            // Usamos localDir para calcular las coordenadas UV
            float u = 0.5f + (Mathf.Atan2(localDir.z, localDir.x) / (2f * Mathf.PI));
            float v = 0.5f - (Mathf.Asin(localDir.y) / Mathf.PI);

            // Muestreamos el ruido
            float noiseValue = Mathf.PerlinNoise(u * noiseScale, v * noiseScale);

            // 3. Decisión de Loot (Umbral estricto)
            // Añadimos un pequeño margen de seguridad en el borde (0.45 - 0.55) si quisieras
            // pero con la corrección local debería ser exacto.
            if (noiseValue < 0.5f)
            {
                rockScript.lootToSpawn = lootBiome1;
                // Debug visual (Opcional): Descomenta para ver si coincide
                // newRock.GetComponent<Renderer>().material.color = Color.green; 
            }
            else
            {
                rockScript.lootToSpawn = lootBiome2;
                // Debug visual (Opcional):
                // newRock.GetComponent<Renderer>().material.color = Color.red; 
            }
        }
    }
}