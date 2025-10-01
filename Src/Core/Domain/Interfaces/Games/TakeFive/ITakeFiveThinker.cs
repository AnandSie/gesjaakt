using Domain.Entities.Game.TakeFive;
using Domain.Interfaces.Components;
using Domain.Interfaces.Games.BaseGame;
using System.Collections.Immutable;

namespace Domain.Interfaces.Games.TakeFive;

public interface ITakeFiveThinker :
    // REFACTOR - DRY - Make the two IDecide interfaces one and reuse this in the player and thinker => somethinglike ITakeFiveDecisions
    
    
    IDecide<ITakeFiveReadOnlyGameState, TakeFiveCard>,
    // REFACTOR - change to IMMUTABLE/READONLY 
    IDecide<IEnumerable<IEnumerable<TakeFiveCard>>, int>,
    IStatefull<IImmutableList<TakeFiveCard>>
{ }
