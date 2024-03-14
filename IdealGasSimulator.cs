using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class IdealGasSimulator : MonoBehaviour
{
    [SerializeField] EditableProperty temperature, volume, moles;
    [SerializeField] TMP_Text pressure, velocity;
    List<Rigidbody2D> Particles = new();
    public GameObject ParticlePrefab, Box;
    [SerializeField] float randomness;
    // Update is called once per frame
    void Update()
    {
        float pressure = (moles.value * 8.31f * temperature.value) / volume.value; //calculate the pressure using the formula P = nRT/V
        //float velocity = Mathf.Sqrt(3 * 8.31f * temperature.value / moles.value); //calculate the velocity using the formula v = sqrt(3RT/M)
        float velocity = Mathf.Sqrt(temperature.value * 2f); //set the velocity to the temp (kinetic energy), turned into velocity by the basic formulas
        float length = Mathf.Sqrt(volume.value); //calculate the length of the container
        randomness = (length / 2) - 0.5f; //set the randomness to half the length of the container.
                                          //0.5f is subtracted to prevent the particles from spawning outside the container, due to wall thickness.

        if (Particles.Count != moles.value)
        {
            //if the number of particles is not equal to the number of moles
            if (Particles.Count < moles.value) //if the number of particles is less than the number of moles
            {
                for (int i = 0; i < moles.value - Particles.Count; i++) //add the difference between the number of particles and the number of moles
                {
                    GameObject particle = Instantiate(ParticlePrefab, Box.transform.position + 
                        new Vector3(Random.Range(-randomness, randomness), Random.Range(-randomness, randomness), 0), Quaternion.identity, transform); //create a new particle
                    Particles.Add(particle.GetComponent<Rigidbody2D>()); //add the particle to the list
                }
            }
            else //if the number of particles is greater than the number of moles
            {
                for (int i = 0; i < Particles.Count - moles.value; i++) //remove the difference between the number of particles and the number of moles
                {
                    Destroy(Particles[Particles.Count - 1].gameObject); //destroy the particle
                    Particles.RemoveAt(Particles.Count - 1); //remove the particle from the list
                }
            }   
        }


        //update the particles
        for (int i = 0; i < Particles.Count; i++)
        {
            Rigidbody2D particle = Particles[i]; //get the particle

            if (particle.velocity.magnitude == 0) //if the velocity of the particle is 0
                particle.velocity = new Vector2(Random.Range(-2, 2), Random.Range(-2, 2)); //give it some velocity

            if (particle.velocity.magnitude != velocity) //if the velocity of the particle is not the desired velocity
                particle.velocity = particle.velocity.normalized * velocity; //normalize the velocity of the particle and multiply it by the velocity
        }

        //labels
        this.pressure.text = pressure.ToString("F2"); //update the pressure text
        this.velocity.text = velocity.ToString("F2"); //update the velocity text

        //resize the container
        for (int i = 0; i < 4; i++)
        {
            GameObject wall = Box.transform.GetChild(i).gameObject; //get the wall
            if (i % 2 == 0) //if the wall is vertical
                wall.transform.localScale = new Vector3(wall.transform.localScale.x, length, wall.transform.localScale.z); //update
            else //if the wall is horizontal
                wall.transform.localScale = new Vector3(length, wall.transform.localScale.y, wall.transform.localScale.z); //update

            int horizValue = i % 2 == 0 ? (i == 0 ? -1 : 1) : 0; //complicated bool for pos
            int vertValue = i % 2 == 1 ? (i == 1 ? -1 : 1) : 0; //complicated bool for pos

            wall.transform.localPosition = new Vector3(horizValue * length / 2, vertValue * length / 2, 0); //reset the position
        }

    }
}
