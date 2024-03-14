using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public struct ParticleCollisionEvent
{
    public float timeToDestroy;
    public float relativeVelocity;
    public ParticleCollisionEvent(float timeToDestroy, float relativeVelocity)
    {
        this.timeToDestroy = timeToDestroy;
        this.relativeVelocity = relativeVelocity;
    }
}

public class CollisionReader : MonoBehaviour
{
    [SerializeField] private float expireTime;
    [SerializeField] private TMP_Text collisionEnergyText;
    public float collisionEnergy;
    public IdealGasSimulator idealGasSimulator;
    Queue<ParticleCollisionEvent> collisionEvents = new();
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Particle")
        {
            collisionEvents.Enqueue(new ParticleCollisionEvent(expireTime + Time.time, collision.relativeVelocity.magnitude));
        }
    }

    private void FixedUpdate()
    {
        while (collisionEvents.Count > 0 && collisionEvents.Peek().timeToDestroy < Time.time)
        {
            collisionEvents.Dequeue();
        }
        collisionEnergy = 0;
        foreach (var collision in collisionEvents)
        {
            collisionEnergy += collision.relativeVelocity;
        }
        collisionEnergy /= expireTime * 2;
        collisionEnergyText.text = collisionEnergy.ToString("F2");
    }
    
    public void ResetExperimentalVelocity()
    {
        while (collisionEvents.Count > 2)
        {
            collisionEvents.Dequeue();
        }
    }
}
