using UnityEngine;
using System.Collections.Generic;

public class Lightsaber : MonoBehaviour 
{
    private GameObject bladeGameObject;

    private readonly float bladeExtendSpeed = 0.1f;

    private readonly float bladeColorIntensity = 3f;

    private readonly float bladeLightIntensity = 6f;

    [field: SerializeField]
    public AudioClip HummingSound { get; private set; }

    private Blade blade;

    private AudioSource lightsaberSound;

    private class Blade
    {
        /// <summary>
        /// The color property in the shader. This will receive the color set via this script.
        /// </summary>
        private const string SHADER_PROPERTY_EMISSION_COLOR = "_EmissionColor";

        // the blade itself
        private GameObject gameObject;

        // the light attached to the blade
        private Light light;

        // minimum blade length
        private float scaleMin;

        // maximum blade length; initialized with length from scene
        private float scaleMax;

        // current scale, lerped between min and max scale
        private float scaleCurrent;

        // the delta is a lerp value within 1 second. it will be initialized depending on the extend speed
        private float extendDelta;

        private float localScaleX;

        private float localScaleZ;

        public Blade(GameObject gameObject, float extendSpeed)
        {
            this.gameObject = gameObject;
            this.light = gameObject.GetComponentInChildren<Light>();

            // remember initial scale values (non extending part of the blade)
            this.localScaleX = gameObject.transform.localScale.x;
            this.localScaleZ = gameObject.transform.localScale.z;

            // remember initial scale values (extending part of the blade)
            this.scaleMin = 0f;
            this.scaleMax = gameObject.transform.localScale.y;

            // initialize variables
            // the delta is a lerp value within 1 second. depending on the extend speed we have to size it accordingly
            extendDelta = this.scaleMax / extendSpeed;
               
            // set blade size to maximum
            scaleCurrent = scaleMax;
            extendDelta *= 1;
        }

        public void SetColor(Color color, float intensity)
        {
            if (light != null)
            {
                light.color = color;
            }
			
			Color bladeColor = color * intensity;

			// set the color in the shader. _EmissionColor is a reference which is defined in the property of the graph
            gameObject.GetComponentInChildren<MeshRenderer>().materials[0].SetColor( SHADER_PROPERTY_EMISSION_COLOR, bladeColor);

        }

        public void UpdateLight(float lightIntensity)
        {
            if (this.light == null) 
                return;

            // light intensity depending on blade size
            this.light.intensity = this.scaleCurrent * lightIntensity;
        }

        public void UpdateSize()
        {
            // consider delta time with blade extension
            scaleCurrent += extendDelta * Time.deltaTime;

            // clamp blade size
            scaleCurrent = Mathf.Clamp(scaleCurrent, scaleMin, scaleMax);

            // scale in z direction
            gameObject.transform.localScale = new Vector3(this.localScaleX, scaleCurrent, this.localScaleZ);

            // show / hide the gameobject
            gameObject.SetActive(true);
        }
    }
    
    // Use this for initialization
    private void Awake()
    {
        bladeGameObject = transform.Find("Blade").gameObject;
        lightsaberSound = GetComponent<AudioSource>();

        blade = new Blade(bladeGameObject, bladeExtendSpeed);

        EnableLightsaberHumming();

        UpdateBlade();
    }

    // Update is called once per frame
	private void Update() 
    {
        if (lightsaberSound.isPlaying == false) 
        {
            EnableLightsaberHumming();
        }

        UpdateBlade();
    }

    private void EnableLightsaberHumming()
    {
        lightsaberSound.clip = HummingSound;
        lightsaberSound.loop = true;
        lightsaberSound.Play();
    }

    private void UpdateBlade()
    {
        blade.UpdateLight(bladeLightIntensity);
        blade.UpdateSize();
    }

    public void MuteLightsaber() 
    {
        lightsaberSound.Stop();
        lightsaberSound.mute = true;
    }

    public void UnmuteLightsaber()
    {
        lightsaberSound.mute = false;
    }

    public void DoLightsaberSound(AudioClip sound) 
    {
        lightsaberSound.Stop();
        lightsaberSound.loop = false;
        lightsaberSound.clip = sound;
        lightsaberSound.Play();
    }

    public void InitializeBladeColor(PlayerData playerData)
    {
        blade.SetColor(playerData.LightsaberColor, bladeColorIntensity);
    }

    public void InitializeBladeColor(EnemyData enemyData)
    {
        blade.SetColor(enemyData.LightsaberColor, bladeColorIntensity);
    }
}
