﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour {
    protected bool isDead = false, AttackReady = true;
    protected string State = "Idle";
    public Animator EnemyAnimator;
    protected int score;
    protected float Speed, Power;
    protected float HP, Max_HP;
    protected Environment environment;
    protected GameObject Target;
}