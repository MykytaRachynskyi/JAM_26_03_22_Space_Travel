using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetController : MonoBehaviour
{
    [SerializeField] AudioSource explosionSFX;
    [SerializeField] GameObject sphereMain;
    [SerializeField] GameObject sphereCells;
    [SerializeField] Rigidbody[] cells;
    [SerializeField] Rigidbody mainRB;
    [SerializeField] GameObject atmosphere;
    [SerializeField] GameObject explosion;
    [SerializeField] Collider mainCollider;

    [SerializeField] float explosionForce = 5f;
    [SerializeField] float explosionRadius = 1f;
    [SerializeField] bool clockwise = true;
    [SerializeField] float orbitSpeed = .1f;

    bool exploded = false;
    float orbitRadius;

    Quaternion rightAngleRotation = Quaternion.Euler(0f, 0f, 90f);

    void Awake()
    {
        orbitRadius = Vector3.Distance(Vector3.zero, transform.position);
    }

    public void Explode()
    {
        exploded = true;

        explosionSFX.Play();
        sphereMain.gameObject.SetActive(false);
        atmosphere.gameObject.SetActive(false);
        mainCollider.enabled = false;

        explosion.SetActive(true);

        sphereCells.gameObject.SetActive(true);

        for (int i = 0; i < cells.Length; ++i)
        {
            cells[i].AddExplosionForce(explosionForce, this.transform.position, explosionRadius, 0f, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        if (exploded) return;

        Vector3 toPlanetVector = (-transform.position).normalized;
        Vector3 forwardVector = (rightAngleRotation * toPlanetVector) * (clockwise ? 1f : -1f);
        forwardVector =
            Vector3.RotateTowards(forwardVector, toPlanetVector,
                Mathf.Clamp(Vector3.Distance(Vector3.zero, transform.position) - orbitRadius, 0f, float.MaxValue), 0f);
        mainRB.velocity = forwardVector * Mathf.PI * Vector3.Distance(Vector3.zero, transform.position) * orbitSpeed;
    }
}