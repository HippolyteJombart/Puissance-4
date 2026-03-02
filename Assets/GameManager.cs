using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private List<List<Image>> casesTableau = new List<List<Image>>();
    [SerializeField] GameObject casesParent;
    
    [SerializeField] private TMP_Text playerText;
    [SerializeField] private Button undoButton;
    [SerializeField] private Button redoButton;
    [SerializeField] private GameObject tieScreen;
    [SerializeField] private GameObject player1WinScreen;
    [SerializeField] private GameObject player2WinScreen;

    private Stack<Coords> pastAction = new ();
    private Stack<Coords> futurAction = new ();
    
    private enum CellType
    {
        Empty = 0,
        Player1 = 1,
        Player2 = 2
    }
    
    private enum Player
    {
        Player1 = 1,
        Player2 = 2
    }
    
    private static int ligne = 6;
    private static int colonne = 7;
    
    private CellType[,] Board = new CellType[ligne, colonne];
    private int[,] pointCase = {
        {0,1,2,3,2,1,0},
        {1,2,3,4,3,2,1},
        {2,3,4,5,4,3,2},
        {2,3,4,5,4,3,2},
        {1,2,3,4,3,2,1},
        {0,1,2,3,2,1,0}};
    private Player currentPlayer = Player.Player1;

    public struct Coords
    {
        public int X; //{get; set;}
        public int Y; //{get; set;}

        public Coords(int posX, int posY)
        {
            X = posX;
            Y = posY;
        }
    }
    
    private void Start()
    {
        InitialisePanel();
        ClearBoard(Board);
        DisplayBoard(Board);
        currentPlayer = Player.Player1;
        tieScreen.SetActive(false);
        player1WinScreen.SetActive(false);
        player2WinScreen.SetActive(false);
        undoButton.interactable = false;
        redoButton.interactable = false;
    }
    
    private void InitialisePanel()
    {
        for (int i = 0; i < ligne; i++)
        {
            casesTableau.Add(new List<Image>());
            for (int j = 0; j < colonne; j++)
            {
                casesTableau[i].Add(casesParent.transform.GetChild((ligne - 1 - i) * colonne + j).gameObject.GetComponent<Image>());
            }
        }
    }
    
    private void ClearBoard(CellType[,] Board)
    {
        for (int i = 0; i < ligne; i++)
        {
            for (int j = 0; j < colonne; j++)
            {
                Board[i, j] = CellType.Empty;
            }
        }
    }

    private void DisplayBoard(CellType[,] Board)
    {
        string affichageLigne = "";
        
        for (int i = ligne - 1; i >= 0; i--)
        {
            
            for (int j = 0; j < colonne; j++)
            {
                
                string element = " ";
                
                switch(Board[i, j])
                {
                    case CellType.Empty:
                        casesTableau[i][j].color = Color.white;
                        element = "1";
                        break;
                    case CellType.Player1:
                        casesTableau[i][j].color = Color.yellow;
                        element = "X";
                        break;
                    case CellType.Player2:
                        casesTableau[i][j].color = Color.red;
                        element = "O";
                        break;
                }
                
                affichageLigne += element;
            }
            
            affichageLigne += "\n";
        }
        Debug.Log(affichageLigne);
    }

    private bool TestToken(CellType[,] Board, int colonneTest)
    {
        return Board[ligne - 1, colonneTest] == CellType.Empty;
    }

    private Coords DropToken(CellType[,] Board, CellType joueur, int colonneSelectionne)
    {
        if (!TestToken(Board, colonneSelectionne))
        {
            return new Coords(-1, -1);
        }

        for (int i = 0; i < ligne; i++)
        {
            if (Board[i, colonneSelectionne] == CellType.Empty)
            {
                return (new Coords(i, colonneSelectionne));
            }
        }
        return new Coords(-1, -1);
    }
    
    private bool TestIfFinished(CellType[,] Board)
    {
        for (int i = 0; i < colonne; i++)
        {
            if (TestToken(Board, i))
            {
                return false;
            }
        }
        return true;
    }
    
    private bool TestIfWon(CellType[,] Board, CellType joueur, Coords coupJoue)
    {
        int[] x = {1,0,1,1};
        int[] y = {0,1,1,-1};

        for (int i = 0; i < x.Length; i++)
        {
            if (SousTestWin(Board, joueur, coupJoue, x[i], y[i]))
            {
                return true;
            }
        }
        return false;
    }
    
    private bool SousTestWin(CellType[,] Board, CellType joueur, Coords coupJoue, int x, int y)
    {
        int aligne = 0;
        for (int i = -3; i < 4; i++)
        {
            if (0 <= coupJoue.X + i * x && coupJoue.X + i * x < ligne && 0 <= coupJoue.Y + i * y && coupJoue.Y + i * y < colonne)
            {
                if (Board[coupJoue.X + i * x, coupJoue.Y + i * y] == joueur)
                {
                    aligne += 1;
                }
                else
                {
                    aligne = 0;
                }
            }
            if (aligne == 4)
            {
                return true;
            }
        }
        return false;
    }

    public void PlayerAction(int i)
    {
        redoButton.interactable = false;
        futurAction.Clear();
        Play(i);
    }

    public void Play(int i)
    {
        if (!TestToken(Board, i))
        {
            Debug.Log("Cette colonne est pleine");
            return;
        }

        Coords coo;
        CellType playing;
        
        if (currentPlayer == Player.Player1)
        {
            playing = CellType.Player1;
        }
        else
        {
            playing = CellType.Player2;
        }
        
        coo = DropToken(Board, playing, i);
        Board[coo.X, coo.Y] = playing;
        pastAction.Push(coo);
        undoButton.interactable = true;
        DisplayBoard(Board);
        
        if (TestIfFinished(Board))
        {
            tieScreen.SetActive(true);
            Debug.Log("Match nul");
            return;
        }
        
        if (TestIfWon(Board, playing, coo))
        {
            if (playing == CellType.Player1)
            {
                player1WinScreen.SetActive(true);
                Debug.Log("Le joueur 1 a gagné");
            }
            else
            {
                player2WinScreen.SetActive(true);
                Debug.Log("Le joueur 2 a gagné");
            }
            return;
        }
        
        SwitchTurn();
    }

    
    private void SwitchTurn()
    {
        if (currentPlayer == Player.Player1)
        {
            Debug.Log("Au tour du joueur 2");
            currentPlayer = Player.Player2;
            playerText.text = "Player2";
            playerText.color = Color.red;
            AI(currentPlayer);
        }
        else
        {
            Debug.Log("Au tour du joueur 1");
            currentPlayer = Player.Player1;
            playerText.text = "Player1";
            playerText.color = Color.yellow;
        }
    }

    private void FakePlay(int i)
    {
        if (!TestToken(Board, i))
        {
            return;
        }

        Coords coo;
        CellType playing;
        
        if (currentPlayer == Player.Player1)
        {
            playing = CellType.Player1;
        }
        else
        {
            playing = CellType.Player2;
        }
        
        coo = DropToken(Board, playing, i);
        Board[coo.X, coo.Y] = playing;
    }
    
    private void AI(Player player)
    {
        CellType cellPlayer;
        CellType cellOpponent;
        if (player == Player.Player1)
        {
            cellPlayer = CellType.Player1;
            cellOpponent = CellType.Player2;
        }
        else
        {
            cellPlayer = CellType.Player2;
            cellOpponent = CellType.Player1;
        }

        float bestScore = -5000;
        int bestColonne = -1;
        CellType[,] cloneBoard = (CellType[,])Board.Clone();
        for (int i = 0; i < colonne; i++)
        {
            float newScore;
            FakePlay(i);
            newScore = Evaluation_Hippolyte_Jombart(Board, cellPlayer) - AI(cellOpponent);
            
            if (!TestToken(Board, i))
            {
                continue;
            }
            if (bestScore < newScore)
            {
                bestScore = newScore;
                bestColonne = i;
            }
            Board = (CellType[,])cloneBoard.Clone();
        }
        Play(bestColonne);
    }

    private float AI(CellType player)
    {
        float bestScore = -500000;
        int bestColonne = -1;
        CellType[,] cloneBoard = (CellType[,])Board.Clone();
        for (int i = 0; i < colonne; i++)
        {
            currentPlayer = Player.Player1;
            FakePlay(i);
            float newScore = Evaluation_Hippolyte_Jombart(Board, player);
            if (!TestToken(Board, i))
            {
                continue;
            }
            if (bestScore < newScore)
            {
                bestScore = newScore;
            }
            Board = (CellType[,])cloneBoard.Clone();
        }
        currentPlayer = Player.Player2;
        DisplayBoard(Board);
        Debug.Log(bestScore);
        return bestScore;
    }

    public void UndoAI()
    {
        tieScreen.SetActive(false);
        player1WinScreen.SetActive(false);
        player2WinScreen.SetActive(false);
        Coords coo = pastAction.Pop();
        Board[coo.X, coo.Y] = CellType.Empty;
        if (pastAction.Count == 0)
        {
            undoButton.interactable = false;
            redoButton.interactable = true;
            return;
        }
        
        coo = pastAction.Pop();
        Board[coo.X, coo.Y] = CellType.Empty;
        futurAction.Push(coo);
        DisplayBoard(Board);
        if (pastAction.Count == 0)
        {
            undoButton.interactable = false;
        }
        redoButton.interactable = true;
    }

    public void Undo()
    {
        tieScreen.SetActive(false);
        player1WinScreen.SetActive(false);
        player2WinScreen.SetActive(false);
        Coords coo = pastAction.Pop();
        Board[coo.X, coo.Y] = CellType.Empty;
        if (pastAction.Count == 0)
        {
            undoButton.interactable = false;
        }
        redoButton.interactable = true;
    }
    
    public void Redo()
    {
        Play(futurAction.Pop().Y);
        if (futurAction.Count == 0)
        {
            redoButton.interactable = false;
        }
    }
    
    public void ReloadScene()
    {
        ClearBoard(Board);
        DisplayBoard(Board);
        currentPlayer = Player.Player1;
        tieScreen.SetActive(false);
        player1WinScreen.SetActive(false);
        player2WinScreen.SetActive(false);
        pastAction.Clear();
        futurAction.Clear();
        undoButton.interactable = false;
    }

    private float Evaluation_Hippolyte_Jombart(CellType[,] Board,CellType joueur)
    {
        int score = 0;

        for (int i = 0; i < ligne; i++)
        {
            for (int j = 0; j < colonne; j++)
            {
                score += HJ_ScoreCase(joueur, new Coords(i, j));
                if (Board[i, j] == joueur)
                {
                    score += pointCase[i,j];
                }
            }
        }
        return score;
    }
    
    private int HJ_ScoreCase(CellType joueur, Coords coupJoue)
    {
        int score = 0;
        int[] x = {1,0,1,1};
        int[] y = {0,1,1,-1};
        
        const int doubleCoefficient = 10;
        const int tripleCoefficient = 100;
        const int tripleCoefficientVide = 100;
        const int puissance4 = 5000;

        for (int i = 0; i < x.Length; i++)
        {
            if (HJ_SousTest(joueur, coupJoue, x[i], y[i],4))
            {
                score += puissance4;
            }
            else if (HJ_SousTest(joueur, coupJoue, x[i], y[i], 3))
            {
                score += tripleCoefficient;
            }
            else if (HJ_SousTest(joueur, coupJoue, x[i], y[i],2))
            {
                score += doubleCoefficient;
            }
        }
        return score;
    }
    
    private bool HJ_SousTest(CellType joueur, Coords coupJoue, int x, int y, int align)
    {
        int aligne = 0;
        for (int i = 0; i < align; i++)
        {
            if (0 <= coupJoue.X + i * x && coupJoue.X + i * x < ligne && 0 <= coupJoue.Y + i * y && coupJoue.Y + i * y < colonne)
            {
                if (Board[coupJoue.X + i * x, coupJoue.Y + i * y] == joueur)
                {
                    aligne += 1;
                }
                else
                {
                    aligne = 0;
                }
            }
            if (aligne == align)
            {
                return true;
            }
        }
        return false;
    }
    
    private bool HJ_SousTestPlus(CellType joueur, Coords coupJoue, int x, int y, int align)
    {
        int aligne = 0;
        int vide = 0;
        for (int i = 0; i < align; i++)
        {
            if (0 <= coupJoue.X + i * x && coupJoue.X + i * x < ligne && 0 <= coupJoue.Y + i * y && coupJoue.Y + i * y < colonne)
            {
                if (Board[coupJoue.X + i * x, coupJoue.Y + i * y] == joueur)
                {
                    aligne += 1;
                }
                else if (Board[coupJoue.X + i * x, coupJoue.Y + i * y] == CellType.Empty)
                {
                    vide += 1;
                }
                else
                {
                    aligne = 0;
                }
            }
            if (aligne == align)
            {
                return true;
            }
        }
        return false;
    }
}

