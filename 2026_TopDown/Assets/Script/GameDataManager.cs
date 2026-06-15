using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class PlayerData
{
    public List<string> collectedITems = new List<string>();
    public int stage = 1;
    public int money = 0; // 새로 추가된 돈(재화) 변수
}

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager instance;

    public PlayerData playerData;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // [추가된 부분] 에디터에서 게임 씬부터 바로 시작할 때를 대비해 강제로 데이터를 초기화합니다.
            playerData = LoadData();
            if (playerData == null)
            {
                playerData = new PlayerData();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveData(PlayerData newPlayerData)
    {
        this.playerData = newPlayerData;
        string filePath = Application.persistentDataPath + "/player_data.json";
        string json = JsonUtility.ToJson(playerData, true);
        System.IO.File.WriteAllText(filePath, json);
        Debug.Log("게임 데이터 저장됨:" + json);
    }

    public PlayerData LoadData()
    {
        string filePath = Application.persistentDataPath + "/player_data.json";
        if (System.IO.File.Exists(filePath))
        {
            string json = System.IO.File.ReadAllText(filePath);
            PlayerData loadedData = JsonUtility.FromJson<PlayerData>(json);
            return loadedData;
        }
        else
        {
            return new PlayerData();
        }
    }

    public void GameStart()
    {
        playerData = LoadData();
        if (playerData == null)
        {
            playerData = new PlayerData();
            SceneManager.LoadScene("Level_1");
        }
        else
        {
            SceneManager.LoadScene("Level_" + playerData.stage);
        }
    }

    public void PlayerDead()
    {
        PlayerData currentData = LoadData();
        if (currentData != null)
        {
            currentData.stage = 1;

            // [패널티] 죽으면 먹었던 돈을 모두 잃음
            currentData.money = 0;

            foreach (string item in currentData.collectedITems.ToList())
            {
                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    currentData.collectedITems.Remove(item);
                }
            }
            SaveData(currentData);
        }
        SceneManager.LoadScene("GameOver");
    }

    // 코인을 먹었을 때 호출될 함수
    public void AddMoney(int amount)
    {
        if (playerData == null) playerData = LoadData();

        playerData.money += amount;
        SaveData(playerData); // 먹을 때마다 즉시 저장
        Debug.Log($"돈 획득! 현재 소지 금액: {playerData.money}");
    }
}