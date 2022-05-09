using UnityEngine;
using System.Collections.Generic;

public class Weapon : MonoBehaviour {

    public GameObject bladeGameObject;

    public float bladeExtendSpeed = 0.1f;

    public Color bladeColor;
	
	public float bladeColorIntensity = 600f; 

	public float lightIntensity = 1000f;

    public AudioClip hummingSound;

    private Blade blade;

    public AudioSource source;


    /// <summary>
    /// The color property in the shader. This will receive the color set via this script.
    /// </summary>
    private const string SHADER_PROPERTY_EMISSION_COLOR = "_EmissionColor";

    private class Blade
    {
        // the blade itself
        public GameObject gameObject;

        // the light attached to the blade
        public Light light;

        // minimum blade length
        private float scaleMin;

        // maximum blade length; initialized with length from scene
        private float scaleMax;

        // current scale, lerped between min and max scale
        private float scaleCurrent;

        public bool active = false;

        // the delta is a lerp value within 1 second. it will be initialized depending on the extend speed
        private float extendDelta;

        private float localScaleX;
        private float localScaleZ;

        public Blade( GameObject gameObject, float extendSpeed, bool active)
        {

            this.gameObject = gameObject;
            this.light = gameObject.GetComponentInChildren<Light>();
            this.active = active;

            // consistency check
            /*
            if (light == null)
            {
                Debug.Log("No light found. Blade should have a light as child");
            }
            */

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

        public void SetColor( Color color, float intensity)
        {
            if (light != null)
            {
                light.color = color;
            }
			
			Color bladeColor = color * intensity;

			// set the color in the shader. _EmissionColor is a reference which is defined in the property of the graph
            gameObject.GetComponentInChildren<MeshRenderer>().materials[0].SetColor( SHADER_PROPERTY_EMISSION_COLOR, bladeColor);

        }

         public void SetActive()
        {
            // whether to scale in positive or negative direction
            extendDelta = Mathf.Abs(extendDelta);

        }

        public void updateLight( float lightIntensity)
        {
            if (this.light == null)
                return;

            // light intensity depending on blade size
            this.light.intensity = this.scaleCurrent * lightIntensity;
        }

        public void updateSize()
        {

            // consider delta time with blade extension
            scaleCurrent += extendDelta * Time.deltaTime;

            // clamp blade size
            scaleCurrent = Mathf.Clamp(scaleCurrent, scaleMin, scaleMax);

            // scale in z direction
            gameObject.transform.localScale = new Vector3(this.localScaleX, scaleCurrent, this.localScaleZ);

            // whether the blade is active or not
            active = scaleCurrent > 0;
             
            // show / hide the gameobject depending on the blade active state
          
            gameObject.SetActive(true);
            
        }
    }
    
    // Use this for initialization
    void Awake () {


        blade = new Blade(bladeGameObject, bladeExtendSpeed, true);

        // initialize audio depending on beam activitiy
        InitializeAudio();

        // light and blade color
        InitializeBladeColor();

        // initially update blade length, so that it isn't set to what we have in unity's visual editor
        UpdateBlades();


    }

    // Update is called once per frame
	void Update () {
        
        // key pressed

        if (source.isPlaying == false) 
        {
            source.clip = hummingSound;
            source.loop = true;
            source.Play();

        }

        UpdateBlades();



    }

    void InitializeAudio()
    {
        
        source.clip = hummingSound;
        source.loop = true;
        source.Play();

    }

    // set the color of the light and the blade
    void InitializeBladeColor()
    {
        // update blade color, light color and glow color
       
        blade.SetColor(bladeColor, bladeColorIntensity);
    
    }
	
    private void UpdateBlades()
    {
       
        blade.updateLight( lightIntensity);
        blade.updateSize();
    }

}
