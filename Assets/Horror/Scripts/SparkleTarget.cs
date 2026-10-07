using UnityEngine;
using GorillaRigging;

public class SparkleTarget : MonoBehaviour
{
    public class SparkleKillStage
    {
        public GameObject[] objects;

        public void SetObjectsActive(bool active)
        {
            for(int i = 0; i < objects.Length; i++)
            {
                objects[i].SetActive(active);
            }
        }
    }
    public RigController RigC;
    public SparkleKillStage[] SKS;

    public void FixedUpdate()
    {
        if(RigC.sparkleDeathProgress > 0)
        {
            for(int i = 0; i < SKS.Length; i++)
            {
                if(i == 0){i++;}
                if(SKS.Length != 0)
                SKS[i].SetObjectsActive((SKS.Length/i) < RigC.sparkleDeathProgress);
            }
        }
    }
}
