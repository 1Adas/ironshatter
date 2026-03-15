using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class BookScript : MonoBehaviour
{
    public Book hints;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] GameObject player;
    [SerializeField] GameObject model;
    private PlayerController controller;
    private bool hasFlipped = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hints = new Book();
        hints.addPage(new Page().setText("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas convallis interdum nulla, et pellentesque risus vestibulum sed."));
        hints.addPage(new Page().setText("Nullam semper ut mi in pharetra. Maecenas ullamcorper odio scelerisque velit imperdiet, ut congue dui cursus."));
        hints.addPage(new Page().setText("Etiam lobortis et ex id volutpat. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae;"));

        controller = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        float scrollDelta = Input.mouseScrollDelta.y;

        if (controller.heldObject == model)
        {
            // Check if the wheel is being moved
            if (scrollDelta != 0)
            {
                // Only trigger if we haven't already processed this specific "scroll tick"
                if (!hasFlipped)
                {
                    if (scrollDelta > 0)
                    {
                        // SCROLL UP
                        hints.nextPage();
                    }
                    else if (scrollDelta < 0)
                    {
                        // SCROLL DOWN
                        hints.previousPage();
                    }

                    hasFlipped = true;
                }
            }
            else
            {
                // Reset the flag once the user stops moving the wheel
                hasFlipped = false;
            }

        }

        text.text = hints.getCurrentPage().getText();
    }
}


public class Book
{
    public List<Page> pages = new List<Page>();
    int pageIndex = 0;


    public void addPage(Page newPage)
    {
        pages.Add(newPage);
    }

    public void setPageIndex(int newPageIndex)
    {
        if (pages.Count > newPageIndex)
        {
            pageIndex = newPageIndex;
        }
        else
        {
            Debug.Log("page index out of bounds.");
        }
    }

    public Page getCurrentPage()
    {
        return pages[pageIndex];
    }

    public int getPageIndex()
    {
        return pageIndex;
    }

    public void nextPage()
    {
        if (pages.Count > pageIndex + 1)
        {
            pageIndex++;
        }
    }

    public void previousPage()
    {
        if (pageIndex - 1 >= 0)
        {
            pageIndex--;
        }
    }
}

public class Page
{
    string page = "";

    public string getText()
    {
        return page;
    }

    public Page setText(string text)
    {
        page = text;
        return this;
    }
}