namespace InGame
{
    public class Item_Rune : Item
    {
        public const int PERFECT_RUNE_COUNT = 9;
        public const int GREAT_RUNE_COUNT = 7;
        public const int GOOD_RUNE_COUNT = 5;
        protected override void OnGet()
        {
            base.OnGet();
            InGameManager.Instance.Rune++;
            SoundManager.Instance.PlaySound("exp", ESoundType.Pitch2Sfx, 0.3f, 2);
        }
    }
}