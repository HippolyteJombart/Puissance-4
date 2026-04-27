using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class Connect4 : MonoBehaviour
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
            DisplayBoard(Board);
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
            Play(IA(Board).Item1);
        }
        else
        {
            Debug.Log("Au tour du joueur 1");
            currentPlayer = Player.Player1;
            playerText.text = "Player1";
            playerText.color = Color.yellow;
        }
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

}

