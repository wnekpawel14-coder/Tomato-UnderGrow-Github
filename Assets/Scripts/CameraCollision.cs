using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    public Transform player;      // Gracz
    public Vector3 dollyDir;      // Kierunek kamery
    public float minDistance = 1f;
    public float maxDistance = 4f;
    public float distance;
    public LayerMask collisionLayers; // Zaznacz "Default" i ściany

    void Awake()
    {
        dollyDir = transform.localPosition.normalized;
        distance = transform.localPosition.magnitude;
    }

    void Update()
    {
        Vector3 desiredCameraPos = transform.parent.TransformPoint(dollyDir * maxDistance);
        RaycastHit hit;

        // Strzelamy promieniem od gracza do idealnej pozycji kamery
        if (Physics.Linecast(transform.parent.position, desiredCameraPos, out hit, collisionLayers))
        {
            // Jeśli coś trafi we ścianę, przybliż kamerę
            distance = Mathf.Clamp((hit.distance * 0.8f), minDistance, maxDistance);
        }
        else
        {
            distance = maxDistance;
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, dollyDir * distance, Time.deltaTime * 10f);
    }
}