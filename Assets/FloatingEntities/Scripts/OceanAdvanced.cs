using UnityEngine;
using System.Collections;

public class OceanAdvanced : MonoBehaviour
{
    class Wave
    {
        public float waveLength { get; set; }
        public float speed { get; set; }
        public float amplitude { get; set; }
        public float sharpness { get; set; }
        public float frequency { get; set; }
        public float phase { get; set; }
        public Vector2 direction { get; set; }

        public Wave(float waveLength, float speed, float amplitude, float sharpness, Vector2 direction)
        {
            this.waveLength = waveLength;
            this.speed = speed;
            this.amplitude = amplitude;
            this.sharpness = sharpness;
            this.direction = direction.normalized;
            frequency = (2 * Mathf.PI) / waveLength;
            phase = frequency * speed;
        }
    };

    public Material ocean;
  public Light sun;
 
  private int interaction_id = 0;
  private Vector4[] interactions = new Vector4[NB_INTERACTIONS];
  
  const int NB_WAVE = 5;
  const int NB_INTERACTIONS = 64;
  static Wave[] waves =
  {
    new Wave(400, 0.5f, 0f, 0.5f, new Vector2(1.0f,  0.2f)),
    new Wave(240, 0.6f, 0f, 0.3f, new Vector2(1.0f,  3.0f)),
    new Wave(80, 1.7f, 0f, 0.4f, new Vector2(2.0f,  4.0f)),
    new Wave(100, 1.4f, 0f, 0.2f, new Vector2(-1.0f, 0.0f)),
    new Wave(32, 1.9f, 0f, 0.5f,new Vector2(-1.0f, 1.2f))
  };

    //new Wave(99, 1.0f, 1.4f, 0.9f, new Vector2(1.0f,  0.2f)),
    //new Wave(60, 1.2f, 0.8f, 0.5f, new Vector2(1.0f,  3.0f)),
    //new Wave(20, 3.5f, 0.4f, 0.8f, new Vector2(2.0f,  4.0f)),
    //new Wave(30, 2.0f, 0.4f, 0.4f, new Vector2(-1.0f, 0.0f)),
    //new Wave(10, 3.0f, 0.05f, 0.9f,new Vector2(-1.0f, 1.2f))

  // Set wave origin
  public float[] waveOrigin;

  void Awake()
  {
        Vector4[] v_waves = new Vector4[NB_WAVE];
    Vector4[] v_waves_dir = new Vector4[NB_WAVE];
    for (int i = 0; i < NB_WAVE; i++)
    {
      v_waves[i] = new Vector4(waves[i].frequency, waves[i].amplitude, waves[i].phase, waves[i].sharpness);
      v_waves_dir[i] = new Vector4(waves[i].direction.x, waves[i].direction.y, 0, 0);
    }

    ocean.SetVectorArray("waves_p", v_waves);
    ocean.SetVectorArray("waves_d", v_waves_dir);

    for (int i = 0; i < NB_INTERACTIONS; i++)
      interactions[i].w = 500.0F;
    ocean.SetVectorArray("interactions", interactions);
    ocean.SetVector("world_light_dir", -sun.transform.forward);

    // Set wave origin
    waveOrigin = new float[waves.Length];
    for(int i = 0; i < waves.Length; i++)
    {
      waveOrigin[i] = waves[i].amplitude;
    }
  }

  void Update()
  {
    
  }

  void FixedUpdate()
  {
    ocean.SetVector("world_light_dir", -sun.transform.forward);
    ocean.SetVector("sun_color", new Vector4(sun.color.r, sun.color.g, sun.color.b, 0.0F));
  }

  public void RegisterInteraction(Vector3 pos, float strength)
  {
    interactions[interaction_id].x = pos.x;
    interactions[interaction_id].y = pos.z;
    interactions[interaction_id].z = strength;
    interactions[interaction_id].w = Time.time;
    ocean.SetVectorArray("interactions", interactions);
    interaction_id = (interaction_id + 1) % NB_INTERACTIONS;
  }

  public static float GetWaterHeight(Vector3 p)
  {
        
    float height = 0;
    for (int i = 0; i < NB_WAVE; i++)
      height += waves[i].amplitude * Mathf.Sin(Vector2.Dot(waves[i].direction, new Vector2(p.x, p.z)) * waves[i].frequency + Time.time * waves[i].phase);
    return height;
  }

  public void SetWaveAmplitude(float amplitudeCoef = 1f)
  {
        print("SetWaveAmplitude");
    for(int i = 0; i < waves.Length; i++)
    {
      waves[i].amplitude = waveOrigin[i] * amplitudeCoef;
    }

    // 
    Vector4[] v_waves = new Vector4[NB_WAVE];
        Vector4[] v_waves_dir = new Vector4[NB_WAVE];
        for (int i = 0; i < NB_WAVE; i++)
        {
            v_waves[i] = new Vector4(waves[i].frequency, waves[i].amplitude, waves[i].phase, waves[i].sharpness);
            v_waves_dir[i] = new Vector4(waves[i].direction.x, waves[i].direction.y, 0, 0);
        }

        ocean.SetVectorArray("waves_p", v_waves);
        ocean.SetVectorArray("waves_d", v_waves_dir);
    }
}
