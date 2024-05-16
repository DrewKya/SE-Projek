using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEnemy : MonoBehaviour
{
    public Animator anime;
    void Start()
    {
        anime = GetComponent<Animator>();   
    }

    public void GettingHit(int currentHP)
    {
        anime.SetBool("IsGetHit", true);

        if (currentHP <= 0) anime.SetBool("IsDied", true);
        else anime.SetBool("IsGetHit", false);
        return;
    }
}
