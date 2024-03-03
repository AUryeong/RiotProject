using DG.Tweening;
using UnityEngine;

namespace InGame
{
    public class Enemy : MonoBehaviour
    {

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
            bone.DOLocalMove(spawnVector, TileManager.Instance.BeatInterval * 2).From(true);
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
            gameObject.SetActive(false);

            Player.Instance.hitAbleEnemyList.Remove(this);

            float distance = transform.position.z - Player.Instance.transform.position.z;
            if (distance <= 4.5f)
            {
                InGameManager.Instance.Rune += Item_Rune.PERFECT_RUNE_COUNT;
                InGameManager.Instance.AddBeatHit(BeatHitType.Perfect);
                PoolManager.Instance.Init("Perfect Effect").transform.position = transform.position;
            }
            else if (distance <= 6.5f)
            {
                InGameManager.Instance.Rune += Item_Rune.GREAT_RUNE_COUNT;
                InGameManager.Instance.AddBeatHit(BeatHitType.Great);
                PoolManager.Instance.Init("Great Effect").transform.position = transform.position;
            }
            else
            {
                InGameManager.Instance.Rune += Item_Rune.GOOD_RUNE_COUNT;
                InGameManager.Instance.AddBeatHit(BeatHitType.Good);
                PoolManager.Instance.Init("Good Effect").transform.position = transform.position;
            }

            PoolManager.Instance.Init("Hit Effect").transform.position = transform.position;
        }
    }
}