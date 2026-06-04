using UnityEngine; 

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    [Header("카메라 이동 속도")]
    public float speed = 5f;

    private Vector3 targetPosition;

    void Awake()
    {
        instance = this;
        targetPosition = transform.position;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
    }

    public void ChangeRoom(Vector3 newRoomPosition)
    {
        targetPosition = new Vector3(newRoomPosition.x, newRoomPosition.y, transform.position.z);
    }
}
