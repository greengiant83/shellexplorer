using UnityEngine;
using System.Collections;
using SocketIO;
using UnityEngine.UI;
using System.Collections.Generic;

public class TumblrDataProvider : MonoBehaviour
{
    #region -- Properties --
    public GameObject TagVisual;
    public Material LineMaterial;
    public Transform SpawnRegion;

    SocketIOComponent socket;
    Dictionary<string, TumblrTag> tags = new Dictionary<string, TumblrTag>();
    #endregion

    #region -- Public Methods --
    public void NewSearch(string text, bool createVisual)
    {
        var statusLabel = GameObject.Find("Status Label").GetComponent<Text>();
        statusLabel.text = "Searching for: " + text;

        var queryPayload = new Dictionary<string, string>()
        {
            { "text", text }
        };

        socket.Emit("search", new JSONObject(queryPayload));

        if(createVisual) createNewTagVisual(text, null);
    }

    public void ClearTags()
    {
        foreach (var tag in tags) Destroy(tag.Value.gameObject);
        tags.Clear();
    }
    #endregion

    #region -- Internal Methods --
    void Start () 
    {
        socket = GameObject.FindObjectOfType<SocketIOComponent>();
        socket.On("newTag", onNewTag);
	}

    void onNewTag(SocketIOEvent e)
    {
        createNewTagVisual(e.data["name"].str, e.data["parentTag"].str);
    }
	
    TumblrTag createNewTagVisual(string name, string parentName)
    {
        if (tags.Count >= 500)
        {
            print("Ignoring new tags because tagCount exceeded.");
            return null;
        }

        bool hasParent = parentName != null && tags.ContainsKey(parentName);


        GameObject tagObject;
        TumblrTag tumblrTagComponent;

        if (tags.ContainsKey(name))
        {
            //This tag already exists lets just look it up
            tumblrTagComponent = tags[name];
            tagObject = tumblrTagComponent.gameObject;
        }
        else
        {
            //This is indeed a new tag
            tagObject = Instantiate<GameObject>(TagVisual);
            tagObject.name = "Tag: " + name;
            tumblrTagComponent = tagObject.GetComponent<TumblrTag>();
            tumblrTagComponent.TagName = name;
            tags.Add(name, tumblrTagComponent);

            //Position it in the world
            Vector3 centerPosition = hasParent ? tags[parentName].transform.position : SpawnRegion.transform.position;
            Vector3 offset = hasParent ? (Vector3)Random.insideUnitCircle * 0.5f * SpawnRegion.transform.localScale.x : Vector3.zero;
            tagObject.transform.position = centerPosition + offset;
        }

        if(hasParent)
        {
            //Add link line to parent
            var parentVisual = tags[parentName];
            var linkLine = tagObject.AddComponent<LinkLine>();
            linkLine.LinkedObject = parentVisual.gameObject.transform;
            linkLine.LineThickness = 0.005f;
            linkLine.Material = LineMaterial;
        }

        return tumblrTagComponent;
    }
    #endregion
}
