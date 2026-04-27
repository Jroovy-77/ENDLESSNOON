using UnityEngine;

public class TreeShadeBlocker : MonoBehaviour
{
    public bool isBlockingSun = true;

    public void OnHitBySun()
    {
        isBlockingSun = false;
        CancelInvoke(nameof(ResetBlock));
        Invoke(nameof(ResetBlock), 0.2f);
    }

    void ResetBlock()
    {
        isBlockingSun = true;
    }
}