using UnityEngine;

public partial class Connect4 : MonoBehaviour
{
    private int[,] pointCase = {
        {0,1,2,3,2,1,0},
        {1,2,3,4,3,2,1},
        {2,3,4,5,4,3,2},
        {2,3,4,5,4,3,2},
        {1,2,3,4,3,2,1},
        {0,1,2,3,2,1,0}};

    /*
    private (int,float) TestCoupPlayer2(CellType[,] tempBoard, int n)
    {

        return (0, 0);
    }*/
    
    
    
    private void FakePlay(CellType[,] cloneBoard, int i)
    {
        if (!TestToken(cloneBoard, i))
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
        
        coo = DropToken(cloneBoard, playing, i);
        cloneBoard[coo.X, coo.Y] = playing;
    }
    
    private (int,float) AI(Player player, int n)
    {
        CellType cellPlayer;
        Player opponent;
        if (player == Player.Player1)
        {
            cellPlayer = CellType.Player1;
            opponent = Player.Player2;
        }
        else
        {
            cellPlayer = CellType.Player2;
            opponent = Player.Player1;
        }
        
        if (n == 0)
        {
            return (0,Evaluation_Hippolyte_Jombart(Board, cellPlayer));
        }

        float bestScore = 50000;
        int bestColonne = 0;
        CellType[,] cloneBoard = (CellType[,])Board.Clone();
        for (int i = 0; i < colonne; i++)
        {
            if (!TestToken(Board, i))
            {
                continue;
            }
            float newScore;
            FakePlay(Board, i);
            newScore = Evaluation_Hippolyte_Jombart(Board, cellPlayer) - AI(opponent, n-1).Item2;
            
            if (bestScore < newScore)
            {
                bestScore = newScore;
                bestColonne = i;
            }
            Board = (CellType[,])cloneBoard.Clone();
        }
        
        print(bestColonne);
        return (bestColonne, bestScore);
    }

    private (int,float) TestCoupPlayer2(CellType[,] tempBoard, int n)
    {
        if (n == 0)
        {
            return (0,Evaluation_Hippolyte_Jombart(tempBoard, CellType.Player2));
        }
        float worstScore = -500000;
        int bestColonne = 0;
        CellType[,] cloneBoard = (CellType[,])tempBoard.Clone();
        for (int i = 0; i < colonne; i++)
        {
            if (!TestToken(cloneBoard, i))
            {
                continue;
            }
            
            float newScore;
            FakePlay(cloneBoard, i);
            newScore = TestCoupPlayer1(cloneBoard, n - 1).Item2;

            Debug.Log((i, newScore, worstScore));
            DisplayBoard(cloneBoard);
            if (worstScore < newScore)
            {
                worstScore = newScore;
                bestColonne = i;
            }
            cloneBoard = (CellType[,])tempBoard.Clone();
        }
        return (bestColonne, worstScore);
    }
    
    
    private (int,float) TestCoupPlayer1(CellType[,] tempBoard, int n)
    {
        float bestScore = 500000;
        int bestColonne = 0;
        CellType[,] cloneBoard = (CellType[,])tempBoard.Clone();
        for (int i = 0; i < colonne; i++)
        {
            if (!TestToken(cloneBoard, i))
            {
                continue;
            }
            float newScore;
            FakePlay(cloneBoard, i);
            newScore = TestCoupPlayer2(cloneBoard, n).Item2;
            
            if (bestScore > newScore)
            {
                bestScore = newScore;
                bestColonne = i;
            }
            
            cloneBoard = (CellType[,])tempBoard.Clone();
        }
        return (bestColonne, bestScore);
    }
    
    private float Evaluation_Hippolyte_Jombart(CellType[,] Board,CellType joueur)
    {
        int score = 0;
        CellType opponent;
        
        if (joueur == CellType.Player1)
        {
            opponent = CellType.Player2;
        }
        else
        {
            opponent = CellType.Player1;
        }
        
        for (int i = 0; i < ligne; i++)
        {
            for (int j = 0; j < colonne; j++)
            {
                score += HJ_ScoreCase(joueur, new Coords(i, j));
                score -= HJ_ScoreCase(opponent, new Coords(i, j));
                if (Board[i, j] == joueur)
                {
                    score += pointCase[i,j];
                }
                else if (Board[i, j] == opponent)
                {
                    score -= pointCase[i, j];
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
                Debug.Log("cacaaaaaaaaaaaaaaaaaaa");
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
