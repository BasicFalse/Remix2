using UnityEngine;
using UnityEngine.UI;

namespace GorillaNetworking
{
    public class WardrobeColorController : RemixScript
    {
        public WorldSpaceSlider redSlider;
        public WorldSpaceSlider greenSlider;
        public WorldSpaceSlider blueSlider;

        public bool canChangeColor = false;
        public bool canHaveColorSet = false;
        public bool initialized;
        public bool locked;

        public AudioClip sliderPop;
        public AudioSource redSource;
        public AudioSource greenSource;
        public AudioSource blueSource;

        public float rS;
        public float gS;
        public float bS;

        public Text RedTXT;
        public Text GreenTXT;
        public Text BlueTXT;

        public Renderer previewRenderer; // e.g. your avatar material

        public void Start()
        {
            canHaveColorSet = true;
            //redSlider.handleRB.maxLinearVelocity=0;
            //greenSlider.handleRB.maxLinearVelocity=0;
            //blueSlider.handleRB.maxLinearVelocity=0;
        }

        public void SetSliderValues(float RedColor, float GreenColor, float BlueColor)
        {
            if(locked) return;
            //Debug.Log("WARDROBE COLOR CONTROLLER : RECEVED COLORS AS ; " + RedColor + ", " + GreenColor + ", " + BlueColor);
            if (canHaveColorSet)
            {
                //Debug.Log("WARDROBE COLOR CONTROLLER : SETTING COLORS AS ; " + RedColor + ", " + GreenColor + ", " + BlueColor);
                redSlider.SetValue(RedColor);
                greenSlider.SetValue(GreenColor);
                blueSlider.SetValue(BlueColor);
                rS = RedColor;
                bS = BlueColor;
                gS = GreenColor;
            }
            canChangeColor = true;
            canHaveColorSet = true;
        }

        void Update()
        {
            float r = redSlider.currentValue;
            float g = greenSlider.currentValue;
            float b = blueSlider.currentValue;
            RedTXT.text = r.ToString();
            GreenTXT.text = g.ToString();
            BlueTXT.text = b.ToString();

            if (previewRenderer != null)
            {
                Color newColor = new(r / 9f, g / 9f, b / 9f);
                previewRenderer.material.color = newColor;
            }

            if (!initialized)
            {
                GorillaComputer.instance.RefreshWardrobeSliders();
                initialized = true;
            }

            if (rS != r || gS != g || bS != b)
            {
                if (canChangeColor)
                {
                    bool rid = rS != r;
                    bool gid = gS != g;
                    bool bid = bS != b;
                    rS = r; gS = g; bS = b;
                    Debug.Log("WARDROBE COLOR CONTROLLER : SETTING COLORS AS ; " + r + ", " + g + ", " + b);
                    canHaveColorSet = false;
                    locked = true;
                    GorillaComputer.instance.SetColors(rS, gS, bS);
                    locked = false;

                    if (rid)
                        redSource.PlayOneShot(sliderPop);
                    if (gid)
                        greenSource.PlayOneShot(sliderPop);
                    if (bid)
                        blueSource.PlayOneShot(sliderPop);

                }
            }
        }
    }
}