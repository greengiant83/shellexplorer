using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SocketIO;
using System.Collections.Generic;
using System.Linq;

public class TumblrMenu : MonoBehaviour 
{
    SocketIOComponent socket;

	void Start () 
    {
	
	}
	
	void Update () 
    {
        socket = GameObject.FindObjectOfType<SocketIOComponent>();
	}

    public void OnClearButtonClick()
    {
        GameObject.FindObjectOfType<TumblrDataProvider>().ClearTags();
    }

    public void OnSearchButtonClick()
    {
        var queryInput = GameObject.Find("Query Input").GetComponent<InputField>();
        GameObject.FindObjectOfType<TumblrDataProvider>().NewSearch(queryInput.text, true);
    }

    public void OnExpandButtonClick()
    {
        throw new System.NotImplementedException();
        //var selectedTags = SelectionManager.Instance.GetSelection().Select(i => i.gameObject.GetComponent<TumblrTag>()).Where(i => i != null);
        //foreach(var tag in selectedTags)
        //{
        //    GameObject.FindObjectOfType<TumblrDataProvider>().NewSearch(tag.TagName, true);
        //}
    }
}
