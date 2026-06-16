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
    public int money = 0;

    public float accumulatedOxygenBonus = 0f;
    public int lastRunMoney = 0;
    public bool hasPendingReward = false;
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

    // [신규] 로컬 JSON 파일을 삭제하고 데이터를 초기화하는 함수
    public void ResetData()
    {
        string filePath = Application.persistentDataPath + "/player_data.json";
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
            Debug.Log("저장된 게임 데이터가 성공적으로 삭제되었습니다.");
        }

        // 메모리의 데이터도 새 객체로 덮어씌워 완전히 초기화합니다.
        playerData = new PlayerData();
        SaveData(playerData);
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
            currentData.lastRunMoney = currentData.money;
            currentData.hasPendingReward = true;
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

    public void AddMoney(int amount)
    {
        if (playerData == null) playerData = LoadData();

        playerData.money += amount;
        SaveData(playerData);
        Debug.Log($"돈 획득! 현재 소지 금액: {playerData.money}");
    }
}