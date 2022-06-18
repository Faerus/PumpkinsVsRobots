using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public TeamSettings GetEnemyTeam(TeamSettings myTeam)
    {
        return this.Team1 == myTeam ? this.Team2 : this.Team1;
    }
}

[Serializable]
public class TeamSettings
{
    [field: SerializeField]
    public string Name { get; set; } = "Team";

    [field: SerializeField]
    public float Health { get; set; } = 1000;

    [field: SerializeField]
    public Transform Spawn { get; set; }

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

    [field: SerializeField]
    public List<UnitTypeSettings> UnitTypes { get; set; } = new List<UnitTypeSettings>();

    public List<Unit> Units { get; set; } = new List<Unit>();

    public event EventHandler OnMoneyChanged;

    public Unit GetClosest(Vector3 position, float range)
    {
        var query = from unit in this.Units
                    let distance = Vector3.Distance(position, unit.transform.position)
                    where distance <= range && unit.State != Unit.States.Dead
                    orderby distance
                    select unit;

        return query.FirstOrDefault();
    }
}

[Serializable]
public class UnitTypeSettings
{
    [field: SerializeField]
    public GameObject Prefab { get; set; }

    [field: SerializeField]
    public Sprite Sprite { get; set; }

    [field: SerializeField]
    public string Name { get; set; } = "Unit";

    [field: SerializeField]
    public int Cost { get; set; } = 50;

    [field: SerializeField]
    public float Health { get; set; } = 100;

    [field: SerializeField]
    public float Power { get; set; } = 10;

    [field: SerializeField]
    public float Speed { get; set; } = 5;

    [field: SerializeField]
    public KeyCode Shortcut { get; set; }
}