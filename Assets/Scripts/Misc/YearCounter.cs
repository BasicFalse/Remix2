using System;
using Unity.Mathematics;
using UnityEngine;

public class YearCounter : MonoBehaviour
{
    public GameObject[] cornStalks;
    public AudioClip ac;
    public int3 up = 0;
    public bool test;

    void FixedUpdate()
    {
        up.x++;
        if(up.x > 0)
        {
            up.x = -5000;
            InfrequentUpdate();
        }
        
        if(up.z == 0)
        {
            up.z = up.y;
        }

        if(up.z != up.y || test)
        {
            AudioSource a = gameObject.GetComponent<AudioSource>();
            up.z = up.y;
            test = false;
            a.PlayOneShot(ac);
        }
    }
    void InfrequentUpdate()
    {
        DateTime Tart = new(2009,8,17);
        int i = HalfYearsSince(Tart);
        up.y = i;
        for (int j = 0; j < cornStalks.Length; j++)
        {
            cornStalks[j].SetActive(i>j);
        }
    }

    int WholeYearsSince(DateTime startDate)
    {
        DateTime now = DateTime.Now;
        int years = now.Year - startDate.Year;
    
        if (now.Month < startDate.Month ||
           (now.Month == startDate.Month && now.Day < startDate.Day))
        {
            years--;
        }
    
        return years;
    }

    int HalfYearsSince(DateTime startDate)
    {
        DateTime now = DateTime.Now;
        int years = now.Year - startDate.Year;
        int months = now.Month - startDate.Month;
        int totalMonths = years * 12 + months;


        if (now.Day < startDate.Day)
            totalMonths--;

        return totalMonths / 6;
    }
}
