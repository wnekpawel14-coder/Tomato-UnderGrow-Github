using UnityEngine;

public class DayNightSystem : MonoBehaviour
{
    [Header("Prędkość czasu")]
    [Tooltip("Wyższa liczba = szybszy upływ czasu")]
    public float timeSpeed = 10f;

    [Header("Oświetlenie")]
    public Light directionalLight; // Tu wrzucisz swoje słońce

    void Update()
    {
        if (directionalLight != null)
        {
            // Obraca słońce wokół osi X (wschód -> zachód)
            directionalLight.transform.Rotate(Vector3.right * timeSpeed * Time.deltaTime);
        }
        else
        {
            // Jeśli zapomniałeś przypisać słońca, spróbuj je znaleźć samemu
            directionalLight = GetComponent<Light>();
        }
    }
}