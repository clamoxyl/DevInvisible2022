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
    private GameObject[] foodPool;
    [SerializeField]
    private int foodSupply = 10;
    [SerializeField]
    private float foodSpawnFreq = 1.5f;
    [SerializeField]
    LayerMask layerMaskFood;
    [SerializeField]
    private int[] levels; // each position represents a level and each number inside these positions represents the number of treats the cat demands
    [SerializeField]
    private Transform mouthPosition;
    [SerializeField]
    private Transform[] foodSpawnPositions;

    [Header("AUDIO")]
    [SerializeField]
    private AudioSource sfxSrc;
    [SerializeField]
    private AudioClip[] meowClips;
        
    [Header("CAT ANIMATION")]
    [SerializeField]
    private Animator catAnimator;
    [SerializeField]
    private Material catMaterial;
    [SerializeField]
    private Texture catIdleTexture;
    [SerializeField]
    private Texture catOpenMouthTexture;
    [SerializeField]
    private Texture catTalkTexture;

    [Header("UI")]
    [SerializeField]
    private TMP_Text levelText;
    [SerializeField]
    private TMP_Text foodText;


    private List<GameObject> food;
    private List<FoodItem> foodItems;
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
        foodItems = new List<FoodItem>();
        GameObject newFood;

        //instantiate food pool
        for (int i = 0; i < foodPool.Length; i++)
        {
            for (int j = 0; j < foodSupply; j++)
            {
                newFood = Instantiate(foodPool[i]);
                newFood.transform.SetParent(transform);
                newFood.transform.position = foodSpawnPositions[(j + (i * foodSupply)) % foodSpawnPositions.Length].position;                
                food.Add(newFood);
                FoodItem newFoodItem = newFood.GetComponent<FoodItem>();
                newFoodItem.DisableFoodItem();
                foodItems.Add(newFoodItem);
                newFood.SetActive(false);
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
            foodText.text = "¡Has saciado a la Bestia y volverá a su cripta! ";
            levelText.text = "";
        }

    }

    void SpawnLevelFood()
    {
        if (currentLevel == 0) // Force candy cane spawn for level 0
        {
            SpawnFood(food[0], foodItems[0], 0);
            return;
        }
        else // Spawn twice as craved food for the rest of the levels
        {
            for (int i = 0; i < levelCravings.Count; i++) //Spawn the required food for the cravings
            {
                FoodAvailable(levelCravings[i] - 1, i);
            }            
        }
    }

    void FoodAvailable (int type, int delay)
    {
        for (int i = foodSupply * type; i < foodSupply * type + foodSupply; i++)
        {
            if (!food[i].activeSelf)
            {
                SpawnFood(food[i], foodItems[i], delay);
                return;
            }
        }
    }

    void DeleteLevelFood()
    {
        for (int i = 0; i < food.Count; i++)
        {
            if (food[i].activeSelf)
            {
                foodItems[i].DisableFoodItem();
            }
        }
    }
    
    void SpawnFood(GameObject foodObj, FoodItem foodIt, int i)
    {
        foodObj.SetActive(true);
        foodIt.EnableFoodItem(i * .5f);
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
                    newCraving = Random.Range(1, foodPool.Length +1);
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
            Meow(5);
            Debug.Log("Right Food");
        }
        else
        {
            Debug.Log("Wrong Food");
            Meow(6);
            GameManager.Instance.ReloadScene(2);
        }

        StartCoroutine(NextCraving());
    }

    IEnumerator NextCraving()
    {
        yield return new WaitForSeconds(2);

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
            StartCoroutine(GoToCredits());
        }
    }

    IEnumerator GoToCredits()
    {
        yield return new WaitForSeconds(3);
        GameManager.Instance.LoadSceneAdditive(3);
        GameManager.Instance.UnloadScene(2);
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
        catMaterial.SetTexture("_MainTex", catTalkTexture);
        StartCoroutine(CloseMouth());
        /*switch (craving)
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
        }*/

        sfxSrc.PlayOneShot(meowClips[craving-1]);
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
