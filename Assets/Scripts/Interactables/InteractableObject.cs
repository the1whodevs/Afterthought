using UnityEngine;

public class InteractableObject : MonoBehaviour, ILootable
{
    public new string name;

    [SerializeField] protected bool doFloat = true;
    [SerializeField] protected bool doRotate = true;

    [SerializeField] protected float floatSpeed = 2.5f;
    [SerializeField] protected float floatDist = 1.0f;
    [SerializeField, Tooltip("Rotations per second")] protected float rotSpeed = 10.0f;

    private float startPosY;
    private float endPosY;
    private float currentFloatSpeed;

    private float floatT = 0.0f;

    public bool IsLooted { get; protected set; }

    private void Start()
    {
        currentFloatSpeed = floatSpeed;
        startPosY = transform.position.y;
        endPosY = startPosY + floatDist;
    }

    private void Update()
    {
        if (doFloat) FloatUpDown();
        if (doRotate) Rotate();
    }

    private void Rotate()
    {
        transform.Rotate(Vector3.up * 360.0f * rotSpeed * Time.deltaTime);
    }

    private void FloatUpDown()
    {
        var pos = transform.position;
        floatT += Time.deltaTime * currentFloatSpeed;
        pos.y = Mathf.Lerp(startPosY, endPosY, floatT);

        if (floatT >= 1.0f)
        {
            floatT = 1.0f;
            currentFloatSpeed = -currentFloatSpeed;
        }
        else if (floatT <= 0.0f)
        {
            floatT = 0.0f;
            currentFloatSpeed = -currentFloatSpeed;
        }

        transform.position = pos;
    }

    public virtual void Interact()
    {
        
    }

    public virtual string GetActionVerb()
    {
        return "";
    }

    public virtual string GetActionPronoun()
    {
        return "";
    }

    public virtual void Loot()
    {
        IsLooted = true;

        gameObject.SetActive(false);
    }

    public virtual void SetLootStatus(bool status)
    {
        IsLooted = status;
    }
}
