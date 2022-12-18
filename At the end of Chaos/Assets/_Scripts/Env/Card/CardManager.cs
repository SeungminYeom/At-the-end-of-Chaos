using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
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

    public TMP_Text remainWood;
    public TMP_Text remainIron;
    public int remainWoodI = 0;
    public int remainIronI = 0;

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
            cards[i].img = cardsGO[i].transform.Find("CardPanel").Find("MainImage").GetComponent<Image>();
            cards[i].title = cardsGO[i].transform.Find("Title").GetChild(0).GetComponent<TMP_Text>();
            cards[i].desc = cardsGO[i].transform.Find("Desc").GetChild(0).GetComponent<TMP_Text>();
            cards[i].resWood = cardsGO[i].transform.Find("CardPanel").Find("Resources").Find("rWood").GetChild(1).GetComponent<TMP_Text>();
            cards[i].resIron = cardsGO[i].transform.Find("CardPanel").Find("Resources").Find("rIron").GetChild(1).GetComponent<TMP_Text>();
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
            if (tmpCardDef.resWood > 10)
            {
                cards[i].resWood.text = "<b><color=red>"+tmpCardDef.resWood.ToString()+"</color></b>";
            } else if(tmpCardDef.resWood == 0)
            {
                cards[i].resWood.text = "<color=#7d7d7d>" + tmpCardDef.resWood.ToString() + "</color>";
            } else
            {
                cards[i].resWood.text = tmpCardDef.resWood.ToString();
            }

            if (tmpCardDef.resIron > 10)
            {
                cards[i].resIron.text = "<b><color=red>" + tmpCardDef.resIron.ToString() + "</color></b>";
            }
            else if (tmpCardDef.resIron == 0)
            {
                cards[i].resIron.text = "<color=#7d7d7d>" + tmpCardDef.resIron.ToString() + "</color>";
            }
            else
            {
                cards[i].resIron.text = tmpCardDef.resIron.ToString();
            }

            cards[i].resWoodI = tmpCardDef.resWood;
            cards[i].resIronI = tmpCardDef.resIron;
            cards[i].rank.sprite = rank[tmpCardDef.rank-1];
            cards[i].rank.color = Color.white;
            tmpDeck.RemoveAt(rand);
        }
        //remainIron.text = remainIronI.ToString();
        //remainWood.text = remainWoodI.ToString();
        remainIron.text = GameManager.instance.ironResource.ToString();
        remainWood.text = GameManager.instance.woodResource.ToString();
    }

    public void CardSelect(int _cNum)
    {
        //_cNum은 -1 쓰는것 주의
        if (remainIronI >= cards[_cNum - 1].resIronI && remainWoodI >= cards[_cNum - 1].resWoodI)
        {
            if (cards[_cNum - 1].def.title == "재활용" && GameManager.instance.trainCount <= 1)
            {
                cards[_cNum - 1].rank.color = Color.blue;
                return;
            }
            //remainIronI -= cards[_cNum - 1].resIronI;
            //remainWoodI -= cards[_cNum - 1].resWoodI;
            GameManager.instance.ironResource -= cards[_cNum - 1].resIronI;
            GameManager.instance.woodResource -= cards[_cNum - 1].resWoodI;

            //remainIron.text = remainIronI.ToString();
            //remainWood.text = remainWoodI.ToString();
            remainIron.text = GameManager.instance.ironResource.ToString();
            remainWood.text = GameManager.instance.woodResource.ToString();

            cards[_cNum - 1].def.Selected();

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
