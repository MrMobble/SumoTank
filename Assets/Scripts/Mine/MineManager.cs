using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineManager : MonoBehaviour
{

    public ParticleSystem ExplosionParticle;
    public float MaxLifeTime;
    public float ExplosionRadius;
    public LayerMask TankMask;
    public float BaseForce;
    public float AddedForce;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Tank")
        {

            Collider[] TankColliders = Physics.OverlapSphere(transform.position, ExplosionRadius, TankMask);

            foreach (Collider C in TankColliders)
            {
                Rigidbody TargetBody = C.GetComponent<Rigidbody>();

                if (!TargetBody) continue;

                TankHealth Tank = TargetBody.GetComponent<TankHealth>();

                if (!Tank) continue;

                //Calculate FinalForce To Apply
                float FinalForce = BaseForce + Tank.GetVulnerability() * AddedForce;

                //Apply ExplosionForce
                TargetBody.AddExplosionForce(FinalForce, transform.position, ExplosionRadius);

                //Add Vulnerability To Tank
                Tank.AddVulnerability();
            }

            // Unparent the particles from the shell.
            ExplosionParticle.transform.rotation = Quaternion.identity;
            ExplosionParticle.transform.parent = null;

            // Play the particle system.
            ExplosionParticle.Play();

            ParticleSystem.MainModule MainModule = ExplosionParticle.main;
            Destroy(ExplosionParticle.gameObject, MainModule.duration);

            //Destroy Bullet
            Destroy(gameObject);

        }
    }
}
