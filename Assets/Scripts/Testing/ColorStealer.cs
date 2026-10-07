using UnityEngine;

public class ColorStealer : MonoBehaviour
{
    public Material materialToSet;
    private Material mat;
    public Light lightToSteal;
    public bool instance;
    public Renderer rend;
    void Start()
    {
        if (instance)
        {
            mat = Instantiate(materialToSet);
            rend.material = mat;
        }
        else
        {
            rend.material = materialToSet;
        }
    }
    void FixedUpdate()
    {
        if (instance)
        {
            mat.color = lightToSteal.color;
        }
        else
        {
            materialToSet.color = lightToSteal.color;
        }
    }
}
