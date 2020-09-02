using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject floor;
    private GameObject floorPrefab;
    private GameObject playerPrefab;
    private GameObject obstaclePrefab;
    private GameObject goalPrefab;

    private string levelText;
    private char playerCharacter = 'P';
    private char GoalCharacter = 'G';

    public int floorWidth, floorLength;
    public int levelNumber = 0;

    private void Awake()
    {
        if(!LoadResources())
        {
            Debug.Log("Failed to get resources");
            return;
        }

        if(!string.IsNullOrEmpty(levelText))
        {
            GenerateLevel();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private bool LoadResources()
    {
        floorPrefab = Resources.Load("Prefabs/PF_Floor") as GameObject;
        playerPrefab = Resources.Load("Prefabs/PF_Player") as GameObject;
        obstaclePrefab = Resources.Load("Prefabs/PF_Obstacle") as GameObject;
        goalPrefab = Resources.Load("Prefabs/PF_Goal") as GameObject;

        levelText = LoadTextFile(levelNumber);

        return floorPrefab != null && playerPrefab != null;
    }

    private string LoadTextFile(int levelNumber)
    {
        var textFile = Resources.Load("Levels/Level " + levelNumber) as TextAsset;
        return textFile?.text;
    }

    private bool GenerateLevel()
    {
        SetUpLevel();

        return true;
    }

    private Vector2 GetCharPos(char character)
    {
        int lines = levelText.Split('\n').Length;
        float posX = 0.5f, posZ = lines - 0.5f;

        foreach (char c in levelText)
        {
            if (c == character)
                return new Vector2(posX, posZ);
            else if (c == '\n')
            {
                posZ--;
                posX = 0.5f;
            }
            else
            {
                posX++;
            }
        }

        return new Vector2(0,0); //didn't find a pos? 
    }

    private bool SetUpLevel()
    {
        int length = levelText.Split('\n')[0].Length - 1;
        int width = levelText.Split('\n').Length;
        GenerateFloor(length, width);

        GenerateObstacles();

        Debug.Log(GetCharPos(playerCharacter));
        var playerPos = GetCharPos(playerCharacter);
        GeneratePlayer(playerPos.x - 0.5f, playerPos.y - 0.5f); //root pos of player is centre so we need the top left corner i.e half the size of the diameter

        return true;
    }

    private bool GenerateFloor(float width, float length)
    {
        floor = Instantiate(floorPrefab, gameObject.transform, true);

        floor.transform.localScale = new Vector3(width, length, 1);
        floor.transform.localPosition = new Vector3((float)width/2, 0,(float)length/2);

        var goal = GenerateGoal();
        goal.transform.SetParent(floor.transform);

        return floor != null;
    }

    private bool GeneratePlayer(float posX, float posZ) //we are working with whole numbers and even space.
    {
        GameObject player = Instantiate(playerPrefab, gameObject.transform, true);

        player.transform.localPosition = new Vector3(posX, 0.5f, posZ);

        return player != null;
    }

    private bool GenerateObstacles()
    {
        int lines = levelText.Split('\n').Length;
        float posX = 0.5f, posZ = lines - 0.5f;

        foreach (char c in levelText)
        {
            if (c == 'X')
            {
                GameObject obstacle = Instantiate(obstaclePrefab, gameObject.transform, true);
                obstacle.transform.localPosition = new Vector3(posX, 0.5f, posZ);
                obstacle.transform.SetParent(floor.transform);
                posX++;
            }
            else if (c == '\n')
            {
                posX = 0.5f;
                posZ -= 1;
            }
            else
            {
                posX++;
            }
        }

        return true;
    }

    private GameObject GenerateGoal()
    {
        Vector2 goalPos = GetCharPos(GoalCharacter);

        GameObject goal = Instantiate(goalPrefab, gameObject.transform, true);
        goal.transform.localPosition = new Vector3(goalPos.x, 0.1f, goalPos.y);
        return goal;
    }
}
