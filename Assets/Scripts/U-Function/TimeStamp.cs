using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using TextAsset = UnityEngine.TextAsset;

namespace UFunction
{
    [CreateAssetMenu(fileName = "TimeStamp", menuName = "U-Function/TimeStamp")]
    public class TimeStamp : ScriptableObject
    {
        public int year;
        public int month;
        public int day;
        public int hour;
        public int minute;
        public int second;
    }
}

