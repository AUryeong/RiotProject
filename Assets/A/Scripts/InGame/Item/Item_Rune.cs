namespace InGame
{
    public class Item_Rune : Item
    {
        private const float RUNE_HP_HEAL_VALUE = 2;
        
        protected override void OnGet()
        {
            base.OnGet();
            Player.Instance.Hp += RUNE_HP_HEAL_VALUE;
            InGameManager.Instance.Rune++;
            SoundManager.Instance.PlaySound("exp", ESoundType.Coin, 0.5f, 2f);
        }
    }
}