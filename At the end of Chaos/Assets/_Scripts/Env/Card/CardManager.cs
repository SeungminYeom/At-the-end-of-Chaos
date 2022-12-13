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
    int remainWoodI = 100;
    int remainIronI = 100;

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

            switch (cards[_cNum - 1].def.cardCode) //임시로 사용
            {
                case 0:
                    GunManager.instance.attackSpeedMultiplier += 15;
                    GunManager.instance.reloadMultiplier *= (100 + (-5)) / 100;
                break;

                case 1:
                    GunManager.instance.damageMultiplier += 75;
                    GunManager.instance.attackSpeedMultiplier *= (100 + (-15)) / 100;
                    GunManager.instance.reloadMultiplier *= (100 + (-15)) / 100;
                    break;

                case 2:
                    GunManager.instance.ammoMultiplier += 15;
                    break;

                case 3:
                    remainIronI += UnityEngine.Random.Range(0, 11);
                    remainWoodI += UnityEngine.Random.Range(0, 11);
                    remainIron.text = remainIronI.ToString();
                    remainWood.text = remainWoodI.ToString();
                    break;

                case 4:
                    GameManager.instance.trainCount++;
                    break;

                case 5:
                    //TrainManager.instance.GetTrain(GameManager.instance.trainCount).GetComponent<Train>().RestoreHealth();
                    break;

                case 6:
                    //TrainManager.instance.GetTrain(GameManager.instance.trainCount).GetComponent<Train>().maxHealth =
                    //    (int)(TrainManager.instance.GetTrain(GameManager.instance.trainCount).GetComponent<Train>().maxHealth * 1.07f);
                    break;

                case 7:
                    break;

                case 8:
                    GameServerManager.instance.player.GetComponent<PlayerMovement>().moveSpeed *= 1.1f;
                    break;

                case 9:
                    GameManager.instance.trainCount++;
                    remainIronI += 10;
                    remainWoodI += 10;
                    remainIron.text = remainIronI.ToString();
                    remainWood.text = remainWoodI.ToString();
                    break;

                case 10:
                    break;

                case 11:
                    GunManager.instance.damageMultiplier += 8;
                    break;

                case 12:
                    GunManager.instance.attackSpeedMultiplier *= (100 + (50)) / 100;
                    GunManager.instance.ammoMultiplier += 100;
                    GunManager.instance.reloadMultiplier *= (100 + (-30)) / 100;
                    break;

                case 13:
                    GunManager.instance.pierceAdd += 5;
                    break;

                case 14:
                    GunManager.instance.attackSpeedMultiplier *= (100 + (10)) / 100;
                    break;

                case 15:
                    GunManager.instance.ammoMultiplier += -10;
                    GunManager.instance.reloadMultiplier *= (100 + (15)) / 100;
                    break;

                case 16:
                    GunManager.instance.reloadMultiplier *= (100 + (15)) / 100;
                    break;

                case 17:
                    GameManager.instance.timeAfternoonValue += 5;
                    GameManager.instance.wfs_Afternoon = new WaitForSeconds(GameManager.instance.timeAfternoonValue);
                    break;

                case 18:
                    //for (int i = 0; i < 5; i++)
                    //{
                    //    TrainManager.instance.GetTrain(i).GetComponent<Train>().maxHealth /= 2;
                    //}
                    ZombieManager.instance.speedMultiplier *= 1.25f;
                    GunManager.instance.reloadMultiplier *= (100 + (300)) / 100;
                    break;

                case 19:
                    break;

                case 20:
                    break;

                case 21:
                    break;

            }

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
