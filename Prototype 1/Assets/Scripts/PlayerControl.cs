﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerControl : MonoBehaviour
{
    [Header("Character references")]
    public Player m_childOne;
    public Player m_childTwo;

    public PlayerMovement m_movement;
    public bool m_waterLeading = true;
    public GameObject m_playerFrontRay;
    public bool m_menuActivated = false;
    private void Start()
    {
        Reset();
    }

    private void Reset()
    {
        //Depending on who is leading when script starts up, set character as lead
        m_waterLeading = LevelLoader.GetInstance().GetLvlWaterLeading();

        //Set starting position
        transform.position = LevelLoader.GetInstance().GetLvlStartingPos().position;
        transform.rotation = LevelLoader.GetInstance().GetLvlStartingPos().rotation;

        m_childOne.GetComponent<Player>().Reset();
        m_childTwo.GetComponent<Player>().Reset();

        if (m_waterLeading)
        {
            m_childOne.gameObject.SetActive(true);
            m_childTwo.gameObject.SetActive(false);
        }
        else
        {
            m_childOne.gameObject.SetActive(false);
            m_childTwo.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !m_menuActivated)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground")
                    || hit.collider.gameObject.layer == LayerMask.NameToLayer("VineBlock"))
                {
                    Debug.Log(LayerMask.LayerToName(hit.collider.transform.gameObject.layer));

                    m_movement.MovePlayer(hit);
                }

                if(hit.collider != null)
                {
                    if (hit.transform.gameObject.tag == "Fire" && m_waterLeading)// && hit.transform.gameObject.transform.forward == transform.position)
                    {
                        //Put out fire
                        m_movement.MoveToTarget(hit.transform.gameObject);
                    }

                    if (hit.transform.gameObject.tag == "WaterBlock" && m_waterLeading)// && hit.transform.gameObject.transform.forward == transform.position)
                    {
                        //Create Ice
                        m_movement.MoveToTarget(hit.transform.gameObject);
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1) && !m_menuActivated)
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
           
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 dir = (hit.transform.position - transform.position);

                // the player is within a radius of 3 units to this game object

                if ((hit.transform.position - transform.position).sqrMagnitude < 3*3)
                {
                    if (hit.transform.gameObject.tag == "Fire" && m_waterLeading)
                    {
                        //Put out fire
                        m_movement.Rotate(hit.transform.gameObject);
                        m_childOne.SpellOne(hit.transform.gameObject);
                    }

                    if (hit.transform.gameObject.tag == "WaterBlock" && m_waterLeading)
                    {
                        //Create Ice
                        //m_movement.MoveToTarget(hit.transform.gameObject);
                        m_childOne.SpellTwo(hit.transform.gameObject);
                    }
                }

                RaycastHit charaHit;
                Ray charaRay = new Ray(transform.position, Vector3.down * 2);
                if (Physics.Raycast(charaRay, out charaHit))
                {
                    //Going down to vine ground
                    if (charaHit.transform.gameObject.tag == "VineBlock" 
                        && hit.transform.gameObject.tag == "VineGround"
                        && !m_waterLeading)
                    {
                        if (hit.transform.gameObject == charaHit.transform.GetChild(6).gameObject)
                        {
                            m_movement.Rotate(charaHit.transform.gameObject);
                            m_childTwo.SpellOne(charaHit.transform.gameObject);
                        }
                    }

                    //Going up to vine block
                    if (charaHit.transform.gameObject.tag == "VineGround"
                        && hit.transform.gameObject.tag == "VineBlock"
                        && !m_waterLeading)
                    {
                        if (hit.transform.gameObject.name == charaHit.transform.parent.name)
                        {
                            m_movement.Rotate(hit.transform.gameObject);
                            m_childTwo.SpellTwo(hit.transform.gameObject);
                        }
                    }
                }
            }
        }
    }

    //Checks who is leading and switches them with the backup chara
    //Handles animation
    public void SwitchCharaPos()
    {
        if (m_waterLeading)
        {
            m_childOne.gameObject.SetActive(false);
            m_childTwo.gameObject.SetActive(true);
            m_childTwo.PlaySwitchAnim();

            //Play animations
            m_waterLeading = false;
        }
        else
        {
            m_childOne.gameObject.SetActive(true);
            m_childTwo.gameObject.SetActive(false);

            m_childOne.PlaySwitchAnim();

            //Play animations
            m_waterLeading = true;
        }
        Debug.Log("Child leading: " + ((m_waterLeading) ? m_childOne.tag : m_childTwo.tag));
    }

    public void SetIsLeading(bool _leading)
    {
        m_waterLeading = _leading;
    }

    public bool GetIsLeading()
    {
        return m_waterLeading;
    }
    
}
