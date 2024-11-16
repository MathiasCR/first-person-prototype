using UnityEngine;

public class AggressiveState : StateMachine
{
    private float m_TimeWithoutLineOfSight = 0f;
    private float m_MaxTimeWithoutLineOfSight = 10f;
    private float m_LostAggroRange = 50f;

    public AggressiveState(Enemy enemy, Health target) : base(enemy)
    {
        Debug.Log("AggressiveState constructor : " + target);
        enemy.SetTarget(target);
    }

    public override void EnterState()
    {
        Debug.Log("Enter AgressiveState");
        enemy.E_NavMeshAgent.speed = enemy.AggroSpeed;
    }

    public override void UpdateState()
    {
        float distanceFromTarget = Vector3.Distance(enemy.transform.position, enemy.Target.transform.position);

        if (enemy.IsTargetInLineOfSight(enemy.Target.gameObject))
        {
            m_TimeWithoutLineOfSight = 0f;
        }
        else
        {
            m_TimeWithoutLineOfSight += Time.deltaTime;
        }

        if (enemy.CanAct && enemy.IsTargetInLineOfSight(enemy.Target.gameObject))
        {
            Vector3 direction = enemy.Target.transform.position;
            direction.y = enemy.transform.position.y;
            enemy.transform.LookAt(direction);
            enemy.Stop();

            if (!enemy.IsUsingAbility && Tools.IsBetween(enemy.AbilityMinRange, enemy.AbilityMaxRange, distanceFromTarget))
            {
                enemy.Ability();
            }
            else if (!enemy.IsAttacking && distanceFromTarget < enemy.AttackRange)
            {
                enemy.Attack();
            }
            else
            {
                enemy.MoveTo(enemy.Target.transform.position);
            }
        }
        else if (enemy.CanAct)
        {
            enemy.MoveTo(enemy.Target.transform.position);
        }

        if (distanceFromTarget > m_LostAggroRange || enemy.Target.Current <= 0 || m_TimeWithoutLineOfSight > m_MaxTimeWithoutLineOfSight)
        {
            enemy.SetState(new RoamingState(enemy));
        }
    }

    public override void ExitState()
    {
        Debug.Log("Exit AgressiveState");
        enemy.SetTarget(null);
        enemy.E_NavMeshAgent.SetDestination(enemy.SpawnPoint);
    }
}
