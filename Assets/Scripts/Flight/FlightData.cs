using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class FlightData
{
    public string callsign;
    public Vector2 position;
    public Vector2 targetPosition;
    public List<Vector2> savedWaypoints = new List<Vector2>();
    public float speed;
    public float currentFuel;
    public int planeMaxFuel = 500;
    public string status = "APPROACHING";

    // --- РЕАЛЬНЫЕ ДАННЫЕ ---
    public string cargo;
    public int cargoAmount;

    // --- ДАННЫЕ МАНИФЕСТА ---
    public string manifestCargo;
    public int manifestCargoAmount;
    public string manifestOrigin;

    // --- ОТВЕТЫ ПИЛОТА ---
    public string spokenCargo = "";
    public string spokenOrigin = "";
    public string spokenWeight = "";
    public string spokenSpeed = "";
    public string customExplanation = "";
    public string explanationCargo = "";
    public string explanationOrigin = "";
    public string explanationWeight = "";
    public string explanationSpeed = "";

    // Флаги допроса и состояния
    public bool isCargoKnown = false;
    public bool decisionMade = false;
    public bool approved = false;
    public bool hasLanded = false;
    public bool hasBeenPinged = false;

    public bool askedCargo;
    public bool askedOrigin;
    public bool askedWeight;
    public bool askedSpeed;
    public bool isInterrogationFinished;

    public bool isUnloading;
    public bool isUnloaded;
    public float unloadTimer;

    public bool isRefueling;
    public bool isRefueled;
    public float refuelTimer;

    public bool isRepairing;
    public bool isRepaired;
    public float repairTimer;

    // СТАНДАРТНЫЙ КОНСТРУКТОР (С поддержкой ручного указания количества груза cgAmount)
    public FlightData(string cs, Vector2 pos, Vector2 target, List<Vector2> wps, float spd, string cg, int cgAmount = -1)
    {
        callsign = cs;
        position = pos;
        targetPosition = target;
        savedWaypoints = wps;
        speed = spd;
        cargo = cg;
        // Если количество не передали, генерируем случайно
        cargoAmount = cgAmount == -1 ? Random.Range(10, 100) : cgAmount;
        currentFuel = Random.Range(100, planeMaxFuel);

        manifestCargo = cargo;
        manifestCargoAmount = cargoAmount;
        manifestOrigin = "Bastion-" + Random.Range(1, 10);
    }

    // СЮЖЕТНЫЙ КОНСТРУКТОР 
    public FlightData(string cs, Vector2 pos, Vector2 target, List<Vector2> wps, float spd,
                      string realCargo, int realAmount,
                      string fakeCargo, int fakeAmount, float fuel,
                      string originPort)
    {
        callsign = cs;
        position = pos;
        targetPosition = target;
        savedWaypoints = wps;
        speed = spd;
        cargo = realCargo;
        cargoAmount = realAmount;
        currentFuel = fuel;

        manifestCargo = fakeCargo;
        manifestCargoAmount = fakeAmount;
        manifestOrigin = originPort;
    }
}