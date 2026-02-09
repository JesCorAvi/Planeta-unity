using UnityEngine;

public class GravityAttractor : MonoBehaviour
{
    public float gravity = -10f;

    public void Attract(Transform body)
    {
        Vector3 gravityUp = (body.position - transform.position).normalized;
        Vector3 localUp = body.up;

        // Aplicar fuerza de gravedad
        body.GetComponent<Rigidbody>().AddForce(gravityUp * gravity);

        // Rotar el cuerpo para que sus pies apunten al centro del planeta
        Quaternion targetRotation = Quaternion.FromToRotation(localUp, gravityUp) * body.rotation;
        body.rotation = Quaternion.Slerp(body.rotation, targetRotation, 50f * Time.deltaTime);
    }
}