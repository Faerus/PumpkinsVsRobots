using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Unit : MonoBehaviour 
{
    private const float COLLISION_RADIUS = 0.9f;
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

    [field:SerializeField]
    public float Health { get; set; }
    [field: SerializeField]
    public float MaxHealth { get; set; }

    [field: SerializeField]
    public float Power { get; set; }
    [field: SerializeField]
    public float AttackFrequency { get; set; } = 1;
    private float CanAttackIf0 { get; set; }
    public Unit TargetEnemy { get; set; }
    
    [field: SerializeField]
    public float Speed { get; set; }

    private SpriteRenderer SpriteRenderer { get; set; }

    private void Awake()
    {
        this.SpriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    public void Initialize(TeamSettings team, UnitTypeSettings unitType)
    {
        this.UnitType = unitType;
        this.Team = team;
        this.Team.Units.Add(this);
        this.EnemyTeam = GameManager.Instance.GetEnemyTeam(team);

        this.State = States.Move;
        this.MaxHealth = unitType.Health;
        this.Health = this.MaxHealth;
        this.Power = unitType.Power;
        this.Speed = unitType.Speed;

        if(this.SpriteRenderer != null)
        { 
            this.SpriteRenderer.sprite = unitType.Sprite;
        }
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

                transform.position = Vector3.MoveTowards(transform.position, this.EnemyTeam.Spawn.position, this.Speed * Time.deltaTime);
                break;

            case States.AttackUnit:
                this.CanAttackIf0 -= Time.deltaTime;
                if(this.CanAttackIf0 <= 0)
                {
                    this.CanAttackIf0 = this.AttackFrequency;
                    this.Attack(this.TargetEnemy);
                }

                this.State = States.Move;
                break;

            case States.AttackTeam:
                this.CanAttackIf0 -= Time.deltaTime;
                if (this.CanAttackIf0 <= 0)
                {
                    this.CanAttackIf0 = this.AttackFrequency;
                    this.AttackEnemyTeam();
                }

                if (this.EnemyTeam.Health < 0)
                {
                    Debug.Log("Victoire !!!");
                }

                this.State = States.Move;
                break;
        }
    }

    private bool CanAttackUnit()
    {
        this.TargetEnemy = this.EnemyTeam.GetClosest(transform.position, COLLISION_RADIUS);
        if (this.TargetEnemy != null)
        {
            this.State = States.AttackUnit;
            return true;
        }

        return false;
    }
    private bool CanAttackTeam()
    {
        if (Vector3.Distance(transform.position, this.EnemyTeam.Spawn.position) < COLLISION_RADIUS * 2)
        {
            this.State = States.AttackTeam;
            return true;
        }

        return false;
    }

    private void AttackEnemyTeam()
    {
        this.EnemyTeam.Health -= this.Power;
        this.ReceiveDamage(this.Power * THORN_DAMAGE_PERCENT);
    }
    private void Attack(Unit enemy)
    {
        enemy.ReceiveDamage(this.Power);
    }
    private void ReceiveDamage(float damage)
    {
        this.Health -= damage;
        this.Knockback(damage * 0.1f);
        if (this.Health < 0)
        {
            this.State = States.Dead;
            Destroy(this.gameObject, 1);
        }
    }
    private void Knockback(float power)
    {
        transform.position = Vector3.MoveTowards(transform.position, this.Team.Spawn.position, power);
    }

    private void OnDestroy()
    {
        this.Team.Units.Remove(this);
    }
}
