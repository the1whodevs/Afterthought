using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalRingInteractable : InteractableObject
{
    [SerializeField, Min(0)] private int sceneBuildIndexDestination = 0;

    public override string GetActionPronoun()
    {
        return "the";
    }

    public override string GetActionVerb()
    {
        return "travel to";
    }

    public override void Interact()
    {
        SceneManager.LoadScene(sceneBuildIndexDestination);
    }
}
