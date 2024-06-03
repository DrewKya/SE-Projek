using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject[] enemyPrefab;

    public Transform playerStation;
    public Transform[] enemyStation;

    Units playerUnits;
    Units[] enemyUnits;
    public BS_HUD bsHUD;
    int itemsIndex;

    [HideInInspector]
    public List<int> DeadIndex;

    private int selectedEnemyIndex = -1;
    [HideInInspector]
    public int currentEnemyIndex;

    [HideInInspector]
    public int CountEnemies;

    [HideInInspector]
    public Vector3 PlayerDirection;

    [HideInInspector]
    public Quaternion PlayerTargetRotation;

    private bool isPlayerAttacking = false;
    public BattleState state;
    private bool cursorLocked = true;

    List<Units> Urutan;

    [HideInInspector]
    public static BattleSystem Instance { get; private set; } // Singleton instance

    private void Awake()
    {
        // Initialize the singleton instance
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        state = BattleState.START;
        bsHUD.UpdateTurnText();

        currentEnemyIndex = 1;
        CountEnemies = enemyPrefab.Length;
        Urutan = new List<Units>();
        DeadIndex = new List<int>();

        PlayerDirection = enemyStation[1].position - playerStation.position;
        playerPrefab.transform.rotation = Quaternion.Euler(0, 90, 0);
        PlayerTargetRotation = Quaternion.LookRotation(PlayerDirection);

        LockCursor();
        StartCoroutine(SetupBattle());
        SelectEnemy(currentEnemyIndex);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            cursorLocked = !cursorLocked;
            LockCursor();
        }

        PlayerTargetRotation = Quaternion.LookRotation(PlayerDirection);
        playerStation.rotation = Quaternion.Euler(0, PlayerTargetRotation.eulerAngles.y - 90, 0);

        if (state == BattleState.PLAYERTURN && !isPlayerAttacking)
        {
            if (CountEnemies > 1)
            {
                if (Input.GetKeyUp(KeyCode.D))
                {
                    // Check if the current index is 0
                    if (currentEnemyIndex == 2)
                    {
                        // Stay at index 0
                        SelectEnemy(currentEnemyIndex);
                    }

                    else
                    {
                        // Check if the previous enemy is not null
                        if (enemyUnits[currentEnemyIndex + 1] != null)
                        {
                            currentEnemyIndex++;
                            SelectEnemy(currentEnemyIndex);
                        }
                        else
                        {
                            bool validEnemyFound = false;
                            for (int i = currentEnemyIndex + 1; i <= 2; i++)
                            {
                                if (enemyUnits[i] != null)
                                {
                                    currentEnemyIndex = i;
                                    validEnemyFound = true;
                                    break;
                                }
                            }
                            if (!validEnemyFound)
                            {
                                Debug.LogWarning("No valid enemy found to the left.");
                            }

                            SelectEnemy(currentEnemyIndex);
                        }
                    }
                }
                if (Input.GetKeyUp(KeyCode.A))
                {
                    // Check if the current index is 0
                    if (currentEnemyIndex == 0)
                    {
                        // Stay at index 0
                        SelectEnemy(currentEnemyIndex);
                    }

                    else
                    {
                        // Check if the previous enemy is not null
                        if (enemyUnits[currentEnemyIndex - 1] != null)
                        {
                            currentEnemyIndex--;
                            SelectEnemy(currentEnemyIndex);
                        }
                        else
                        {
                            bool validEnemyFound = false;
                            for (int i = currentEnemyIndex - 1; i >= 0; i--)
                            {
                                if (enemyUnits[i] != null)
                                {
                                    currentEnemyIndex = i;
                                    validEnemyFound = true;
                                    break;
                                }
                            }
                            if (!validEnemyFound)
                            {
                                Debug.LogWarning("No valid enemy found to the left.");
                            }

                            SelectEnemy(currentEnemyIndex);
                        }
                    }
                }
            }

            //attacking
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isPlayerAttacking = true;
                Attacking(currentEnemyIndex);
            }
        }
    }

    IEnumerator SetupBattle()
    {
        GameObject playerGO = Instantiate(playerPrefab, playerStation);
        playerUnits = playerGO.GetComponent<Units>();
        enemyUnits = new Units[enemyStation.Length];

        for (int i = 0; i < enemyStation.Length; i++)
        {
            if (i < enemyPrefab.Length && enemyPrefab[i] != null)
            {
                GameObject enemyGO = Instantiate(enemyPrefab[i], enemyStation[i]);
                enemyUnits[i] = enemyGO.GetComponent<Units>();

                // Rotate musuh ke arah player
                Vector3 direction = playerStation.position - enemyGO.transform.position;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
                enemyGO.transform.rotation = targetRotation;
            }
            else
            {
                Debug.LogWarning("Enemy prefab not assigned or missing at index: " + i);
            }
        }

        List<Units> allUnits = new List<Units>();
        allUnits.Add(playerUnits);
        allUnits.AddRange(enemyUnits);

        allUnits.Sort((a, b) => a.GetAV().CompareTo(b.GetAV()));
        Urutan = allUnits;

        Urutan[0].Selisih = 0;
        for (int i = 1; i < Urutan.Count; i++)
        {
            Urutan[i].Selisih = (int)Mathf.Max(0, Urutan[i].GetAV() - Urutan[0].GetAV());
        }

        Urutan.Sort((a, b) => a.Selisih.CompareTo(b.Selisih));
        yield return new WaitForSeconds(3f);
        SortTurnOrder();
        Debug.Log("Setup done!");
    }

    void SelectEnemy(int index)
    {
        if (enemyUnits != null && index >= 0 && index < enemyUnits.Length && enemyUnits[index] != null)
        {
            if (selectedEnemyIndex != -1)
            {
                changeColor previousRenderer = enemyUnits[selectedEnemyIndex].GetComponentInChildren<changeColor>();
                if (previousRenderer != null)
                {
                    previousRenderer.SetNewColor(Color.red);
                }
            }

            selectedEnemyIndex = index;
            changeColor currentRenderer = enemyUnits[selectedEnemyIndex].GetComponentInChildren<changeColor>();
            if (currentRenderer != null)
            {
                currentRenderer.SetNewColor(Color.green);
            }

            PlayerDirection = enemyStation[index].position - playerStation.position;
        }
        else
        {
            Debug.LogWarning("Error");
        }
    }

    void PlayerTurn()
    {
        Debug.Log("Player turn...");
        state = BattleState.PLAYERTURN;

        itemsIndex = bsHUD.GetItemsIndex();
    }

    public void Attacking(int Index)
    {
        if (state != BattleState.PLAYERTURN) return;
        StartCoroutine(PlayerAttack(Index));
    }

    IEnumerator PlayerAttack(int Index)
    {
        //damage enemy
        bool isDead = false;
        Debug.Log("EnemyIndex[" + Index + "]  && ItemsIndex[" + itemsIndex + "] ");
        if (itemsIndex == enemyUnits[Index].unitLevel)
        {
            isDead = enemyUnits[Index].TakeDamage(playerUnits.damage);
            enemyUnits[Index].GetEnemyAnimHurt();
        }
        PlayerDirection = enemyStation[1].position - playerStation.position;

        //check if the enemy is dead
        if (isDead)
        {
            yield return new WaitForSeconds(2f);
            Destroy(enemyUnits[Index].gameObject);
            CountEnemies--;
            DeadIndex.Add(Index);
            Urutan.Remove(enemyUnits[Index]);

            //if there is enemy
            if (CountEnemies > 0)
            {
                if (selectedEnemyIndex > 0)
                {
                    do
                    {
                        currentEnemyIndex--;
                        if (currentEnemyIndex < 0) currentEnemyIndex = enemyUnits.Length - 1;
                    } while (enemyUnits[currentEnemyIndex] == null);
                }
                else
                {
                    do
                    {
                        currentEnemyIndex++;
                        if (currentEnemyIndex >= enemyUnits.Length) currentEnemyIndex = 0;
                    } while (enemyUnits[currentEnemyIndex] == null);
                }
                SelectEnemy(currentEnemyIndex);
            }

            //there is no enemy left
            if (CountEnemies <= 0)
            {
                state = BattleState.WON;
                EndBattle();
            }
            else
            {
                playerUnits.Selisih = (int)playerUnits.GetAV();
                SortTurnOrderSetup();
            }
        }
        else
        {
            playerUnits.Selisih = (int)playerUnits.GetAV();
            SortTurnOrderSetup();
        }

        yield return new WaitForSeconds(2f);
        isPlayerAttacking = false;
    }

    IEnumerator EnemyTurn(int Enemyindex)
    {
        yield return new WaitForSeconds(2f);
        bool isDead = playerUnits.TakeDamage(enemyUnits[Enemyindex].damage);
        Debug.Log("Enemy" + Enemyindex + " is attacking...");

        yield return new WaitForSeconds(1f);
        if (isDead)
        {
            Destroy(playerUnits.gameObject);
            Urutan.Remove(playerUnits);
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            EnemyAction(Enemyindex);
            SortTurnOrderSetup();
        }
    }

    void EndBattle()
    {
        if (state == BattleState.WON)
        {
            bsHUD.UpdateTurnText(); Debug.Log("player WON");
        }
        else if (state == BattleState.LOST)
        {
            bsHUD.UpdateTurnText(); Debug.Log("Defeated");
        }
    }

    void LockCursor()
    {
        if (cursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
            Cursor.visible = false; // Hide the cursor
        }
        else
        {
            Cursor.lockState = CursorLockMode.None; // Release the cursor
            Cursor.visible = true; // Make the cursor visible
        }
    }

    void SortTurnOrderSetup()
    {
        // Urutkan list berdasarkan nilai action
        Urutan.Sort((a, b) => a.Selisih.CompareTo(b.Selisih));

        for (int i = Urutan.Count - 1; i >= 0; i--)
        {
            Urutan[i].Selisih -= Urutan[0].Selisih;
            if (Urutan[i].Selisih < 0)
            {
                Urutan[i].Selisih *= -1;
            }
        }
        SortTurnOrder();
    }

    public void SortTurnOrder()
    {
        //munculin gambar
        bsHUD.SetImageSequence(Urutan);

        Debug.LogWarning("Cycle ");
        for (int i = 0; i < Urutan.Count; i++)
        {
            Debug.Log(i + Urutan[i].unitName + " Action:... " + Urutan[i].Selisih);
            bsHUD.TextTurnValue(Urutan, i);
        }

        if (Urutan[0].tag == "Player")
        {
            PlayerTurn();
            bsHUD.UpdateTurnText();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            if (Urutan[0].tag == "Enemy")
            {
                int index = FindEnemyIndex();
                StartCoroutine(EnemyTurn(index));
            }
            bsHUD.UpdateTurnText();
        }
    }

    public float EnemyAction(int index)
    {
        enemyUnits[index].Selisih = (int)enemyUnits[index].GetAV();
        return enemyUnits[index].Selisih;
    }

    public BattleState GetCurrentState()
    {
        return state;
    }

    public int FindEnemyIndex()
    {
        for (int i = 0; i < enemyUnits.Length; i++)
        {
            if (Urutan[0] == enemyUnits[i])
            {
                return i;
            }
        }
        return -1;
    }
}
