using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Settings")]
    public float turnSpeed = 4.0f;       // kecepatan putar kamera pakai mouse
    public float distance = 3.2f;        // jarak kamera dari char_point_cam
    public Vector3 offset = new Vector3(0, 1.5f, -3.2f); // offset default

    private Transform target;            // reference ke char_point_cam
    private Vector3 currentOffset;

    void Start()
    {
        // cari player berdasarkan tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player dengan tag 'Player' tidak ditemukan!");
            return;
        }

        // cari child bernama char_point_cam di semua hierarchy player
        Transform[] allChildren = player.GetComponentsInChildren<Transform>();
        foreach (Transform t in allChildren)
        {
            if (t.name == "char_point_cam")
            {
                target = t;
                break;
            }
        }

        if (target == null)
        {
            Debug.LogError("Child 'char_point_cam' tidak ditemukan pada Player!");
            return;
        }

        // simpan offset awal
        currentOffset = offset;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // rotasi offset berdasarkan mouse input
        float mouseX = Input.GetAxis("Mouse X");
        currentOffset = Quaternion.AngleAxis(mouseX * turnSpeed, Vector3.up) * currentOffset;

        // posisi kamera = posisi target + offset
        transform.position = target.position + currentOffset;

        // kamera selalu melihat target
        transform.LookAt(target.position);
    }
}
