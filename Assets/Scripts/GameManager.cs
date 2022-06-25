using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : InstanceMonoBehaviour<GameManager>
{
    [field: Header("Money")]
    [field: SerializeField]
    public int MaxMoney { get; set; } = 1000;
    [field: SerializeField]
    public int MoneyIncrement { get; set; } = 5;
    [field: SerializeField]
    public float MoneyIncrementFrequency { get; set; } = .5f;
    [field: SerializeField]
    public float MoneyEarnedPercent { get; set; } = .8f;

    [field: Header("Teams")]
    [field: SerializeField]
    public TeamSettings Team1 { get; set; }
    [field: SerializeField]
    public TeamSettings Team2 { get; set; }

    private void Awake()
    {
        this.Team1.Initialize();
        this.Team2.Initialize();

        this.InvokeRepeating("IncrementMoney", this.MoneyIncrementFrequency, this.MoneyIncrementFrequency);
    }

    private void Update()
    {
        // Cheat code for max money :)
        if(Input.GetKey(KeyCode.LeftControl)
        && Input.GetKey(KeyCode.C)
        && Input.GetKey(KeyCode.V))
        {
            this.Team1.Money = this.MaxMoney;
            this.Team2.Money = this.MaxMoney;
        }
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
    private float _health = 1000;
    public float Health
    {
        get { return _health; }
        set
        {
            _health = value;
            this.HealthBar.SetHealth(value);

            if(_health <= 0)
            {
                this.OnTeamDead?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    [field: SerializeField]
    public HealthBar HealthBar { get; set; }

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
    public int UnitCounter { get; set; }

    public event EventHandler OnTeamDead;
    public event EventHandler OnMoneyChanged;

    public void Initialize()
    {
        this.HealthBar.SetMaxHealth(this.Health);
    }

    public IEnumerable<Unit> GetClosests(Vector3 position, float range)
    {
        return from unit in this.Units
               let distance = this.DistanceX(unit.transform.position, position)
               where distance <= range && unit.State != Unit.States.Dead
               orderby distance
               select unit;
    }
    public Unit GetClosest(Vector3 position, float range)
    {
        return this.GetClosests(position, range).FirstOrDefault();
    }
    public bool IsInRange(Vector3 position, float range, Vector3 targetPosition)
    {
        return this.DistanceX(position, targetPosition) <= range;
    }
    private float DistanceX(Vector3 position1, Vector3 position2)
    {
        return position1.x > position2.x ? position1.x - position2.x : position2.x - position1.x;
    }
}

[Serializable]
public class UnitTypeSettings
{
    [field: SerializeField]
    public string Name { get; set; } = "Unit";

    [field: SerializeField]
    public float Health { get; set; } = 100;

    [field: SerializeField]
    public float Speed { get; set; } = 5;

    [field: SerializeField]
    public int Cost { get; set; } = 50;

    [field: SerializeField]
    public float CoolDown { get; set; } = 5;
    public float CanBuildAt0 { get; set; }
    public Button Button { get; set; }

    [field: SerializeField]
    public KeyCode Shortcut { get; set; }

    [field: Header("Attack")]
    [field: SerializeField]
    public float Power { get; set; } = 10;

    [field: SerializeField]
    public float AttackFrequency { get; set; } = 1;

    [field: SerializeField]
    public float AttackDistance { get; set; } = 1f;

    [field: SerializeField]
    public bool HasAreaAttack { get; set; }

    [field: SerializeField]
    public int KnockbackCount { get; set; } = 3;

    [field: Header("Visual")]
    [field: SerializeField]
    public GameObject Prefab { get; set; }

    [field: SerializeField]
    public Sprite Sprite { get; set; }

    [field: SerializeField]
    public float Scale { get; set; } = 1;

    [field: Header("Animation")]
    [field: SerializeField]
    public RuntimeAnimatorController AnimatorController { get; set; }

    [field: SerializeField]
    public float AttackAnimDuration { get; set; } = 0.5f;

    public void StartCooldown()
    {
        this.CanBuildAt0 = this.CoolDown;
    }
    public bool CanBuild(TeamSettings team)
    {
        return team.Money >= this.Cost && this.CanBuildAt0 <= 0;
    }

    public bool ShouldKnockback(float initialHealth, float damage)
    {
        // ex:
        // 100 PV et 3 knockbacks = 25, 50, 75
        // 100 PV et 4 knockbacks = 20, 40, 60, 80
        float step = this.Health / (this.KnockbackCount + 1);
        List<float> steps = new List<float>();
        for(int i = 1; i < this.KnockbackCount; ++i)
        {
            steps.Add(step * i);
        }

        return steps.Any(s => initialHealth >= s && (initialHealth - damage) < s);
    }
}