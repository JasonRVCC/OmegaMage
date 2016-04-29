using UnityEngine;
using System.Collections;

public class ElementInventoryButton : MonoBehaviour
{
	public ElementType type;

	void Awake()
	{
		char c = gameObject.name[0];
		string s = c.ToString();
		int typeNum = int.Parse(s);

		type = (ElementType)typeNum;
	}

	void OnMouseUpAsButton()
	{
		//Tell mage to add this type
		Mage.S.SelectElement(type);
	}
}