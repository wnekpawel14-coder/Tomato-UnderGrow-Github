using Mirror;
using UnityEngine;

public class PlayerInterakcja : NetworkBehaviour
{
    [Header("Wymagane Elementy")]
    public Camera kameraGracza;
    public Transform punktWyrzutu; // Miejsce przed graczem, gdzie spada item
    public float zasieg = 3f;

    private PlayerToolbar toolbar;

    void Start()
    {
        toolbar = GetComponent<PlayerToolbar>();
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        // PODNOSZENIE (E)
        if (Input.GetKeyDown(KeyCode.E))
        {
            StrzelRaycastem();
        }

        // WYRZUCANIE (Q)
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ItemData itemDoWyrzucenia = toolbar.GetActiveItem();
            if (itemDoWyrzucenia != null)
            {
                // Usuwamy z naszego lokalnego paska
                toolbar.UsunAktywnyPrzedmiot();
                
                // Mówimy serwerowi: "Zresp ten plik przed moją twarzą!"
                CmdWyrzucPrzedmiot(itemDoWyrzucenia.name, punktWyrzutu.position, punktWyrzutu.rotation);
            }
        }
    }

    void StrzelRaycastem()
    {
        RaycastHit hit;
        // Strzał ze środka kamery do przodu
        if (Physics.Raycast(kameraGracza.transform.position, kameraGracza.transform.forward, out hit, zasieg))
        {
            ItemPickup podnoszonyItem = hit.collider.GetComponentInParent<ItemPickup>();
            if (podnoszonyItem != null)
            {
                // Jeśli udało się dodać do paska...
                if (toolbar.DodajPrzedmiot(podnoszonyItem.itemData, podnoszonyItem.ilosc))
                {
                    // Rozkazujemy serwerowi: Zniszcz to z mapy!
                    CmdZniszczPrzedmiot(podnoszonyItem.gameObject);
                }
            }
        }
    }

    // --- MAGIA SERWERA ---
    
    [Command]
    void CmdZniszczPrzedmiot(GameObject obiektDoZniszczenia)
    {
        NetworkServer.Destroy(obiektDoZniszczenia);
    }

    [Command]
    void CmdWyrzucPrzedmiot(string nazwaPlikuItemu, Vector3 pozycja, Quaternion rotacja)
    {
        // Serwer ładuje plik ze specjalnego folderu Resources/Items
        ItemData daneItemu = Resources.Load<ItemData>("Items/" + nazwaPlikuItemu);
        
        if (daneItemu != null && daneItemu.prefabDropu != null)
        {
            GameObject nowyDrop = Instantiate(daneItemu.prefabDropu, pozycja, rotacja);
            NetworkServer.Spawn(nowyDrop); // Zespawnuj u wszystkich graczy!
        }
    }
}