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

    void Start()
    {
        hints = new Book();
        hints.addPage(new Page().setText("V zadní místnosti se nachází klíè na odemèení dveøí"));
        hints.addPage(new Page().setText("V pravém zadním rohu se nachází klíè k truhle"));
        hints.addPage(new Page().setText("Klíè z truhly se dá použít na pravou místnost"));
        hints.addPage(new Page().setText("V zadním rohu místnosti se za senem nachází klíè od poslední místnosti"));
        hints.addPage(new Page().setText("Když se vejde do místnosti tak vpravo je poslední klíè"));

        controller = player.GetComponent<PlayerController>();
    }

    void Update()
    {
        float scrollDelta = Input.mouseScrollDelta.y;

        if (controller.heldObject == model)
        {
            if (scrollDelta != 0)
            {
                if (!hasFlipped)
                {
                    if (scrollDelta > 0)
                    {
                        hints.nextPage();
                    }
                    else if (scrollDelta < 0)
                    {
                        hints.previousPage();
                    }

                    hasFlipped = true;
                }
            }
            else
            {
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