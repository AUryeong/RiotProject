namespace InGame
{
    public class Item_Rune : Item
    {
        protected override void OnGet()
        {
            base.OnGet();
            InGameManager.Instance.Rune++;
            SoundManager.Instance.PlaySound("exp", ESoundType.Pitch2Sfx, 0.3f, 2);
        }
    }
}