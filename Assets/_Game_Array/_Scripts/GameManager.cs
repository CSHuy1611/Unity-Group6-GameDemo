using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; 

    [Header("Nhiệm vụ")]
    public int[][] tasks;

    [Header("Checkpoint")]
    public GameObject[] checkpoints;

    [Header("Giao diện chiến thắng")]
    public GameObject winUI;

    [Header("Theo dõi tiến độ")]
    public int currentLevel = 0; 
    public int coinsCollected = 0;
    public int enemiesKilled = 0;

    void Awake()
    {
        Instance = this; 

        tasks = new int[2][];

        // Level 1: Cần 2 Xu, giết 1 Quái
        tasks[0] = new int[] { 2, 1 };

        // Level 2: Cần 3 Xu, giết 2 Quái
        tasks[1] = new int[] { 3, 2 };

        Debug.Log("Game Start! Nhiệm vụ Màn 1: Nhặt " + tasks[0][0] + " xu, Giết " + tasks[0][1] + " quái.");
    }

    public void AddCoin()
    {
        coinsCollected++;
        Debug.Log("Đã nhặt xu: " + coinsCollected + "/" + tasks[currentLevel][0]);
        CheckMission();
    }

    public void AddKill()
    {
        enemiesKilled++;
        Debug.Log("Đã giết quái: " + enemiesKilled + "/" + tasks[currentLevel][1]);
        CheckMission();
    }

    void CheckMission()
    {

        int requiredCoins = tasks[currentLevel][0];
        int requiredKills = tasks[currentLevel][1];

        if (coinsCollected >= requiredCoins && enemiesKilled >= requiredKills)
        {
            Debug.Log("--- HOÀN THÀNH NHIỆM VỤ MÀN " + (currentLevel + 1) + " ---");
            Debug.Log("CHECKPOINT ĐÃ MỞ! Hãy đi tới Checkpoint.");
            checkpoints[currentLevel].SetActive(true);
        }
    }

    public void NextLevel()
    {

        checkpoints[currentLevel].SetActive(false);

        currentLevel++;


        if (currentLevel < tasks.Length)
        {
            coinsCollected = 0; 
            enemiesKilled = 0;
            Debug.Log("--- BẮT ĐẦU NHIỆM VỤ 2 ---");
        }
        else
        {

            Debug.Log("CHÚC MỪNG CHIẾN THẮNG!");
            if (winUI != null) winUI.SetActive(true);
            Time.timeScale = 0;
        }
    }


}
