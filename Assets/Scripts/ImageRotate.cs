using UnityEngine;

public class ImageRotate : MonoBehaviour
{
    public float rotationSpeed = 180f; // degrees per second

    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}
