using UnityEngine;
using System.Collections.Generic;

public class PlanetGenerator : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject rockPrefab; // Prefab de la roca interactiva
    public int numberOfRocks = 50;
    public float planetRadius = 10f;
    
    [Header("Biomas")]
    public Color biome1Color = Color.green; // Ejemplo: Bosque
    public Color biome2Color = Color.red;   // Ejemplo: Desierto
    public float noiseScale = 2.0f; // Tamaño de las manchas de bioma

    void Start()
    {
        GenerateBiomesAndRocks();
    }

    void GenerateBiomesAndRocks()
    {
        // 1. Pintar el planeta (simple) o generar mapa lógico
        // Aquí simplemente instanciaremos rocas y les asignaremos el bioma según su posición
        
        for (int i = 0; i < numberOfRocks; i++)
        {
            SpawnRock();
        }
    }

    void SpawnRock()
    {
        // Posición aleatoria en la esfera
        Vector3 randomDir = Random.onUnitSphere;
        Vector3 spawnPos = transform.position + (randomDir * planetRadius);
        
        // Calcular Bioma usando Perlin Noise 3D (aproximado proyectando a 2D o usando librería externa)
        // Truco rápido: Usamos las coordenadas locales normalizadas para el ruido
        float noiseValue = Mathf.PerlinNoise(randomDir.x * noiseScale + 100, randomDir.y * noiseScale + 100);
        
        // Instanciar roca
        GameObject rock = Instantiate(rockPrefab, spawnPos, Quaternion.identity, transform);
        
        // Alinear con la gravedad
        rock.transform.up = randomDir;

        // Configurar la roca según el bioma
        RockInteraction rockScript = rock.GetComponent<RockInteraction>();
        if (noiseValue > 0.5f)
        {
            // Bioma 1
            rockScript.SetBiome(BiomeType.Forest); 
            rock.GetComponent<Renderer>().material.color = biome1Color; // Feedback visual
        }
        else
        {
            // Bioma 2
            rockScript.SetBiome(BiomeType.Desert);
            rock.GetComponent<Renderer>().material.color = biome2Color; // Feedback visual
        }
        
        // Asignar gravedad a la roca
        GravityBody gb = rock.AddComponent<GravityBody>();
        gb.attractor = GetComponent<GravityAttractor>();
    }
}