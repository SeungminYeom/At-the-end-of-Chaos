using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Image img;
    public TMP_Text title;
    public TMP_Text desc;
    public TMP_Text resWood;
    public TMP_Text resIron;
    public int resWoodI;
    public int resIronI;
    public CardDef def;
    public Image rank;

    Color tmpColor;

    CardManager cMgr;

    private void Start()
    {
        cMgr = GameServerManager.instance.GetComponent<CardManager>();
        rank = GetComponent<Image>();
    }

    private void Update()
    {
        tmpColor = rank.color;
        tmpColor.g += 0.01f;
        tmpColor.b += 0.01f;

        rank.color= tmpColor;
    }

    void OnClick()
    {
        cMgr.CardSelect(int.Parse((Regex.Match(gameObject.name, "(?:Card?)([0-9]?)").Groups[1].Value)));
    }
}
