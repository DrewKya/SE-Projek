using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BattleHandler : MonoBehaviour
{
    public GameObject[] enemies;
    public GameObject Player;
    private int selectedEnemyIndex = -1;
    public int currentEnemyIndex;
    public int PlayerDamage = 40;
    public int CountEnemies;

    GameHandler enemy;

    public void Start()
    {
        currentEnemyIndex = 0;
        SelectEnemy(currentEnemyIndex);
        CountEnemies = enemies.Length;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
            {
                if (currentEnemyIndex < 2)
                {
                    if (enemies[currentEnemyIndex + 1] != null || enemies[0] != null && enemies[2] != null) currentEnemyIndex++;
                }
                while (enemies[currentEnemyIndex] == null && currentEnemyIndex < 2)
                {
                    currentEnemyIndex++;
                }
                SelectEnemy(currentEnemyIndex);
            }
        if (Input.GetKeyDown(KeyCode.A))
            {
                if (currentEnemyIndex > 0)
                {
                    if (enemies[currentEnemyIndex - 1] != null || enemies[0] != null && enemies[2] != null) currentEnemyIndex--;
                }
                while (enemies[currentEnemyIndex] == null && currentEnemyIndex > 0)
                {
                    currentEnemyIndex--;
                }
                SelectEnemy(currentEnemyIndex);
            }
        if (Input.GetKeyDown(KeyCode.Space))
            {
                enemy = enemies[selectedEnemyIndex].GetComponent<GameHandler>();
                enemy.gotDamage(PlayerDamage);
                if (enemy.health <= 0)
                {
                    Destroy(enemies[selectedEnemyIndex]);
                    CountEnemies--;
                    Debug.Log("Selected: " + selectedEnemyIndex + "   Curr: " + currentEnemyIndex);
                    if (selectedEnemyIndex > 0)
                    {
                        do
                        {
                            currentEnemyIndex--;
                            Debug.Log("Curr: " + currentEnemyIndex);
                            if (enemies[currentEnemyIndex] != null) break;
                        } while (currentEnemyIndex < 3 );
                    }
                    else
                    {
                        do
                        {
                            currentEnemyIndex++;
                            Debug.Log("Curr: " + currentEnemyIndex);
                            if (enemies[currentEnemyIndex] != null) break;
                        } while (currentEnemyIndex > 0);
                    }
                    Debug.Log("Lenght= " + CountEnemies);
                    SelectEnemy(currentEnemyIndex);
                }
            }
    }
    void SelectEnemy(int index)
    {
        if (index >= 0 && index < enemies.Length && enemies[index] != null)
        {

            if (selectedEnemyIndex != -1)
            {
                enemies[selectedEnemyIndex].GetComponent<Renderer>().material.color = Color.white;
            }
            selectedEnemyIndex = index;
            enemies[selectedEnemyIndex].GetComponent<Renderer>().material.color = Color.red;
        }
        else
        {
            Debug.LogWarning("Invalid enemy index or enemy does not exist.");
        }
    }
 
}