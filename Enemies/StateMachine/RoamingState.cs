using UnityEngine;
using UnityEngine.AI;

public class RoamingState : StateMachine
{
    private Vector3 m_nextWaypoint;
    private float m_IdleDuration = 5f;
    private Activity m_CurrentActivity;
    private float m_CurrentIdleTime = 0f;
    private bool m_GoingToActivity = false;
    private int m_RandomPointTentatives = 0;
    private float m_maxRoamingDistance = 10f;

    public RoamingState(Enemy enemy) : base(enemy)
    {
    }

    public override void EnterState()
    {
        Debug.Log("Enter : " + GetType().Name);
        enemy.E_NavMeshAgent.speed = enemy.RoamingSpeed;
    }

    private void Roam()
    {
        //Debug.Log("Roam");
        m_CurrentIdleTime = 0f;

        //Generate next way point for roaming
        m_RandomPointTentatives = 0;
        m_nextWaypoint = RandomNavmeshLocation(m_maxRoamingDistance);

        //Move to waypoint
        enemy.MoveTo(m_nextWaypoint);
    }

    private void Activity()
    {
        Debug.Log("Activity");
        m_CurrentIdleTime = 0f;
        enemy.MoveTo(m_CurrentActivity.Spot.transform.position);
        m_GoingToActivity = true;
    }

    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += enemy.transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = enemy.transform.position;

        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
            enemy.E_NavMeshAgent.SetDestination(finalPosition);
        }
        else if (m_RandomPointTentatives < 10)
        {
            m_RandomPointTentatives++;
            RandomNavmeshLocation(radius);
        }

        return finalPosition;
    }

    public override void UpdateState()
    {
        Collider[] hitColliders = new Collider[1];
        int nbrColliders = Physics.OverlapSphereNonAlloc(enemy.transform.position, enemy.VisualRange, hitColliders, enemy.TargetLayerMask);
        if (nbrColliders > 0 && hitColliders[0].TryGetComponent(out Health target) && target.Current > 0)
        {
            if (Vector3.Distance(enemy.transform.position, target.transform.position) < enemy.FeelingRange || enemy.IsTargetInLineOfSight(target.gameObject))
            {
                if (m_CurrentActivity != null)
                {
                    enemy.E_Animator.SetBool("Activity", false);
                    m_CurrentActivity = null;
                }
                enemy.SetState(new AggressiveState(enemy, target));
            }
        }

        if (Vector3.Distance(enemy.transform.position, m_nextWaypoint) < 2f)
        {
            enemy.Stop();
        }

        if (!enemy.Moving && m_CurrentActivity == null)
        {
            m_CurrentIdleTime += Time.deltaTime;
        }

        if (m_CurrentActivity != null && Vector3.Distance(enemy.transform.position, m_CurrentActivity.Spot.transform.position) <= enemy.E_NavMeshAgent.stoppingDistance
            && m_GoingToActivity)
        {
            enemy.transform.Rotate(0f, 180f, 0f);
            m_CurrentActivity.StartActivity();
            enemy.E_Animator.SetBool("Activity", true);

            m_CurrentActivity.OnActivityEnd += () =>
            {
                enemy.E_Animator.SetBool("Activity", false);
                m_CurrentActivity = null;
            };

            m_GoingToActivity = false;
        }

        if (m_CurrentIdleTime >= m_IdleDuration)
        {
            int nbrActivitySpots = Physics.OverlapSphereNonAlloc(enemy.transform.position, enemy.VisualRange, hitColliders, 1 << LayerMask.NameToLayer("Activity"));
            if (nbrActivitySpots > 0 && hitColliders[Random.Range(0, nbrActivitySpots)].TryGetComponent(out ActivitySpot activitySpot)
                && activitySpot.SpotType == enemy.Activity.Type && activitySpot.IsAvailable)
            {
                m_CurrentActivity = enemy.Activity;
                m_CurrentActivity.Spot = activitySpot;
                Activity();
            }
            else
            {
                Roam();
            }
        }
    }

    public override void ExitState()
    {
        enemy.E_NavMeshAgent.isStopped = true;
        enemy.E_Animator.SetBool("Moving", false);
    }
}
