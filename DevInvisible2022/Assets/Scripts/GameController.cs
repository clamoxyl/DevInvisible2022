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


    private List<GameObject> food;
    private bool startGame = true;
    private float startTime;
    private float lastFoodSpawnTime = -Mathf.Infinity;
    private int currentLevel = 1;
    private List<int> levelCravings;

    private int currentCraving = 0;

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

        GenerateLevelCravings(levels[currentLevel]);
        
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnFood();
        }*/
        FoodSpawnRateControl();
        CheckClickOnFood();

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

    void GenerateLevelCravings(int amount)
    {
        levelCravings = new List<int>();
        for (int i = 0; i < amount; i++)
        {
            levelCravings.Add(Random.Range(1, foodItems.Length));
        }

        levelText.text = "Level " + currentLevel.ToString();
    }

    private void NomNom (int foodInMouth)
    {
        catAnimator.SetTrigger("Nom");
        catMaterial.SetTexture("_MainTex", catOpenMouthTexture);
        StartCoroutine(CloseMouth());

        if (foodInMouth == levelCravings[currentCraving])
        {
            currentCraving++;
            Purr();
        }
        else MeowInDisgust();

        Meow(levelCravings[currentCraving]);
    }

    void CheckClickOnFood()
    {
        if (Input.GetMouseButtonDown(0))
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
}
