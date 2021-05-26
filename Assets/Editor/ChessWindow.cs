using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ChessWindow : EditorWindow
{
	const int xsize = 6;
	const int ysize = 4; // Set the size of the board as  6 by 4
	int[,] Cell; // 1 : Black, 2 : White
	Texture[] CellTextures;
	bool blackTurn = true;
	bool isEnd = false;
	bool selectPhase = true;


	[MenuItem("Examples/SkillChess")]
	static void Init()
	{
		ChessWindow window = GetWindow<ChessWindow>();
		window.minSize = new Vector2(400, 400);
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
				if (blackTurn)
				{
					if (selectPhase)
					{
						if (GUILayout.Button(GetTexture(i, j), GUILayout.Width(60), GUILayout.Height(60)) && Cell[i, j] == 1)
						{
							Cell[i, j] = 0;
							selectPhase = !selectPhase;
						}
					}

					else
					{
						if (GUILayout.Button(GetTexture(i, j), GUILayout.Width(60), GUILayout.Height(60)) && Cell[i, j] == 0)
						{
							Cell[i, j] = 1;
							selectPhase = !selectPhase;
							blackTurn = !blackTurn;
						}
					}
				}

				else
				{
					if (selectPhase)
					{
						if (GUILayout.Button(GetTexture(i, j), GUILayout.Width(60), GUILayout.Height(60)) && Cell[i, j] == 2)
						{
							Cell[i, j] = 0;
							selectPhase = !selectPhase;
						}
					}

					else
					{
						if (GUILayout.Button(GetTexture(i, j), GUILayout.Width(60), GUILayout.Height(60)) && Cell[i, j] == 0)
						{
							Cell[i, j] = 2;
							selectPhase = !selectPhase;
							blackTurn = !blackTurn;
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
