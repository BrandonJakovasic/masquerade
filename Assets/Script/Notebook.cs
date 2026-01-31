using UnityEngine;
using System.Collections;
using System;

public class Notebook : MonoBehaviour
{
    public static Notebook instance;

    public Sprite logicGridSpr_UNKNOWN;
    public Sprite logicGridSpr_ELIM_MANUAL;
    public Sprite logicGridSpr_ELIM_IMPLIED;
    public Sprite logicGridSpr_CONFIRMED;

    [SerializeField] private Vector3 enabledSpot;
    [SerializeField] private Vector3 disabledSpot;
    [SerializeField] private float time;

    private bool into = false;
    private Coroutine activeCo;

    private int currPage = 0; // front cover
    [SerializeField] private GameObject[] pageHinges;
    [SerializeField] private GameObject binding;

    public static event Action notebookOpened = () => { };
    public static event Action notebookClosed = () => { };

    private void Start()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    public void transition()
    {
        into = !into;
        if (activeCo != null)
        {
            StopCoroutine(activeCo);
        }

        if (into)
        {
            notebookOpened.Invoke();
            activeCo = StartCoroutine(slidingTransition(enabledSpot, time));
        }
        else
        {
            notebookClosed.Invoke();
            activeCo = StartCoroutine(slidingTransition(disabledSpot, time));
        }
    }


    public void turnPage(int by)
    {
        if(activeCo == null)
        {
            int temp = currPage + by;
            if(temp > 0 && temp < pageHinges.Length + 2)
            {
                if(by > 0)
                {
                    if(currPage == 0)
                        activeCo = StartCoroutine(pageTurnAnimation(temp, Quaternion.Euler(0, 0, 0), time, true));
                    else
                        activeCo = StartCoroutine(pageTurnAnimation(temp, Quaternion.Euler(0, 179.99f, 0), time, false));
                } else
                {
                    if (currPage == 1)
                        activeCo = StartCoroutine(pageTurnAnimation(temp, Quaternion.Euler(0, 0, 0), time, true));
                    else
                        activeCo = StartCoroutine(pageTurnAnimation(temp, Quaternion.Euler(0, 179.99f, 0), time, false));
                }
                Debug.Log("page turned");
            }
        }
    }



    // sliding transition
    private IEnumerator slidingTransition(Vector3 moveTo, float time)
    {
        Vector3 startPos = transform.localPosition;
        Vector3 endPos = moveTo;

        float tick = 0;
        while (tick < time)
        {
            tick += Time.deltaTime;

            float factor = Mathf.Pow(tick / time, 2);

            transform.localPosition = Vector3.Lerp(startPos, endPos, factor);

            yield return null;
        }

        activeCo = null;
        yield return null;
    }


    // sliding transition
    private IEnumerator pageTurnAnimation(int toWhichPage, Quaternion rotateTo, float time, bool withBinding)
    {
        Quaternion startRot = pageHinges[currPage].transform.localRotation;
        Quaternion endRot = rotateTo;

        float tick = 0;
        while (tick < time)
        {
            tick += Time.deltaTime;

            float factor = Mathf.Pow(tick / time, 2);

            pageHinges[currPage].transform.localRotation = Quaternion.Lerp(startRot, endRot, factor);
            Debug.Log("Turning " + pageHinges[currPage].name);
            if (withBinding)
            {
                binding.transform.localRotation = Quaternion.Lerp(startRot, endRot, factor);
            }

            yield return null;
        }

        activeCo = null;
        currPage = toWhichPage;
        yield return null;
    }
}
