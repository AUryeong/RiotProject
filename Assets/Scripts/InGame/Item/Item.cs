using DG.Tweening;
using UnityEngine;

namespace InGame
{
    public class Item : MonoBehaviour
    {
        protected Vector3 defaultScale;
        protected virtual void Awake()
        {
            defaultScale = transform.localScale;
        }

        protected virtual void OnEnable()
        {
            transform.DOKill();
            transform.localScale = Vector3.zero;
            transform.DOScale(defaultScale, 0.5f).SetEase(Ease.OutBack);
        }
        protected virtual void OnGet()
        {
            gameObject.SetActive(false);
        }
    
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                OnGet();
        }
    }
}