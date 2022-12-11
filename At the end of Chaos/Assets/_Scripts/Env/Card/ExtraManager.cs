using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExtraManager : MonoBehaviour
{
    public void ReloadCard()
    {
        CardManager.instance.ResetCard();
        
    }

    public void Ready()
    {
        GameServerManager.instance.pv.RPC("CardReady", Photon.Pun.RpcTarget.All, GameServerManager.instance.character - 1);
        CardManager.instance.DisableCard();
        GameServerManager.instance.IReady = true;
    }

    
}
