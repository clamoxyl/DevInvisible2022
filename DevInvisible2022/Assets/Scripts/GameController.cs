using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    [SerializeField]
    private GameObject cat;
    [SerializeField]
    private GameObject[] foodItems;
    [SerializeField]
    private int foodSupply = 10;
    [SerializeField]
    private float foodSpawnFreq = 1.5f;


    private List<GameObject> food;
    private bool startGame = true;
    private float startTime;
    private float lastFoodSpawnTime = -Mathf.Infinity;


    // Start is called before the first frame update
    void Start()
    {
        food = new List<GameObject>();
        GameObject newFood;

        //instantiate each food pool
        for (int i = 0; i < foodItems.Length; i++)
        {
            for (int j = 0; j < foodSupply; j++)
            {
                newFood = Instantiate(foodItems[i]);
                newFood.transform.SetParent(transform);
                newFood.SetActive(false);
                food.Add(newFood);           
            }
        }

        startTime = Time.time;
        //spawn initial food supply
        

        
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnFood();
        }*/
        FoodSpawnRateControl();
    }

    void FoodSpawnRateControl()
    {
        if (Time.time - lastFoodSpawnTime >= foodSpawnFreq)
        {
            SpawnFood();
            lastFoodSpawnTime = Time.time;
        }
    }

    void SpawnFood()
    {
        int type = Random.Range(0, foodItems.Length);

        for (int i = foodSupply * type; i < foodSupply * type + foodSupply; i++)
        {
            if (!food[i].activeSelf)
            {
                Vector2 circle = Random.insideUnitCircle * 3.0f;
                Vector3 newPos = new Vector3(circle.x * 2.0f, 5.0f, circle.y - 3); // set a random position 1.5 units away from the center, inside a 4.5 units radius circle, 5 units high
                food[i].transform.rotation = Random.rotation;
                food[i].transform.position = cat.transform.position + newPos;
                food[i].SetActive(true);
                return;
            }            
        }
    }
}
