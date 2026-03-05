using UnityEngine;

public class SpinBehavior : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(Vector3.up, 20f * Time.deltaTime);
    }
}
