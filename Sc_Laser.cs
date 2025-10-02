using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Sc_Laser : MonoBehaviour
{
    public Animator heroAnimator; // assign hero's animator
    public Transform muzzle; // titik asal laser (biasanya child dari pistol)
    public float maxDistance = 50f;

    private LineRenderer lr;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.enabled = false;
    }

    void Update()
    {
        if (heroAnimator == null || muzzle == null) return;

        bool aiming = heroAnimator.GetBool("stat_aim");
        lr.enabled = aiming;

        if (!aiming) return;

        Vector3 origin = muzzle.position;
        Vector3 dir = muzzle.forward;
        RaycastHit hit;
        if (Physics.Raycast(origin, dir, out hit, maxDistance))
        {
            lr.SetPosition(0, origin);
            lr.SetPosition(1, hit.point);
            // optional: instantiate impact effect at hit.point
        }
        else
        {
            lr.SetPosition(0, origin);
            lr.SetPosition(1, origin + dir * maxDistance);
        }
    }
}
