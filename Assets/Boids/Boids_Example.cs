using UnityEngine;
using UnityEngine.UI;

public class Boids_Example : MonoBehaviour
{
    [SerializeField] GameObject FlockPrefab;

    [SerializeField] Slider seperationSlider;
    [SerializeField] Slider alignmentSlider;
    [SerializeField] Slider cohesionSlider;

    [SerializeField] int birdNum = 100;

    public Boid[] boids;
    GameObject[] flocks;

    void Awake()
    {
        alignmentSlider.value = 1;
        alignmentSlider.minValue = 0;
        alignmentSlider.maxValue = 5;

        cohesionSlider.value = 1;
        cohesionSlider.minValue = 0;
        cohesionSlider.maxValue = 5;

        seperationSlider.value = 1;
        seperationSlider.minValue = 0;
        seperationSlider.maxValue = 5;

        boids = new Boid[birdNum];
        flocks = new GameObject[birdNum];

        for (int i = 0; i < birdNum; i++)
        {
            //Change width and height to bounds
            var position = new Vector2(Random.Range(0,Screen.width), Random.Range(0, Screen.height));
            var velocity = new Vector2(Random.value, Random.value).normalized * Random.Range(2f,4f);
            var acceleration = new Vector2();
            var maxForce = 0.2f;
            var maxSpeed = 5;

            boids[i] = new Boid(position, velocity, acceleration, maxForce, maxSpeed);

            var newflock = Instantiate(FlockPrefab);
            newflock.name = $"Flock {i}";
            flocks[i] = newflock;
        }
    }

    void Update()
    {
        for (int i = 0; i < boids.Length; i++)
        {
            boids[i].EdgeRespawn(Screen.width, Screen.height);
            boids[i].Update(boids, seperationSlider.value, alignmentSlider.value, cohesionSlider.value);

            flocks[i].transform.position = boids[i].Position;
        }
    }

    public class Boid
    {
        Vector2 position;
        public Vector2 Position { get => position; }

        Vector2 velocity;
        Vector2 acceleration;
        float maxForce;
        float maxSpeed;
        float perceptionRadius;

        public Boid(Vector2 startpos, Vector2 startVelocity, Vector2 startAcceleration,
            float maxForce, float maxSpeed)
        {
            this.position = startpos;
            this.velocity = startVelocity;
            this.acceleration = startAcceleration;
            this.maxForce = maxForce;
            this.maxSpeed = maxSpeed;

            perceptionRadius = 50;
        }

        //TODO replace this with bounds instead?
        public void EdgeRespawn(float width, float height)
        {
            //Bounds b;
            //if (b.Encapsulate(position))

            if (position.x > width) position.x -= width;
            else if (position.x < 0) position.x += width;


            if (position.y > height) position.y -= height;
            else if (position.y < 0) position.y += height;
        }

        Vector2 seperation(in Boid[] boids)
        {
            var steering = new Vector2();
            var nearbyCount = 0;
            foreach (var boid in boids)
            {
                var dist = position - boid.position;
                if (boid != this && dist < perceptionRadius)
                {
                    steering += (position - boid.position) / (dist * dist);
                    nearbyCount++;
                }
            }
            if (nearbyCount > 0)
            {
                steering /= nearbyCount;
                steering = steering.normalized * maxSpeed;
                steering -= velocity;
                steering = Mathf.Clamp(steering.magnitude, 0, maxForce) * steering.normalized;
            }

            return steering;
        }

        Vector2 alignment(in Boid[] boids)
        {
            var steering = new Vector2();
            var nearbyCount = 0;

            foreach (var boid in boids)
            {
                var dist = Vector2.Distance(position, boid.position);
                if (boid != this && dist < perceptionRadius)
                {
                    steering += boid.position;
                    nearbyCount++;
                }
            }

            if (nearbyCount > 0)
            {
                steering /= nearbyCount;
                steering = steering.normalized * maxSpeed;
                steering -= velocity;
                steering = Mathf.Clamp(steering.magnitude, 0, maxForce) * steering.normalized;
            }

            return steering;
        }
        
        Vector2 cohesion(in Boid[] boids)
        {
            var steering = new Vector2();
            var nearbyCount = 0;

            foreach (var boid in boids)
            {
                var dist = Vector2.Distance(position, boid.position);
                if (boid != this && dist < perceptionRadius)
                {
                    steering += boid.position;
                    nearbyCount++;
                }
            }

            if (nearbyCount > 0)
            {
                steering /= nearbyCount;
                steering -= position;
                steering = steering.normalized * maxSpeed;
                steering -= velocity;
                steering = Mathf.Clamp(steering.magnitude, 0, maxForce) * steering.normalized;
            }

            return steering;
        }

        public void Update(in Boid[] boids,
            float seperationMult, float alignmentMult, float cohesionMul)
        {
            var seperation = this.seperation(boids) * cohesionMul;
            var alignment = this.alignment(boids) * seperationMult;
            var cohesion = this.cohesion(boids) * alignmentMult;

            acceleration += seperation + alignment + cohesion;

            position += velocity;
            velocity += acceleration;
            velocity = Mathf.Clamp(velocity.magnitude, 0, maxSpeed) * velocity.normalized;
            acceleration *= 0;
        }
    }
}