using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Units
{
    public void GetEnemyAnimHurt()
    {
        StartCoroutine(EnemyAnimHurt());
    }
    IEnumerator EnemyAnimHurt()
    {
        anim.SetBool("IsGetHit", true);
        yield return new WaitForSeconds(0.3f);

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
