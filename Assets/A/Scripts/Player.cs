using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using InGame;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public enum Direction
{
    Left,
    Up,
    Right,
    Down
}

public class Player : Singleton<Player>
{
    private Rigidbody rigid;
    private Animator animator;

    public bool IsAlive { get; private set; }
    private const float BEAT_HIT_DISTANCE = 8;

    public float Speed => originSpeed + SpeedAddValue;
    [FormerlySerializedAs("speed")] public float originSpeed;

    public float SpeedAddValue
    {
        get => speedAddValue;
        set
        {
            speedAddValue = value;

            enemyCheckColliders.size = new Vector3(10, 10, Speed / originSpeed * BEAT_HIT_DISTANCE);
            enemyCheckColliders.center = new Vector3(0, 0, originSpeed / Speed * BEAT_HIT_DISTANCE / 2);
        }
    }

    private float speedAddValue;

    public float jumpPower;
    private int jumpCount;
    private const int MAX_JUMP_COUNT = 2;

    public float maxHp;
    public float hpRemoveValue;

    [HideInInspector] public List<Enemy> hitAbleEnemyList;
    [SerializeField] private BoxCollider enemyCheckColliders;
    private int attackIndex;

    public float Hp
    {
        get => hp;
        set
        {
            if (!IsAlive) return;

            hp = Mathf.Clamp(value, 0, maxHp);
            InGameManager.Instance.uiManager.UpdateHpBar(hp / maxHp);
            if (hp <= 0)
                Die();
        }
    }

    private float hp;

    public float BoostDuration { get; private set; }
    [SerializeField] private BoxCollider boostBlockFallCollider;
    [SerializeField] private ParticleSystem boostParticle;

    public float MagnetDuration { get; private set; }
    [SerializeField] private ParticleSystem magnetParticle;
    private Collider[] magnetOverlapColliders;

    protected override void OnCreated()
    {
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        Reset();
    }

    public void Reset()
    {
        rigid.velocity = Vector3.zero;
        
        boostParticle.Stop();
        magnetParticle.Stop();
        boostBlockFallCollider.gameObject.SetActive(false);
        
        animator.Play("run");
        
        speedAddValue = 0;
        BoostDuration = 0;
        attackIndex = 0;
    }

    public void GameStart()
    {
        Reset();
        hitAbleEnemyList = new List<Enemy>();

        IsAlive = true;
        Hp = maxHp;
        jumpCount = MAX_JUMP_COUNT;
    }

    private void Update()
    {
        if (GameManager.Instance.isGaming)
            GamingUpdate();
        else
            OutGamingUpdate();
    }

    private void GamingUpdate()
    {
        if (!IsAlive) return;

        Move();
        UpdateHp();
        CheckDeath();
        CheckBoost();
        CheckMagnet();
    }

    private void OutGamingUpdate()
    {
        Move();
        CheckBoost();
    }

    public void Magnet(float duration)
    {
        if (MagnetDuration <= 0)
            magnetParticle.Play();

        MagnetDuration = duration;
    }

    private void CheckMagnet()
    {
        if (MagnetDuration <= 0) return;

        MagnetDuration -= Time.deltaTime;
        magnetOverlapColliders = Physics.OverlapSphere(transform.position, Item_Magnet.MAGNET_RADIUS, LayerMask.GetMask("Getable"));
        foreach (var overlapCollider in magnetOverlapColliders)
            overlapCollider.transform.position = Vector3.MoveTowards(overlapCollider.transform.position, transform.position, Item_Magnet.MAGNET_SPEED * Time.deltaTime);

        if (MagnetDuration > 0) return;

        magnetParticle.Stop();
    }

    public void Boost(float duration)
    {
        if (BoostDuration <= 0)
        {
            boostParticle.Play();
            boostBlockFallCollider.gameObject.SetActive(true);
            SpeedAddValue += Item_Boost.BOOST_SPEED_ADD_VALUE;
            SoundManager.Instance.PlaySound("Boost", ESoundType.Sfx);
        }

        BoostDuration = duration;
    }

    private void CheckBoost()
    {
        if (BoostDuration <= 0) return;

        BoostDuration -= Time.deltaTime;
        boostBlockFallCollider.transform.position = new Vector3(transform.position.x, 0, transform.position.z);

        if (BoostDuration > 0) return;

        boostParticle.Stop();
        boostBlockFallCollider.gameObject.SetActive(false);
        SpeedAddValue -= Item_Boost.BOOST_SPEED_ADD_VALUE;
    }

    private void UpdateHp()
    {
        if (BoostDuration > 0) return;
        
        Hp -= Time.deltaTime * hpRemoveValue;
    }

    private void CheckDeath()
    {
        if (transform.position.y > -5) return;

        Die();
    }

    private void Move()
    {
        float moveValue = (originSpeed + SpeedAddValue) * Time.deltaTime;
        transform.Translate(Vector3.forward * moveValue);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider == null) return;

        if (collision.collider.CompareTag("Fall"))
            OnHitFallObject();

        if (collision.collider.CompareTag("Enemy"))
            OnHitEnemy();

        if (collision.collider.CompareTag("Road"))
            jumpCount = MAX_JUMP_COUNT;
    }

    private void OnHitFallObject()
    {
        if (BoostDuration > 0)
        {
            return;
        }

        Die();
    }


    private void OnHitEnemy()
    {
        if (BoostDuration > 0)
        {
            CheckInput(Direction.Down);
            return;
        }

        Die();
    }

    private void Die()
    {
        IsAlive = false;
        
        transform.DOKill();
        
        animator.CrossFade("Death", 0.2f);
        
        boostParticle.Stop();
        magnetParticle.Stop();
        
        var obj = PoolManager.Instance.Init("Hit Effect");
        obj.transform.position = transform.position;
        
        InGameManager.Instance.Die();
    }

    private void Jump()
    {
        if (jumpCount <= 0)
            return;

        if (hitAbleEnemyList.Count > 0)
        {
            var hitAbleList = hitAbleEnemyList.FindAll(enemy =>
                enemy.transform.position.y > TileManager.TILE_DISTANCE / 2
                && Mathf.Abs(enemy.transform.position.x - transform.position.x) < TileManager.TILE_DISTANCE / 2);
            if (hitAbleList.Count > 0)
            {
                var hitEnemy = hitAbleList.OrderBy(enemy => enemy.transform.position.z - transform.position.z).First();
                hitEnemy.Hit(1);
                PoolManager.Instance.Init("Spin Attack", transform).transform.localPosition = Vector3.up;
            }
        }

        jumpCount--;
        rigid.velocity = Vector3.up * jumpPower;
        animator.CrossFade("Jump", 0.1f, -1, 0);
    }

    public void CheckInput(Direction direction)
    {
        if (!IsAlive) return;

        switch (direction)
        {
            case Direction.Left:
                Move(-TileManager.TILE_DISTANCE);
                if (hitAbleEnemyList.Count > 0)
                {
                    var hitAbleList = hitAbleEnemyList.FindAll(enemy => enemy.transform.position.x - transform.position.x < -TileManager.TILE_DISTANCE / 2);
                    if (hitAbleList.Count > 0)
                    {
                        var hitEnemy = hitAbleList.OrderBy(enemy => Mathf.Abs(enemy.transform.position.z - transform.position.z)).First();
                        hitEnemy.Hit(1);
                        PoolManager.Instance.Init("Right To Left Attack", transform).transform.localPosition = Vector3.up;
                        animator.CrossFade("Left Attack", 0.1f, -1, 0);
                        break;
                    }
                }

                if (jumpCount >= MAX_JUMP_COUNT)
                    animator.CrossFade("Move", 0.1f);

                break;
            case Direction.Right:
                Move(TileManager.TILE_DISTANCE);
                if (hitAbleEnemyList.Count > 0)
                {
                    var hitAbleList = hitAbleEnemyList.FindAll(enemy => enemy.transform.position.x - transform.position.x > TileManager.TILE_DISTANCE / 2);
                    if (hitAbleList.Count > 0)
                    {
                        var hitEnemy = hitAbleList.OrderBy(enemy => Mathf.Abs(enemy.transform.position.z - transform.position.z)).First();
                        hitEnemy.Hit(1);
                        PoolManager.Instance.Init("Left To Right Attack", transform).transform.localPosition = Vector3.up;
                        animator.CrossFade("Right Attack", 0.1f, -1, 0);
                        break;
                    }
                }

                if (jumpCount >= MAX_JUMP_COUNT)
                    animator.CrossFade("Move", 0.1f, -1, 0);

                break;
            case Direction.Up:
                Jump();
                break;
            case Direction.Down:
                if (hitAbleEnemyList.Count > 0)
                {
                    var hitAbleList = hitAbleEnemyList.FindAll(enemy => Mathf.Abs(enemy.transform.position.x - transform.position.x) < TileManager.TILE_DISTANCE / 2);
                    if (hitAbleList.Count > 0)
                    {
                        var hitEnemy = hitAbleList.OrderBy(enemy => enemy.transform.position.z - transform.position.z).First();
                        hitEnemy.Hit(1);
                        switch (attackIndex)
                        {
                            case 0:
                                PoolManager.Instance.Init("Right To Left Attack", transform).transform.localPosition = Vector3.up;
                                animator.CrossFade("Left Attack", 0.1f, -1, 0);
                                break;
                            case 1:
                                PoolManager.Instance.Init("Left To Right Attack", transform).transform.localPosition = Vector3.up;
                                animator.CrossFade("Right Attack", 0.1f, -1, 0);
                                break;
                            case 2:
                                PoolManager.Instance.Init("Spin Attack", transform).transform.localPosition = Vector3.up;
                                animator.CrossFade("Three Attack", 0.1f, -1, 0);
                                break;
                        }

                        attackIndex = (attackIndex + 1) % 3;
                    }
                }

                break;
        }
    }

    private void Move(float moveX)
    {
        transform.DOKill(true);
        transform.DOMoveX(moveX, 0.2f).SetRelative();
    }
}