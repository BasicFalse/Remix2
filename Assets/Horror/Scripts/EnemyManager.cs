using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static volatile EnemyManager inst;
    public List<MonsterBase> Enemies;
    public bool lightsOn;
    void Awake()
    {
        
        if (inst != null)
        {
            if (inst != this) { Destroy(this); }
        }
        else { inst = this; }
    }

    public void FixedUpdate()
    {
        lightsOn = Enemies.Count < 50;
        foreach(MonsterBase ec in Enemies)
        {
            if(ec == null){Enemies.Remove(ec); continue;}

            ec.UpdateMonster();
            ec.ToggleLight(lightsOn);

        }
    }
    
    public void AddMonster(MonsterBase EC)
    {
        if (!Enemies.Contains(EC)) { Enemies.Add(EC); }
    }
}
