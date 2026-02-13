using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityBody : MonoBehaviour
{
    [SerializeField] private GravityAttractor attractor;

    public GravityAttractor Attractor
    {
        get => attractor;
        set => attractor = value;
    }

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void FixedUpdate()
    {
        if (attractor != null)
        {
            attractor.Attract(transform);
        }
    }
}