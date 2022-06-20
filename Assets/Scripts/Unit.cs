using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Unit : MonoBehaviour 
{
    private const float THORN_DAMAGE_PERCENT = 0.5f;

    public enum States
    {
        Move,
        AttackUnit,
        AttackTeam,
        Dead
    }
    public States State { get; private set; }

    public TeamSettings EnemyTeam { get; set; }
    public TeamSettings Team { get; set; }
    public UnitTypeSettings UnitType { get; set; }

    private float _health;
    private float Health
    {
        get { return _health; }
        set
        {
            _health = value;
            this.HealthBar.SetHealth(value);
        }
    }

    private float CanAttackIf0 { get; set; }
    private Unit TargetEnemy { get; set; }
    private float Direction { get; set; } = 1;
    private SpriteRenderer SpriteRenderer { get; set; }

    [field:SerializeField]
    private HealthBar HealthBar { get; set; }

    private void Awake()
    {
        this.SpriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    public void Initialize(TeamSettings team, UnitTypeSettings unitType)
    {
        this.Team = team;
        this.Team.Units.Add(this);
        this.EnemyTeam = GameManager.Instance.GetEnemyTeam(team);
        this.Direction = transform.position.x > this.EnemyTeam.Spawn.position.x ? -1 : 1;
        this.UnitType = unitType;

        this.State = States.Move;
        this.Health = unitType.Health;
        this.HealthBar.SetMaxHealth(unitType.Health);
        this.CanAttackIf0 = Random.Range(0, this.UnitType.AttackFrequency);

        transform.localScale *= unitType.Scale;
        this.SpriteRenderer.sprite = unitType.Sprite;
    }

    private void Update()
    {
        switch (this.State)
        {
            case States.Move:
                if (this.CanAttackUnit())
                {
                    break;
                }

                if (this.CanAttackTeam())
                {
                    break;
                }
                
                transform.position += new Vector3(this.Direction * this.UnitType.Speed * Time.deltaTime, 0, 0);
                break;

            case States.AttackUnit:
                this.CanAttackIf0 -= Time.deltaTime;
                if(this.CanAttackIf0 <= 0)
                {
                    this.CanAttackIf0 = this.UnitType.AttackFrequency;
                    this.Attack(this.TargetEnemy);
                }

                this.State = States.Move;
                break;

            case States.AttackTeam:
                this.CanAttackIf0 -= Time.deltaTime;
                if (this.CanAttackIf0 <= 0)
                {
                    this.CanAttackIf0 = this.UnitType.AttackFrequency;
                    this.AttackEnemyTeam();
                }

                this.State = States.Move;
                break;
        }
    }

    private bool CanAttackUnit()
    {
        this.TargetEnemy = this.EnemyTeam.GetClosest(transform.position, this.UnitType.AttackDistance);
        if (this.TargetEnemy != null)
        {
            this.State = States.AttackUnit;
            return true;
        }

        return false;
    }
    private bool CanAttackTeam()
    {
        if (Vector3.Distance(transform.position, this.EnemyTeam.Spawn.position) < this.UnitType.AttackDistance)
        {
            this.State = States.AttackTeam;
            return true;
        }

        return false;
    }

    private void AttackEnemyTeam()
    {
        this.EnemyTeam.Health -= this.UnitType.Power;
        this.ReceiveDamage(this.UnitType.Power * THORN_DAMAGE_PERCENT);
    }
    private void Attack(Unit enemy)
    {
        enemy.ReceiveDamage(this.UnitType.Power);
    }
    private void ReceiveDamage(float damage)
    {
        this.Health -= damage;
        this.Knockback(damage * 0.1f);
        if (this.Health < 0)
        {
            this.State = States.Dead;
            Destroy(this.gameObject, 1);

            // Give money to other team
            this.EnemyTeam.Money += this.UnitType.Cost - 5;
        }
    }
    private void Knockback(float power)
    {
        transform.position -= new Vector3(this.Direction * power, 0, 0);
    }

    private void OnDestroy()
    {
        this.Team.Units.Remove(this);
    }
}
