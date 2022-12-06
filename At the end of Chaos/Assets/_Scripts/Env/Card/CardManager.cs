using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    [SerializeField] GameObject UIAnchor;
    [SerializeField] GameObject[] cardsGO;
    [SerializeField] GameObject[] playersGO;

    [SerializeField] Card[] cards;

    void Start()
    {
        cardsGO = new GameObject[4];
        playersGO = new GameObject[4];
        cards = new Card[4];
        for (int i = 0; i < 4; i++)
        {
            cardsGO[i] = UIAnchor.transform.GetChild(1).GetChild(i).gameObject;
            playersGO[i] = UIAnchor.transform.GetChild(2).GetChild(i).gameObject;

            cards[i].img = cardsGO[i].transform.GetChild(0).GetComponent<Image>();
            cards[i].title = cardsGO[i].transform.GetChild(1).GetComponent<TMP_Text>();
            cards[i].desc = cardsGO[i].transform.GetChild(1).GetComponent<TMP_Text>();
        }
        
    }

    //void SetCard(int _order, )
    //{
    //    cardsGO[_order]


    //}
}
