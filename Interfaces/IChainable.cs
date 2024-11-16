using System;
using System.Collections.Generic;

public interface IChainable
{
    int CurrentChain { get; }
    List<AbilityChainStat> AbilityChainStats { get; }

    void AbilityNextChain();
}


[Serializable]
public struct AbilityChainStat
{
    public float Amount;
    public float Range;
}
