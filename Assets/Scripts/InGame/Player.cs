using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using InGame;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : Singleton<Player>
{
    public int line;

    private Rigidbody rigid;
    private Animator animator;

    [Space(15f)] public SkinnedMeshRenderer[] defaultMeshes;
    public SkinnedMeshRenderer[] dissolveMeshes;

    [Space(15f)] public PlayerEffectData effectData;

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

            enemyCheckColliders.size = new Vector3(6, 10, Speed / originSpeed * BEAT_HIT_DISTANCE);
            enemyCheckColliders.center = new Vector3(0, 0, originSpeed / Speed * BEAT_HIT_DISTANCE / 2);
        }
    }

    private float speedAddValue;

    public float jumpPower;
    private int jumpCount;
    private const int MAX_JUMP_COUNT = 2;

    [HideInInspector] public List<Enemy> hitAbleEnemyList;

    [Space(10f)] [SerializeField] private BoxCollider enemyCheckColliders;
    private int attackIndex;

    [SerializeField] private BoxCollider boostBlockFallCollider;
    [SerializeField] private ParticleSystem boostParticle;
    public float BoostDuration { get; private set; }

    [SerializeField] private ParticleSystem magnetParticle;
    private float magnetDuration;
    private Collider[] magnetOverlapColliders;

    protected override void OnCreated()
    {
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        Reset();
        effectData.Init();
    }

    public void Reset()
    {
        line = 0;

        rigid.velocity = Vector3.zero;
        rigid.useGravity = false;

        boostParticle.Stop();
        magnetParticle.Stop();
        boostBlockFallCollider.gameObject.SetActive(false);

        animator.Play("run");

        speedAddValue = TileManager.Instance.bgmData.speedAdder;
        BoostDuration = 0;
        attackIndex = 0;

        foreach (var skinnedMeshRenderer in defaultMeshes)
            skinnedMeshRenderer.gameObject.SetActive(true);

        foreach (var skinnedMeshRenderer in dissolveMeshes)
            skinnedMeshRenderer.gameObject.SetActive(false);
    }

    public void GameStart()
    {
        Reset();
        hitAbleEnemyList = new List<Enemy>();

        IsAlive = true;
        jumpCount = MAX_JUMP_COUNT;
        rigid.useGravity = true;

#if UNITY_EDITOR
        if (!GameManager.Instance.isDebugging) return;
        boostBlockFallCollider.gameObject.SetActive(true);
#endif
    }

    private void Update()
    {
        if (GameManager.Instance.isGaming)
            GamingUpdate();
        else
            OutGamingUpdate();
    }

    private void FixedUpdate()
    {
        if (!IsAlive && GameManager.Instance.isGaming) return;

        Move(Time.fixedDeltaTime);
    }

    private void GamingUpdate()
    {
        if (!IsAlive) return;

        CheckDeath();
        CheckBoost();
        CheckMagnet();
        CheckInv();
    }

    private void CheckInv()
    {
#if UNITY_EDITOR
        if (GameManager.Instance.isDebugging) return;
#endif

        boostBlockFallCollider.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }

    private void OutGamingUpdate()
    {
        CheckBoost();
    }

    public void Magnet(float duration)
    {
        if (magnetDuration <= 0)
            magnetParticle.Play();

        magnetDuration = duration;
    }

    private void CheckMagnet()
    {
        if (magnetDuration <= 0) return;

        magnetDuration -= Time.deltaTime;
        magnetOverlapColliders = Physics.OverlapSphere(transform.position, Item_Magnet.MAGNET_RADIUS, LayerMask.GetMask("Getable"));
        foreach (var overlapCollider in magnetOverlapColliders)
            overlapCollider.transform.position = Vector3.MoveTowards(overlapCollider.transform.position, transform.position, Item_Magnet.MAGNET_SPEED * Time.deltaTime);

        if (magnetDuration > 0) return;

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
#if UNITY_EDITOR
        if (GameManager.Instance.isDebugging)
            boostBlockFallCollider.gameObject.SetActive(false);
#endif
        SpeedAddValue -= Item_Boost.BOOST_SPEED_ADD_VALUE;
    }

    private void CheckDeath()
    {
        if (transform.position.y > -5) return;

        Die();
    }

    private void Move(float deltaTime)
    {
        float moveValue = Speed * deltaTime / TileManager.Instance.BeatInterval;
        transform.Translate(Vector3.forward * moveValue);
    }

    private void MoveLine(float moveX)
    {
        int moveXSign = (int)Mathf.Sign(moveX);
        int moveXLine = (int)Mathf.Sign(line);
        line += moveXSign;

        if (Mathf.Abs(line) >= 4)
            if (moveXLine == moveXSign)
                return;

        transform.DOKill(true);
        transform.DOMoveX(moveX, 0.2f).SetRelative();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider == null) return;

        if (collision.collider.CompareTag("Road"))
            jumpCount = MAX_JUMP_COUNT;

#if UNITY_EDITOR
        if (GameManager.Instance.isDebugging) return;
#endif

        if (collision.collider.CompareTag("Fall"))
            OnHitFallObject();

        if (collision.collider.CompareTag("Enemy"))
            OnHitEnemy();
    }

    private void OnHitFallObject()
    {
        if (BoostDuration > 0) return;

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
        if (!IsAlive) return;

        IsAlive = false;

        SoundManager.Instance.PlaySound("Dead", ESoundType.Sfx, 0.5f, 0.6f);

        Material material = dissolveMeshes[0].material;
        material.SetFloat("_DissolvePower", 0);
        material.DOFloat(1, "_DissolvePower", 3).SetDelay(1).OnStart(() =>
        {
            foreach (var skinnedMeshRenderer in defaultMeshes)
                skinnedMeshRenderer.gameObject.SetActive(false);

            foreach (var skinnedMeshRenderer in dissolveMeshes)
                skinnedMeshRenderer.gameObject.SetActive(true);
        });

        transform.DOKill();

        animator.CrossFade("Death", 0.2f);

        boostParticle.Stop();
        magnetParticle.Stop();

        var obj = PoolManager.Instance.Init("Hit Effect");
        obj.transform.position = transform.position;

        InGameManager.Instance.GameOver();
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
                HitEnemy(hitAbleList);
                effectData.GetEffect(EffectType.Spin).transform.localPosition = Vector3.up;
            }
        }

        jumpCount--;
        rigid.velocity = Vector3.up * jumpPower;
        animator.CrossFade("Jump", 0.1f, -1, 0);
    }

    private void HitEnemy(List<Enemy> hitAbleList)
    {
        var hitAbleOrderBy = hitAbleList.OrderBy(enemy => Mathf.Abs(enemy.transform.position.z - transform.position.z));
        hitAbleOrderBy.First().Hit();
    }

    public void CheckInput(Direction direction)
    {
        if (!IsAlive) return;

        switch (direction)
        {
            case Direction.Left:
                MoveLine(-TileManager.TILE_DISTANCE);
                if (hitAbleEnemyList.Count > 0)
                {
                    var hitAbleList = hitAbleEnemyList.FindAll(enemy => enemy.transform.position.x - transform.position.x < -TileManager.TILE_DISTANCE / 2);
                    if (hitAbleList.Count > 0)
                    {
                        HitEnemy(hitAbleList);

                        effectData.GetEffect(EffectType.Right).transform.localPosition = Vector3.up;
                        animator.CrossFade("Left Attack", 0.1f, -1, 0);
                        break;
                    }
                }

                if (jumpCount >= MAX_JUMP_COUNT)
                    animator.CrossFade("Move", 0.1f);

                break;
            case Direction.Right:
                MoveLine(TileManager.TILE_DISTANCE);
                if (hitAbleEnemyList.Count > 0)
                {
                    var hitAbleList = hitAbleEnemyList.FindAll(enemy => enemy.transform.position.x - transform.position.x > TileManager.TILE_DISTANCE / 2);
                    if (hitAbleList.Count > 0)
                    {
                        HitEnemy(hitAbleList);

                        effectData.GetEffect(EffectType.Left).transform.localPosition = Vector3.up;
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
            case Direction.Center:
            case Direction.Down:
                if (hitAbleEnemyList.Count > 0)
                {
                    var hitAbleList = hitAbleEnemyList.FindAll(enemy => Mathf.Abs(enemy.transform.position.x - transform.position.x) < TileManager.TILE_DISTANCE / 2);
                    if (hitAbleList.Count > 0)
                    {
                        HitEnemy(hitAbleList);

                        switch (attackIndex)
                        {
                            case 0:
                                effectData.GetEffect(EffectType.Left).transform.localPosition = Vector3.up;
                                animator.CrossFade("Left Attack", 0.1f, -1, 0);
                                break;
                            case 1:
                                effectData.GetEffect(EffectType.Right).transform.localPosition = Vector3.up;
                                animator.CrossFade("Right Attack", 0.1f, -1, 0);
                                break;
                            case 2:
                                effectData.GetEffect(EffectType.Spin).transform.localPosition = Vector3.up;
                                animator.CrossFade("Three Attack", 0.1f, -1, 0);
                                break;
                        }

                        attackIndex = (attackIndex + 1) % 3;
                    }
                }

                break;
        }
    }
}