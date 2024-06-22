using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Units : MonoBehaviour
{
    public string unitName;
    public int unitLevel;
    public int SPD;
    public float ExtraSPD;

    [HideInInspector] public float action;
    public int damage;
    [HideInInspector] public int Selisih;

    [HideInInspector] public int maxHP;
    [HideInInspector] public int currentHP;

    public HealthBar healthBar;
    [HideInInspector] public float AV;

    [SerializeField]
    protected Animator anim;
    
    public void Start()
    {
        currentHP = maxHP;
        healthBar.SetMaxHealth(maxHP);
        anim = GetComponent<Animator>();
    }
    private float CalculateAction()
    {
        float adjustedSPD = SPD * ExtraSPD;
        AV = (10000 / SPD);
        action = AV * (SPD / (SPD + adjustedSPD));
        return action;
    }

    public bool TakeDamage(int dmg)
    {
        currentHP -= dmg;

        healthBar.SetHealth(currentHP);

        if (currentHP <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public int GetCurrHP()
    {
        return currentHP;
    }
    public float GetAV()
    {
        return CalculateAction(); ;
    }

    public void GetEnemyAnimHurt()
    {
        StartCoroutine(EnemyAnimHurt());
    }
    IEnumerator EnemyAnimHurt()
    {
        anim.SetBool("IsGetHit", true);
        yield return new WaitForSeconds(0.3f);
        
        if(currentHP <= 0){
            anim.SetBool("IsDied", true);
        }
        else {
            anim.SetBool("IsGetHit", false);
        }
    }

    public void GetPlayerAnimHurt()
    {
        StartCoroutine(PlayerAnimHurt());
    }
    IEnumerator PlayerAnimHurt()
    {
        anim.SetBool("IsGetHit", true);
        yield return new WaitForSeconds(0.1f);

        if (currentHP <= 0)
        {
            anim.SetBool("IsDied", true);
        }
        else
        {
            anim.SetBool("IsGetHit", false);
        }
    }
}
