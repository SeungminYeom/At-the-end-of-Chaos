using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable] public class CardDef
{
    public Sprite img;
    public string title;
    public string desc;
    public int cardCode;
    public int resWood;
    public int resIron;
    public bool reuseable;
    public int rank;
    public int type;
}

public class CSVReader : MonoBehaviour
{
    //StreamReader sReader;
    TextAsset cardDefCsv;
    bool endOfFile = false;

    [SerializeField] CardDef tmpCard;
    CardManager cMgr;
    void Start()
    {
        cMgr = GameServerManager.instance.GetComponent<CardManager>();
        cardDefCsv = Resources.Load<TextAsset>("CardDef");
        StringReader sReader = new StringReader(cardDefCsv.text); 
        //sReader = new StreamReader("Assets/Resources/CardDef.csv");
        while (!endOfFile)
        {
            string data = sReader.ReadLine();
            if (data == null)
            {
                endOfFile= true;
                break;
            }
            var data_value = data.Split(',');
            tmpCard = new CardDef();
            for (int i = 0; i < data_value.Length; i++)
            {
                if (data_value[i] == "표시이름") break;
                switch (i)
                {
                    case 0:
                        tmpCard.title = data_value[i];
                        break;

                    case 1:
                        tmpCard.cardCode = int.Parse(data_value[i]);
                        break;

                    case 2:
                        tmpCard.desc = data_value[i];
                        break;

                    case 3:
                        tmpCard.resWood = int.Parse(data_value[i]);
                        break;

                    case 4:
                        tmpCard.resIron = int.Parse(data_value[i]);
                        break;

                    case 5:
                        tmpCard.reuseable = bool.Parse(data_value[i]);
                        break;

                    case 6:
                        tmpCard.rank = int.Parse(data_value[i]);
                        break;

                    case 7:
                        tmpCard.type = int.Parse(data_value[i]);
                        break;

                    default:
                        break;
                }
            }
            cMgr.deck.Add(tmpCard);
        }
    }

    void Update()
    {
        
    }
}
