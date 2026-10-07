using System;
using UnityEngine;
using UnityEngine.UI;

namespace GorillaModManager
{
	// Token: 0x02000003 RID: 3
	public class Nametag : MonoBehaviour
	{
		
		private Text _text;
		
		public Text Text
		{
			get
			{
				return this._text;
			}
		}

		private void Awake()
		{
			this.Init();
		}

		public void Toggle(bool tog)
        {
            _text.enabled = tog;
        }


		//public void ChangeText(string name)
		//{
		//	bool flag = this._text == null;
		//	if (flag)
		//	{
		//		this.Init();
		//	}
		//	this._text.gameObject.SetActive(true); // nah
		//	this._text.text = name;
		//}


		private void Init()
		{
			this._text = base.GetComponent<Text>();
		}
	}
}