using UnityEngine;

public class CanvasController : MonoBehaviour
{
    void OnEnable() => UICameraFollower.uiList.Add(transform);
    void OnDisable() => UICameraFollower.uiList.Remove(transform);
}
