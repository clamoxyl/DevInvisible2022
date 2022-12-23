using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    [SerializeField]
    LayerMask layerMaskFood;
    [SerializeField]
    private int[] levels; // each position represents a level and each number inside these positions represents the number of treats the cat demands
    [SerializeField]
    private AudioSource audioSrc;
    [SerializeField]
    private AudioClip[] meowClips;
    [SerializeField]
    private Transform mouthPosition;

    [Header("CAT ANIMATION")]
    [SerializeField]
    private Animator catAnimator;
    [SerializeField]
    private Material catMaterial;
    [SerializeField]
    private Texture catIdleTexture;
    [SerializeField]
    private Texture catOpenMouthTexture;

    [Header("UI")]
    [SerializeField]
    private TMP_Text levelText;
    [SerializeField]
    private TMP_Text foodText;


    private List<GameObject> food;
    private bool startGame = true;
    private bool levelFoodServed = false;
    private float startTime;
    private float lastFoodSpawnTime = -Mathf.Infinity;
    private int currentLevel = 0;
    private List<int> levelCravings;

    private int currentCraving = 0;

    private bool inhibitClicks = false;

    private int lastGeneratedCraving = 0;
        

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.PlayMusic(2);

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

        GenerateLevelCravings(levels[currentLevel]); //generate number of cravings for current level

        Meow(levelCravings[currentCraving]); // ask for the first food item

    }

    // Update is called once per frame
    void Update()
    {
        if (startGame)
        {
            //spawn food
            if (!levelFoodServed)
            {
                SpawnLevelFood();
                levelFoodServed = true;
            }
            //FoodSpawnRateControl();
            //check food interactions
            CheckClickOnFood();
        }
        else
        {
            foodText.text = "¡Has calmado a la Bestia! ¡Feliz Navidad!";
            levelText.text = "";
        }

    }

    void SpawnLevelFood()
    {
        if (currentLevel == 0) // Force candy cane spawn for level 0
        {
            SpawnFood(food[0]);
            return;
        }
        else // Spawn twice as craved food for the rest of the levels
        {
            GameObject availableFood;

            for (int i = 0; i < levelCravings.Count; i++) //Spawn the required food for the cravings
            {
                availableFood = FoodAvailable(levelCravings[i] - 1);

                SpawnFood(availableFood);
            }
            /*
            int type;

            for (int i = 0; i < levelCravings.Count * 2; i++) //Spawn extra random food, twice as cravings, so 3 times the cravings in total
            {                
                do
                {
                    type = Random.Range(0, foodItems.Length);
                    availableFood = FoodAvailable(type);
                } while (availableFood != null);

                Vector2 circle = Random.insideUnitCircle * 2.0f;
                Vector3 newPos = new Vector3(circle.x * 2.0f, 5.0f, circle.y - 3); // set a random position 1.5 units away from the center, inside a 4 units radius circle, 5 units high
                availableFood.transform.rotation = Random.rotation;
                availableFood.transform.position = cat.transform.position + newPos;
                availableFood.SetActive(true);                
            }*/
        }
    }

    GameObject FoodAvailable (int type)
    {
        for (int i = foodSupply * type; i < foodSupply * type + foodSupply; i++)
        {
            if (!food[i].activeSelf)
            {
                return food[i];
            }
        }
        return null; 
    }

    void DeleteLevelFood()
    {
        for (int i = 0; i < food.Count; i++)
        {
            if (food[i].activeSelf)
            {
                food[i].GetComponent<FoodItem>().DisableFoodItem();
            }
        }
    }

    /*void FoodSpawnRateControl() // Constant rate. Not in use for the game.
    {
        if (Time.time - lastFoodSpawnTime >= foodSpawnFreq)
        {
            SpawnFood();
            lastFoodSpawnTime = Time.time;
        }
    }

    //old food spawner.
    void SpawnFood() 
    {
        int type = Random.Range(0, foodItems.Length);

        for (int i = foodSupply * type; i < foodSupply * type + foodSupply; i++)
        {
            if (!food[i].activeSelf)
            {
                Vector2 circle = Random.insideUnitCircle * 2.0f;
                Vector3 newPos = new Vector3(circle.x * 2.0f, 5.0f, circle.y - 3); // set a random position 1.5 units away from the center, inside a 4 units radius circle, 5 units high
                food[i].transform.rotation = Random.rotation;
                food[i].transform.position = cat.transform.position + newPos;
                food[i].SetActive(true);
                return;
            }            
        }
    }*/

    void SpawnFood(GameObject food)
    {
        Vector2 circle = Random.insideUnitCircle;
        Vector3 newPos = new Vector3(circle.x * 2, 5.0f, circle.y - 4); 
        food.transform.rotation = Random.rotation;
        food.transform.position = cat.transform.position + newPos;
        food.SetActive(true);
    }

    void GenerateLevelCravings(int amount)
    {
        levelCravings = new List<int>();
        if (currentLevel == 0)
        {
            levelCravings.Add(1); // Force just one candy cane for level 1, as tutorial
        }
        else
        {
            for (int i = 0; i < amount; i++)
            {
                int newCraving;
                do
                {
                    newCraving = Random.Range(1, foodItems.Length);
                }
                while (newCraving == lastGeneratedCraving);
                lastGeneratedCraving = newCraving;
                levelCravings.Add(newCraving);
            }
        }
        foreach (var item in levelCravings)
        {
            Debug.Log(item.ToString());
        }

        levelText.text = "Nivel " + (currentLevel + 1).ToString();
    }

    private void NomNom (int foodInMouth)
    {
        inhibitClicks = true;

        catAnimator.SetTrigger("Nom");
        catMaterial.SetTexture("_MainTex", catOpenMouthTexture);
        StartCoroutine(CloseMouth());
        StartCoroutine(ReactivateClicks());

        if (foodInMouth == levelCravings[currentCraving]) //Check if cat ate what it was craving
        {
            currentCraving++;
            Purr();
            Debug.Log("Right Food");
        }
        else
        {
            Debug.Log("Wrong Food");
            MeowInDisgust();
            GameManager.Instance.ReloadScene(2);
        }

        if (currentCraving <= levelCravings.Count - 1) //Ask for the next craving or insist in what the cat wants
        {
            Meow(levelCravings[currentCraving]);
        }
        else if (currentLevel < levels.Length - 1) //Advance level, update cravings, delete old food and spawn new food
        {
            currentLevel++;
            currentCraving = 0;
            GenerateLevelCravings(levels[currentLevel]);
            Meow(levelCravings[currentCraving]); //Ask for the next craving
            //DeleteLevelFood();
            levelFoodServed = false;
        }
        else
        {
            startGame = false;
        }
    }

    void CheckClickOnFood()
    {
        if (!inhibitClicks && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitData = new RaycastHit();
            if (Physics.Raycast(ray, out hitData, 1000, layerMaskFood))
            {
                Debug.Log(hitData.collider.gameObject.name);
                FoodItem clickedFood = hitData.collider.transform.parent.gameObject.GetComponent<FoodItem>();
                clickedFood.MouthPosition = mouthPosition;
                NomNom(clickedFood.FoodID);
                clickedFood.EatFoodItem();
            }
        }
    }

    void Meow (int craving)
    {
        foodText.text = "(" + currentCraving.ToString() + "/" + levels[currentLevel].ToString() + ") ";

        switch (craving)
        {
            case 1:
                foodText.text = foodText.text + "Bastón de Caramelo";
                break;
            case 2:
                foodText.text = foodText.text + "Hombre de Jengibre";
                break;
            case 3:
                foodText.text = foodText.text + "Mantecado";
                break;
            case 4:
                foodText.text = foodText.text + "Galleta de Reno";
                break;
        }
    }

    void MeowInDisgust()
    {

    }

    void Purr()
    {

    }

    private IEnumerator CloseMouth()
    {
        yield return new WaitForSeconds(0.22f);
        catMaterial.SetTexture("_MainTex", catIdleTexture);
    }

    private IEnumerator ReactivateClicks()
    {
        yield return new WaitForSeconds(0.4f);
        inhibitClicks = false;
    }
}
