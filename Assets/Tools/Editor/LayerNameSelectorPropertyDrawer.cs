using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

//Original by DYLAN ENGELMAN http://jupiterlighthousestudio.com/custom-inspectors-unity/
//Altered by Brecht Lecluyse http://www.brechtos.com
[CustomPropertyDrawer(typeof(LayerNameSelectorAttribute))]
public class LayerNameSelectorPropertyDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.String)
        {
            EditorGUI.BeginProperty(position, label, property);

            var attrib = this.attribute as LayerNameSelectorAttribute;

            if (attrib.UseDefaultTagFieldDrawer)
            {
                property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
            }
            else
            {
                //generate the taglist + custom tags
                List<string> layerList = new List<string>();

                string propertyString = property.stringValue;
                int index = -1; 

                layerList.Add($"[EMPTY]");
                layerList.AddRange(UnityEditorInternal.InternalEditorUtility.layers);

                var nameArray = new string[layerList.Count];
                layerList.CopyTo(nameArray);

                //check if there is an entry that matches the entry and get the index
                //we skip index 0 as that is a special custom case
                for (int i = 1; i < nameArray.Length; i++) nameArray[i] = $"{i - 1}: {nameArray[i]}";

                if (propertyString == "")
                {
                    //The tag is empty
                    index = 0; //first index is the special <notag> entry
                }
                else
                {
                    //check if there is an entry that matches the entry and get the index
                    //we skip index 0 as that is a special custom case
                    for (int i = 1; i < layerList.Count; i++)
                    {
                        if (layerList[i] == propertyString)
                        {
                            index = i;
                            break;
                        }
                    }
                }

                //Draw the popup box with the current selected index
                index = EditorGUI.Popup(position, label.text, index, nameArray);

                //Adjust the actual string value of the property based on the selection
                if (index >= 1)
                {
                    property.stringValue = layerList[index];
                }
                else
                {
                    property.stringValue = "";
                }
            }

            EditorGUI.EndProperty();
        }
        else
        {
            EditorGUI.PropertyField(position, property, label);
        }
    }
}