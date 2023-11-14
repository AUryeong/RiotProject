using DG.Tweening;
using UnityEngine;

namespace InGame
{
    public class Enemy : MonoBehaviour
    {
        private const float ENEMY_HP_HEAL_VALUE = 10;
        private const float ENEMY_MOVE_DURATION = 2;

        private Animator animator;
        private Vector3 defaultLocalPos;
        private Quaternion defaultBoneQuaternion;

        [SerializeField] private Vector3 spawnVector;
        [SerializeField] private Transform bone;
        [SerializeField] private GameObject hitAbleEffect;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            defaultLocalPos = transform.localPosition;
            defaultBoneQuaternion = bone.rotation;
        }

        private void OnEnable()
        {
            transform.DOKill();
            animator.Play("Idle");
            hitAbleEffect.gameObject.SetActive(false);
            gameObject.layer = LayerMask.NameToLayer("Enemy");

            transform.localPosition = defaultLocalPos;

            bone.localPosition = Vector3.zero;
            bone.rotation = defaultBoneQuaternion;
            bone.DOLocalMove(spawnVector, TileManager.Instance.beatInterval * 2).From(true);
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

        public void Hit()
        {
            gameObject.layer = LayerMask.NameToLayer("DeathEnemy");
            SoundManager.Instance.PlaySound("clap", ESoundType.Sfx, 0.5f);

            // bone.DOMoveX(Random.Range(0,2) == 0 ? Random.Range(-15f,-20f) : Random.Range(15f,20f), ENEMY_MOVE_DURATION).SetRelative(true);
            // bone.DOMoveY(Random.Range(5f, 10f), ENEMY_MOVE_DURATION).SetEase(Ease.OutBack).SetRelative(true);
            // bone.DOMoveZ(Random.Range(30, 50f), ENEMY_MOVE_DURATION).SetRelative(true).OnComplete(() => gameObject.SetActive(false));
            // bone.DORotateQuaternion(Quaternion.Euler(Random.Range(-360f, 360f), Random.Range(-360f, 360f), Random.Range(-360f, 360f)), ENEMY_MOVE_DURATION);
            gameObject.SetActive(false);

            Player.Instance.hitAbleEnemyList.Remove(this);
            Player.Instance.Hp += ENEMY_HP_HEAL_VALUE;

            float distance = transform.position.z - Player.Instance.transform.position.z;
            if (distance <= 4f)
            {
                InGameManager.Instance.Rune += 9;
                PoolManager.Instance.Init("Perfect Effect").transform.position = transform.position;
            }
            else if (distance <= 5)
            {
                InGameManager.Instance.Rune += 7;
                PoolManager.Instance.Init("Great Effect").transform.position = transform.position;
            }
            else
            {
                InGameManager.Instance.Rune += 5;
                PoolManager.Instance.Init("Good Effect").transform.position = transform.position;
            }

            PoolManager.Instance.Init("Hit Effect").transform.position = transform.position;
        }
    }
}