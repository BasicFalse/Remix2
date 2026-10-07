using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GorillaLevelScreen : MonoBehaviour
{
	public static readonly string mainText = "WELCOME TO REMIX!\n\n\nYOU CAN EITHER JOIN A PUBLIC LOBBY OR GO TO THE TERMINAL TO JOIN A PRIVATE LOBBY / ADJUST YOUR SETTINGS.";

	public static readonly string leaveroomText = "GOING OVER HERE WILL MEAN YOU LEAVE THE CURRENT LOBBY AND JOIN A NEW ONE.";
	public static readonly string thisroomText = "THIS IS THE PLAY AREA FOR THE CURRENT ROOM.";

	public Material goodMaterial;

	public Material badMaterial;

	public Text myText;

	public MiscGrabs mg;

	private void Start()
	{
		StartCoroutine(ConnectScreen());
	}

	private void Awake()
	{
		myText.text = mainText;
	}

	public void UpdateText(string newText, bool setToGoodMaterial)
	{
		Material[] materials = new Material[1];
		myText.text = newText;
		if(GetComponent<MeshRenderer>()){
		materials = GetComponent<MeshRenderer>().materials;}
		materials[1] = setToGoodMaterial ? goodMaterial : badMaterial;
		GetComponent<MeshRenderer>().materials = materials;
	}

	public void ReveluateStatus(bool thislobby)
	{
		myText.text = thislobby ? thisroomText : leaveroomText;
		Material[] materials = GetComponent<MeshRenderer>().materials;
		materials[1] = thislobby ? goodMaterial : badMaterial;
		GetComponent<MeshRenderer>().materials = materials;
	}

	private IEnumerator ConnectScreen()
	{
		do
		{
			yield return new WaitForSeconds(1f);
			MiscGrabs.grab.LevelScreens.Add(this);
		}
		while (!MiscGrabs.grab.LevelScreens.Contains(this));
	}
}
