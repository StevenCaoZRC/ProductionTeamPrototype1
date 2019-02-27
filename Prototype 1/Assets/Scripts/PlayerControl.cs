﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerControl : MonoBehaviour
{
    [Header("Character references")]
    public Player m_childOne;
    public Player m_childTwo;
    public Animator m_doubleCharaAnimator;
    public PlayerMovement m_movement;
    public LayerMask m_layerMask;

    [Header("Target Positions For Characters to stay at (Idle)")]
    public GameObject m_selectedPos;
    public GameObject[] m_backupPos;

    [Header("Private //just for checking")]
    [SerializeField]
    private bool m_childOneLeading = true;
    private bool Climbed = false;
    
    private void Start()
    {
        //Depending on who is leading when script starts up, set character as lead
        if (m_childOneLeading)
            SwitchPos(m_childTwo, m_childOne);
        else
            SwitchPos(m_childOne, m_childTwo);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(LayerMask.LayerToName(hit.collider.transform.gameObject.layer));
                if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground")
                    || hit.collider.gameObject.layer == LayerMask.NameToLayer("VineBlock"))
                {
                    m_movement.MovePlayer(hit);
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
           
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.collider.transform.gameObject.name);

                if (hit.transform.gameObject.tag == "Fire" && m_childOneLeading)// && hit.transform.gameObject.transform.forward == transform.position)
                {
                    //Put out fire
                    m_movement.MoveToTarget(hit.transform.gameObject);
                    
                    m_childOne.SpellOne(hit.transform.gameObject);

                }

                if (hit.transform.gameObject.tag == "WaterBlock" && m_childOneLeading)// && hit.transform.gameObject.transform.forward == transform.position)
                {
                    //Create Ice
                    m_movement.MoveToTarget(hit.transform.gameObject);

                    m_childOne.SpellTwo(hit.transform.gameObject);
                }

                if (hit.transform.gameObject.tag == "VineBlock" && !m_childOneLeading && !Climbed)
                {
                   
                    Debug.Log("GROW VINES");
                    m_childTwo.SpellOne(hit.transform.gameObject);
                    Climbed = true;
                    //m_movement.MoveToTarget(hit.transform.gameObject);

                }

                if (hit.transform.gameObject.tag == "VineGround" && !m_childOneLeading && Climbed)
                {
                    Debug.Log("NANI");
                    //m_movement.MoveToTarget(hit.transform.gameObject);
                    m_childTwo.SpellOne(hit.transform.gameObject);
                    Climbed = false;
                   
                }
            }
        }
        //Single Mouse Right click
        if (Input.GetMouseButtonDown(2))
        {
            SwitchCharaPos();
        }
    }

    //Checks who is leading and switches them with the backup chara
    //Handles animation
    public void SwitchCharaPos()
    {
        m_childOne.PlaySwitchAnim();
        m_childTwo.PlaySwitchAnim();

        if (m_childOneLeading)
        {
            SwitchPos(m_childOne, m_childTwo);

            //Play animations
            m_childOneLeading = false;

        }
        else
        {
            SwitchPos(m_childTwo, m_childOne);

            //Play animations
            m_childOneLeading = true;
        }
        Debug.Log("Child leading: " + ((m_childOneLeading) ? m_childOne.tag : m_childTwo.tag));
    }

    //Switches positions of two players
    private void SwitchPos(Player _p1, Player _p2)
    {
        
        //Switches backup player to the front
        _p2.transform.position = m_selectedPos.transform.position;

        //Switches leading player to the back
        _p1.transform.position = m_backupPos[Random.Range(0, 3)].transform.position;
    }


}
