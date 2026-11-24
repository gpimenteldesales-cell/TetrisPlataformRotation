using UnityEngine;

public class TitleAnim : MonoBehaviour
{
    public int directionY= 1;
    public int directionX= 1;
    public int speedX = 3;
    public int speedY = 10;
    public int maxRotation = 10;
    public Transform initialPoint;
    public Transform finalPoint;
    void Start()
    {
        transform.position = new Vector3(transform.position.x, initialPoint.position.y, transform.position.z);
    }
    void Update()
    {
        transform.Rotate(Vector3.forward * directionX * speedX * Time.deltaTime);
        transform.position += Vector3.down * directionY * speedY * Time.deltaTime;
        
        if(directionY == 1 && transform.position.y < finalPoint.position.y)
        {
            directionY = -1;
        }
        else if(directionY == -1 && transform.position.y > initialPoint.position.y)
        {
            directionY = 1;
        }
        if (transform.rotation.eulerAngles.z > maxRotation && transform.rotation.eulerAngles.z < 180)
        {
            directionX = -1;
            }
        else if (transform.rotation.eulerAngles.z < 360 - maxRotation && transform.rotation.eulerAngles.z > 180)
        {
            directionX = 1;
            }

    }
}
