namespace InGame
{
    public class Item_Rune : Item
    {
        protected override void OnGet()
        {
            base.OnGet();
            InGameManager.Instance.Rune++;
            SoundManager.Instance.PlaySound("exp", ESoundType.Coin, 0.5f, 2f);
        }
    }
}