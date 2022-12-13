using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GunType
{
    pistol, //0
    shotgun, //1
    sniperRifle, //2
    assaultRifle //3
}

public class GunManager : MonoBehaviour
{
    static public GunManager instance;

    [SerializeField] bool[] gunUseable = { true, false, false, false };
    [SerializeField] float[] gunRanges = { 2, 2, 5, 3};
    [SerializeField] int[] gunRounds = { 10, 5, 1, 30 };

    [SerializeField] float _gunDamage = 2;
    [SerializeField] float _gunRange = 4;
    [SerializeField] float _gunReloadTime = 2;

    public float damageMultiplier = 100;
    public float attackSpeedMultiplier = 100;
    public float ammoMultiplier = 100;
    public float reloadMultiplier = 100;
    public float pierceAdd = 0;


    public float gunDamage
    {
        get { return _gunDamage; }
        set { _gunDamage = value; }
    }

    public float gunRange
    {
        get { return _gunRange; }
        set { _gunRange = value; }
    }

    public float gunReloadTime
    {
        get { return _gunReloadTime; }
        set { _gunReloadTime = value; }
    }



    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != null) Destroy(gameObject);
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetGunType(int gunTypeNum)
    {
        gunUseable[gunTypeNum] = true;
    }

    public int GetGunRounds(GunType gunTypeNum)
    {
        return gunRounds[(int)gunTypeNum];
    }

    public float GetGunRange(int gunTypeNum)
    {
        return gunRange * gunRanges[gunTypeNum];
    }

    public bool IsGunUseable(int gunTypeNum)
    {
        return gunUseable[gunTypeNum];
    }
}
