using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    [SerializeField] GameObject UIAnchor;
    [SerializeField] GameObject[] cardsGO;
    [SerializeField] GameObject[] playersGO;

    [SerializeField] Card[] cards;

    public List<CardDef> deck;
    public List<CardDef> tmpDeck;

    int[] randPool;

    Image tmpIMG;
    TMP_Text tmpTitle;
    TMP_Text tmpDesc;

    [SerializeField] Sprite[] images;
    [SerializeField] Sprite[] rank;

    void Start()
    {
        images = Resources.LoadAll<Sprite>("Sprites/Card/CardSprites");
        rank = Resources.LoadAll<Sprite>("Sprites/Card/RankSprites");
        cardsGO = new GameObject[4];
        playersGO = new GameObject[4];
        cards = new Card[4];
        for (int i = 0; i < 4; i++)
        {
            cardsGO[i] = UIAnchor.transform.GetChild(1).GetChild(i).gameObject;
            playersGO[i] = UIAnchor.transform.GetChild(2).GetChild(i).gameObject;
            cards[i] = cardsGO[i].GetComponent<Card>();

            cards[i].img = cardsGO[i].transform.GetChild(0).gameObject.GetComponent<Image>();
            cards[i].title = cardsGO[i].transform.GetChild(1).GetChild(0).gameObject.GetComponent<TMP_Text>();
            cards[i].desc = cardsGO[i].transform.GetChild(2).GetChild(0).gameObject.GetComponent<TMP_Text>();
            cards[i].resWood = cardsGO[i].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>();
            cards[i].resIron = cardsGO[i].transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<TMP_Text>();
        }
    }

    public void ResetCard()
    {
        tmpDeck = new List<CardDef>();
        tmpDeck = deck.ToList();
        CardDef tmpCardDef;
        int rand;
        for (int i = 0; i < cardsGO.Length; i++)
        {
            rand = UnityEngine.Random.Range(1, tmpDeck.Count);
            tmpCardDef = tmpDeck[rand];
            cards[i].img.sprite = images[0];
            cards[i].title.text = tmpCardDef.title;
            cards[i].desc.text = tmpCardDef.desc.Replace("\\n", "\n");
            cards[i].def = tmpCardDef;
            cards[i].resWood.text = tmpCardDef.resWood.ToString();
            cards[i].resIron.text = tmpCardDef.resIron.ToString();
            Debug.Log(tmpCardDef.rank - 1);
            cards[i].gameObject.GetComponent<Image>().sprite = rank[tmpCardDef.rank-1];
            tmpDeck.RemoveAt(rand);
            if (!tmpCardDef.reuseable)
            {
                deck.Remove(tmpCardDef);
            }
        }
    }

    public void CardSelect(int _cNum)
    {
        //playersGO[GameServerManager.instance.character - 1].transform.GetChild(0).GetComponent<TMP_Text>().text = "<color=white>I'm Ready!</color>";
        //playersGO[GameServerManager.instance.character - 1].transform.GetChild(1).GetComponent<TMP_Text>().text = "<color=green>" + _cNum.ToString() + ". " + cards[_cNum - 1].def.title + "</color>";
        //GameServerManager.instance.IReady = true;
        //Debug.Log(cards[_cNum - 1].def.title + " : " + cards[_cNum - 1].def.desc);
        ResetCard();
    }
}
