using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : Singleton<ShopManager>
{
    public List<StageTileData> stageTileDatas = new List<StageTileData>();
    public List<PlayerEffectData> playerEffectDatas = new List<PlayerEffectData>();

    protected override void OnCreated()
    {
        base.OnCreated();
        
    }
}
