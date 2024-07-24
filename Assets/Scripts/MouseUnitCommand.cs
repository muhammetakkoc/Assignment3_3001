using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

// Allows us to select and command Units (GameObjects on the Unit collisionLayer that implement IMoveCommandable) to move to specific locations!

public class MouseUnitCommand : TargetingSystem
{
    Camera mainCamera;

    [SerializeField]
    Canvas screenspaceCanvas;

    [SerializeField]
    List<RadarMarkerUI> targetMarkerPool = new List<RadarMarkerUI>();

    [SerializeField]
    GameObject targetMarkerPrefab;

    [SerializeField]
    LayerMask selectMask;

    [SerializeField]
    LayerMask moveCommandMask;

    [SerializeField]
    WorldspaceMarker cursor;

    [SerializeField]
    AudioSource commandAudioSource;

    [SerializeField]
    PlayerUIConfig playerUIConfig;

    //clicking and dragging over to select in a box
    bool isBoxSelecting = false;

    //Holding shift for selecting multiple individual targets or queueing movement
    bool isModifierHeld = false;

    private Vector3 selectionStart;
    private Vector3 selectionEnd;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();

        UpdateRadarInfo();
    }

    void HandleInput()
    {
        isModifierHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        Vector2 mousePos = Input.mousePosition;
        Ray pointerRay = mainCamera.ScreenPointToRay(mousePos);
        RaycastHit hitInfo;

        string actionName = "";
        Unit hoveringOver = null;
        bool validMoveSpot = false;
        cursor.gameObject.SetActive(false);


        RaycastHit groundHit;
        validMoveSpot = Physics.Raycast(pointerRay.origin, pointerRay.direction, out groundHit, mainCamera.farClipPlane, moveCommandMask);


        /////////////////////////////////
        // Unit Command
        /////////////////////////////////
        if (GetSelectedTargets().Count > 0)
        {
            // Check under cursor for valid move command options 
            if (validMoveSpot && !isBoxSelecting)
            {
                cursor.gameObject.SetActive(true);
                cursor.transform.position = groundHit.point + Vector3.up * 0.5f;
                cursor.marker.icon.sprite = playerUIConfig.moveCommandSprite;
                actionName = "Move";
                cursor.marker.text.text = actionName;
            }

            if (Input.GetMouseButtonDown(1) && validMoveSpot) // RMB -- move or attack command
            {
                //Calculate average position for formation movement
                Vector3 avgPosition = Vector3.zero;
                foreach (Unit selected in currentTargets)
                {
                    avgPosition += selected.transform.position;
                }
                avgPosition /= currentTargets.Count;

                //Send command to one or more units, possibly to maintain whatever formation they were in already
                foreach (Unit selected in currentTargets)
                {
                    IMoveCommandable locomotionControl = selected.GetComponent<IMoveCommandable>();

                    if (locomotionControl != null)
                    {
                        Vector3 target = groundHit.point + Vector3.up * 0.1f;
                        Vector3 relativePos = selected.transform.position - avgPosition;
                        Vector3 formationPosition = target;

                        if (relativePos.sqrMagnitude > 0.1f)
                        {
                            formationPosition += (relativePos.normalized) * locomotionControl.GetPositionTolerance();
                        }

                        SendMoveCommand(selected, locomotionControl, formationPosition, isModifierHeld);
                    }
                }
            }
        }



        /////////////////////////////////
        // Unit Selection
        /////////////////////////////////

        //Check under cursor for target
        if (Physics.SphereCast(pointerRay.origin, 4, pointerRay.direction, out hitInfo, mainCamera.farClipPlane, selectMask))
        {
            hoveringOver = hitInfo.collider.GetComponent<Unit>();
            if (hoveringOver == null)
            {
                hoveringOver = hitInfo.collider.GetComponentInParent<Unit>();
            }
            if (hoveringOver != null)
            {
                if(!isBoxSelecting)
                {
                    cursor.gameObject.SetActive(true);
                    cursor.transform.position = hitInfo.point + Vector3.up * 0.5f;
                    cursor.marker.icon.sprite = playerUIConfig.canSelectSprite;
                    actionName = "Select";
                    cursor.marker.text.text = actionName;
                }
            }
        }

        if (Input.GetMouseButtonDown(0)) // LMB -- select or deselect if not hovering anything
        {
            selectionStart = groundHit.point;
            selectionEnd = selectionStart;
            isBoxSelecting = true;
        }

        Vector3 selectSize = Vector3.zero;
        Vector3 selectVec = selectionEnd - selectionStart;

        if (isBoxSelecting)
        {
            cursor.gameObject.SetActive(true);
            actionName = "Select";
            cursor.marker.icon.sprite = null;
            cursor.marker.text.text = actionName;

            selectionEnd = groundHit.point;

            selectSize = new Vector3(Mathf.Abs(selectVec.x), Mathf.Abs(selectVec.y), Mathf.Abs(selectVec.z));
            Vector3 midpoint = selectionStart + selectVec * 0.5f;
            cursor.transform.position = midpoint + Vector3.up * -0.5f;
            cursor.transform.localScale = (selectSize) + new Vector3(0, 2, 0);

        }
        else
        {
            cursor.transform.localScale = new Vector3(6f, 6f, 6f);
        }

        if (Input.GetMouseButtonUp(0))
        {
            // selectionEnd = hitInfo.point;
            bool actionSuccess = false;

            if (selectSize.sqrMagnitude > 2 && isBoxSelecting)
            {

                DeselectTargets();
                Collider[] colliders = Physics.OverlapBox(selectionStart + selectVec * 0.5f, selectSize * 0.5f);
                foreach (Collider collider in colliders)
                {
                    Unit unit = collider.GetComponent<Unit>();
                    if (unit != null)
                    {
                        AddSelection(unit);
                    }
                }
                actionSuccess = true;
            } else
            {
                //Individual Select
                if(isModifierHeld)
                {
                    //Add to existing selection
                    actionSuccess = AddSelection(hoveringOver);
                } else
                {
                    //Change selection
                    actionSuccess = SelectTarget(hoveringOver);
                }
            }

            if (actionSuccess)
            {
                commandAudioSource.PlayOneShot(playerUIConfig.selectTargetSound);
            }
            isBoxSelecting = false;
        }
        ///////////////


        
        ///////////////
    }

    private bool QueueCommand(IMoveCommandable locomotionControl, Vector3 position)
    {
        //Holding shift means to queue a waypoint
        if (isModifierHeld)
        {
            return locomotionControl.RequestAddWaypoint(position);
        }
        else
        {
            return locomotionControl.RequestMoveTo(position);
        }
    }

    /// <summary>
    /// Pathfind to the target point
    /// </summary>
    /// <param name="locomotionControl"></param>
    /// <param name="position"></param>
    private void SendMoveCommand(Unit selected, IMoveCommandable locomotionControl, Vector3 position, bool shouldQueue)
    {
        bool commandSuccess;
        NavMeshPath path = new NavMeshPath();
        Vector3 startPosition;

        if(shouldQueue)
        {
            startPosition = locomotionControl.GetFinalTargetLocation();
        } else
        {
            startPosition = selected.transform.position;
        }

        bool pathfindSuccess = NavMesh.CalculatePath(startPosition, position, NavMesh.AllAreas, path);
        commandSuccess = pathfindSuccess;

        if(!pathfindSuccess)
        {
            Debug.Log("Pathfinding failed: " + path.status.ToString());
        }

        if (path.status != NavMeshPathStatus.PathInvalid)
        {
            if(shouldQueue)
            {
                foreach(Vector3 waypoint in path.corners)
                {
                    commandSuccess &= locomotionControl.RequestAddWaypoint(waypoint);
                }

            } else
            {
                commandSuccess &= locomotionControl.RequestSetWaypoints(path.corners);
            }
        } else
        {
            //Even if the pathfinding was not successful, send units toward the desired direction
            commandSuccess &= QueueCommand(locomotionControl, position);
        }

        if (commandSuccess)
        {
            locomotionControl.AcknowledgeMoveCommand();
            commandAudioSource.PlayOneShot(playerUIConfig.acknowledgeCommandSound);
        }
        else
        {
            Debug.DrawLine(position - new Vector3(2, 0, 2), position + new Vector3(2, 0, 2), Color.red, 1.0f, false);
            Debug.DrawLine(position - new Vector3(-2, 0, 2), position + new Vector3(-2, 0, 2), Color.red, 1.0f, false);
            commandAudioSource.PlayOneShot(playerUIConfig.disallowedActionSound);
        }
    }


    /////////////////////////////////

    /// <summary>
    /// Display HUD information
    /// </summary>
    void UpdateRadarInfo()
    {
        int targetID = 0;
        foreach (Unit unit in targetAcquirer.GetCopyOfContactsList())
        {
            if (targetID >= targetAcquirer.Contacts.Count) break;
            while (targetID >= targetMarkerPool.Count)
            {
                targetMarkerPool.Add(Instantiate(targetMarkerPrefab, screenspaceCanvas.transform).GetComponent<RadarMarkerUI>());
            }

            RadarMarkerUI marker = targetMarkerPool[targetID];

            if (unit != null)
            {
                Vector3 contactPos = unit.transform.position;
                Bounds bounds = unit.GetComponent<Collider>().bounds;
                Vector3 screenPos;

                // float range = Vector3.Distance(unit.transform.position, targetAcquirer.transform.position);


                if (currentTargets.Contains(unit))
                {
                    screenPos = mainCamera.WorldToScreenPoint(contactPos);
                    marker.TargetingState = ContactTargetingState.Selected;
                    Vector3 max = mainCamera.WorldToScreenPoint(bounds.max);
                    float estimateX = Mathf.Abs(max.x - screenPos.x) * 2;
                    float estimateY = Mathf.Abs(max.y - screenPos.y) * 2;
                    float size = Mathf.Max(estimateX, estimateY);
                    float minSize = 16;
                    size = Mathf.Max(size, minSize);

                    //float size = (estimatedSize * 2) + minSize;
                    marker.icon.rectTransform.sizeDelta = new Vector2(size, size);
                    Vector2 iconPos = marker.icon.rectTransform.position;

                    if(currentTargets.Count < 2)
                    {
                        marker.text.text = unit.GetUnitName() + " " + unit.name + "\nHP:" + unit.GetHealth();
                    } else
                    {
                        marker.text.text = "\nHP:" + unit.GetHealth();
                    }
                }
                else
                {
                    contactPos.y = bounds.max.y;
                    screenPos = mainCamera.WorldToScreenPoint(contactPos);
                    marker.TargetingState = ContactTargetingState.Contact;
                    marker.icon.rectTransform.sizeDelta = new Vector2(8, 8);
                    marker.text.text = "";
                }

                marker.text.rectTransform.anchoredPosition = new Vector2(0, marker.icon.rectTransform.rect.height / 2);

                if (screenPos.z < 0)
                {
                    marker.transform.localScale = Vector3.zero;
                }
                else
                {
                    IFF_Tag iffResponse = unit.IFF_GetResponse(Team.Blue);

                    screenPos.z = 0;
                    marker.transform.position = screenPos;
                    marker.transform.localScale = Vector3.one;

                    marker.UpdateColor(iffResponse);
                }
                targetID++;
            }
            else
            {
                targetAcquirer.DeregisterContact(unit);
            }
        }

        //Remaining unused target Markers go bye bye
        while (targetID < targetMarkerPool.Count)
        {
            targetMarkerPool[targetID].transform.localScale = Vector3.zero;
            targetID++;
        }
    }
}
