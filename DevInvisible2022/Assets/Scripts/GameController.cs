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
    private int[] levels;
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
            FoodSpawnRateControl();
            //check food interactions
            CheckClickOnFood();
        }
        else
        {
            foodText.text = "¡Has calmado a la Bestia! ¡Feliz Navidad!";
            levelText.text = "";
        }

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
                Vector2 circle = Random.insideUnitCircle * 2.0f;
                Vector3 newPos = new Vector3(circle.x * 2.0f, 5.0f, circle.y - 3); // set a random position 1.5 units away from the center, inside a 4 units radius circle, 5 units high
                food[i].transform.rotation = Random.rotation;
                food[i].transform.position = cat.transform.position + newPos;
                food[i].SetActive(true);
                return;
            }            
        }
    }

    void GenerateLevelCravings(int amount)
    {
        levelCravings = new List<int>();
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
        }

        if (currentCraving <= levelCravings.Count - 1) //Ask for the next craving or insist in what the cat wants
        {
            Meow(levelCravings[currentCraving]);
        }
        else if (currentLevel < levels.Length - 1) //Advance level and update cravings
        {
            currentLevel++;
            currentCraving = 0;
            GenerateLevelCravings(levels[currentLevel]);
            Meow(levelCravings[currentCraving]); //Ask for the next craving
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
                foodText.text = foodText.text + "Hombre de Gengibre";
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
