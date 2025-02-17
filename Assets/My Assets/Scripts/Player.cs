﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private GameManager gm;
    public int unitPoints = 10;
    public GameObject selection;


    public UiManager uiManager;

    // Start is called before the first frame update
    private void Awake()
    {
        unitPoints = 10;
    }

    void Start()
    {
        gm = GameObject.Find("Game Managers and debug").GetComponent<GameManager>();
        uiManager = GameObject.Find("Game Managers and debug").GetComponent<UiManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Input.inputString);

        if (Input.anyKeyDown)
        {
            PlayerInputCheck(Input.inputString);
        }
    }

    void PlayerInputCheck(String playerInput)
    {
        //Input categorised by gamephase

        //Mouse Input
        if (Input.GetMouseButtonDown(0))
        {
            if (gm.gamePhase == GameManager.Phase.SpawningPlayerUnits)
            {
                // mask =  LayerMask.NameToLayer("Player Unit Spawns");
                if (SelectionRaycast())
                {
                    GameObject unit = gm.selectedUnitToPlace;
                    // in selectionraycast bool select the unit you want to spawn. amd put it as parameter for placeunit
                    PlaceUnit(unit, selection.transform.position, unit.GetComponent<Soldier>().unitCost);
                    selection.GetComponent<GridSpace>().spaceMovable = false;
                    Debug.Log("placed unit");
                }
                else
                {
                    Debug.Log("Didnt place unit");
                }
            }

            if (gm.gamePhase == GameManager.Phase.BattlePlayer)
            {
                if (SelectionRaycast())
                {
                    Debug.Log("Select unit action");
                }
            }

            if (gm.gamePhase == GameManager.Phase.SelectPlayerUnitAction)
            {
                if (SelectionRaycast())
                {
                    Debug.Log("it dit it till here lol");
                }
            }
        }

        //keyboard input actions (specific to upper and lower cased letters)
        if (playerInput == "h")
        {
            Debug.Log("H works");
        }

        if (playerInput == "r")
        {
            Debug.Log("R works");
        }
    }

    public bool SelectionRaycast()
    {
        // selection categorised by gamephase
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            selection = hit.collider.gameObject;
        }

        if (gm.gamePhase == GameManager.Phase.SelectingPlayerUnit)
        {
            if (selection.layer == LayerMask.NameToLayer("Unit Selection"))
            {
                //check if raycast is hitting a player unit

                return true;
            }
        }


        if (gm.gamePhase == GameManager.Phase.SpawningPlayerUnits)
        {
            if (selection.layer == LayerMask.NameToLayer("Player Unit Spawns"))
            {
                // spawn unit on location 
                Debug.Log("just before true spawn unit");
                Debug.Log("layer 8 hit");

                return true;
            }
        }

        if (gm.gamePhase == GameManager.Phase.BattlePlayer)
        {
            if (selection.layer == LayerMask.NameToLayer("Player Unit")) ;
            {
                Soldier instance = selection.GetComponent<Soldier>();

                for (int i = 0; i < gm.redTeam.Count; i++)
                {
                    Soldier listUnit = gm.redTeam[i].GetComponent<Soldier>();

                    // this happens when you select a different unit while other unit is selected
                    if (listUnit.unitState == Soldier.unitStatus.Selected)
                    {
                        listUnit.unitState = Soldier.unitStatus.Active;
                    }
                }

                if (instance.unitState != Soldier.unitStatus.Inactive)
                {
                    instance.Select();
                    gm.selectedActiveUnit = selection;
                }
                else
                {
                    uiManager.UpdateStatus("this unit has no more actions");
                }
            }
        }

        // only do this one if you want it to return true otherwise find a other way
        // ummmm make a way so you can use this for a lot of unit actions now im limited to a few
        if (gm.gamePhase == GameManager.Phase.SelectPlayerUnitAction)
        {
            if (selection.layer == LayerMask.NameToLayer("Player Unit"))
            {
                for (int i = 0; i < gm.redTeam.Count; i++)
                {
                    Soldier listUnit = gm.redTeam[i].GetComponent<Soldier>();

                    // this happens when you select a different unit while other unit is selected
                    if (listUnit.unitState == Soldier.unitStatus.Selected)
                    {
                        listUnit.unitState = Soldier.unitStatus.Active;
                    }
                }

                selection.GetComponent<Soldier>().Select();
                gm.selectedActiveUnit = selection;
                return false;
            }

            if (selection.layer == LayerMask.NameToLayer("Player Unit Spawns"))
            {
                return true;
            }
        }

        //Debug.DrawRay(ray.origin, ray.direction, Color.red,20 );
        //Debug.Log("raycast void end");
        Debug.Log("just before false");
        return false;
    }

    void PlaceUnit(GameObject unit, Vector3 spawnPos, int unitCost)
    {
        //Soldier unitScript = unit.GetComponent;
        if (unitPoints > 0)
        {
            GameObject unitInstance = Instantiate(unit, new Vector3(spawnPos.x, spawnPos.y + 0.5f, spawnPos.z), Quaternion.identity);
            ;
            Soldier soldierInstance = unitInstance.GetComponent<Soldier>();
            unitPoints = unitPoints - unitCost;
            uiManager.UpdatePoints();
            soldierInstance.ocupiedSpace = selection;
            gm.redTeam.Add(unitInstance);
            // - 1 because count wil return 1 too much
            soldierInstance.unitId = gm.redTeam.Count - 1;
        }

        Debug.Log("Placing unit");
    }
}
