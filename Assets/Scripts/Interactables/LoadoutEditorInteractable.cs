using UnityEngine;

public class LoadoutEditorInteractable : InteractableObject
{
    [SerializeField] private bool IsTraining = false;

    public override string GetActionPronoun()
    {
        return "the";
    }

    public override string GetActionVerb()
    {
        return "open";
    }

    public override void Interact()
    {
        Player.Active.Controller.EnterUI();

        if (!IsTraining) Time.timeScale = 0.0f;

        LoadoutEditor.Active.ShowWindow();
    }
}
