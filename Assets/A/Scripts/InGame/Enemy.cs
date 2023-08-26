using DG.Tweening;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private const float ENEMY_HP_HEAL_VALUE = 10;
    
    public int hp;
    
    private Animator animator;
    private Vector3 defaultLocalPos;
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
        bone.DOLocalMove(new Vector3(0, 6, 2), 0.5f).From(true);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider == null) return;
        
        if (collision.collider.CompareTag("Player"))
            animator.CrossFade("Attack", 0.1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;
        if (other.CompareTag("Player"))
        {
            hitAbleEffect.gameObject.SetActive(true);
            Player.Instance.hitAbleEnemyList.Add(this);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other == null) return;
        if (other.CompareTag("Player"))
        {
            hitAbleEffect.gameObject.SetActive(false);
            Player.Instance.hitAbleEnemyList.Remove(this);
        }
    }

    public void Hit(int damage)
    {
        hp -= damage;
        if(hp <= 0)
            Die();
    }
    
    private void Die()
    {
        gameObject.layer = LayerMask.NameToLayer("DeathEnemy");
        SoundManager.Instance.PlaySound("clap", ESoundType.Sfx);
        
        Player.Instance.hitAbleEnemyList.Remove(this);
        Player.Instance.Hp += ENEMY_HP_HEAL_VALUE;
        gameObject.SetActive(false);
        
        var obj = PoolManager.Instance.Init("Hit Effect");
        obj.transform.position = transform.position;
    }
}
