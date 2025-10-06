using UnityEngine;
using System.Collections.Generic;

public class NPCManager : MonoBehaviour
{
    [Header("Car Setup")]
    public GameObject carPrefab;
    public Transform[] spawnPoints;
    public Transform[] waypoints;
    public Transform shopPoint;

    public int totalCars = 5;
    private List<CarAI> cars = new List<CarAI>();
    private CarAI currentShopCar;

    void Start()
    {
        // Spawn cars
        for (int i = 0; i < totalCars; i++)
        {
            Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject car = Instantiate(carPrefab, spawn.position, spawn.rotation);
            CarAI ai = car.GetComponent<CarAI>();
            ai.waypoints = waypoints;
            cars.Add(ai);
        }

        // Pick shop customers
        InvokeRepeating(nameof(ChooseShopCustomer), 5f, 20f);
    }

    void ChooseShopCustomer()
    {
        if (currentShopCar != null) return;

        CarAI chosen = cars[Random.Range(0, cars.Count)];
        currentShopCar = chosen;
        chosen.GoToShop(shopPoint);
    }

    public void FinishService()
    {
        if (currentShopCar != null)
        {
            currentShopCar.LeaveShop();
            currentShopCar = null;
        }
    }
}
