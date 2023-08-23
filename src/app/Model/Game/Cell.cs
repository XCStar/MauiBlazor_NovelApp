
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp3.Model.Game
{
    public class Cell
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public string CssClass { get; set; }
        public Cell(int row, int column, string css)
        {
            this.Row = row;
            this.Column = column;
            this.CssClass = css;
        }
        public Cell(int row, int column) : this(row, column, "")
        {

        }
    }
    public class CellCollection
    {
        public List<Cell> _cells { get; set; } = new List<Cell>();
        public bool HasRow(int row)
        {
            return _cells.Any(x => x.Row == row);
        }
        public bool HasColumn(int column)
        {
            return _cells.Any(x => x.Column == column);
        }
        public bool Contains(int row, int column)
        {
            return _cells.Any(x => x.Row == row && x.Column == column);
        }
        public void Add(Cell cell)
        {
            _cells.Add(cell);
        }
        public void Add(int row, int column)
        {
            Add(new Cell(row, column));
        }
        public void AddMany(List<Cell> cells, string cssClass)
        {
            foreach (Cell cell in cells)
            {
                _cells.Add(new Cell(cell.Row, cell.Column, cssClass));
            }
        }
        public List<Cell> GetAll()
        {
            return _cells;
        }
        public List<Cell> GetAllInRow(int col)
        {
            return _cells.Where(x => x.Column == col).ToList();
        }
        public void SetCssClass(int row, string cssClass)
        {
            _cells.Where(x => x.Row == row).ToList().ForEach(x => x.CssClass = cssClass);
        }
        public string GetCssClass(int row, int column)
        {
            return _cells.Where(x=>x.Row==row&&x.Column==column).FirstOrDefault().CssClass;
        }
        public void CollapseRows(List<int> rows)
        {
            var selectedCells = _cells.Where(x => rows.Contains(x.Row));
            var toRemove = new List<Cell>();
            foreach (var item in selectedCells)
            {
                toRemove.Add(item);
            }
            _cells.RemoveAll(x => toRemove.Contains(x));
            foreach (var cell in _cells)
            {
                int numberOfLessRows = rows.Where(x => x <= cell.Row).Count();
                cell.Column -= numberOfLessRows;
            }
        }
        public List<Cell> GetRightmost()
        {
            var cells = new List<Cell>();
            foreach (var item in _cells)
            {
                if (!Contains(item.Row+1, item.Column))
                {
                    cells.Add(item);
                }
            }
            return cells;
        }
        public List<Cell> GetLeftmost()
        {
            var cells = new List<Cell>();
            foreach (var item in _cells)
            {
                if (!Contains(item.Row-1, item.Column))
                {
                    cells.Add(item);
                }
            }
            return cells;
        }
        public List<Cell> GetLowest()
        {
            var cells = new List<Cell>();
            foreach (var item in _cells)
            {
                if (!Contains(item.Row, item.Column-1))
                {
                    cells.Add(item);
                }
            }
            return cells;
        }
    }
    public enum GameState
    {
        NotStarted,
        Playing,
        GameOver
    }
    public enum TetrominoStyle
    {
        Straight=1,
        Block,
        TShaped,
        LeftZigZag,
        RightZigZag,
        LShaped,
        ReverseLShaped
    }
    public enum TetrominoOrientation
    {
        UpDown,
        LeftRight,
        DownUp,
        RightLeft
    }
    public class Grid

    {
        public Grid(int width,int height)
        {
            this.Width = width;
            this.Height = height;
        }
        public Grid():this(10,20)
        {
            
        }
        public int Height { get; internal set; }
        public int Width { get; internal set; }
        public CellCollection Cells { get; set; } = new CellCollection();
        public GameState State { get; internal set; } = GameState.NotStarted;
        public bool IsStarted
        {
            get
            {
                return State == GameState.Playing || State == GameState.GameOver;
            }
        }
    }
    public class Tetromino
    {
        public Tetromino(Grid grid)
        {
            this.Grid = grid;
             CenterPieceColumn = grid.Height;
            CenterPieceRow = grid.Width / 2;
        }
        public Grid Grid { get; set; }
        public TetrominoOrientation Orientation { get; set; }
        = TetrominoOrientation.LeftRight;
        public int CenterPieceRow { get; set; }
        public int CenterPieceColumn { get; set; }
        public virtual TetrominoStyle Style { get; }
        public virtual string CssClass { get; set; }
        public virtual CellCollection CoveredCells { get; }
        public bool CanMoveLeft()
        {
            foreach (Cell cell in CoveredCells.GetLeftmost())
            {
                if (Grid.Cells.Contains(cell.Row-1, cell.Column))
                {
                    return false;
                }
            }
            if (CoveredCells.HasRow(1))
            {
                return false;
            }
            return true;
        }
        public bool CanMoveRight()
        {
            foreach (Cell cell in CoveredCells.GetRightmost())
            {
                if (Grid.Cells.Contains(cell.Row+1, cell.Column))
                {
                    return false;
                }
            }
            if (CoveredCells.HasRow(Grid.Width))
            {
                return false;
            }
            return true;
        }
        public void MoveLeft()
        {
            if (CanMoveLeft())
            {
                 CenterPieceRow--;
            }
        }
        public void MoveRight()
        {
            if (CanMoveRight())
            {
                CenterPieceRow++;
            }
        }
        public bool CanMoveDown()
        {
            foreach (var coord in CoveredCells.GetLowest())
            {
                if (Grid.Cells.Contains(coord.Row, coord.Column-1))
                {
                    return false;
                }
            }
            if (CoveredCells.HasColumn(1))
            {
                return false;
            }
            return true;
        }
        public void MoveDown()
        {
            if (CanMoveDown())
            {
                CenterPieceColumn--;
            }
        }
        public void Rotate()
        {
            switch (Orientation)
            {
                case TetrominoOrientation.UpDown:
                    Orientation = TetrominoOrientation.RightLeft;
                    break;
                case TetrominoOrientation.RightLeft:
                    Orientation = TetrominoOrientation.DownUp;
                    break;
                case TetrominoOrientation.DownUp:
                    Orientation = TetrominoOrientation.LeftRight;
                    break;
                case TetrominoOrientation.LeftRight:
                    Orientation = TetrominoOrientation.UpDown;
                    break;
            }
            var coveredSpaces = CoveredCells;
            if (coveredSpaces.HasColumn(-1))
            {
                CenterPieceColumn += 2;
            }
            else if (coveredSpaces.HasColumn(12))
            {
                CenterPieceColumn -= 2;
            }
            else if (coveredSpaces.HasColumn(0))
            {
                CenterPieceColumn++;
            }
            else if (coveredSpaces.HasColumn(11))
            {
                CenterPieceColumn--;
            }
        }
    }
    public class Block : Tetromino
    {
        public Block(Grid grid) : base(grid)
        {

        }
        public override TetrominoStyle Style => TetrominoStyle.Block;
        public override string CssClass => "tetris-yellow-cell";
        public override CellCollection CoveredCells
        {
            get
            {
                CellCollection cells = new CellCollection();
                cells.Add(CenterPieceRow, CenterPieceColumn);
                cells.Add(CenterPieceRow, CenterPieceColumn + 1);
                cells.Add(CenterPieceRow - 1, CenterPieceColumn);
                cells.Add(CenterPieceRow - 1, CenterPieceColumn + 1);
                return cells;
            }
        }
    }
    public class Straight : Tetromino
    {
        public Straight(Grid grid) : base(grid)
        {
        }
        public override TetrominoStyle Style => TetrominoStyle.Straight;
        public override string CssClass => "tetris-lightblue-cell";
        public override CellCollection CoveredCells
        {
            get
            {
                var cells = new CellCollection();
                cells.Add(CenterPieceRow, CenterPieceColumn);
                if (this.Orientation == TetrominoOrientation.LeftRight)
                {
                    cells.Add(CenterPieceRow, CenterPieceColumn + 1);
                    cells.Add(CenterPieceRow, CenterPieceColumn - 1);
                    cells.Add(CenterPieceRow, CenterPieceColumn - 2);
                }
                else if (this.Orientation == TetrominoOrientation.DownUp)
                {
                    cells.Add(CenterPieceRow - 1, CenterPieceColumn);
                    cells.Add(CenterPieceRow + 1, CenterPieceColumn);
                    cells.Add(CenterPieceRow + 2, CenterPieceColumn);
                }
                else if (this.Orientation == TetrominoOrientation.RightLeft)
                {

                    cells.Add(CenterPieceRow, CenterPieceColumn + 1);
                    cells.Add(CenterPieceRow, CenterPieceColumn - 1);
                    cells.Add(CenterPieceRow, CenterPieceColumn + 2);
                }
                else if (this.Orientation == TetrominoOrientation.UpDown)
                {
                    cells.Add(CenterPieceRow - 1, CenterPieceColumn);
                    cells.Add(CenterPieceRow + 1, CenterPieceColumn);
                    cells.Add(CenterPieceRow + 2, CenterPieceColumn);
                }
                return cells;
            }
        }

    }
    public class LShaped : Tetromino
    {
        public LShaped(Grid grid) : base(grid)
        {
        }
        public override TetrominoStyle Style => TetrominoStyle.LShaped;
        public override string CssClass => "tetris-orange-cell";
        public override CellCollection CoveredCells
        {
            get
            {
                var cells = new CellCollection();
                cells.Add(CenterPieceRow, CenterPieceColumn);

                switch (this.Orientation)
                {

                    case TetrominoOrientation.LeftRight:
                        cells.Add(CenterPieceRow, CenterPieceColumn - 1);
                        cells.Add(CenterPieceRow, CenterPieceColumn - 2);
                        cells.Add(CenterPieceRow + 1, CenterPieceColumn);
                        break;

                    case TetrominoOrientation.DownUp:
                        cells.Add(CenterPieceRow, CenterPieceColumn + 1);
                        cells.Add(CenterPieceRow + 1, CenterPieceColumn);
                        cells.Add(CenterPieceRow + 2, CenterPieceColumn);
                        break;

                    case TetrominoOrientation.RightLeft:
                        cells.Add(CenterPieceRow, CenterPieceColumn + 1);
                        cells.Add(CenterPieceRow, CenterPieceColumn + 2);
                        cells.Add(CenterPieceRow - 1, CenterPieceColumn);
                        break;

                    case TetrominoOrientation.UpDown:
                        cells.Add(CenterPieceRow, CenterPieceColumn - 1);
                        cells.Add(CenterPieceRow - 1, CenterPieceColumn);
                        cells.Add(CenterPieceRow - 2, CenterPieceColumn);
                        break;
                }
                return cells;
            }
        }
    }
    public class TShaped : Tetromino
    {
        public TShaped(Grid grid) : base(grid)
        {
        }
        public override TetrominoStyle Style => TetrominoStyle.TShaped;
        public override string CssClass => "tetris-purple-cell";
        public override CellCollection CoveredCells
        {
            get
            {
                var cells = new CellCollection();
                cells.Add(CenterPieceRow, CenterPieceColumn);

                switch (this.Orientation)
                {

                    case TetrominoOrientation.LeftRight:
                        cells.Add(CenterPieceRow, CenterPieceColumn + 1);
                        cells.Add(CenterPieceRow, CenterPieceColumn - 1);
                        cells.Add(CenterPieceRow-1, CenterPieceColumn);
                        break;

                    case TetrominoOrientation.DownUp:
                        cells.Add(CenterPieceRow-1, CenterPieceColumn);
                        cells.Add(CenterPieceRow + 1, CenterPieceColumn);
                        cells.Add(CenterPieceRow , CenterPieceColumn+1);
                        break;

                    case TetrominoOrientation.RightLeft:
                        cells.Add(CenterPieceRow, CenterPieceColumn + 1);
                        cells.Add(CenterPieceRow, CenterPieceColumn -1 );
                        cells.Add(CenterPieceRow + 1, CenterPieceColumn);
                        break;

                    case TetrominoOrientation.UpDown:
                        cells.Add(CenterPieceRow - 1, CenterPieceColumn);
                        cells.Add(CenterPieceRow + 1, CenterPieceColumn);
                        cells.Add(CenterPieceRow, CenterPieceColumn-1);
                        break;
                }
                return cells;
            }
        }
    }
    public class LeftZigZag : Tetromino
    {
        public LeftZigZag(Grid grid) : base(grid) { }

        public override TetrominoStyle Style => TetrominoStyle.LeftZigZag;

        public override string CssClass => "tetris-red-cell";

        public override CellCollection CoveredCells
        {
            get
            {
                CellCollection cells = new CellCollection();
                cells.Add(CenterPieceRow, CenterPieceColumn);

                switch (Orientation)
                {
                    case TetrominoOrientation.LeftRight:
                        cells.Add(CenterPieceRow + 1, CenterPieceColumn);
                        cells.Add(CenterPieceRow + 1, CenterPieceColumn - 1);
                        cells.Add(CenterPieceRow, CenterPieceColumn + 1);
                        break;

                    case TetrominoOrientation.DownUp:
                        cells.Add(CenterPieceRow - 1, CenterPieceColumn);
                        cells.Add(CenterPieceRow, CenterPieceColumn + 1);
                        cells.Add(CenterPieceRow + 1, CenterPieceColumn + 1);
                        break;

                    case TetrominoOrientation.RightLeft:
                        cells.Add(CenterPieceRow, CenterPieceColumn - 1);
                        cells.Add(CenterPieceRow - 1, CenterPieceColumn);
                        cells.Add(CenterPieceRow - 1, CenterPieceColumn + 1);
                        break;

                    case TetrominoOrientation.UpDown:
                        cells.Add(CenterPieceRow + 1, CenterPieceColumn);
                        cells.Add(CenterPieceRow, CenterPieceColumn - 1);
                        cells.Add(CenterPieceRow - 1, CenterPieceColumn - 1);
                        break;
                }
                return cells;
            }
        }
    }
    public class ReverseLShaped : Tetromino
    {
        public ReverseLShaped(Grid grid) : base(grid) { }

        public override TetrominoStyle Style => TetrominoStyle.ReverseLShaped;

        public override string CssClass => "tetris-darkblue-cell";

        public override CellCollection CoveredCells
        {
            get
            {
                CellCollection cells = new CellCollection();
                cells.Add(CenterPieceRow, CenterPieceColumn);

                switch (Orientation)
                {
                    case TetrominoOrientation.LeftRight:
                        cells.Add(CenterPieceRow, CenterPieceColumn + 1);
                        cells.Add(CenterPieceRow, CenterPieceColumn + 2);
                        cells.Add(CenterPieceRow + 1, CenterPieceColumn);
                        break;

                    case TetrominoOrientation.DownUp:
                        cells.Add(CenterPieceRow, CenterPieceColumn + 1);
                        cells.Add(CenterPieceRow - 1, CenterPieceColumn);
                        cells.Add(CenterPieceRow - 2, CenterPieceColumn);
                        break;

                    case TetrominoOrientation.RightLeft:
                        cells.Add(CenterPieceRow, CenterPieceColumn - 1);
                        cells.Add(CenterPieceRow, CenterPieceColumn - 2);
                        cells.Add(CenterPieceRow - 1, CenterPieceColumn);
                        break;

                    case TetrominoOrientation.UpDown:
                        cells.Add(CenterPieceRow, CenterPieceColumn - 1);
                        cells.Add(CenterPieceRow + 1, CenterPieceColumn);
                        cells.Add(CenterPieceRow + 2, CenterPieceColumn);
                        break;
                }
                return cells;
            }
        }
    }

    public class RightZigZag : Tetromino
    {
        public RightZigZag(Grid grid) : base(grid) { }

        public override TetrominoStyle Style => TetrominoStyle.RightZigZag;

        public override string CssClass => "tetris-green-cell";

        public override CellCollection CoveredCells
        {
            get
            {
                CellCollection cells = new CellCollection();
                cells.Add(CenterPieceRow, CenterPieceColumn);

                switch (Orientation)
                {
                    case TetrominoOrientation.LeftRight:
                        cells.Add(CenterPieceRow, CenterPieceColumn - 1);
                        cells.Add(CenterPieceRow + 1, CenterPieceColumn);
                        cells.Add(CenterPieceRow + 1, CenterPieceColumn + 1);
                        break;

                    case TetrominoOrientation.DownUp:
                        cells.Add(CenterPieceRow, CenterPieceColumn + 1);
                        cells.Add(CenterPieceRow + 1, CenterPieceColumn);
                        cells.Add(CenterPieceRow - 1, CenterPieceColumn + 1);
                        break;

                    case TetrominoOrientation.RightLeft:
                        cells.Add(CenterPieceRow, CenterPieceColumn + 1);
                        cells.Add(CenterPieceRow - 1, CenterPieceColumn);
                        cells.Add(CenterPieceRow - 1, CenterPieceColumn - 1);
                        break;

                    case TetrominoOrientation.UpDown:
                        cells.Add(CenterPieceRow, CenterPieceColumn - 1);
                        cells.Add(CenterPieceRow - 1, CenterPieceColumn);
                        cells.Add(CenterPieceRow + 1, CenterPieceColumn - 1);
                        break;
                }
                return cells;
            }
        }
    }
}
