namespace InGame
{
    public class Item_Magnet : Item
    {
        private const float MAGNET_DURATION = 5;
        public const float MAGNET_RADIUS = 12;
        public const float MAGNET_SPEED = 45;
        protected override void OnGet()
        {
            base.OnGet();
            Player.Instance.Magnet(MAGNET_DURATION);
        }
    }
}