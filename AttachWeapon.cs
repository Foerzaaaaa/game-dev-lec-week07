using UnityEngine;

public class AttachWeapon : MonoBehaviour
{
    [Header("Prefab Senjata")]
    public GameObject weaponPrefab; // drag pistol prefab

    [Header("Titik Attachment (RightHand)")]
    public Transform handBone;

    [Header("Offset Transform")]
    public Vector3 localPosition = Vector3.zero;
    public Vector3 localEuler = Vector3.zero;
    public Vector3 localScale = Vector3.one; // 🔹 bisa diubah di Inspector

    private GameObject instance;

    void Start()
    {
        if (weaponPrefab == null || handBone == null)
        {
            Debug.LogWarning("AttachWeapon: assign weaponPrefab and handBone in inspector.");
            return;
        }

        // Buat senjata sebagai child dari handBone
        instance = Instantiate(weaponPrefab, handBone);

        // Set posisi, rotasi, dan skala
        instance.transform.localPosition = localPosition;
        instance.transform.localEulerAngles = localEuler;
        instance.transform.localScale = localScale;
    }

    // 🔹 Opsional: method untuk ganti skala runtime
    public void SetWeaponScale(Vector3 newScale)
    {
        if (instance != null)
            instance.transform.localScale = newScale;
    }
}
