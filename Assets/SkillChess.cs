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
    private AudioSource audioSource;

    const int isize = 6, jsize = 4;
    int[,] Cell;
    int cur_i, cur_j;
    Texture[] CellTextures;
    bool blackTurn = false, isEnd = false, start = false, selectPhase = true;
    [SerializeField] private Text DebugText;
    [SerializeField] private AudioClip Victory;
    [SerializeField] private AudioClip Piece;
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
        audioSource = GetComponent<AudioSource>();
        Cell = new int[isize, jsize];
        CellTextures = new Texture[2] { Resources.Load<Texture>("R_Black"), Resources.Load<Texture>("R_White") };
        for (int i = 0; i < 4; i++)
        {
            Cell[5, i] = 1;
            Cell[0, i] = 2; // Black : 1, White : 2
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
                //PlaySound("Victory");
                GUI.enabled = false;
                DebugText.text = blackTurn ? "White Wins" : "Black Wins";
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
                                        if ((2 * i - cur_i > 5) || (2 * i - cur_i < 0) || (2 * j - cur_j > 3) || (2 * j - cur_j < 0)) // 놓은 자리에 말이 있고, 그 말이 장외 판정을 받을 때
                                        {
                                            Cell[i, j] = 1;
                                        }
                                        else // 놓은 자리에 말이 있고, 그 말이 장외 판정이 아닐 때
                                        {
                                            if (Cell[2 * i - cur_i, 2 * j - cur_j] == 0) // 밀려난 말이 이동할 위치에 아무 것도 없을 때
                                            {
                                                Cell[2 * i - cur_i, 2 * j - cur_j] = Cell[i, j];
                                                Cell[i, j] = 1;
                                            }
                                            else // 밀려난 말이 이동할 위치에 다른 말이 있을 때
                                            {
                                                if ((3 * i - 2 * cur_i > 5) || (3 * i - 2 * cur_i < 0) || (3 * j - 2 * cur_j > 3) || (3 * j - 2 * cur_j < 0))
                                                    // 두번째로 밀려난 말이 장외 판정일 때
                                                {
                                                    Cell[2 * i - cur_i, 2 * j - cur_j] = Cell[i, j];
                                                    Cell[i, j] = 1;
                                                }
                                                else
                                                {
                                                    if (Cell[3 * i - 2 * cur_i, 3 * j - 2 * cur_j] == 0) // 두번째로 밀려난 말이 이동할 위치에 아무 것도 없을 때
                                                    {
                                                        Cell[3 * i - 2 * cur_i, 3 * j - 2 * cur_j] = Cell[2 * i - cur_i, 2 * j - cur_j];
                                                        Cell[2 * i - cur_i, 2 * j - cur_j] = Cell[i, j];
                                                        Cell[i, j] = 1;
                                                    }
                                                    else // 두번째로 밀려난 말이 이동할 위치에 다른 말이 있을 때
                                                    {
                                                        if ((4 * i - 3 * cur_i > 5) || (4 * i - 3 * cur_i < 0) || (4 * j - 3 * cur_j > 3) || (4 * j - 3 * cur_j < 0))
                                                        // 세번째로 밀려난 말이 장외 판정일 때
                                                        {
                                                            Cell[3 * i - 2 * cur_i, 3 * j - 2 * cur_j] = Cell[2 * i - cur_i, 2 * j - cur_j];
                                                            Cell[2 * i - cur_i, 2 * j - cur_j] = Cell[i, j];
                                                            Cell[i, j] = 1;
                                                        }
                                                        else // 세번째로 밀려난 말이 장외 판정이 아닐 때
                                                        {
                                                            if (Cell[4 * i - 3 * cur_i, 4 * j - 3 * cur_j] == 0) // 세번째로 밀려난 말이 이동할 위치에 아무것도 없을 때
                                                            {
                                                                Cell[4 * i - 3 * cur_i, 4 * j - 3 * cur_j] = Cell[3 * i - 2 * cur_i, 3 * j - 2 * cur_j];
                                                                Cell[3 * i - 2 * cur_i, 3 * j - 2 * cur_j] = Cell[2 * i - cur_i, 2 * j - cur_j];
                                                                Cell[2 * i - cur_i, 2 * j - cur_j] = Cell[i, j];
                                                                Cell[i, j] = 1;
                                                            }
                                                            else // 세번째로 밀려난 말이 이동할 위치에 다른 말이 있을 때
                                                            {
                                                                if ((5 * i - 4 * cur_i > 5) || (5 * i - 4 * cur_i < 0) || (5 * j - 4 * cur_j > 3) || (5 * j - 4 * cur_j < 0))
                                                                // 네번째로 밀려난 말이 장외 판정일 때
                                                                {
                                                                    Cell[4 * i - 3 * cur_i, 4 * j - 3 * cur_j] = Cell[3 * i - 2 * cur_i, 3 * j - 2 * cur_j];
                                                                    Cell[3 * i - 2 * cur_i, 3 * j - 2 * cur_j] = Cell[2 * i - cur_i, 2 * j - cur_j];
                                                                    Cell[2 * i - cur_i, 2 * j - cur_j] = Cell[i, j];
                                                                    Cell[i, j] = 1;
                                                                }
                                                                else // 네번째로 밀려난 말이 장외 판정이 아닐 때
                                                                {
                                                                    if (Cell[5 * i - 4 * cur_i, 5 * j - 4 * cur_j] == 0) // 네번째로 밀려난 말이 이동할 위치에 아무것도 없을 때
                                                                    {
                                                                        Cell[5 * i - 4 * cur_i, 5 * j - 4 * cur_j] = Cell[4 * i - 3 * cur_i, 4 * j - 3 * cur_j];
                                                                        Cell[4 * i - 3 * cur_i, 4 * j - 3 * cur_j] = Cell[3 * i - 2 * cur_i, 3 * j - 2 * cur_j];
                                                                        Cell[3 * i - 2 * cur_i, 3 * j - 2 * cur_j] = Cell[2 * i - cur_i, 2 * j - cur_j];
                                                                        Cell[2 * i - cur_i, 2 * j - cur_j] = Cell[i, j];
                                                                        Cell[i, j] = 1;
                                                                    }
                                                                    else // 네번째로 밀려난 말이 이동할 위치에 다른 말이 있을 때
                                                                    {
                                                                        if ((6 * i - 5 * cur_i > 5) || (6 * i - 5 * cur_i < 0) || (6 * j - 5 * cur_j > 3) || (6 * j - 5 * cur_j < 0))
                                                                        // 다섯번째로 밀려난 말이 장외 판정일 때 (4 x 6 이므로 다섯번째로 밀려난 말은 무조건 장외 판정)
                                                                        {
                                                                            Cell[5 * i - 4 * cur_i, 5 * j - 4 * cur_j] = Cell[4 * i - 3 * cur_i, 4 * j - 3 * cur_j];
                                                                            Cell[4 * i - 3 * cur_i, 4 * j - 3 * cur_j] = Cell[3 * i - 2 * cur_i, 3 * j - 2 * cur_j];
                                                                            Cell[3 * i - 2 * cur_i, 3 * j - 2 * cur_j] = Cell[2 * i - cur_i, 2 * j - cur_j];
                                                                            Cell[2 * i - cur_i, 2 * j - cur_j] = Cell[i, j];
                                                                            Cell[i, j] = 1;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    //PlaySound("Piece");
                                    selectPhase = !selectPhase;
                                    blackTurn = !blackTurn;
                                    isCheckmate();
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

                        else // Once you have selected which piece to move, move it only in the adjacent cell
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
                                        if ((2 * i - cur_i > 5) || (2 * i - cur_i < 0) || (2 * j - cur_j > 3) || (2 * j - cur_j < 0)) // 놓은 자리에 말이 있고, 그 말이 장외 판정을 받을 때
                                        {
                                            Cell[i, j] = 2;
                                        }
                                        else // 놓은 자리에 말이 있고, 그 말이 장외 판정이 아닐 때
                                        {
                                            if (Cell[2 * i - cur_i, 2 * j - cur_j] == 0) // 밀려난 말이 이동할 위치에 아무 것도 없을 때
                                            {
                                                Cell[2 * i - cur_i, 2 * j - cur_j] = Cell[i, j];
                                                Cell[i, j] = 2;
                                            }
                                            else // 밀려난 말이 이동할 위치에 다른 말이 있을 때
                                            {
                                                if ((3 * i - 2 * cur_i > 5) || (3 * i - 2 * cur_i < 0) || (3 * j - 2 * cur_j > 3) || (3 * j - 2 * cur_j < 0))
                                                // 두번째로 밀려난 말이 장외 판정일 때
                                                {
                                                    Cell[2 * i - cur_i, 2 * j - cur_j] = Cell[i, j];
                                                    Cell[i, j] = 2;
                                                }
                                                else
                                                {
                                                    if (Cell[3 * i - 2 * cur_i, 3 * j - 2 * cur_j] == 0) // 두번째로 밀려난 말이 이동할 위치에 아무 것도 없을 때
                                                    {
                                                        Cell[3 * i - 2 * cur_i, 3 * j - 2 * cur_j] = Cell[2 * i - cur_i, 2 * j - cur_j];
                                                        Cell[2 * i - cur_i, 2 * j - cur_j] = Cell[i, j];
                                                        Cell[i, j] = 2;
                                                    }
                                                    else // 두번째로 밀려난 말이 이동할 위치에 다른 말이 있을 때
                                                    {
                                                        if ((4 * i - 3 * cur_i > 5) || (4 * i - 3 * cur_i < 0) || (4 * j - 3 * cur_j > 3) || (4 * j - 3 * cur_j < 0))
                                                        // 세번째로 밀려난 말이 장외 판정일 때
                                                        {
                                                            Cell[3 * i - 2 * cur_i, 3 * j - 2 * cur_j] = Cell[2 * i - cur_i, 2 * j - cur_j];
                                                            Cell[2 * i - cur_i, 2 * j - cur_j] = Cell[i, j];
                                                            Cell[i, j] = 2;
                                                        }
                                                        else // 세번째로 밀려난 말이 장외 판정이 아닐 때
                                                        {
                                                            if (Cell[4 * i - 3 * cur_i, 4 * j - 3 * cur_j] == 0) // 세번째로 밀려난 말이 이동할 위치에 아무것도 없을 때
                                                            {
                                                                Cell[4 * i - 3 * cur_i, 4 * j - 3 * cur_j] = Cell[3 * i - 2 * cur_i, 3 * j - 2 * cur_j];
                                                                Cell[3 * i - 2 * cur_i, 3 * j - 2 * cur_j] = Cell[2 * i - cur_i, 2 * j - cur_j];
                                                                Cell[2 * i - cur_i, 2 * j - cur_j] = Cell[i, j];
                                                                Cell[i, j] = 2;
                                                            }
                                                            else // 세번째로 밀려난 말이 이동할 위치에 다른 말이 있을 때
                                                            {
                                                                if ((5 * i - 4 * cur_i > 5) || (5 * i - 4 * cur_i < 0) || (5 * j - 4 * cur_j > 3) || (5 * j - 4 * cur_j < 0))
                                                                // 네번째로 밀려난 말이 장외 판정일 때
                                                                {
                                                                    Cell[4 * i - 3 * cur_i, 4 * j - 3 * cur_j] = Cell[3 * i - 2 * cur_i, 3 * j - 2 * cur_j];
                                                                    Cell[3 * i - 2 * cur_i, 3 * j - 2 * cur_j] = Cell[2 * i - cur_i, 2 * j - cur_j];
                                                                    Cell[2 * i - cur_i, 2 * j - cur_j] = Cell[i, j];
                                                                    Cell[i, j] = 2;
                                                                }
                                                                else // 네번째로 밀려난 말이 장외 판정이 아닐 때
                                                                {
                                                                    if (Cell[5 * i - 4 * cur_i, 5 * j - 4 * cur_j] == 0) // 네번째로 밀려난 말이 이동할 위치에 아무것도 없을 때
                                                                    {
                                                                        Cell[5 * i - 4 * cur_i, 5 * j - 4 * cur_j] = Cell[4 * i - 3 * cur_i, 4 * j - 3 * cur_j];
                                                                        Cell[4 * i - 3 * cur_i, 4 * j - 3 * cur_j] = Cell[3 * i - 2 * cur_i, 3 * j - 2 * cur_j];
                                                                        Cell[3 * i - 2 * cur_i, 3 * j - 2 * cur_j] = Cell[2 * i - cur_i, 2 * j - cur_j];
                                                                        Cell[2 * i - cur_i, 2 * j - cur_j] = Cell[i, j];
                                                                        Cell[i, j] = 2;
                                                                    }
                                                                    else // 네번째로 밀려난 말이 이동할 위치에 다른 말이 있을 때
                                                                    {
                                                                        if ((6 * i - 5 * cur_i > 5) || (6 * i - 5 * cur_i < 0) || (6 * j - 5 * cur_j > 3) || (6 * j - 5 * cur_j < 0))
                                                                        // 다섯번째로 밀려난 말이 장외 판정일 때 (4 x 6 이므로 다섯번째로 밀려난 말은 무조건 장외 판정)
                                                                        {
                                                                            Cell[5 * i - 4 * cur_i, 5 * j - 4 * cur_j] = Cell[4 * i - 3 * cur_i, 4 * j - 3 * cur_j];
                                                                            Cell[4 * i - 3 * cur_i, 4 * j - 3 * cur_j] = Cell[3 * i - 2 * cur_i, 3 * j - 2 * cur_j];
                                                                            Cell[3 * i - 2 * cur_i, 3 * j - 2 * cur_j] = Cell[2 * i - cur_i, 2 * j - cur_j];
                                                                            Cell[2 * i - cur_i, 2 * j - cur_j] = Cell[i, j];
                                                                            Cell[i, j] = 2;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    //PlaySound("Piece");
                                    selectPhase = !selectPhase;
                                    blackTurn = !blackTurn;
                                    isCheckmate();
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

    void isCheckmate()
    {
        int blackCount = 0;
        int whiteCount = 0;
        for (int i = 0; i < isize; i++)
        {
            for (int j = 0; j < jsize; j++)
            {
                if (Cell[i, j] == 1) blackCount += 1;
                else if (Cell[i, j] == 2) whiteCount += 1;
            }
        }
        if ((blackCount == 0) || (whiteCount == 0)) isEnd = true;
        else return;
    }

    void PlaySound(String action)
    {
        switch (action)
        {
            case "Victory":
                audioSource.clip = Victory;
                break;
            case "Piece":
                audioSource.clip = Piece;
                break;
        }
    }
}