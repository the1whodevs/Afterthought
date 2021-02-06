using System.Collections;
using UnityEngine;

public class CameraAnimation : MonoBehaviour
{
    public static CameraAnimation Instance;

    private void Awake()
    {
        if (Instance) Destroy(this);
        Instance = this;
    }

    public void DeathAnimation()
    {
        transform.SetParent(null);

        StartCoroutine(DeathAnim());
    }

    private IEnumerator DeathAnim()
    {
        // frame 0 to frame 40:
        // pos.y moves by: -0.479
        var posYmove = -0.479f;
        var frames = 40;
        
        var step = 1.0f / 40.0f;
        
        var startPos = transform.localPosition;
        
        var t = 0.0f;
        
        while (frames > 0)
        {
            frames--;
        
            t += step;
        
            var pos = transform.localPosition;
            pos.y = Mathf.Lerp(startPos.y, startPos.y + posYmove, t);
            transform.localPosition = pos;
            yield return new WaitForEndOfFrame();
        }
        
        frames = 30;
        step = 1.0f / 30.0f;
        t = 0.0f;
        posYmove = -0.297f;
        
        var posXmove = 0.695f;
        var posZmove = -0.154f;
        var rotXmove = 4.135f;
        var rotYmove = -0.65f;
        var rotZmove = -74.138f;

        startPos = transform.localPosition;

        var startRot = transform.localRotation;
        
        // frame 40 to frame 70:
        // pos.x moves by: 0.695
        // pos.y moves by: -0.297
        // pos.z moves by: -0.154
        // rot.x moves by: 4.135
        // rot.y moves by: -0.65
        // rot.z moves by: -74.138
        while (frames > 0)
        {
            frames--;
        
            t += step;
        
            var pos = transform.localPosition;
            
            pos.y = Mathf.Lerp(startPos.y, startPos.y + posYmove, t);
            pos.x = Mathf.Lerp(startPos.x, startPos.x + posXmove, t);
            pos.z = Mathf.Lerp(startPos.z, startPos.z + posZmove, t);
            transform.localPosition = pos;

            var euler = startRot.eulerAngles;

            euler.x = Mathf.Lerp(startRot.eulerAngles.x, startRot.eulerAngles.x + rotXmove, t);
            euler.y = Mathf.Lerp(startRot.eulerAngles.y, startRot.eulerAngles.y + rotYmove, t);
            euler.z = Mathf.Lerp(startRot.eulerAngles.z, startRot.eulerAngles.z + rotZmove, t);
            transform.localRotation = Quaternion.Euler(euler);

            yield return new WaitForEndOfFrame();
        }

        frames = 20;
        t = 0.0f;
        rotXmove = -4.135f;
        rotYmove = 0.65f;
        rotZmove = -15.0f;
        step = 1.0f / 20.0f;

        startRot = transform.localRotation;

        // frame 70 to frame 90:
        // rot.x moves by: -4.135
        // rot.y moves by: 0.65
        // rot.z moves by: -15
        while (frames > 0)
        {
            frames--;
        
            t += step;
    
            var euler = startRot.eulerAngles;

            euler.x = Mathf.Lerp(startRot.eulerAngles.x, startRot.eulerAngles.x + rotXmove, t);
            euler.y = Mathf.Lerp(startRot.eulerAngles.y, startRot.eulerAngles.y + rotYmove, t);
            euler.z = Mathf.Lerp(startRot.eulerAngles.z, startRot.eulerAngles.z + rotZmove, t);
            transform.localRotation = Quaternion.Euler(euler);

            yield return new WaitForEndOfFrame();
        }
        
        Destroy(Player.Active.gameObject);
        yield return null;
    }
}
