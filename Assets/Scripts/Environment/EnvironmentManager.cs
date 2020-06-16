using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public List<BasicAgent> agents;

    #region Properties
    private List<BaseTarget> Targets { get; set; }
    private List<BaseSource> Sources { get; set; }
    private List<BaseStructure> Structures { get; set; }
    #endregion

    void Start()
    {
        Targets = GetComponentsInChildren<BaseTarget>().ToList();
        Sources = GetComponentsInChildren<BaseSource>().ToList();
        Structures = GetComponentsInChildren<BaseStructure>().ToList();
        
        foreach (var agent in agents)
        {
            agent.onTaskDone.AddListener(OnTaskDone);
        }
    }

    /// <summary>
    /// Returns structures that have yet to be completed.
    /// </summary>
    private IEnumerable<BaseStructure> GetPendingStructures()
    {
        return Structures.Where(s => !s.IsComplete);
    }

    private void OnTaskDone(BasicAgent sender)
    {
        // TODO 
    }
}
