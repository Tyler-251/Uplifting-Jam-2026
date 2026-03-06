using UnityEngine;

public class SpinBehavior : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(Vector3.up, 5f * Time.deltaTime);
    }
}
