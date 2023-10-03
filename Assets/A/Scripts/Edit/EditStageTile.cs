using System.Collections.Generic;
using UnityEngine;

namespace Edit
{
    [CreateAssetMenu(fileName = "Edit Stage Tile Data", menuName = "A/Edit Stage Tile Data", order = 0)]
    public class EditStageTile : ScriptableObject
    {
        public StageTileData tileData;
        public List<EditTile> tiles;
    }
}