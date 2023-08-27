using UnityEngine;

public class Item_Boost : Item
{
    private const float BOOST_DURATION = 5;
    public const float BOOST_SPEED_ADD_VALUE = 12;
    protected override void OnGet()
    {
        base.OnGet();
        Player.Instance.Boost(BOOST_DURATION);
    }
}