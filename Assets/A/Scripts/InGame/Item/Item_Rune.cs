namespace InGame
{
    public class Item_Rune : Item
    {
        protected override void OnGet()
        {
            base.OnGet();
            InGameManager.Instance.Rune++;
            SoundManager.Instance.PlaySound("exp", ESoundType.Sfx, 0.5f);
        }
    }
}