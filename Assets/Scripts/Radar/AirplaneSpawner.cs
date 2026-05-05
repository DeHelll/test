using UnityEngine;
using System.Collections.Generic;

public class AirplaneSpawner : MonoBehaviour
{
    public static AirplaneSpawner Instance;

    [Header("Settings")]
    public GameObject airplanePrefab;
    public Transform radarContent;
    public int maxAirplanes = 5;

    public float minSpawnTime = 5f;
    public float maxSpawnTime = 12f;
    public float spawnRadius = 1300f;

    [Range(0f, 1f)]
    public float landingProbability = 0.5f;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (FlightDataManager.Instance == null || !FlightDataManager.Instance.isShiftActive) return;

        if (FlightDataManager.Instance.landedPlanes >= FlightDataManager.Instance.maxPlanes) return;

        FlightDataManager.Instance.globalSpawnTimer -= Time.deltaTime;

        if (FlightDataManager.Instance.globalSpawnTimer <= 0)
        {
            Transform currentContent = GetActiveRadarContent();
            if (currentContent == null) return;

            int currentCount = GetCurrentPlanesCount(currentContent);

            // Если на радаре есть место для нового самолета
            if (currentCount < maxAirplanes)
            {
                // Если в очереди есть СЮЖЕТНЫЙ самолет
                if (FlightDataManager.Instance.scriptedFlightsQueue.Count > 0)
                {
                    // Достаем самолет и таймер до следующего
                    FlightData data = FlightDataManager.Instance.scriptedFlightsQueue.Dequeue();

                    float nextDelay = 5f; // Страховочное значение
                    if (FlightDataManager.Instance.scriptedDelaysQueue.Count > 0)
                    {
                        nextDelay = FlightDataManager.Instance.scriptedDelaysQueue.Dequeue();
                    }

                    SpawnStoryPlane(data, currentContent);

                    // Устанавливаем долгую задержку из сценария
                    FlightDataManager.Instance.globalSpawnTimer = nextDelay;
                }
                // Иначе — спавним ОБЫЧНЫЙ рандомный самолет
                else
                {
                    SpawnRandomAirplane(currentContent);
                    FlightDataManager.Instance.globalSpawnTimer = Random.Range(minSpawnTime, maxSpawnTime);
                }
            }
            else
            {
                // Если мест на радаре нет (уже кружат 5 бортов), спавнер ждет 3 секунды и пробует снова,
                // НЕ сбрасывая долгий таймер (чтобы не сломать сюжет).
                FlightDataManager.Instance.globalSpawnTimer = 3f;
            }
        }
    }

    void SpawnStoryPlane(FlightData data, Transform contentParent)
    {
        // 1. Берем ЖЕСТКИЕ заскриптованные позиции из FlightData (ИСПРАВЛЕНО: targetPosition)
        Vector2 startPos = data.position;
        Vector2 targetPos = data.targetPosition;

        GameObject newPlane = Instantiate(airplanePrefab, contentParent, false);
        UIAirplane planeScript = newPlane.GetComponent<UIAirplane>();

        if (planeScript != null)
        {
            planeScript.InitializeFromData(data);
            planeScript.SetFlightPath(startPos, targetPos);

            // --- КРИТИЧЕСКИ ВАЖНО: Сразу регистрируем данные в глобальном списке ---
            if (FlightDataManager.Instance != null && !FlightDataManager.Instance.savedFlights.Contains(data))
            {
                FlightDataManager.Instance.savedFlights.Add(data);
            }

            if (RadarManager.Instance != null)
            {
                RadarManager.Instance.RegisterAirplane(planeScript);
            }
        }
    }

    void SpawnRandomAirplane(Transform contentParent)
    {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector2 startPos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnRadius;

        Vector2 targetPos = Vector2.zero;
        if (Random.value >= landingProbability)
        {
            float endAngle = angle + Random.Range(120f, 240f) * Mathf.Deg2Rad;
            targetPos = new Vector2(Mathf.Cos(endAngle), Mathf.Sin(endAngle)) * (spawnRadius + 200f);
        }

        GameObject newPlane = Instantiate(airplanePrefab, contentParent, false);
        UIAirplane planeScript = newPlane.GetComponent<UIAirplane>();

        if (planeScript != null)
        {
            planeScript.SetFlightPath(startPos, targetPos);
            if (RadarManager.Instance != null)
            {
                RadarManager.Instance.RegisterAirplane(planeScript);
            }
        }
    }

    Transform GetActiveRadarContent()
    {
        if (radarContent != null && radarContent.gameObject.activeInHierarchy)
            return radarContent;

        BigRadarLoader loader = FindFirstObjectByType<BigRadarLoader>();
        if (loader != null && loader.radarContent != null)
            return loader.radarContent;

        return null;
    }

    int GetCurrentPlanesCount(Transform contentParent)
    {
        if (RadarManager.Instance != null) return RadarManager.Instance.GetPlanesCount();
        return contentParent.GetComponentsInChildren<UIAirplane>().Length;
    }
}