using UnityEngine;

public class KillerOrbs : MonoBehaviour, IDamageable
{




    [Header("Detection")]
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float attackRange = 2f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;

    [Header("Attack")]
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackRate = 1f;

    [Header("Pre-Attack")]
    [SerializeField] private float preAttackTime = 1.5f;

    [Header("Orb Visuals")]
    [SerializeField] private Renderer[] orbs;
    [SerializeField] private Color idleColor = Color.white;
    [SerializeField] private Color chaseColor = Color.yellow;
    [SerializeField] private Color preAttackColor = new Color(1f, 0.5f, 0f);
    [SerializeField] private Color attackColor = Color.red;

    private Transform player;

    private float attackTimer;
    private float preAttackTimer;

    private bool isPreAttacking = false;
    [SerializeField] private float MaxHealth;
    private float health;

    void Awake()
    {
        health = MaxHealth;
    }

    void Start()
    {
        
        foreach (Renderer orb in orbs)
        {
            orb.material = new Material(orb.material);
        }
    }

    void Update()
    {
        
        if (player == null)
        {
            ResetState();

            SetOrbColorSmooth(idleColor);
            FindPlayer();
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        
        if (distance > detectionRange)
        {
            player = null;
            ResetState();
            return;
        }

        LookAtPlayer();

        if (distance > attackRange)
        {
            
            isPreAttacking = false;
            SetOrbColorSmooth(chaseColor);
            MoveToPlayer();
        }
        else
        {
            
            HandlePreAttack();
        }
    }

    private void ResetState()
    {
        isPreAttacking = false;
        preAttackTimer = 0f;
        attackTimer = 0f;
    }

    private void FindPlayer()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            float dist = Vector3.Distance(transform.position, p.transform.position);
            if (dist <= detectionRange)
            {
                player = p.transform;
            }
        }
    }

    private void MoveToPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;

        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    private void LookAtPlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;

        if (dir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }

    private void HandlePreAttack()
    {
        if (!isPreAttacking)
        {
            isPreAttacking = true;
            preAttackTimer = preAttackTime;
        }

        preAttackTimer -= Time.deltaTime;

        
        if (preAttackTimer > 0f)
        {
            float pulse = Mathf.PingPong(Time.time * 5f, 1f);
            Color pulseColor = Color.Lerp(preAttackColor, attackColor, pulse);
            SetOrbColorSmooth(pulseColor);
            return;
        }

      
        SetOrbColorSmooth(attackColor);
        Attack();

        isPreAttacking = false;

        attackTimer = 0f;
    }

    private void Attack()
    {
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            if (player == null) return;

            IDamageable damageable = player.GetComponent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(attackDamage);
            }

            attackTimer = 1f / attackRate;
        }
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void SetOrbColorSmooth(Color targetColor)
    {
        foreach (Renderer orb in orbs)
        {
            orb.material.color = Color.Lerp(orb.material.color, targetColor, 10f * Time.deltaTime);

            orb.material.EnableKeyword("_EMISSION");
            orb.material.SetColor("_EmissionColor", targetColor * 2f);
        }
    }
}