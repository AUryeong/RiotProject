using UnityEngine;

[CreateAssetMenu(fileName = "Player Effect Data", menuName = "A/Player Effect Data", order = 0)]
public class PlayerEffectData : ScriptableObject
{
    [Space(10f)] public GameObject leftEffect;
    [Space(10f)] public GameObject rightEffect;
    [Space(10f)] public GameObject spinEffect;

    [HideInInspector] public string leftEffectName;
    [HideInInspector] public string rightEffectName;
    [HideInInspector] public string spinEffectName;

    public void Init()
    {
        leftEffectName = name + "_Effect_Left";
        rightEffectName = name + "_Effect_Right";
        spinEffectName = name + "_Effect_Spin";

        PoolManager.Instance.JoinPoolingData(leftEffectName, leftEffect);
        PoolManager.Instance.JoinPoolingData(rightEffectName, rightEffect);
        PoolManager.Instance.JoinPoolingData(spinEffectName, spinEffect);
    }

    public GameObject GetEffect(EffectType type)
    {
        switch (type)
        {
            case EffectType.Left:
                return PoolManager.Instance.Init(leftEffectName, Player.Instance.transform);
            case EffectType.Right:
                return PoolManager.Instance.Init(rightEffectName, Player.Instance.transform);
            default:
            case EffectType.Spin:
                return PoolManager.Instance.Init(spinEffectName, Player.Instance.transform);
        }
    }
}

public enum EffectType
{
    Left,
    Right,
    Spin
}
