using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
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

    public float Speed => originSpeed + speedAddValue;
    [FormerlySerializedAs("speed")] public float originSpeed;
    public float speedAddValue;

    public float jumpPower;
    private int jumpCount;
    private const int MAX_JUMP_COUNT = 2;

    public float maxHp;
    public float hpRemoveValue;

    private int attackIndex;

    public float Hp
    {
        get => hp;
        set
        {
            if (!IsAlive) return;

            hp = Mathf.Clamp(value, 0, maxHp);
            UIManager.Instance.UpdateHpBar(hp / maxHp);
            if (hp <= 0)
                Die();
        }
    }

    private float hp;
    [HideInInspector] public List<Enemy> hitAbleEnemyList;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        hitAbleEnemyList = new List<Enemy>();

        IsAlive = true;
        Hp = maxHp;
        attackIndex = 0;
        jumpCount = MAX_JUMP_COUNT;
    }

    private void Update()
    {
        if (!IsAlive) return;

        Move();
        UpdateHp();
        CheckDeath();
    }

    private void UpdateHp()
    {
        Hp -= Time.deltaTime * hpRemoveValue;
    }

    private void CheckDeath()
    {
        if (transform.position.y > -5) return;
        
        Die();
    }

    private void Move()
    {
        float moveValue = (originSpeed + speedAddValue) * Time.deltaTime;
        transform.Translate(Vector3.forward * moveValue);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider == null) return;

        if (collision.collider.CompareTag("Fall"))
            Die();

        if (collision.collider.CompareTag("Enemy"))
            Die();

        if (collision.collider.CompareTag("Road"))
            jumpCount = MAX_JUMP_COUNT;
    }

    private void Die()
    {
        SoundManager.Instance.PlaySound("", ESoundType.Bgm);
        transform.DOKill();
        animator.CrossFade("Death", 0.2f);
        IsAlive = false;
    }

    private void Jump()
    {
        if (jumpCount <= 0)
            return;

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