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


    private List<GameObject> food;
        
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
        

        //spawn initial food supply
        

        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnFood();
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
                Vector3 newPos = new Vector3(circle.x + 1.5f, 5.0f, circle.y + 1.5f); // set a random position 1.5 units away from the center, inside a 4.5 units radius circle, 5 units high
                food[i].transform.rotation = Quaternion.identity;
                food[i].transform.position = cat.transform.position + newPos;
                food[i].SetActive(true);
                return;
            }
            
        }
    }
}
