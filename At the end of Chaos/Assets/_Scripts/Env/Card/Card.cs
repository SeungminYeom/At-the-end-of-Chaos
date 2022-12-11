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
    public CardDef def;

    CardManager cMgr;

    private void Start()
    {
        cMgr = GameServerManager.instance.GetComponent<CardManager>();
    }

    void OnClick()
    {
        cMgr.CardSelect(int.Parse((Regex.Match(gameObject.name, "(?:Card?)([0-9]?)").Groups[1].Value)));
    }
}
