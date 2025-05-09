using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header("Textures")]
    [SerializeField]
    private Texture2D skyboxNight;

    [SerializeField]
    private Texture2D skyboxSunrise;

    [SerializeField]
    private Texture2D skyboxSunset;

    [SerializeField]
    private Texture2D skyboxDay;


    [Header("Gradients")]
    [SerializeField]
    private Gradient gradientNightToSunrise;

    [SerializeField]
    private Gradient gradientSunriseToDay;

    [SerializeField]
    private Gradient gradientDayToSunset;

    [SerializeField]
    private Gradient gradientSunsetToNight;

    [SerializeField]
    private Color DefaultLightColor;


    [Header("Global Light")]
    [SerializeField]
    private Light globalLight;

    private int minutes;
    public int Minutes { get { return minutes; } set { minutes = value; OnMinuteChange(value); } }

    private int hours;
    public int Hours {get { return hours; } set { hours = value; OnHourChange(value); } }

    private int days;
    public int Days { get { return days; } set { days = value; } }

    private float tempSecond;

    public bool progressTime;

    private Texture2D FirstTexture;
    private Texture2D SecondTexture;

    private void Start()
    {
        FirstTexture = (Texture2D)RenderSettings.skybox.GetTexture("_Texture1");
        SecondTexture = (Texture2D)RenderSettings.skybox.GetTexture("_Texture2");
    }

    public void Update()
    {
        if (!progressTime) return;
        tempSecond += Time.deltaTime;
        if(tempSecond >= 1) 
        {
            Minutes += 1;
            tempSecond = 0;
        }

    }

    public void SetTime() 
    {
        // To do: control time as i want
        //Add potencial Nights with and without moon
    }


    private void OnMinuteChange(int value) 
    {
        globalLight.transform.Rotate(Vector3.up,(1f/1440f)*360f, Space.World);
        if(value >= 60) 
        {
            Hours++;
            minutes = 0;
        }

        if(Hours >= 24) 
        {
            Hours = 0;
            Days++;
        }
    }

    private void OnHourChange(int value)
    {
        if(value == 5)
        {
            StartCoroutine(LerpSkyBox(skyboxNight, skyboxSunrise, 10f));
            StartCoroutine(LerpLight(gradientNightToSunrise, 10f));
        }
        else if(value == 9) 
        {
            StartCoroutine(LerpSkyBox(skyboxSunrise, skyboxDay, 10f));
            StartCoroutine(LerpLight(gradientSunriseToDay, 10f));
        }
        else if(value == 18)
        {
            StartCoroutine(LerpSkyBox(skyboxDay, skyboxSunset, 10f));
            StartCoroutine(LerpLight(gradientDayToSunset, 10f));
        }
        else if(value == 22) 
        {
            StartCoroutine(LerpSkyBox(skyboxSunset, skyboxNight, 10f));
            StartCoroutine(LerpLight(gradientSunsetToNight, 10f));
        }
    }

    private IEnumerator LerpSkyBox(Texture2D a, Texture2D b, float time) 
    {
        RenderSettings.skybox.SetTexture("_Texture1", a);
        RenderSettings.skybox.SetTexture("_Texture2", b);
        RenderSettings.skybox.SetFloat("_Blend", 0);

        for(float i = 0; i < time; i+= Time.deltaTime) 
        {
            RenderSettings.skybox.SetFloat("_Blend", i / time);
            yield return null;
        }
        RenderSettings.skybox.SetTexture("_Texture1", b);
    }

    private IEnumerator LerpLight(Gradient lightGradient, float time) 
    {
        for (float i = 0; i < time; i += Time.deltaTime)
        {
            globalLight.color = lightGradient.Evaluate(i / time);
            RenderSettings.fogColor = globalLight.color;
            yield return null;
        }
    }

    private void OnApplicationQuit()
    {
        RenderSettings.skybox.SetTexture("_Texture1", FirstTexture);
        RenderSettings.skybox.SetTexture("_Texture2", SecondTexture);
        RenderSettings.skybox.SetFloat("_Blend", 0);
        globalLight.color = DefaultLightColor;
    }
}
