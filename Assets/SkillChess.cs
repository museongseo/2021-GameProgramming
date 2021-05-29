using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class SkillChess : MonoBehaviourPunCallbacks
{
    const int isize = 6, jsize = 4;
    int[,] Cell;
    int cur_i, cur_j;
    Texture[] CellTextures;
    bool blackTurn = false, isEnd = false, start = false, selectPhase = true;
    [SerializeField] private Text DebugText;
    private PhotonView PV;




    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
        Connect();

    }
    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 2 }, null);
    }

    public override void OnJoinedRoom()
    {
        GameInit();
    }
    void GameInit()
    {
        Cell = new int[isize, jsize];
        CellTextures = new Texture[2] { Resources.Load<Texture>("R_Black"), Resources.Load<Texture>("R_White") };
        for (int i = 0; i < 4; i++)
        {
            Cell[0, i] = 1;
            Cell[5, i] = 2;
        }
        start = true;
        if (PhotonNetwork.IsMasterClient == true)
        {
            blackTurn = true;
        }

    }


    void OnGUI()
    {
        if (start)
        {
            if (isEnd)
            {
                GUI.enabled = false;
                DebugText.text = blackTurn ? "Black Wins" : "White Wins";
            }
            else
            {
                DebugText.text = blackTurn ? "Black's Turn" : "White's Turn";
            }


            for (int i = 0; i < isize; i++)
            {
                GUILayout.BeginHorizontal();
                for (int j = 0; j < jsize; j++)
                {
                    if (blackTurn) // Black Player's Turn
                    {
                        if (selectPhase) // If the player has not chosen which piece to move, select the piece to move
                        {
                            if (GUILayout.Button(GetTexture(i, j), GUILayout.Width(60), GUILayout.Height(60)) && Cell[i, j] == 1)
                            {
                                Cell[i, j] = 0;
                                cur_i = i;
                                cur_j = j;
                                selectPhase = !selectPhase;
                            }
                        }

                        else // Once you have selected which piece to move, move it only in the adjacent cell
                        {
                            if (GUILayout.Button(GetTexture(i, j), GUILayout.Width(60), GUILayout.Height(60)))
                            {
                                if ((Math.Abs(cur_i - i) <= 1) && (Math.Abs(cur_j - j) <= 1))
                                {
                                    if (Cell[i, j] == 0) // If there is no other piece in the cell to move
                                    {
                                        Cell[i, j] = 1;
                                    }
                                    else // If there is another piece in the cell to move
                                    {
                                        if ((2 * i - cur_i > 5) || (2 * i - cur_i < 0) || (2 * j - cur_j > 3) || (2 * j - cur_j < 0))
                                        {
                                            Cell[i, j] = 1;
                                        }
                                        else
                                        {
                                            Cell[2 * i - cur_i, 2 * j - cur_j] = Cell[i, j];
                                            Cell[i, j] = 1;
                                        }
                                    }
                                    selectPhase = !selectPhase;
                                    blackTurn = !blackTurn;
                                }
                            }
                        }
                    }

                    else // White Player's Turn
                    {
                        if (selectPhase)
                        {
                            if (GUILayout.Button(GetTexture(i, j), GUILayout.Width(60), GUILayout.Height(60)) && Cell[i, j] == 2)
                            {
                                Cell[i, j] = 0;
                                cur_i = i;
                                cur_j = j;
                                selectPhase = !selectPhase;
                            }
                        }

                        else
                        {
                            if (GUILayout.Button(GetTexture(i, j), GUILayout.Width(60), GUILayout.Height(60)))
                            {
                                if ((Math.Abs(cur_i - i) <= 1) && (Math.Abs(cur_j - j) <= 1))
                                {
                                    if (Cell[i, j] == 0) // If there is no other piece in the cell to move
                                    {
                                        Cell[i, j] = 2;
                                    }
                                    else // If there is another piece in the cell to move
                                    {
                                        if ((2 * i - cur_i > 5) || (2 * i - cur_i < 0) || (2 * j - cur_j > 3) || (2 * j - cur_j < 0))
                                        {
                                            Cell[i, j] = 2;
                                        }
                                        else
                                        {
                                            Cell[2 * i - cur_i, 2 * j - cur_j] = Cell[i, j];
                                            Cell[i, j] = 2;
                                        }
                                    }
                                    selectPhase = !selectPhase;
                                    blackTurn = !blackTurn;
                                }
                            }
                        }
                    }

                }
                GUILayout.EndHorizontal();
            }
        }
    }


    Texture GetTexture(int i, int j)
    {
        if (Cell[i, j] == 1) return CellTextures[0];
        else if (Cell[i, j] == 2) return CellTextures[1];
        return null;
    }
}