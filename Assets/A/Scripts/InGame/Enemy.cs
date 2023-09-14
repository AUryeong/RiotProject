using DG.Tweening;
using UnityEngine;

namespace InGame
{
    public class Enemy : MonoBehaviour
    {
        private const float ENEMY_HP_HEAL_VALUE = 10;
        private const int ENEMY_RUNE_GET_VALUE = 5;
        private const float ENEMY_MOVE_DURATION = 0.5f;

        public int hp;

        private Animator animator;
        private Vector3 defaultLocalPos;

        [SerializeField] private Vector3 spawnVector;
        [SerializeField] private Transform bone;
        [SerializeField] private GameObject hitAbleEffect;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            defaultLocalPos = transform.localPosition;
        }

        private void OnEnable()
        {
            transform.DOKill();
            animator.Play("Idle");
            hitAbleEffect.gameObject.SetActive(false);
            gameObject.layer = LayerMask.NameToLayer("Enemy");

            transform.localPosition = defaultLocalPos;
            bone.DOLocalMove(spawnVector, ENEMY_MOVE_DURATION).From(true);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider == null) return;

            if (collision.collider.CompareTag("Player"))
                OnHitPlayer();
        }

        private void OnHitPlayer()
        {
            if (Player.Instance.BoostDuration <= 0)
                animator.CrossFade("Attack", 0.1f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other == null) return;
            if (other.CompareTag("PlayerCheck"))
            {
                hitAbleEffect.gameObject.SetActive(true);
                Player.Instance.hitAbleEnemyList.Add(this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other == null) return;
            if (other.CompareTag("PlayerCheck"))
            {
                hitAbleEffect.gameObject.SetActive(false);
                Player.Instance.hitAbleEnemyList.Remove(this);
            }
        }

        public void PosChangeBySpeed(float speedMultiplier)
        {
            transform.DOKill(true);
            
            var enemyPos = transform.position;

            float playerPos = Player.Instance.transform.position.z;
            float distance = enemyPos.z - playerPos;
            float zPos = distance * speedMultiplier + playerPos;

            float moveDuration = ENEMY_MOVE_DURATION * Random.Range(0.75f, 1.25f);
            
            transform.DOMoveX(TileManager.Instance.GetLengthToRoadData(zPos).summonLine * TileManager.TILE_DISTANCE, moveDuration);
            transform.DOMoveY(enemyPos.y + 2, moveDuration / 2).SetLoops(2, LoopType.Yoyo);
            transform.DOMoveZ(zPos, moveDuration);
        }

        public void Hit(int damage)
        {
            hp -= damage;
            if (hp <= 0)
                Die();
        }

        private void Die()
        {
            gameObject.layer = LayerMask.NameToLayer("DeathEnemy");
            SoundManager.Instance.PlaySound("clap", ESoundType.Sfx, 0.5f);

            Player.Instance.hitAbleEnemyList.Remove(this);
            Player.Instance.Hp += ENEMY_HP_HEAL_VALUE;
            InGameManager.Instance.Rune += ENEMY_RUNE_GET_VALUE;
            gameObject.SetActive(false);

            var obj = PoolManager.Instance.Init("Hit Effect");
            obj.transform.position = transform.position;
        }
    }
}