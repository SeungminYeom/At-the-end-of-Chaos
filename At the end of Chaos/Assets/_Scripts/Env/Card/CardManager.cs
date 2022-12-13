using Photon.Pun;
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
    public static CardManager instance;

    [SerializeField] GameObject UIAnchor;
    [SerializeField] GameObject[] cardsGO;
    public GameObject[] playersGO;

    [SerializeField] Card[] cards;

    public List<CardDef> deck;
    public List<CardDef> tmpDeck;

    int[] randPool;

    Image tmpIMG;
    TMP_Text tmpTitle;
    TMP_Text tmpDesc;

    [SerializeField] Sprite[] images;
    [SerializeField] Sprite[] rank;

    TMP_Text remainWood;
    TMP_Text remainIron;
    int remainWoodI = 10;
    int remainIronI = 10;

    Button ready;
    Button reload;

    void Start()
    {
        instance = this;    

        images = Resources.LoadAll<Sprite>("Sprites/Card/CardSprites");
        rank = Resources.LoadAll<Sprite>("Sprites/Card/RankSprites");
        cardsGO = new GameObject[4];
        playersGO = new GameObject[4];
        cards = new Card[4];
        for (int i = 0; i < 4; i++)
        {
            cardsGO[i] = UIAnchor.transform.Find("CardPanel").GetChild(i).gameObject;
            playersGO[i] = UIAnchor.transform.Find("Other UI").GetChild(i).gameObject;

            cards[i] = cardsGO[i].GetComponent<Card>();
            cards[i].img = cardsGO[i].transform.Find("MainSprite").GetComponent<Image>();
            cards[i].title = cardsGO[i].transform.Find("Title").GetChild(0).GetComponent<TMP_Text>();
            cards[i].desc = cardsGO[i].transform.Find("Desc").GetChild(0).GetComponent<TMP_Text>();
            cards[i].resWood = cards[i].img.transform.GetChild(0).Find("rWood").GetChild(0).GetComponent<TMP_Text>();
            cards[i].resIron = cards[i].img.transform.GetChild(0).Find("rIron").GetChild(0).GetComponent<TMP_Text>();
            cards[i].rank = cardsGO[i].transform.Find("Shield").GetComponent<Image>();

            switch (GameServerManager.instance.resolutionMode)
            { //카드의 설명이 낮은 해상도에서 너무 커지는 문제 수정용
                case 0:
                    cards[i].desc.fontSizeMax = 35;
                    cards[i].title.fontSizeMax = 45;
                    break;

                case 1:
                    cards[i].desc.fontSizeMax = 20;
                    cards[i].title.fontSizeMax = 20;
                    break;

                default:
                    break;
            }
        }

        remainWood = UIAnchor.transform.Find("CardPanel").Find("Extras").Find("Resources").Find("Remain").Find("Wood").GetChild(1).GetComponent<TMP_Text>();
        remainIron = UIAnchor.transform.Find("CardPanel").Find("Extras").Find("Resources").Find("Remain").Find("Iron").GetChild(1).GetComponent<TMP_Text>();
        ready = UIAnchor.transform.Find("CardPanel").Find("Extras").Find("ReadyBtn").GetChild(0).GetComponent<Button>();
        reload = UIAnchor.transform.Find("CardPanel").Find("Extras").Find("ReloadBtn").GetChild(0).GetComponent<Button>();
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
            try
            { //있으면 가져다 쓰고
                //Debug.Log(tmpCardDef.title + " : " + tmpCardDef.cardCode);
                cards[i].img.sprite = images[tmpCardDef.cardCode];
            }
            catch
            { //없으면 기본 이미지로
                cards[i].img.sprite = images[images.Length - 1];
            }
            
            cards[i].title.text = tmpCardDef.title;
            cards[i].desc.text = tmpCardDef.desc.Replace("\\n", "\n");
            cards[i].def = tmpCardDef;
            cards[i].resWood.text = tmpCardDef.resWood.ToString();
            cards[i].resIron.text = tmpCardDef.resIron.ToString();
            cards[i].resWoodI = tmpCardDef.resWood;
            cards[i].resIronI = tmpCardDef.resIron;
            cards[i].rank.sprite = rank[tmpCardDef.rank-1];
            cards[i].rank.color = Color.white;
            tmpDeck.RemoveAt(rand);
        }
        remainIron.text = remainIronI.ToString();
        remainWood.text = remainWoodI.ToString();
    }

    public void CardSelect(int _cNum)
    {
        //_cNum은 -1 쓰는것 주의
        if (remainIronI >= cards[_cNum - 1].resIronI && remainWoodI >= cards[_cNum - 1].resWoodI)
        {
            remainIronI -= cards[_cNum - 1].resIronI;
            remainWoodI -= cards[_cNum - 1].resWoodI;

            remainIron.text = remainIronI.ToString();
            remainWood.text = remainWoodI.ToString();

            if (!cards[_cNum-1].def.reuseable)
            {
                deck.Remove(cards[_cNum - 1].def);
            }

            ResetCard();
        } else
        {
            cards[_cNum - 1].rank.color = Color.red;
        }
    }

    public void EnableCard()
    {
        for (int i = 0; i < 4; i++)
        {
            cardsGO[i].GetComponent<Button>().interactable = true;
            if (GameServerManager.instance.whatCharacterIsActive[i] == true)
            {
                playersGO[GameServerManager.instance.character - 1].transform.GetChild(0).GetComponent<TMP_Text>().text = "";
                playersGO[GameServerManager.instance.character - 1].GetComponent<Image>().color = Color.white;
            }
        }
        reload.interactable = true;
        ready.interactable = true;
    }
    
    public void DisableCard()
    {
        reload.interactable = false;
        ready.interactable = false;
        for (int i = 0; i < 4; i++)
        {
            cardsGO[i].GetComponent<Button>().interactable = false;
        }

    }

    public void PlayerInit()
    {
        Vector2 tmpVec = Vector2.zero;
        float offset = 0;
        float expandOffset = 1;
        expandOffset /= PhotonNetwork.CurrentRoom.PlayerCount;
        for (int i = 0; i < 4; i++)
        {
            if (GameServerManager.instance.whatCharacterIsActive[i] == false)
            {
                playersGO[i].SetActive(false);
            } else
            {
                tmpVec.y = 0;
                tmpVec.x = offset;
                playersGO[i].GetComponent<RectTransform>().anchorMin = tmpVec;
                offset += expandOffset;
                tmpVec.x = offset;
                tmpVec.y = 1;
                playersGO[i].GetComponent<RectTransform>().anchorMax = tmpVec;
            }
        }
    }
}
