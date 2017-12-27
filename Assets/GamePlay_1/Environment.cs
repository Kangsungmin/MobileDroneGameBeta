﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;


public class Environment : MonoBehaviour {
    const int EXIT = -1, DIE = 0, IDLE = 1, ATTACKED = 2;

    public DroneCamera D_CAM;
    GameObject PlayerDrone, Enemy;
    public GameObject mainHuman;
    //======UI GameObject=======
    public GameObject moveJoystickLeft, swipeControllor, upButton, downButton;
    public UIManager ui_Manager;
    public MarkGenerator MarkManager;
    //======UI GameObject=======

    //======Env variable========
    public static bool GameOver = false;
    public static int EnemyCount = 0;
    public int LeftBombCount = 10;
    //======Env variable========
    //======Main Human Variable=======
    private int MainState = IDLE;
    public float Main_HP = 100.0f, Main_MaxHP = 100.0f;
    //======Main Human Variable=======

    public int AmountMoney=0, MissionScore=0;
    public Stopwatch SW;
    public Dictionary<int, int> NowGetParts = new Dictionary<int, int>();//지금까지 얻은 아이템

    public Transform DroneSpawn, EnemySpawn;

    public Action<bool> isSetScore;
    bool isSuccess;


    private void Awake()
    {
        isSetScore = ((bool updatescore) =>
        {
            isSuccess = updatescore;
        });

        GameOver = false;
        //현재 기기가 모바일 일때,
        //플레이어데이터매니저에 값이 없다면 다시 Init씬으로 간다.
        //모바일이 아닐때는 그냥 실행
        
#if (UNITY_ANDROID == true && UNITY_EDITOR == false)
        if (PlayerDataManager.userID == null) SceneManager.LoadScene("InitScene");
#endif

        SW = new Stopwatch();
        DroneSpawn = transform.Find("DroneSpawnPoint");
        if (PlayerDataManager.nowUsingModel != null) PlayerDrone = Resources.Load("Prefabs/Drones/Drone_" + PlayerDataManager.nowUsingModel.getTitle()) as GameObject;
        else PlayerDrone = Resources.Load("Prefabs/Drones/Drone_Beginner") as GameObject;
        PlayerDrone = Instantiate(PlayerDrone, DroneSpawn.position, DroneSpawn.localRotation);
        GameObject[] tempRefs = new GameObject[6];//Env, UI, 조이스틱L,R
        tempRefs[0] = gameObject;
        tempRefs[1] = ui_Manager.gameObject;
        tempRefs[2] = moveJoystickLeft;
        tempRefs[3] = swipeControllor;
        PlayerDrone.SendMessage("SetReference", tempRefs);//파라미터 : 게임오브젝트배열
        MarkManager.DroneGened(PlayerDrone);

        tempRefs = new GameObject[2];
        tempRefs[0] = PlayerDrone;
        tempRefs[1] = mainHuman;
        ui_Manager.SendMessage("SetReference", tempRefs);

        tempRefs = new GameObject[1];
        tempRefs[0] = PlayerDrone;
        mainHuman.SendMessage("SetReference", tempRefs);



        //================아이템 변수 초기화==================
        NowGetParts.Add(1, 0);
        NowGetParts.Add(2, 0);
        NowGetParts.Add(3, 0);
        NowGetParts.Add(4, 0);
        NowGetParts.Add(5, 0);
        //================아이템 변수 초기화==================

        //===========================적 생성========================================
        //첫 번째 웨이브
        float[] pas = new float[4];
        pas[0] = 1.0f;//Enemy 종류
        pas[1] = 1.0f;//시간
        pas[2] = 2.0f;//적 수
        pas[3] = 4.0f;//남은 웨이브 호출
        StartCoroutine(EnemyWave(pas));

        //첫번째 웨이브 보스
        pas = new float[4];
        pas[0] = 4.0f;//Enemy 종류
        pas[1] = 40.0f;//시간
        pas[2] = 1.0f;//적 수
        pas[3] = 1.0f;//남은 웨이브 호출
        StartCoroutine(EnemyWave(pas));

        //두번째 웨이브
        pas = new float[4];
        pas[0] = 2.0f;//Enemy 종류
        pas[1] = 80.0f;//시간
        pas[2] = 3.0f;//적 수
        pas[3] = 5.0f;//남은 웨이브 호출
        StartCoroutine(EnemyWave(pas));

        //세번째 웨이브
        pas = new float[4];
        pas[0] = 3.0f;//Enemy 종류
        pas[1] = 150.0f;//시간
        pas[2] = 1.0f;//적 수
        pas[3] = 3.0f;//남은 웨이브 호출
        StartCoroutine(EnemyWave(pas));

        pas = new float[4];
        pas[0] = 5.0f;//Enemy 종류
        pas[1] = 170.0f;//시간
        pas[2] = 1.0f;//적 수
        pas[3] = 1.0f;//남은 웨이브 호출
        StartCoroutine(EnemyWave(pas));

        pas = new float[4];
        pas[0] = 4.0f;//Enemy 종류
        pas[1] = 185.0f;//시간
        pas[2] = 2.0f;//적 수
        pas[3] = 2.0f;//남은 웨이브 호출
        StartCoroutine(EnemyWave(pas));

        pas = new float[4];
        pas[0] = 5.0f;//Enemy 종류 녹색 괴물
        pas[1] = 195.0f;//시간
        pas[2] = 1.0f;//적 수
        pas[3] = 1.0f;//남은 웨이브 호출
        StartCoroutine(EnemyWave(pas));

        //===========================적 생성========================================

        //===========================장애물 생성====================================

        //===========================장애물 생성====================================
    }
    // Use this for initialization
    void Start() {

        SW.Start();
    }

    // Update is called once per frame
    void Update() {
        //적의 수를 확인하고 리스폰 한다.

        switch (MainState)
        {
            case EXIT:
                break;
            case DIE:
                mainHuman.SendMessage("Die");
                MainState = EXIT;
                GameEnd();
                break;
            case IDLE:
 
                break;
            case ATTACKED:
                
                break;
        }
    }

    IEnumerator EnemyWave(float[] pas)
    {
        yield return new WaitForSeconds(pas[1]);
        switch ((int)pas[0])
        {
            case 1:
                Enemy = Resources.Load("Prefabs/Enemy/Enemy_kid") as GameObject;
                break;
            case 2:
                Enemy = Resources.Load("Prefabs/Enemy/Enemy_adult") as GameObject;
                break;
            case 3:
                Enemy = Resources.Load("Prefabs/Enemy/Enemy_maam") as GameObject;
                break;
            case 4:
                Enemy = Resources.Load("Prefabs/Enemy/Boss_Enemy_kid") as GameObject;
                break;
            case 5:
                Enemy = Resources.Load("Prefabs/Enemy/Boss_Enemy_green") as GameObject;
                break;
            case 6:
                Enemy = Resources.Load("Prefabs/Enemy/Enemy_mom") as GameObject;
                break;
        }
        
        GameObject[] tempRefs = new GameObject[2];
        tempRefs[0] = gameObject;
        tempRefs[1] = mainHuman;
        int NumofEnemy = (int) pas[2];
        for (int i = 0; i < NumofEnemy; i++)
        {
            float temp_x = UnityEngine.Random.Range(-90.0f, 90.0f);
            int sign = UnityEngine.Random.value < .5 ? -1 : 1;
            float temp_z = sign * Mathf.Sqrt(8100.0f - Mathf.Pow(temp_x, 2.0f) ); //원의 방정식, x를 랜덤하게 설정 y는 원의 반지름(90)에 의해 자동으로 결정
            Vector3 temp = new Vector3(temp_x, 0.0f, temp_z);

            Enemy = Instantiate(Enemy, temp, Quaternion.identity);
            EnemyCount++;
            Enemy.SendMessage("SetReference", tempRefs);
            MarkManager.EnmeyGened(Enemy);
        }
        pas[1] = 15.0f;
        pas[2] += 2;
        pas[3]--;//다음 웨이브 호출 가능 수 감소
        if(pas[3] > 1)
        {
            StartCoroutine(EnemyWave(pas));
        }
        
    }

    public void IncreaseScore(int amount, int type)//점수와 업적
    {
        MissionScore += amount;
        ui_Manager.IncreaseScoreAni();
        ui_Manager.DamageAni();
    }

    public void IncreaseMoney(int amount)
    {
        AmountMoney += amount;
    }

    public void AttackMain(float damage)
    {
        if (MainState > DIE)
        {
            MainState = ATTACKED;
            Main_HP -= damage;
            if (Main_HP <= 0.0f) MainState = DIE;
        }
    }

    public void GameEnd()
    {
        SW.Stop();
        
        PlayerDrone.GetComponent<Drone>().GameOver = true;
        PlayerDrone.GetComponent<Drone>().DronePowerOn = false;
        PlayerDrone.GetComponent<Drone>().DroneAnimator.enabled = false;
        //서버 통신
#if (UNITY_ANDROID == true && UNITY_EDITOR == false)
        StartCoroutine(MissionClear_To_DB(MissionScore, NowGetParts));
#endif
        //UI 표시
        ui_Manager.GameEnd(MissionScore, NowGetParts);

        PlayGamesPlatform.Activate();
        Social.Active.ReportScore(MissionScore, GPGSIds.leaderboard_scoreboard, isSetScore);


        GameOver = true;
    }

    IEnumerator MissionClear_To_DB(int getScore, Dictionary<int, int> getPartID)
    {
        int getExp = 0, amountMoney=0;
        WWWForm form = new WWWForm();
        form.AddField("clearEXPPost", getExp);
        form.AddField("clearMoneyPost", amountMoney);
        form.AddField("userIDPost", PlayerDataManager.userID);

        WWW data = new WWW("http://13.124.188.186/mission_clear.php", form);
        yield return data;

        string user_Data = data.text;
        if (!user_Data.Equals(""))
        {
            PlayerDataManager.exp += getExp;
            PlayerDataManager.money += amountMoney;
            PlayerDataManager.exp += getExp;
            PlayerDataManager.money += amountMoney;
        }
        foreach (KeyValuePair<int, int> getparts in getPartID)
        {
            // ----------- 얻은 parts playerdatamanager(클라이언트 데이터) 데이터로 저장 -----
            PlayerDataManager.ownParts[getparts.Key] += getPartID[getparts.Key];
            // -------------------------------------------------------------------------------
            print(getPartID[getparts.Key] + " " + getparts.Key);
            form.AddField("itemIDPost", getparts.Key);
            form.AddField("itemNumPost", PlayerDataManager.ownParts[getparts.Key]);
            form.AddField("userIDPost", PlayerDataManager.userID);

            data = new WWW("http://13.124.188.186/mission_clear_items.php", form);
            yield return data;
        }
    }


}
