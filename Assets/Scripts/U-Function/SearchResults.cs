using System.Collections.Generic;
using UnityEngine;
using TextAsset = UnityEngine.TextAsset;

namespace UFunction
{
    [CreateAssetMenu(fileName = "SearchResults", menuName = "U-Function/SearchResults")]
    public class SearchResults : ScriptableObject
    {
        [System.Serializable]
        public class SearchFull
        {
            public List <string> Querys;
            public TextAsset Result;
        }

        public SearchFull[] Results;
    }
}

