using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class GameManager : InstanceMonoBehaviour<GameManager>
{
    [field: Header("Money")]
    [field: SerializeField]
    public int MaxMoney { get; set; } = 1000;
    [field: SerializeField]
    public int MoneyIncrement { get; set; } = 5;
    [field: SerializeField]
    public float MoneyIncrementFrequency { get; set; } = 0.5f;

    [field: Header("Teams")]
    [field: SerializeField]
    public TeamSettings Team1 { get; set; }
    [field: SerializeField]
    public TeamSettings Team2 { get; set; }

    private void Awake()
    {
        this.InvokeRepeating("IncrementMoney", this.MoneyIncrementFrequency, this.MoneyIncrementFrequency);
    }

    private void IncrementMoney()
    {
        // Increment team 1
        if(this.Team1.Money + this.MoneyIncrement > this.MaxMoney)
        {
            this.Team1.Money = this.MaxMoney;
        }
        else
        {
            this.Team1.Money += this.MoneyIncrement;
        }

        // Increment team 2
        if (this.Team2.Money + this.MoneyIncrement > this.MaxMoney)
        {
            this.Team2.Money = this.MaxMoney;
        }
        else
        {
            this.Team2.Money += this.MoneyIncrement;
        }
    }
}

[Serializable]
public class TeamSettings
{
    [field: SerializeField]
    public string Name { get; set; } = "Team";

    [field: SerializeField]
    public int Health { get; set; } = 1000;

    [SerializeField]
    private int _money = 0;
    public int Money
    {
        get { return _money;  } 
        set
        {
            int previousValue = _money;
            _money = value;
            if(previousValue != value)
            {
                this.OnMoneyChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    [field: Header("Unit types")]
    [field: SerializeField]
    public UnitTypeSettings UnitType1 { get; set; }
    [field: SerializeField]
    public UnitTypeSettings UnitType2 { get; set; }
    [field: SerializeField]
    public UnitTypeSettings UnitType3 { get; set; }
    [field: SerializeField]
    public UnitTypeSettings UnitType4 { get; set; }
    [field: SerializeField]
    public UnitTypeSettings UnitType5 { get; set; }

    public event EventHandler OnMoneyChanged;
}

[Serializable]
public class UnitTypeSettings
{
    [field: SerializeField]
    public GameObject Prefab { get; set; }

    [field: SerializeField]
    public string Name { get; set; } = "Unit";

    [field: SerializeField]
    public int Cost { get; set; } = 50;

    [field: SerializeField]
    public int Health { get; set; } = 100;

    [field: SerializeField]
    public float Power { get; set; } = 10;

    [field: SerializeField]
    public float Speed { get; set; } = 5;

    [field: SerializeField]
    public KeyCode Shortcut { get; set; }
}