﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls whose turn it is, the actions available to the players, and other game logic
/// </summary>
public class GameController : MonoBehaviour
{
    public WaterFlowController WaterFlowController;
    public SceneLoader SceneLoader;

    public GameObject OraclePrefab;
    public Vector3 OracleSpawnPoint;

    public GameObject AttackerUI;

    public Reservoir Reservoir;

    public Text TurnCounter;
    public Text ReservoirCounter;

    public Image ScreenCover;
    public GameObject GameUI;
    public GameObject GameBoard;
    public Text TurnText;

    public int NumberOfAttacksPerTurn = 1;
    public int NumberOfOracles = 1;
    public int NumAvailableAttacks { get; set; }

    private int Turn = 0;

    public int ReservoirLimit = 10;
    public int TurnLimit = 15;

    //public Text TurnTimer;
    //private DateTime ActiveTurnTimer;
    //private DateTime EndTurnTimer;
    //private bool ActiveTurn;


    public GameState GameState = GameState.AttackerTurn;

    private List<Oracle> oracles;

    private void Awake()
    {
        this.NumAvailableAttacks = this.NumberOfAttacksPerTurn;
        Results.ReservoirLimit = ReservoirLimit;
        this.oracles = new List<Oracle>();
        TurnText.gameObject.SetActive(true);
        ScreenCover.gameObject.SetActive(false);
        ScreenCover.fillCenter = true;
    }

    private void Start()
    {
        for (int i = 0; i < this.NumberOfOracles; i++)
        {
            var newOracle = Instantiate(this.OraclePrefab, new Vector3(this.OracleSpawnPoint.x + (i * 2), this.OracleSpawnPoint.y, this.OracleSpawnPoint.z), this.OraclePrefab.transform.rotation);
            oracles.Add(newOracle.GetComponent<Oracle>());
        }

        this.EndTurn();
        //ActiveTurnTimer = DateTime.Now;
        //EndTurnTimer = DateTime.Now.AddSeconds(15);
        //ActiveTurn = true;
    }

    public void EndTurn()
    {
        //ActiveTurn = false;

        if (this.GameState == GameState.AttackerTurn)
        {
            this.oracles.ForEach(o => o.InputActive = true);
            this.GameState = GameState.DefenderTurn;
            this.AttackerUI.SetActive(false);
            TurnText.text = "Defender's Turn";
            TurnText.color = new Color(0, .5F, 1F);
        }
        else
        {
            this.GameState = GameState.AttackerTurn;
            this.NumAvailableAttacks = this.NumberOfAttacksPerTurn;

            this.AttackerUI.SetActive(true);
            foreach (Oracle o in this.oracles)
            {
                o.InputActive = false;
                o.ApplyRule();
            }

            this.WaterFlowController.TickModules();

            if (Reservoir.Fill >= ReservoirLimit)
            {
                Results.ReservoirFill = Reservoir.Fill;
                this.SceneLoader.LoadNextScene();
            }

            if (++Turn >= TurnLimit)
            {
                Results.ReservoirFill = Reservoir.Fill;
                this.SceneLoader.LoadNextScene();
            }
            ReservoirCounter.text = Reservoir.Fill.ToString() + "/" + ReservoirLimit;
            TurnCounter.text = "Turn: " + Turn + "/" + TurnLimit;
            TurnText.text = "Attacker's Turn";
            TurnText.color = new Color(1F, 0, 0);
        }

        ScreenCover.gameObject.GetComponentsInChildren<Text>()[0].text = TurnText.text;
        ScreenCover.gameObject.GetComponentsInChildren<Text>()[0].color = TurnText.color;

        StartCoroutine(WaitForClick());
    }

    //void Update()
    //{
    //    //if (ActiveTurn)
    //    //{
    //    //    ActiveTurnTimer = DateTime.Now;
    //    //    TurnTimer.text = "Time left: " + (EndTurnTimer.Second - ActiveTurnTimer.Second).ToString();

    //    //    if (ActiveTurnTimer > EndTurnTimer)
    //    //    {
    //    //        EndTurn();   
    //    //    }
    //    //}

    //}

    IEnumerator WaitForClick()
    {
        ScreenCover.gameObject.SetActive(true);
        GameUI.SetActive(false);
        GameBoard.SetActive(false);
        //TurnTimer.gameObject.SetActive(false);

        yield return new WaitWhile(() => !Input.GetMouseButtonDown(0));

        ScreenCover.gameObject.SetActive(false);
        //TurnTimer.gameObject.SetActive(true);
        GameUI.SetActive(true);
        GameBoard.SetActive(true);

        //ActiveTurn = true;
        // EndTurnTimer = DateTime.Now.AddSeconds(15);
    }
}
