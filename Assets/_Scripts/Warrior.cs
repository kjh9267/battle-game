using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Warrior : MonoBehaviour
{
    [Header("Unit Settings")]
    public string enemyTag = "";           // 적군 태그
    public float attackRange = 3.25f;         // 공격 범위
    public float attackCooldown = 1f;      // 공격 간격
    public int damage = 1;
    public Arrow arrowPrefab;
    private Arrow arrowInstance;

    private float lastAttackTime;
    private Transform target;
    private NavMeshAgent agent;

    [Header("Unit Stats")]
    public bool isArcher = false;
    public int maxHealth = 10;   // 최대 체력
    public int currentHealth = 10;    // 현재 체력
    
    private Animator anim;
    private string _isMoving = "IsMoving";
    private string _attack = "Attack";
    private string _die = "Die";
    private bool isDead = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
        agent.stoppingDistance = attackRange;
        currentHealth = maxHealth;

        if (isArcher)
        {
            arrowInstance = Instantiate(arrowPrefab);
            arrowInstance.gameObject.SetActive(false);
        }

        if (CompareTag("TeamA")) enemyTag = "TeamB";
        else if (CompareTag("TeamB")) enemyTag = "TeamA";
    }

    void Start()
    {
        // 0.25초마다 경로 업데이트 및 공격 체크
        StartCoroutine(CombatRoutine());
    }

    private IEnumerator CombatRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.25f);

        while (true)
        {
            if (isDead) yield break;

            FindNearestEnemy();

            if (target != null)
            {
                float distance = Vector3.Distance(transform.position, target.position);

                if (distance <= attackRange)
                {
                    // 이동 완전 차단
                    agent.isStopped = true;
                    agent.ResetPath();
                    agent.velocity = Vector3.zero;

                    // 공격 방향 바라보기
                    if (target != null)
                        transform.LookAt(target.position);
                    
                    anim.SetBool(_isMoving, false);
                    AttackTarget();
                }
                else
                {
                    agent.isStopped = false;
                    agent.SetDestination(target.position);
                    anim.SetBool(_isMoving, true);
                }
            }
            else
            {
                agent.isStopped = true;
                anim.SetBool(_isMoving, false);
            }

            yield return wait;
        }
    }

    private void FindNearestEnemy()
    {
        float closestDist = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag(enemyTag))
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestEnemy = enemy.transform;
            }
        }

        target = closestEnemy;
    }

    private void AttackTarget()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;

            anim.SetTrigger(_attack);
        }
    }
    
    // 🎯 애니메이션 이벤트에서 호출할 메서드
    public void ShootArrow()
    {
        if (target == null) return;

        // 캐릭터 전방 + 약간 위쪽에서 화살 생성
        Vector3 spawnPos = transform.position + transform.forward * 1.0f + Vector3.up * 1.0f;
        arrowInstance.transform.position = spawnPos;

        // 타겟 방향으로 회전
        Quaternion lookRot = Quaternion.LookRotation(target.position - arrowInstance.transform.position);

        // 회전 보정 (화살 모델 방향에 따라 필요)
        Quaternion fixRot = lookRot * Quaternion.Euler(0f, 90f, 0f);

        arrowInstance.transform.rotation = fixRot;

        // 화살 발사
        arrowInstance.Init(target);
    }
    
    // Animator Event 호출용
    public void OnAttackHit()
    {
        Debug.Log("on attack hit called");

        if (target == null) return;

        Warrior enemyWarrior = target.GetComponent<Warrior>();
        if (enemyWarrior != null)
        {
            enemyWarrior.TakeDamage(damage);
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        anim.SetTrigger(_die);

        // Destroy는 애니메이션 끝에 이벤트에서 호출
        // 이벤트 함수: OnDieAnimationEnd
    }

    // Die 애니메이션 이벤트에 연결
    public void OnDieAnimationEnd()
    {
        Destroy(gameObject);
    }
}
