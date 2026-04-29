using UnityEngine;

public class CameraFix : MonoBehaviour
{
    public Transform playerTransform;   // Tutaj w Inspektorze przeciągnij swoją Kapsułę/Gracza
    public float maxDistance = 3.0f;    // Jak daleko ma być kamera normalnie
    public float minDistance = 0.5f;    // Jak blisko może podejść do pleców
    public float smoothSpeed = 10.0f;   // Jak szybko ma się przybliżać
    public LayerMask wallLayers;        // Tu zaznacz warstwy, które są ścianami (np. Default)

    private Vector3 currentRotation;
    private float currentDistance;

    void Start()
    {
        currentDistance = maxDistance;
        
        // Jeśli zapomnisz przypisać gracza, skrypt sam spróbuje go znaleźć po Tagu
        if (playerTransform == null && GameObject.FindGameObjectWithTag("Player") != null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void LateUpdate() // LateUpdate jest lepszy dla kamer, bo wykonuje się po ruchu gracza
    {
        if (playerTransform == null) return;

        // 1. Obliczamy idealną pozycję kamery (tam, gdzie CHCIAŁABY być)
        Vector3 dir = (transform.position - playerTransform.position).normalized;
        Vector3 targetPos = playerTransform.position + dir * maxDistance;

        // 2. Strzelamy promieniem (Raycast), żeby sprawdzić czy po drodze jest ściana
        RaycastHit hit;
        if (Physics.Linecast(playerTransform.position, targetPos, out hit, wallLayers))
        {
            // Jeśli trafiliśmy w ścianę, nowym dystansem jest miejsce uderzenia (trochę bliżej)
            float tempDistance = Vector3.Distance(playerTransform.position, hit.point) * 0.9f;
            currentDistance = Mathf.Clamp(tempDistance, minDistance, maxDistance);
        }
        else
        {
            // Jeśli nie ma ściany, wracamy do max dystansu
            currentDistance = Mathf.MoveTowards(currentDistance, maxDistance, Time.deltaTime * smoothSpeed);
        }

        // 3. Ustawiamy kamerę na nowej, bezpiecznej pozycji
        transform.position = playerTransform.position + dir * currentDistance;
    }
}