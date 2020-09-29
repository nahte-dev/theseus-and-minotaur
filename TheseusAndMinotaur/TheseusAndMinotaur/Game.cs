using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheseusAndMinotaur
{
    public enum Moves
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        PAUSE,
    }

    public class Game : ILevelHolder, IMoveableHolder, IMoveable
    { 
        public List<string> levels = new List<string>();
        public List<Level> allLevels = new List<Level>();

        public int LevelWidth { get; set; }
        public int LevelHeight { get; set; }
        public string CurrentLevelName { get; set; } = "No levels loaded";
        public int LevelCount { get; set; }

        // Implementing IMoveableHolder members
        
        public int MinotaurRow { get; set; }
        public int MinotaurColumn { get; set; }
        public int TheseusRow { get; set; }
        public int TheseusColumn { get; set; }
        public int MoveCount { get; set; }

        // Implementing IMoveable members
        public int Row { get; set; }
        public int Column { get; set; }

        // Declaring level array
        Square[,] level;

        public void MoveTheseus(Moves direction)
        {
            switch (direction)
            {
                case Moves.UP:
                    GoUp();
                    break;
                case Moves.DOWN:
                    GoDown();
                    break;
                case Moves.LEFT:
                    GoLeft();
                    break;
                case Moves.RIGHT:
                    GoRight();
                    break;
                case Moves.PAUSE:
                    Pause();
                    break;
                default:
                    break;
            }
        }

        public void GoUp()
        {
            if (!WhatIsAt(TheseusRow, TheseusColumn).Top && !WhatIsAt(TheseusRow - 1, TheseusColumn).Bottom)
            {
                WhatIsAt(TheseusRow, TheseusColumn).Theseus = false;
                TheseusRow--;
                WhatIsAt(TheseusRow, TheseusColumn).Theseus = true;

                MoveCount++;
            }
        }

        public void GoDown()
        {
            if (!WhatIsAt(TheseusRow, TheseusColumn).Bottom && !WhatIsAt(TheseusRow + 1, TheseusColumn).Top)
            {
                WhatIsAt(TheseusRow, TheseusColumn).Theseus = false;
                TheseusRow++;
                WhatIsAt(TheseusRow, TheseusColumn).Theseus = true;

                MoveCount++;
            }
        }

        public void GoLeft()
        {

            if (!WhatIsAt(TheseusRow, TheseusColumn).Left && !WhatIsAt(TheseusRow, TheseusColumn - 1).Right)
            {
                WhatIsAt(TheseusRow, TheseusColumn).Theseus = false;
                TheseusColumn--;
                WhatIsAt(TheseusRow, TheseusColumn).Theseus = true;

                MoveCount++;
            }
        }

        public void GoRight()
        {
            if (!WhatIsAt(TheseusRow, TheseusColumn).Right && !WhatIsAt(TheseusRow, TheseusColumn + 1).Left)
            {
                WhatIsAt(TheseusRow, TheseusColumn).Theseus = false;
                TheseusColumn++;
                WhatIsAt(TheseusRow, TheseusColumn).Theseus = true;

                MoveCount++;
            }
        }

        public void Pause()
        {
            MoveCount++;
        }

        public void AddLevel(string name, int width, int height, string data)
        {
            Level newLevel = new Level();

            newLevel.Name = name;
            newLevel.Width = width;
            newLevel.Height = height;
            newLevel.Data = data;

            LevelCount++;
            levels.Add(name);
            allLevels.Add(newLevel);

            LoadLevel(newLevel);
        }

        public void LoadLevel(Level loadedLevel)
        {
            level = new Square[loadedLevel.Height, loadedLevel.Width];

            LevelWidth = loadedLevel.Width;
            LevelHeight = loadedLevel.Height;
            CurrentLevelName = loadedLevel.Name;

            string[] squareData = loadedLevel.Data.Split(" ");
            int wallIndex = 3;

            bool topValue = false;
            bool leftValue = false;
            bool bottomValue = false;
            bool rightValue = false;

            bool theseusValue = false;
            bool minotaurValue = false;
            bool exitValue = false;

            for (int i = 0; i < loadedLevel.Height; i++) 
            {
                for (int j = 0; j < loadedLevel.Width; j++)
                {
                    // 4th element of full string in wallData i.e 1011
                    var wallData = squareData[wallIndex];
                    int[] wallPosition = { Convert.ToInt32(wallData.Substring(0, 1)), Convert.ToInt32(wallData.Substring(1, 1)),
                        Convert.ToInt32(wallData.Substring(2, 1)), Convert.ToInt32(wallData.Substring(3, 1)) };

                    topValue = Convert.ToBoolean(wallPosition[0]);
                    rightValue = Convert.ToBoolean(wallPosition[1]);
                    bottomValue = Convert.ToBoolean(wallPosition[2]);
                    leftValue = Convert.ToBoolean(wallPosition[3]);

                    level[i, j] = new Square(topValue, leftValue, bottomValue, rightValue, theseusValue, minotaurValue,
                        exitValue);

                    wallIndex++;
                }
            }

            int[] minotaurData = { Convert.ToInt32(loadedLevel.Data.Substring(0, 2)), Convert.ToInt32(loadedLevel.Data.Substring(2, 2)) };
            MinotaurRow = minotaurData[0];
            MinotaurColumn = minotaurData[1];

            int[] theseusData = { Convert.ToInt32(loadedLevel.Data.Substring(5, 2)), Convert.ToInt32(loadedLevel.Data.Substring(7, 2)) };
            TheseusRow = theseusData[0];
            TheseusColumn = theseusData[1];

            int[] exitData = { Convert.ToInt32(loadedLevel.Data.Substring(10, 2)), Convert.ToInt32(loadedLevel.Data.Substring(12, 2)) };

            WhatIsAt(TheseusRow, TheseusColumn).Theseus = true;
            WhatIsAt(MinotaurRow, MinotaurColumn).Minotaur = true;
            WhatIsAt(exitData[0], exitData[1]).Exit = true;
        }

        public List<string> LevelNames()
        {
            return levels;
        }

        public void SetLevel(string name)
        {
            foreach (Level storedLevel in allLevels)
            {
                string levelName = storedLevel.Name;

                if (levelName == name)
                {
                    LoadLevel(storedLevel);
                }
                else
                {
                    Console.WriteLine("Level does not exist");
                }
            }
        }

        public void LoadNextLevel(string name)
        {
            for (int i = 0; i < allLevels.Count - 1; i++)
            {
                string levelName = allLevels[i].Name;

                if (levelName == name)
                {
                    LoadLevel(allLevels[i + 1]);
                    break;
                }
            }
        }

        public void EatTheseus()
        {
            Console.WriteLine("Game over! You were eaten by the Minotaur");
            SetLevel(CurrentLevelName);
        }

        public void TheseusExits()
        {
            Console.WriteLine("Congratulations! You completed this level!");

            Console.WriteLine("\nLoading new level... ");

            LoadNextLevel(CurrentLevelName);
        }

        public bool HasMinotaurWon()
        {
            if (WhatIsAt(MinotaurRow, MinotaurColumn) == WhatIsAt(TheseusRow, TheseusColumn))
            {
                EatTheseus();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool HasTheseusWon()
        {
            if (WhatIsAt(TheseusRow, TheseusColumn).Theseus == WhatIsAt(TheseusRow, TheseusColumn).Exit)
            {
                TheseusExits();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void MoveMinotaur()
        {
            bool canMoveVertically = true;

            // Deciding if horizontal movement
            if (MinotaurColumn > TheseusColumn)
            {
                if (!WhatIsAt(MinotaurRow, MinotaurColumn).Left && !WhatIsAt(MinotaurRow, MinotaurColumn - 1).Right)
                {
                    WhatIsAt(MinotaurRow, MinotaurColumn).Minotaur = false;
                    MinotaurColumn--;
                    WhatIsAt(MinotaurRow, MinotaurColumn).Minotaur = true;
                }
            }
            else if (MinotaurColumn < TheseusColumn)
            {
                if (!WhatIsAt(MinotaurRow, MinotaurColumn).Right && !WhatIsAt(MinotaurRow, MinotaurColumn + 1).Left)
                {
                    WhatIsAt(MinotaurRow, MinotaurColumn).Minotaur = false;
                    MinotaurColumn++;
                    WhatIsAt(MinotaurRow, MinotaurColumn).Minotaur = true;
                }
            }
            else
            {
                canMoveVertically = true;
            }

            // If minotaur can't move horizontally, attempts to move vertically using previously
            // defined flag variable
            if (canMoveVertically)
            {
                if (MinotaurRow > TheseusRow)
                {
                    if (!WhatIsAt(MinotaurRow, MinotaurColumn).Top && !WhatIsAt(MinotaurRow - 1, MinotaurColumn).Bottom)
                    {
                        WhatIsAt(MinotaurRow, MinotaurColumn).Minotaur = false;
                        MinotaurRow--;
                        WhatIsAt(MinotaurRow, MinotaurColumn).Minotaur = true;
                    }
                }
            }
            else if (MinotaurRow < TheseusRow)
            { 
                if (!WhatIsAt(MinotaurRow, MinotaurColumn).Bottom && !WhatIsAt(MinotaurRow + 1, MinotaurColumn).Top)
                {
                    WhatIsAt(MinotaurRow, MinotaurColumn).Minotaur = false;
                    MinotaurRow++;
                    WhatIsAt(MinotaurRow, MinotaurColumn).Minotaur = true;
                }
            }
            
        }

        public Square WhatIsAt(int row, int col)
        {
            return level[row, col];
        }
    }

    public class Square
    {
        // Grouping square variables
        public bool Minotaur { get; set; }
        public bool Theseus { get; set; }
        public bool Exit { get; set; }

        public bool Top;
        public bool Left;
        public bool Bottom;
        public bool Right;

        // Contructor
        public Square(bool top, bool left, bool bottom, bool right, bool hasMinotaur, bool hasTheseus, bool isExit)
        {
            this.Top = top;
            this.Left = left;
            this.Bottom = bottom;
            this.Right = right;
            this.Minotaur = hasMinotaur;
            this.Theseus = hasTheseus;
            this.Exit = isExit;
        }
    }

    public class Level : ILevel
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Data { get; set; }
    }
}

