using UnityEngine;
using UnityEngine.AI;

public class Soldier : MonoBehaviour
{
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    public int damage = 10;
    public int maxHP = 100;

    private Transform target;
    private NavMeshAgent agent;
    private float lastAttackTime;
    private int currentHP;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentHP = maxHP;
        InvokeRepeating(nameof(FindClosestEnemy), 0, 1f); // 매초 적 찾기
    }

    void Update()
    {
        if (target != null)
        {
            float dist = Vector3.Distance(transform.position, target.position);

            if (dist <= attackRange)
            {
                agent.isStopped = true;
                Attack();
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(target.position);
            }
        }
    }

    void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(gameObject.tag == "TeamA" ? "TeamB" : "TeamA");

        float minDist = Mathf.Infinity;
        Transform closest = null;

        foreach (var enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = enemy.transform;
            }
        }

        target = closest;
    }

    void Attack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            if (target != null)
            {
                var health = target.GetComponent<Soldier>();
                if (health != null)
                    health.TakeDamage(damage);
            }

            lastAttackTime = Time.time;
        }
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
