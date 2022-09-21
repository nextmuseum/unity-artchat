using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ListViewComments : MonoBehaviour
{

    private List<Comment> comments = new List<Comment>();
    private List<GameObject> items = new List<GameObject>();
    private GameObject itemTemplate;


    void Start()
    {
        itemTemplate = transform.GetChild(0).gameObject;
        /*
        comments.Add(new Comment(
            "Oh ja, das ist schon sehr cut�.",
            new Vector3(7,874,124),
            new Vector3(45,45,45),
            "artworkID",
            "userID:Miriam",
            "_id",
            "23.04.2021"
            ));

        comments.Add(new Comment(
            "Das ist doch ein Fisch?!",
            new Vector3(6,812, 926),
            new Vector3(45, 45, 45),
            "artworkID",
            "userID:Michael",
            "_id",
            "24.04.2021"
            ));
        */
        LoadList();
    }

    public void LoadList()
    {
        if (comments.Count == 0)
        {
            Destroy(itemTemplate);
            return;
        }

        GameObject currentItem;
            
        for (int i = 0; i < comments.Count; i++)
        {
            currentItem = Instantiate(itemTemplate, transform);

            //ggf Button f�r Like?!
            //currentItem.GetComponent<Button>().AddEventListener(i, ItemClicked);

            currentItem.transform.Find("Text - User").GetComponent<TextMeshProUGUI>().text = comments[i].userId;
            currentItem.transform.Find("Text - Comment").GetComponent<TextMeshProUGUI>().text = comments[i].text;
            currentItem.transform.Find("Text - Date").GetComponent<TextMeshProUGUI>().text = comments[i].date;

            items.Add(currentItem);

            // TODO: Anpassen auf current user!
            if (currentItem.transform.Find("Text - User").GetComponent<TextMeshProUGUI>().text == "Miriam")
            {
                currentItem.GetComponent<Image>().color = new Color32(165, 229, 221, 225);
            }
        }

        Destroy(itemTemplate);
    }


    void ItemClicked(int itemIndex)
    {
        Debug.Log($"Item {itemIndex} clicked: {comments[itemIndex].userId}");
    }
}
