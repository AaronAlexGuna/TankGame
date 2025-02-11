using UnityEngine;
using UnityEngine.AI;

public class PTController : MonoBehaviour
{
    public bool isAI = false;
    public float moveSpeed = 5f;
    public Transform turretTransform;
    public GameObject bulletPrefab;
    public Transform barrelTip;
    public float bulletSpeed = 20f;
    public float fireRate = 1f;
    public int hitPoints = 3;
    public float detectionRange = 15f;
    public float patrolRadius = 10f;

    private Rigidbody rb;
    private NavMeshAgent agent;
    private Transform player;
    private float fireCooldown = 0f;
    private Vector3 moveDirection;
    private Vector3 patrolTarget;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (isAI)
        {
            agent = GetComponent<NavMeshAgent>();
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

            if (agent != null)
                SetNewPatrolPoint(); // AI starts patrolling
        }
    }

    private void Update()
    {
        fireCooldown -= Time.deltaTime;

        if (!isAI)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
            if (Input.GetKeyDown(KeyCode.Space) && fireCooldown <= 0f)
            {
                Shoot();
                fireCooldown = fireRate;
            }
        }
        else if (isAI)
        {
            if (player != null && Vector3.Distance(transform.position, player.position) <= detectionRange)
            {
                agent.SetDestination(player.position); // Chase player

                if (fireCooldown <= 0f)
                {
                    Shoot();
                    fireCooldown = fireRate;
                }
            }
            else if (!agent.pathPending && agent.remainingDistance < 1f)
            {
                SetNewPatrolPoint(); // Continue patrolling if player is not detected
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isAI && moveDirection != Vector3.zero)
        {
            rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(Quaternion.LookRotation(moveDirection));
        }
    }

    private void Shoot()
    {
        if (bulletPrefab != null && barrelTip != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, barrelTip.position, barrelTip.rotation);

            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.linearVelocity = barrelTip.forward * bulletSpeed; // Moves forward
            }
        }
    }

    private void SetNewPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
        {
            patrolTarget = hit.position;
            agent.SetDestination(patrolTarget);
        }
    }

    public void TakeDamage(int damage)
    {
        hitPoints -= damage;
        if (hitPoints <= 0)
        {
            Destroy(gameObject);
            if (!isAI) GameManager.Instance.GameOver();
            else GameManager.Instance.AddScore(1);
        }
    }
}
