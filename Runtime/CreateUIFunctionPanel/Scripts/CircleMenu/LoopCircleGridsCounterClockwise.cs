using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoopCircleGridsCounterClockwise : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform Viewport;
    public GameObject CellTemplate;
    public int CellCountPerPage;
    public Vector3 AngleStep;
    public float StartAngle;
    public float EndAngle;

    public bool enableSelectCell = true;

    private Deque<RectTransform> Cells = new Deque<RectTransform>();

    public IEnumerator<RectTransform> ForeachCell()
    {
        return Cells.GetEnumerator();
    }

    // The offset from handle position to mouse down position
    private float m_PointerStartLocalAngle = 0;
    private Vector3 m_ContentStartEulerAngle = Vector2.zero;

    private bool m_Dragging;

    private bool m_PointerDown;

    private RectTransform m_selectedCell;

    public GameObject SelectedCell
    {
        get
        {
            return m_selectedCell != null ? m_selectedCell.gameObject : null;
        }
    }

    public System.Action OnSelectChange;

    // Use this for initialization
    void Awake()
    {
        AngleStep.x = 0;
        AngleStep.y = 0;
    }
    
    public GameObject AddCell(int index)
    {
        if (CellTemplate != null)
        {
            GameObject newCell = GameObject.Instantiate(CellTemplate, Viewport);
            newCell.SetActive(true);
            RectTransform rt = newCell.GetComponent<RectTransform>();
            rt.anchoredPosition = Vector2.zero;

            //newCell.transform.GetChild(0).GetComponentInChildren<Text>().text = index.ToString();
            newCell.transform.Find("Image/Text").GetComponent<Text>().text = index.ToString();
            //
            RectTransform lastCell = Cells.Back;
            if (lastCell == null)
            {
                rt.eulerAngles = Vector3.zero;
            }
            else
            {
                rt.eulerAngles = lastCell.eulerAngles + AngleStep;
            }

            Cells.PushBack(rt);

            return newCell;
        }

        return null;
    }

    public void RemoveAllCell()
    {
        while (Cells.Count > 0)
        {
            RectTransform cell = Cells.PopBack();
            GameObject.Destroy(cell.gameObject);
        }

        m_selectedCell = null;
        if (OnSelectChange != null)
            OnSelectChange();
    }

    public void SelectCell(RectTransform cell)
    {
        IEnumerator<RectTransform> ie = Cells.GetEnumerator();
        while (ie.MoveNext())
        {
            if (cell == ie.Current)
            {
                m_selectedCell = cell;
            }
            ie.Current.SendMessage("OnSelectChange", cell == ie.Current);
        }

        StartCoroutine(AlignSelected());
    }

    private void CheckSelect()
    {
        return;

        RectTransform selected = null;

        float rangeMin = StartAngle + AngleStep.z;
        float rangeMax = StartAngle + AngleStep.z * 2;
        IEnumerator<RectTransform> ie = Cells.GetEnumerator();
        while (ie.MoveNext())
        {
            float angle = ClampAngle(ie.Current.eulerAngles.z + AngleStep.z * 0.5f);
            if (angle > rangeMin && angle < rangeMax)
            {
                selected = ie.Current;
            }
            ie.Current.SendMessage("OnSelectChange", selected == ie.Current);
        }

        if (m_selectedCell != selected)
        {
            m_selectedCell = selected;
            if (OnSelectChange != null)
                OnSelectChange();
        }
    }

    private bool CanDrag()
    {
        return Cells.Count > 0 && Cells.Count > CellCountPerPage;
    }

    private float ClampAngle(float angle)
    {
        while (angle < StartAngle)
        {
            angle += 360.0f;
        }
        while (angle >= StartAngle + 360.0f)
        {
            angle -= 360.0f;
        }

        return angle;
    }

    private float GetVectorAngle(Vector2 vec)
    {
        Vector3 cross = Vector3.Cross(Vector3.right, vec);
        float angle = Vector2.Angle(Vector2.right, vec);
        angle = cross.z > 0 ? angle : 360 - angle;
        return angle;
    }

    private void SetOffset(Vector3 offset)
    {
        if (Cells.Count == 0)
        {
            return;
        }

        Vector3 angle = offset;

        IEnumerator<RectTransform> ie = Cells.GetEnumerator();
        while (ie.MoveNext())
        {
            ie.Current.eulerAngles = angle;
            angle += AngleStep;
        }
    }

    private bool CheckLoop()
    {
        if (Cells.Count == 0)
        {
            return false;
        }

        bool changed = false;

        float startAngle = ClampAngle(Cells.Front.eulerAngles.z);
        float endAngle = ClampAngle(Cells.Back.eulerAngles.z + AngleStep.z);
        if (startAngle < endAngle)
        {
            while (startAngle > StartAngle)
            {
                RectTransform cell = Cells.PopBack();
                startAngle -= AngleStep.z;
                cell.eulerAngles = new Vector3(0.0f, 0.0f, startAngle);
                Cells.PushFront(cell);
                changed = true;
            }
        }
        else
        {
            while (endAngle < EndAngle)
            {
                RectTransform cell = Cells.PopFront();
                cell.eulerAngles = new Vector3(0.0f, 0.0f, startAngle);
                endAngle += AngleStep.z;
                Cells.PushBack(cell);
                changed = true;
            }
        }

        if (changed)
        {
            int index = 1;
            IEnumerator<RectTransform> ie = Cells.GetEnumerator();
            while (ie.MoveNext())
            {
                ie.Current.SetSiblingIndex(index);
                index++;
            }
        }

        return changed;
    }

    private IEnumerator AlignSelected()
    {
        if (m_selectedCell != null)
        {
            float angle = ClampAngle(m_selectedCell.eulerAngles.z);
            float angle2 = StartAngle + AngleStep.z;
            float angleOffset = angle2 - angle;

            Vector3 tarAngle = Cells.Front.eulerAngles + new Vector3(0.0f, 0.0f, angleOffset);
            Vector3 startAngle = Cells.Front.eulerAngles;

            float t = 0.0f;
            float angleSpeed = AngleStep.z * 5.0f;
            float alignDuration = Mathf.Abs(angleOffset / angleSpeed);
            while (t < alignDuration)
            {
                float nt = t / alignDuration;
                Vector3 interAngle = Vector3.Lerp(startAngle, tarAngle, nt);
                SetOffset(interAngle);

                if (CheckLoop())
                {
                    angleOffset = tarAngle.z - interAngle.z;
                    startAngle = Cells.Front.eulerAngles;
                    tarAngle = Cells.Front.eulerAngles + new Vector3(0.0f, 0.0f, angleOffset);
                    t = 0.0f;
                    alignDuration = Mathf.Abs(angleOffset / angleSpeed);
                }

                yield return null;

                t += Time.deltaTime;
            }

            SetOffset(tarAngle);
            CheckLoop();
        }

        yield return null;
    }

    private IEnumerator AlignGrid()
    {
        float minLenAngle = ClampAngle(Cells.Front.eulerAngles.z);
        float minLen = Mathf.Abs(minLenAngle - StartAngle);

        IEnumerator<RectTransform> ie = Cells.GetEnumerator();
        while (ie.MoveNext())
        {
            float angle = ClampAngle(ie.Current.eulerAngles.z);
            float len = Mathf.Abs(angle - StartAngle);
            if (len < minLen)
            {
                minLen = len;
                minLenAngle = angle;
            }
        }

        if (minLen > 0)
        {
            float angleOffset = StartAngle - minLenAngle;
            if (minLen > (Mathf.Abs(AngleStep.z * 0.5f)))
            {
                angleOffset += AngleStep.z;
            }
            Vector3 tarAngle = Cells.Front.eulerAngles + new Vector3(0.0f, 0.0f, angleOffset);
            Vector3 startAngle = Cells.Front.eulerAngles;

            float t = 0.0f;
            float alignDuration = 0.2f;
            while (t < alignDuration)
            {
                float nt = t / alignDuration;
                float lt = 1.0f - (nt - 1.0f) * (nt - 1.0f);
                Vector3 interAngle = Vector3.Lerp(startAngle, tarAngle, lt);
                SetOffset(interAngle);

                yield return null;

                t += Time.deltaTime;
            }

            SetOffset(tarAngle);
        }

        CheckSelect();

        yield return null;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        m_PointerDown = true;

        StopAllCoroutines();
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (m_PointerDown)
        {
            m_PointerDown = false;

            Vector2 localCursor = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(Viewport, eventData.position, eventData.pressEventCamera, out localCursor);
            float localAngle = ClampAngle(GetVectorAngle(localCursor) - 90.0f);

            IEnumerator<RectTransform> ie = Cells.GetEnumerator();
            while (ie.MoveNext())
            {
                float angleMin = ClampAngle(ie.Current.eulerAngles.z);
                float angleMax = ClampAngle(ie.Current.eulerAngles.z + AngleStep.z);
                bool hit = false;
                if (angleMax > angleMin)
                {
                    hit = localAngle >= angleMin && localAngle < angleMax;
                }
                else
                {
                    hit = localAngle < angleMax || localAngle >= angleMin;
                }

                if (hit)
                {
                    if (m_selectedCell != ie.Current)
                    {
                        if (enableSelectCell)
                        {
                            SelectCell(ie.Current);
                        }
                        if (OnSelectChange != null)
                            OnSelectChange();
                    }

                    break;
                }
            }
        }
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        m_PointerDown = false;

        if (!CanDrag())
            return;

        StopAllCoroutines();

        Vector2 pointerStartLocalCursor = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(Viewport, eventData.position, eventData.pressEventCamera, out pointerStartLocalCursor);
        m_PointerStartLocalAngle = GetVectorAngle(pointerStartLocalCursor);
        m_ContentStartEulerAngle = Cells.Front.eulerAngles;
        m_Dragging = true;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        m_Dragging = false;

        StartCoroutine(AlignGrid());
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        m_PointerDown = false;

        if (!CanDrag())
            return;

        Vector2 localCursor;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(Viewport, eventData.position, eventData.pressEventCamera, out localCursor))
            return;

        float localAngle = GetVectorAngle(localCursor);
        var angleDelta = localAngle - m_PointerStartLocalAngle;
        Vector3 euler = m_ContentStartEulerAngle + new Vector3(0.0f, 0.0f, angleDelta);

        SetOffset(euler);

        if (CheckLoop())
        {
            m_ContentStartEulerAngle = Cells.Front.eulerAngles;
            m_PointerStartLocalAngle = localAngle;
        }
    }
}

