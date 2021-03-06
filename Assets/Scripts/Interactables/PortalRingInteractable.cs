using System.Collections;
using UnityEngine;

public class PortalRingInteractable : InteractableObject
{
    [SerializeField, Min(0)] private int sceneBuildIndexDestination = 0;

    private IEnumerator Start()
    {
        while (!PortalMenu.Active) 
        {
            Debug.Log("Waiting for Portal Menu Reference");

            yield return new WaitForEndOfFrame();
        } 

        PortalMenu.Active.onPortalAccept += () => { LoadingManager.Active.LoadLevel(sceneBuildIndexDestination); };
    }

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
        UIManager.Active.ShowPortalMenu();
    }
}
