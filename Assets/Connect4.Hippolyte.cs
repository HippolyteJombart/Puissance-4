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

    private int maxIteration = 6;
    private int compteurEvaluation;
    
    private void FakePlay(CellType[,] cloneBoard, int i,CellType player)
    {
        if (!TestToken(cloneBoard, i))
        {
            return;
        }

        Coords coo;
        
        coo = DropToken(cloneBoard, player, i);
        cloneBoard[coo.X, coo.Y] = player;
    }
    
    private (int, float) IA(CellType[,] tempBoard, int n = 0)
    {
        float bestScore = -500000;
        int bestColonne = 0;
        CellType[,] cloneBoard = (CellType[,])tempBoard.Clone();
        
        for (int i = 0; i < colonne; i++)
        {
            if (!TestToken(cloneBoard, i))
            {
                continue;
            }

            float newScore;
            FakePlay(cloneBoard, i,CellType.Player2);
            if (n == maxIteration)
            {
                newScore = Evaluation_Hippolyte_Jombart(cloneBoard,CellType.Player2);
            }
            else
            {
                if (Mathf.Abs(Evaluation_Hippolyte_Jombart(cloneBoard, CellType.Player2)) > 4500)
                {
                    return (i, 5000);
                }
                newScore = PredictionAdversaire(cloneBoard, n + 1);
            }

            //Debug.Log((i, newScore, bestScore));
            //DisplayBoard(cloneBoard);
            if (bestScore < newScore)
            {
                bestScore = newScore;
                bestColonne = i;
            }
            cloneBoard = (CellType[,])tempBoard.Clone();
        }
        return (bestColonne, bestScore);
    }
    
    private float PredictionAdversaire(CellType[,] tempBoard,  int n)
    {
        float worstScore = 500000;
        CellType[,] cloneBoard = (CellType[,])tempBoard.Clone();
        
        for (int i = 0; i < colonne; i++)
        {
            if (!TestToken(cloneBoard, i))
            {
                continue;
            }
            
            float newScore;
            FakePlay(cloneBoard, i,CellType.Player1);
            if (n == maxIteration)
            {
                newScore = Evaluation_Hippolyte_Jombart(cloneBoard,CellType.Player2);
            }
            else
            {
                if (Mathf.Abs(Evaluation_Hippolyte_Jombart(cloneBoard, CellType.Player2)) > 4500)
                {
                    return -5000;
                }
                newScore = IA(cloneBoard, n + 1).Item2;
            }
            
            //Debug.Log((i, newScore, worstScore));
            //DisplayBoard(cloneBoard);
            if (newScore < worstScore)
            {
                worstScore = newScore;
            }
            cloneBoard = (CellType[,])tempBoard.Clone();
        }
        return worstScore;
    }
    
    
    private float Evaluation_Hippolyte_Jombart(CellType[,] Board,CellType joueur)
    {
        compteurEvaluation ++;
        int score = 0;
        for (int i = 0; i < ligne; i++)
        {
            for (int j = 0; j < colonne; j++)
            {
                score += HJ_ScoreCase(Board, joueur, new Coords(i, j));
                score -= HJ_ScoreCase(Board, CellType.Player1, new Coords(i, j));
                if (Board[i, j] == joueur)
                {
                    score += pointCase[i,j];
                }
                else if (Board[i, j] == CellType.Player1)
                {
                    score -= pointCase[i,j];
                }
            }
        }
        return score;
    }
    
    private int HJ_ScoreCase(CellType[,] Board, CellType joueur, Coords coupJoue)
    {
        int score = 0;
        int[] x = {1,0,1,1};
        int[] y = {0,1,1,-1};
        
        const int doubleCoefficient = 10;
        const int tripleCoefficient = 100;
        const int puissance4 = 5000;

        for (int i = 0; i < x.Length; i++)
        {
            if (HJ_SousTest(Board, joueur, coupJoue, x[i], y[i],4))
            {
                score += puissance4;
            }
            else if (HJ_SousTest(Board, joueur, coupJoue, x[i], y[i], 3))
            {
                score += tripleCoefficient;
            }
            else if (HJ_SousTest(Board, joueur, coupJoue, x[i], y[i],2))
            {
                score += doubleCoefficient;
            }
        }
        return score;
    }
    
    private bool HJ_SousTest(CellType[,] Board ,CellType joueur, Coords coupJoue, int x, int y, int align)
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
    
    /*
    private int HJ_ScoreCase2(CellType[,] Board, CellType joueur)
    {
        return EvaluationLigne(Board, joueur) + EvaluationColonne(Board, joueur) + EvaluationDiagonale(Board, joueur) + EvaluationDiagonaleOppose(Board, joueur);
    }

    private int EvaluationLigne(CellType[,] Board, CellType joueur)
    {
        int aligne = 0;
        int doubleCase = 0;
        int tripleCase = 0;
        for (int i = 0; i < ligne; i++)
        {
            for (int j = 0; j < colonne; j++)
            {
                if (Board[i, j] == joueur)
                {
                    aligne += 1;
                }
                else
                {
                    switch (aligne)
                    {
                        case 2:
                            doubleCase += 1;
                            break;
                        case 3:
                            tripleCase += 1;
                            break;
                        case 4:
                            return 5000;
                    }

                    aligne = 0;
                }
            }
            switch (aligne)
            {
                case 2:
                    doubleCase += 1;
                    break;
                case 3:
                    tripleCase += 1;
                    break;
                case 4:
                    return 5000;
            }

            aligne = 0;
        }
        return doubleCase * 10 + tripleCase * 100;
    }
    
    private int EvaluationColonne(CellType[,] Board, CellType joueur)
    {
        int aligne = 0;
        int doubleCase = 0;
        int tripleCase = 0;
        for (int j = 0; j < colonne; j++)
        {
            for (int i = 0; i < ligne; i++)
            {
                if (Board[i, j] == joueur)
                {
                    aligne += 1;
                }
                else
                {
                    switch (aligne)
                    {
                        case 2:
                            doubleCase += 1;
                            break;
                        case 3:
                            tripleCase += 1;
                            break;
                        case 4:
                            return 5000;
                    }

                    aligne = 0;
                }
            }
            switch (aligne)
            {
                case 2:
                    doubleCase += 1;
                    break;
                case 3:
                    tripleCase += 1;
                    break;
                case 4:
                    return 5000;
            }

            aligne = 0;
        }
        return doubleCase * 10 + tripleCase * 100;
    }
    
    private int EvaluationDiagonale(CellType[,] Board, CellType joueur)
    {
        int aligne = 0;
        int doubleCase = 0;
        int tripleCase = 0;
        for (int i = 0; i < ligne; i++)
        {
            if (i < ligne && i < colonne)
            {
                if (Board[i, i] == joueur)
                {
                    aligne += 1;
                }
                else
                {
                    switch (aligne)
                    {
                        case 2:
                            doubleCase += 1;
                            break;
                        case 3:
                            tripleCase += 1;
                            break;
                        case 4:
                            return 5000;
                    }

                    aligne = 0;
                }
            }
            switch (aligne)
            {
                case 2:
                    doubleCase += 1;
                    break;
                case 3:
                    tripleCase += 1;
                    break;
                case 4:
                    return 5000;
            }

            aligne = 0;
        }
        return doubleCase * 10 + tripleCase * 100;
    }
    
    private int EvaluationDiagonaleOppose(CellType[,] Board, CellType joueur)
    {
        int aligne = 0;
        int doubleCase = 0;
        int tripleCase = 0;
        for (int i = 0; i < ligne; i++)
        {
            if (i < ligne && -i >= 0)
            {
                if (Board[i, -i] == joueur)
                {
                    aligne += 1;
                }
                else
                {
                    switch (aligne)
                    {
                        case 2:
                            doubleCase += 1;
                            break;
                        case 3:
                            tripleCase += 1;
                            break;
                        case 4:
                            return 5000;
                    }

                    aligne = 0;
                }
            }
            switch (aligne)
            {
                case 2:
                    doubleCase += 1;
                    break;
                case 3:
                    tripleCase += 1;
                    break;
                case 4:
                    return 5000;
            }

            aligne = 0;
        }
        return doubleCase * 10 + tripleCase * 100;
    }*/

    /*private int HJ_SousTest2(CellType[,] Board ,CellType joueur, Coords coupJoue, int x, int y)
{
    int aligne = 0;
    int maxAligne = 0;
    for (int i = 0; i < 4; i++)
    {
        if (0 <= coupJoue.X + i * x && coupJoue.X + i * x < ligne && 0 <= coupJoue.Y + i * y && coupJoue.Y + i * y < colonne)
        {
            if (Board[coupJoue.X + i * x, coupJoue.Y + i * y] == joueur)
            {
                aligne += 1;
            }
            else
            {
                maxAligne = aligne;
                aligne = 0;
            }
        }
    }
    return maxAligne;
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
}*/
    
    
    // VIEUX TEST QUI MARCHE PAS
    
    /*
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
            FakePlay(Board, i, cellPlayer);
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
            FakePlay(cloneBoard, i,CellType.Player2);
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
            FakePlay(cloneBoard, i,CellType.Player1);
            newScore = TestCoupPlayer2(cloneBoard, n).Item2;
            
            if (bestScore > newScore)
            {
                bestScore = newScore;
                bestColonne = i;
            }
            
            cloneBoard = (CellType[,])tempBoard.Clone();
        }
        return (bestColonne, bestScore);
    }*/
}