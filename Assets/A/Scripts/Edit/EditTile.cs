using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Edit
{
    [System.Serializable]
    public class EditTile
    {
        public float length;
        public List<GameObject> objects;
        public bool isBlank;
    }
}