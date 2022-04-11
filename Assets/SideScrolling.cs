using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SideScrolling : MonoBehaviour
{
    public float ScrollingSpeed = 0.1f;
    public bool Started = false;
    public bool Paused = false;
    public bool Finished = false;
    public GameObject SectionPrefab;

    public GameObject Previous;
    public GameObject Current;
    public GameObject Next;

    public int Score { get; private set; }


    void Start()
    {
        Score = 0;
        var gos = GameObject.FindGameObjectsWithTag("Section");
        foreach (var go in gos) InitPillars(go, !Started);
    }

    void Update()
    {
        if (Paused || Finished) return;

        var p = Current.transform.position.x;
        var newP = new Vector3(p - Time.deltaTime * ScrollingSpeed, 0, 0);
        Current.transform.position = newP;
        Previous.transform.position = new Vector3(newP.x - 7.5f, 0, 0);
        Next.transform.position = new Vector3(newP.x + 7.5f, 0, 0);

        if (Started && CrossedPillar(Current, p, newP.x))
            Score += 1;

        if (Current.transform.position.x < -3.75)
        {
            Destroy(Previous);
            Previous = Current;
            Current = Next;
            Next = Instantiate(SectionPrefab, new Vector3(Current.transform.position.x + 7.5f, 0, 0), Quaternion.identity);
            Next.transform.parent = transform;
            InitPillars(Next, !Started);
        }
    }

    void InitPillars(GameObject go, bool disable)
    {
        var lTr = go.transform.GetChild(0);
        var mTr = go.transform.GetChild(1);
        var rTr = go.transform.GetChild(2);

        if (disable)
        {
            lTr.gameObject.SetActive(false);
            mTr.gameObject.SetActive(false);
            rTr.gameObject.SetActive(false);
        }
        else
        {
            lTr.position = new Vector3(lTr.position.x, lTr.position.y + Random.Range(-2f, 2f), lTr.position.z);
            mTr.position = new Vector3(mTr.position.x, mTr.position.y + Random.Range(-2f, 2f), mTr.position.z);
            rTr.position = new Vector3(rTr.position.x, rTr.position.y + Random.Range(-2f, 2f), rTr.position.z);
        }
    }

    bool CrossedPillar(GameObject go, float lastX, float currentX) {
        // as we're moving the world and not the player, any of these pillars 
        // just needs to cross the 0 boundary for this to pass
        var lTr = go.transform.GetChild(0);
        var mTr = go.transform.GetChild(1);
        var rTr = go.transform.GetChild(2);

        var diff = Mathf.Abs(currentX - lastX);
        if (lTr.gameObject.activeInHierarchy && lTr.position.x < 0 && lTr.position.x + diff > 0) return true;
        if (mTr.gameObject.activeInHierarchy && mTr.position.x < 0 && mTr.position.x + diff > 0) return true;
        if (rTr.gameObject.activeInHierarchy && rTr.position.x < 0 && rTr.position.x + diff > 0) return true;

        return false;
    }
}
