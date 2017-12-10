﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {
    public Drone Player;
    public Environment environment;
    public GameObject MainHuman;
    
    //========스코어 변수==========
    public Text ScoreText, MoneyText, TimeText, AchivementText, BombCountText;
    public Animator ScoreAni, AchiveAni;
    string TimeCount = "";
    public Transform DamageSpawn;
    public GameObject Damage;
    //========스코어 변수==========

    //========게임종료 변수===========
    public Text ResultScore, ResultTime;
    //========게임종료 변수===========

    //========랜덤 장애물아이템 변수============
    enum Barrier{
        Box,
        Concrete,
        Container,
        Statue_Head,
        NumOfBarriers
    };
    //========랜덤 장애물아이템 변수============

    public GameObject GameEndPanel, ShopPanel, MissionPanelButtons, PauseMenu, GaugeUI, LiftButtonBG, LiftButton, BatteryChangeButton;
    public Image LeftJoystick, RightJoystick, FuelBar, MainHPBar;
    public GameObject LiftButtonAni;
    private float Fuel = 0.0f, MaxFuel= 0.0f, Main_HP = 100.0f, Main_MaxHP = 100.0f;

    private void Awake()
    {
        BatteryChangeButton.SetActive(false);
        environment = GameObject.Find("ENVIRONMENT").GetComponent<Environment>();
    }

    public void SetReference(GameObject[] Refs)//Environment에서 호출
    {
        Player = Refs[0].GetComponent<Drone>();
        MainHuman = Refs[1];

        GameObject[] tempRefs = new GameObject[3];
        tempRefs[0] = Refs[0];//Drone Object
        tempRefs[1] = Refs[0].transform.Find("Claw").gameObject;//Claw
        tempRefs[2] = LiftButtonBG;//리프팅 버튼 Ani
        //LiftButton.SendMessage("SetReference", tempRefs);//드론, Claw, GrabModeCtrl

        tempRefs = new GameObject[2];
        tempRefs[0] = Refs[0];
        tempRefs[1] = Refs[1];
        BatteryChangeButton.SendMessage("SetReference", tempRefs);
    }
    

    // Update is called once per frame
    void Update()
    {
        if (!Environment.GameOver)
        {
            ScoreText.text = environment.MissionScore.ToString();
            MoneyText.text = environment.AmountMoney.ToString();
            BombCountText.text = "X "+environment.LeftBombCount.ToString();
            TimeCount = string.Format("{0:0} 초", environment.SW.ElapsedMilliseconds * 0.001);
            TimeText.text = TimeCount;
            Fuel = Player.Fuel;
            MaxFuel = Player.Max_Fuel;
            FuelBar.fillAmount = (float)Fuel / (float)MaxFuel;
            if (Fuel < 20) FuelBar.color = new Color32(255, 66, 66, 208);
            else FuelBar.color = new Color32(255, 255, 169, 208);

            Main_HP = environment.Main_HP;
            Main_MaxHP = environment.Main_MaxHP;
            MainHPBar.fillAmount = (float)Main_HP / (float)Main_MaxHP;
            if (Main_HP < 20) MainHPBar.color = new Color32(255, 66, 66, 208);
            else MainHPBar.color = new Color32(214, 214, 214, 255);

            //MainHuman과 Drone의 거리를 재서 배터리 교체버튼 활성화 여부 결정
            if (Vector3.Distance(MainHuman.transform.position, Player.transform.position) < 4.5f) BatteryChangeButton.SetActive(true);
            else BatteryChangeButton.SetActive(false);
        }
    }
    

    public void BuyItem(string name)
    {
        switch (name)
        {

            case "Drink":
                if (environment.AmountMoney >= 70)
                {
                    environment.AmountMoney -= 70;
                    environment.Main_HP += 20.0f;
                }
                break;
            case "Barrier":
                if (environment.AmountMoney >= 50)
                {
                    environment.AmountMoney -= 50;

                    Barrier temp = (Barrier) Random.Range(0, (int) Barrier.NumOfBarriers);//랜덤하게 장애물 변환
                    Player.SendMessage("SpawnItem", temp.ToString());
                }
                break;
        }
    }

    public void Fire()
    {
        if(environment.LeftBombCount > 0)
        {
            Player.SendMessage("SpawnItem", "RocketGen");
            environment.LeftBombCount--;
        }

    }

    public void ShopCall()
    {
        if (ShopPanel.activeSelf)
        {
            Time.timeScale = 1;
            environment.SW.Start();
            ShopPanel.SetActive(false);
        }
        else
        {
            Time.timeScale = 0;
            environment.SW.Stop();
            ShopPanel.SetActive(true);
        }

    }
    public void IncreaseScoreAni()
    {
        ScoreAni.Play("IncreaseScore", 0, 0.0f);
    }

    public void DamageAni()
    {
        GameObject temp = Instantiate(Damage, DamageSpawn.position, Quaternion.identity);
        temp.transform.SetParent(DamageSpawn);
        temp.GetComponent<Animator>().SetInteger("value",Random.Range(1,4));
    }
    

    public void OnPause()
    {
        PauseMenu.SetActive(true);
        Time.timeScale = 0;
        environment.SW.Stop();
    }

    public void ContinuePlay()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1;
        environment.SW.Start();
    }

    public void ForceEnd()
    {
        environment.GameEnd();
    }

    public void GameEnd(int getScore, Dictionary<int, int> getPartID)
    {
        
        GameEndPanel.SetActive(true);
        ResultScore.text = getScore.ToString();
        ResultTime.text = TimeCount;
    }

    public void ExitGamePlay()
    {
        SceneManager.LoadScene("Menu");
    }

}
