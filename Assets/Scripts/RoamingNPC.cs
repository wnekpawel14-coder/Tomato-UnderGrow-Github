using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class RoamingNPC : NetworkBehaviour
{
    public float walkRadius = 10f;
    public float waitTime = 3f;
    private NavMeshAgent agent;
    private float moveTimer;

    [Header("Dialog Settings")]
    [TextArea(2, 5)]
    public string[] introduction;
    
    [SyncVar] private bool isTalking = false;
    private float cooldownTimer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        moveTimer = waitTime;
    }

    void Update()
    {
        // Lokalna interakcja u gracza
        if (NetworkClient.localPlayer != null)
        {
            float dist = Vector3.Distance(transform.position, NetworkClient.localPlayer.transform.position);
            
            if (cooldownTimer > 0) cooldownTimer -= Time.deltaTime;

            if (dist < 4f && Input.GetKeyDown(KeyCode.E) && cooldownTimer <= 0)
            {
                cooldownTimer = 0.5f; // Blokada na pół sekundy
                
                if (!isTalking)
                {
                    Debug.Log("Rozpoczynam rozmowę z: " + name);
                    CmdSetTalking(true);
                    DialogueManager.instance.StartDialogue(introduction);
                }
                else
                {
                    DialogueManager.instance.DisplayNextSentence();
                }
            }

            // Automatyczne zamykanie jeśli gracz ucieknie
            if (isTalking && dist > 6f)
            {
                CmdSetTalking(false);
                DialogueManager.instance.EndDialogue();
            }
        }

        // Ruch NPC (Tylko serwer)
        if (!isServer || isTalking) 
        {
            if(agent.isOnNavMesh) agent.isStopped = isTalking;
            return;
        }

        moveTimer += Time.deltaTime;
        if (moveTimer >= waitTime)
        {
            Vector3 newPos = RandomNavMeshLocation(walkRadius);
            if (agent.isOnNavMesh) agent.SetDestination(newPos);
            moveTimer = 0;
        }
    }

    [Command(requiresAuthority = false)]
    void CmdSetTalking(bool state)
    {
        isTalking = state;
    }

    public Vector3 RandomNavMeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1)) return hit.position;
        return transform.position;
    }
}