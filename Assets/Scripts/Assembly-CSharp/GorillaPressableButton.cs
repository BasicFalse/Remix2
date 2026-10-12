using UnityEngine;
using UnityEngine.UI;

public class GorillaPressableButton : RemixScript
{
	public Material pressedMaterial;

	public Material unpressedMaterial;

	public MeshRenderer buttonRenderer;

	public bool ChangeMaterial = true;

	public bool isOn;

	public float debounceTime = 0.25f;

	public float touchTime;

	public bool testPress;

	[TextArea]
	public string offText;

	[TextArea]
	public string onText;

	public Text myText;

	public override void StartExt()
	{
	}

	private void Update()
	{
		if (testPress)
		{
			testPress = false;
			ButtonActivation();
		}
	}

	public void TriggerFromOther(Collider collider){
		MainButtoning(collider);
	}

	private void OnTriggerEnter(Collider collider)
	{
		MainButtoning(collider);
	}

	private void MainButtoning(Collider collider)
    {
        if (!base.enabled || !(touchTime + debounceTime < Time.time))
		{
			return;
		}
		touchTime = Time.time;
		Debug.Log("collision detected" + collider, collider);
		if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null)
		{
			GorillaTriggerColliderHandIndicator component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();
			if (component != null)
			{
				Debug.Log("buttan press");
				ButtonActivation();
				PlayerBase.i.GorillaVibration.StartVibration(component.isLeftHand, GorillaVibration.tapHapticStrength / 2f, GorillaVibration.tapHapticDuration);
			}
		}
    }

	public virtual void UpdateColor()
	{
		if (ChangeMaterial)
		{
			if (isOn)
			{
				Material[] mats = buttonRenderer.materials;
				mats[0] = pressedMaterial;
				buttonRenderer.materials = mats;
				if(myText){
				myText.text = onText;}
			}
			else
			{
				Material[] mats = buttonRenderer.materials;
				mats[0] = unpressedMaterial;
				buttonRenderer.materials = mats;
				if(myText){
				myText.text = offText;}
			}
		}
	}

	public virtual void ButtonActivation()
	{
	}
}
