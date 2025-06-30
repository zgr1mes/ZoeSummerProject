using UnityEngine;

public class SocketDetectorScript : MonoBehaviour
{
    public AudioSource clueSFX;
    [SerializeField] bool SFXBool = false;

    public Transform clueSocket;
    public GameObject clueObject;
    public Camera player;
    public MeshRenderer clueMaterial;

    [SerializeField] private float maxDistance;
    [SerializeField] private float minDistance;
    [SerializeField] private float distance;
    [SerializeField] private float volume;

    void Start()
    {
        // ---- Uncomment when you create a scriptable object. ----//
        //clueSFX = GetComponent<AudioSource>();

        clueMaterial = clueObject.GetComponent<MeshRenderer>();
        maxDistance = 1;
        minDistance = 0;
    }

    void FixedUpdate()
    {
        CompareDistance();

        if(distance >= maxDistance)
        {
            clueMaterial.material.color = Color.red;

            Debug.Log("Max distance reached");
        }

        else
        {
            clueMaterial.material.color = Color.white;
        }
    }

    //When the clueObject is close to the clueSocket, then the volume will become stronger. 
    public void CompareDistance()
    {
        //Grab the location of the socket and the clue
        Transform socketTransform = clueSocket.GetComponent<Transform>();
        Transform clueTransform = clueObject.GetComponent<Transform>();

        //if the clue socket is active, find the distance between the clue and socket.
        if (clueSocket)
        {
            // Find the distance between the socket and the clue.
            distance = Vector3.Distance(socketTransform.position, clueTransform.position);

            float distancelimit = Mathf.Clamp(distance, minDistance, maxDistance);

            clueSFX.volume = 1 - distancelimit;

            Debug.Log("Distance between Clue and Socket is: "+ distancelimit);
        
        }
    }

    //Plays the sound effect
    public void PlaySong()
    {
        if (SFXBool)
        {
            Debug.Log("Sound played");
        }
        else if (!SFXBool)
        {
            clueSFX.Play();
            SFXBool = true;
        }
    }
}
