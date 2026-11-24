using UnityEngine;

public class PlataformaFudida : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(0, 0.5f, 0  * Time.deltaTime);
    }
}
