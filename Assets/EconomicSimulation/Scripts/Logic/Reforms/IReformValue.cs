﻿using Nashet.ValueSpace;

namespace Nashet.EconomicSimulation.Reforms
{
    public interface IReformValue
    {        
        bool IsAllowed(object firstObject, object secondObject, out string description);
        bool IsAllowed(object firstObject, object secondObject);
        float getVotingPower(PopUnit forWhom);
        bool IsMoreConservative(AbstractReformValue another);
        int GetRelativeConservatism(AbstractReformValue two);
        Procent howIsItGoodForPop(PopUnit pop);
        Procent LifeQualityImpact { get; }
    }
}