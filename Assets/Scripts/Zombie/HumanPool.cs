using System.Collections.Generic;
using UnityEngine;

public class HumanPool : Singletone<HumanPool>
{
    private List<HumanBehaviour> _humans;
    private int _dangerHumansCount;

    public override void Awake()
    {
        base.Awake();

        _humans = new List<HumanBehaviour>();
    }

    public List<HumanBehaviour> GetDangerHumans()
    {
        List<HumanBehaviour> dangerHumans = new List<HumanBehaviour>();

        foreach (var item in _humans)
        {
            if (item.isDanger())
            {
                dangerHumans.Add(item);
            }
        }

        return dangerHumans;
    }

    public void AddHuman(HumanBehaviour human)
    {
        _humans.Add(human);
    }

    public void RemoveHuman(HumanBehaviour human)
    {
        _humans.Remove(human);
    }
}
