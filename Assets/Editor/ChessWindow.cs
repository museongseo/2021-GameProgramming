using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ChessWindow : EditorWindow
{
	const int xsize = 6;
	const int ysize = 4; // Set the size of the board as  6 by 4
	int[,] Cell; // 1 : Black, 2 : White
	int cur_i = -1;
	int cur_j = -1;
	Texture[] CellTextures;
	bool blackTurn = true;
	bool isEnd = false;
	bool selectPhase = true;


	[MenuItem("Examples/SkillChess")]
	static void Init()
	{
		ChessWindow window = GetWindow<ChessWindow>();
		window.minSize = new Vector2(300, 300);
		window.GameInit();
	}


	void GameInit() 
	{
		Cell = new int[xsize, ysize];
		CellTextures = new Texture[2] { Resources.Load<Texture>("R_Black"), Resources.Load<Texture>("R_White") };
		for (int i = 0; i < 4; i++)
		{
			Cell[0, i] = 1;
			Cell[5, i] = 2;
		}
	}


	void OnGUI()
	{
		if (isEnd)
		{
			GUI.enabled = false;
			EditorGUILayout.LabelField(blackTurn ? "Black Wins" : "White Wins");
		}
		else
		{
			EditorGUILayout.LabelField(blackTurn ? "Black Turn" : "White Turn");
		}


		for (int i = 0; i < xsize; i++)
		{
			GUILayout.BeginHorizontal();
			for (int j = 0; j < ysize; j++)
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
									Cell[2 * i - cur_i, 2 * j - cur_j] = Cell[i, j];
									Cell[i, j] = 1;
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
									Cell[2 * i - cur_i, 2 * j - cur_j] = Cell[i, j];
									Cell[i, j] = 2;
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

	Texture GetTexture(int i, int j)
	{
		if (Cell[i, j] == 1) return CellTextures[0];
		else if (Cell[i, j] == 2) return CellTextures[1];
		return null;
	}
}
