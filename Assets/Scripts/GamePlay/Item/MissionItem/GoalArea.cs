﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalArea : MonoBehaviour
{
    Playenv playEnvironment;
    GameObject Player;
    // Use this for initialization
    void Start()
    {
        playEnvironment = GameObject.Find("PlayEnvironment").GetComponent<Playenv>();
        Player = GameObject.FindGameObjectWithTag("Player");
    }
    
    void OnTriggerEnter(Collider col)
    {
        if (col.tag.Contains("Box"))
        {
            playEnvironment.SendMessage("AddParts", col.GetComponent<Box>().PartIdList());//획득한 부품 알린다( 파라미터타입 : int[] ) 
            playEnvironment.IncreaseScore(50);
            col.gameObject.SetActive(false);
            Player.transform.Find("Claw").SendMessage("RemoveBoxList", col.gameObject);//Claw의 BoxList에서 제거
            Player.SendMessage("GoalInParticlePlay");
            Playenv.SpawnBoxCount--;
            playEnvironment.ArrowOff(true);
            playEnvironment.PlayerDataUpdate();
        }
        else if(col.tag.Contains("Player"))
        {
            col.GetComponentInParent<Drone>().DropSomthing();
        }
    }
}