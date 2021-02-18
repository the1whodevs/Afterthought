using UnityEngine;

public class LoadoutEditorInteractable : InteractableObject
{
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
        Player.Active.Controller.GetInUI();
        Time.timeScale = 0.0f;
        LoadoutEditor.Active.ShowWindow();
    }
}
