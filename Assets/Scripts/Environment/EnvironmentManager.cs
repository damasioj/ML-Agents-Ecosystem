using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public List<BasicAgent> agents;

    #region Properties
    private List<BaseSource> Sources { get; set; }
    private List<BaseStructure> Structures { get; set; }
    #endregion

    void Start()
    {
        Sources = GetComponentsInChildren<BaseSource>().ToList();
        Structures = GetComponentsInChildren<BaseStructure>().ToList();
    }

    void Update()
    {
        
    }

    /// <summary>
    /// Returns all non-empty sources.
    /// </summary>
    private IEnumerable<BaseSource> GetValidSources()
    {
        return Sources.Where(s => s.ResourceCount > 0);
    }

    /// <summary>
    /// Returns structures that have yet to be completed.
    /// </summary>
    private IEnumerable<BaseStructure> GetPendingStructures()
    {
        return Structures.Where(s => !s.IsComplete);
    }
}
