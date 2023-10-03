using UnityEngine;

namespace Lobby
{
    public abstract class LobbyUIActiveLink : MonoBehaviour, IActiveLink
    {
        protected virtual void Awake()
        {
        }
        
        public virtual void Active()
        {
            gameObject.SetActive(true);
        }

        public virtual void DeActive()
        {
        }
    }
}