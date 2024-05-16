using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BS_HUD : MonoBehaviour
{
    public TextMeshProUGUI turnText;
    public Image items;
    public Sprite[] SpriteItems;
    public List<Image> LogoTurnSequence;
    public Sprite[] LogoTurn;
    int countDead;

    public void Start()
    {
        countDead = LogoTurnSequence.Count;
    }

    public void UpdateTurnText()
    {
        if (turnText != null)
        {
            BattleState currentState = BattleSystem.Instance.GetCurrentState();

            switch (currentState)
            {
                case BattleState.START:
                    turnText.text = "Battle Start";
                    break;
                case BattleState.PLAYERTURN:
                    turnText.text = "Player Turn";
                    break;
                case BattleState.ENEMYTURN:
                    turnText.text = "Enemy Turn";
                    break;
                case BattleState.WON:
                    turnText.text = "You Won!";
                    break;
                case BattleState.LOST:
                    turnText.text = "You Lost!";
                    break;
                default:
                    turnText.text = "";
                    break;
            }
        }
        else
        {
            Debug.LogWarning("Turn Text not assigned.");
        }
    }

    public int GetItemsIndex()
    {
        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, SpriteItems.Length);
            //Debug.Log("+++++++ Rand: " + randomIndex);
        }
        while (BattleSystem.Instance.DeadIndex.Contains(randomIndex));
        items.sprite = SpriteItems[randomIndex];
        return randomIndex;
    }

    public void SetImageSequence(List<Units> Allunits)
    {
        for (int i = 0; i < Allunits.Count; i++)
        {
            if (Allunits[i].tag == "Player")
            {
                LogoTurnSequence[i].sprite = LogoTurn[3];
            }
            else if (Allunits[i].unitName == "B3")
            {
                LogoTurnSequence[i].sprite = LogoTurn[0];
            }
            else if (Allunits[i].unitName == "Nonorganik")
            {
                LogoTurnSequence[i].sprite = LogoTurn[1];
            }
            else if (Allunits[i].unitName == "Organik")
            {
                LogoTurnSequence[i].sprite = LogoTurn[2];
            }
            else
            {
                Debug.LogError("Out of Index");
            }
        }

        if (Allunits.Count < countDead)
        {
            countDead--;
            Destroy(LogoTurnSequence[countDead].gameObject);
        }
    }

    // Method moved outside SetImageSequence
    public string GetTextFromLogoTurnSequence(int index)
    {
        if (index >= 0 && index < LogoTurnSequence.Count)
        {
            TextMeshProUGUI textComponent = LogoTurnSequence[index].GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                return textComponent.text;
            }
            else
            {
                Debug.LogWarning("TextMeshProUGUI component not found in the child object of LogoTurnSequence[" + index + "]");
                return null;
            }
        }
        else
        {
            Debug.LogWarning("Index out of range.");
            return null;
        }
    }

    public void TextTurnValue(List<Units> Allunits, int index)
    {
        //untuk tulisan index turn
        TextMeshProUGUI textComponent = LogoTurnSequence[index].GetComponentInChildren<TextMeshProUGUI>();
        string avString;

        avString = Allunits[index].Selisih.ToString("F0");
        textComponent.text = avString;
    }
}
