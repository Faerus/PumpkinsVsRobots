using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
public class Unit : MonoBehaviour 
{
    private const float THORN_DAMAGE_PERCENT = 0.5f;

    public enum States
    {
        Move,
        AttackUnit,
        AttackCastle,
        AnimationInProgress,
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
    private Animator Animator { get; set; }
    private bool AnimationInProgress { get; set; }

    [field:SerializeField]
    private HealthBar HealthBar { get; set; }

    private void Awake()
    {
        this.SpriteRenderer = this.GetComponent<SpriteRenderer>();
        this.Animator = this.GetComponent<Animator>();
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
        this.Animator.runtimeAnimatorController = unitType.AnimatorController;
        this.PlayAnimation("Walk");
    }

    private void Update()
    {
        if(this.AnimationInProgress)
        {
            return;
        }

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

                this.PlayAnimation("Walk");
                transform.position += new Vector3(this.Direction * this.UnitType.Speed * Time.deltaTime, 0, 0);
                break;

            case States.AttackUnit:
            case States.AttackCastle:
                this.CanAttackIf0 -= Time.deltaTime;
                if (this.CanAttackIf0 <= 0)
                {
                    this.CanAttackIf0 = this.UnitType.AttackFrequency;
                    this.PlayAndInvoke("Attack", this.State.ToString(), this.UnitType.AttackAnimDuration);
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
            this.State = States.AttackCastle;
            return true;
        }

        return false;
    }

    private void PlayAnimation(string animation)
    {
        if(this.Animator != null && this.Animator.runtimeAnimatorController != null)
        {
            this.Animator.Play(animation);
        }
    }
    private void PlayAndInvoke(string animation, string callbackMethod, float callbackDelay)
    {
        this.PlayAnimation(animation);
        this.AnimationInProgress = true;
        this.Invoke(callbackMethod, callbackDelay);
    }
    private void AttackCastle()
    {
        // Miss if ennemi is not in range anymore
        if(this.Team.IsInRange(transform.position, this.UnitType.AttackDistance, this.EnemyTeam.Spawn.position))
        {
            this.EnemyTeam.Health -= this.UnitType.Power;
            this.ReceiveDamage(this.UnitType.Power * THORN_DAMAGE_PERCENT);
        }
        else
        {
            // Miss if enemy is not in range anymore
            Debug.Log("Attack missed !");
        }

        this.AnimationInProgress = false;
    }
    private void AttackUnit()
    {
        if (this.Team.IsInRange(transform.position, this.UnitType.AttackDistance, this.TargetEnemy.transform.position))
        {
            this.TargetEnemy.ReceiveDamage(this.UnitType.Power);
        }
        else
        {
            // Miss if enemy is not in range anymore
            Debug.Log("Attack missed !");
        }
        
        this.AnimationInProgress = false;
    }
    private void ReceiveDamage(float damage)
    {
        this.Health -= damage;
        this.Knockback(damage * 0.1f);
        if (this.Health < 0)
        {
            this.State = States.Dead;
            this.PlayAnimation("Die");
            Destroy(this.gameObject, 3);

            // Give money to other team
            this.EnemyTeam.Money += this.UnitType.Cost - 5;
        }
    }
    private void Knockback(float power)
    {
        float newX = transform.position.x - this.Direction * power;
        if((this.Direction < 0 && newX > this.Team.Spawn.position.x)
        || (this.Direction > 0 && newX < this.Team.Spawn.position.x))
        {
            newX = this.Team.Spawn.position.x;
        }

        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }

    private void OnDestroy()
    {
        this.Team.Units.Remove(this);
    }
}
