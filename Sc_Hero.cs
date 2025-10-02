using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Sc_Hero : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 3f;
    public float rotationSpeed = 10f;
    public float gravity = -9.81f;

    [Header("References")]
    public Animator animator;
    public Transform charPointCam;

    [Header("Look At Target")]
    public float lookAtRange = 6f;
    public string itemTag = "item";
    public bool holdToAim = true; // true = tahan RMB, false = toggle RMB

    private CharacterController controller;
    private Vector3 velocity;
    private GameObject currentTarget;
    private bool isAiming = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleMovement();
        HandleAimingInput();

        // cari target item tiap 0.2 detik untuk hemat performa
        if (Time.frameCount % 10 == 0)
            currentTarget = FindClosestTargetWithTag(itemTag, lookAtRange);

        // contoh pickup item (tekan E)
        if (currentTarget != null && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Picked up item: " + currentTarget.name);
            Destroy(currentTarget);
            currentTarget = null;
        }
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 input = new Vector3(h, 0f, v);

        if (input.magnitude > 0.1f)
        {
            Transform cam = Camera.main ? Camera.main.transform : null;
            Vector3 moveDir;
            if (cam)
            {
                Vector3 camForward = cam.forward; camForward.y = 0f; camForward.Normalize();
                Vector3 camRight = cam.right; camRight.y = 0f; camRight.Normalize();
                moveDir = camForward * v + camRight * h;
            }
            else
            {
                moveDir = transform.forward * v + transform.right * h;
            }

            // rotate hero ke arah gerak
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);

            controller.Move(moveDir.normalized * speed * Time.deltaTime);
            animator.SetBool("stat_jalan", true);
        }
        else
        {
            animator.SetBool("stat_jalan", false);
        }

        // gravity
        if (controller.isGrounded && velocity.y < 0) velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleAimingInput()
    {
        if (holdToAim)
        {
            isAiming = Input.GetMouseButton(1); // hold RMB
        }
        else
        {
            if (Input.GetMouseButtonDown(1)) isAiming = !isAiming; // toggle RMB
        }

        animator.SetBool("stat_aim", isAiming);
    }

   // IK untuk LookAt
    void OnAnimatorIK(int layerIndex)
    {
        if (animator == null) return;
    
        // 1. Jika lagi AIM, utamakan lihat target (default kamu)
        if (isAiming && currentTarget != null)
        {
            animator.SetLookAtWeight(1.0f, 0.15f, 1.0f, 1.0f, 0.5f);
            Vector3 lookPos = Vector3.Lerp(transform.forward, currentTarget.transform.position - transform.position, 0.5f);
            animator.SetLookAtPosition(transform.position + lookPos);
        }
        // 2. Jika tidak AIM tapi ada item dekat → kepala otomatis nengok
        else if (!isAiming && currentTarget != null)
        {
            animator.SetLookAtWeight(0.8f, 0.1f, 1f, 0.8f, 0.5f);
            animator.SetLookAtPosition(currentTarget.transform.position);
        }
        // 3. Jika tidak ada target
        else
        {
            animator.SetLookAtWeight(0f);
        }
    }

    GameObject FindClosestTargetWithTag(string tag, float range)
    {
        GameObject[] arr = GameObject.FindGameObjectsWithTag(tag);
        GameObject best = null;
        float bestSqr = range * range;
        Vector3 pos = transform.position;
        foreach (GameObject g in arr)
        {
            float d = (g.transform.position - pos).sqrMagnitude;
            if (d < bestSqr)
            {
                bestSqr = d;
                best = g;
            }
        }

        if (best != null) Debug.Log("Found target: " + best.name);
        return best;
    }

    // Debug radius di Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, lookAtRange);
    }
}

