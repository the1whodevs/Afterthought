using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public new string name;

    [SerializeField] protected float floatSpeed = 2.5f;
    [SerializeField] protected float floatDist = 1.0f;
    [SerializeField] protected float rotSpeed = 10.0f;

    private float startY;
    private float toY;

    private const float TOLERANCE = 0.0001f;

    private void Start()
    {
        startY = transform.position.y;

        toY = startY + floatDist;
    }

    private void Update()
    {
        FloatUpDown();
        Rotate();
    }

    private void Rotate()
    {
        transform.Rotate(Vector3.up * rotSpeed * Time.deltaTime);
    }

    private void FloatUpDown()
    {
        var pos = transform.position;

        pos.y = Mathf.Lerp(pos.y, toY, Time.deltaTime * floatSpeed);

        if (Mathf.Abs(pos.y - toY) <= TOLERANCE) 
        {
            if (Mathf.Approximately(toY, startY))
                toY = startY + floatDist;
            else
                toY = startY;
        }

        transform.position = pos;
    }

    public virtual void Interact()
    {

    }
}
