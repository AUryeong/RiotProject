using UnityEngine;

namespace InGame
{
    public class Item_Magnet : Item
    {
        private const float MAGNET_DURATION = 5;
        public const float MAGNET_RADIUS = 12;
        public const float MAGNET_SPEED = 45;

        [SerializeField] private MeshRenderer magnetMeshRenderer;

        protected override void OnGet()
        {
            base.OnGet();
            Player.Instance.Magnet(MAGNET_DURATION);
        }

        private void Update()
        {
            magnetMeshRenderer.transform.Rotate(new Vector3(0,90*Time.deltaTime,0), Space.World);
        }
    }
}