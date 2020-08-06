using System.Collections.Generic;

public interface IHasGoal
{
    BaseStructure Goal { get; }

    /// <summary>
    /// Makes the agent update their goal based on the provided structures.
    /// </summary>
    /// <param name="baseStructures">Structures</param>
    void UpdateGoal(IEnumerable<BaseStructure> baseStructures);
}
