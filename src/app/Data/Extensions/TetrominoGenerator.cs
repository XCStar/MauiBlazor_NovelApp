using MauiApp3.Model.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp3.Data.Extensions
{
    public class TetrominoGenerator
    {
        public Tetromino CreateFromStyle(TetrominoStyle style, Model.Game.Grid grid)
        {
            return style switch
            {
                TetrominoStyle.Block => new Block(grid),
                TetrominoStyle.Straight => new Straight(grid),
                TetrominoStyle.TShaped => new TShaped(grid),
                TetrominoStyle.LeftZigZag => new LeftZigZag(grid),
                TetrominoStyle.RightZigZag => new RightZigZag(grid),
                TetrominoStyle.LShaped => new LShaped(grid),
                TetrominoStyle.ReverseLShaped => new ReverseLShaped(grid),
                _ => new Block(grid),
            };
        }
        public TetrominoStyle Next(params TetrominoStyle[] unusableStyles)
        {
            Random rand = new Random(DateTime.Now.Millisecond);

            //Randomly generate one of the eight possible tetrominos
            var style = (TetrominoStyle)rand.Next(1, 8);

            //Re-generate the new tetromino until it is 
            //a style that is not one of the upcoming styles.
            while (unusableStyles.Contains(style))
                style = (TetrominoStyle)rand.Next(1, 8);

            return style;
        }
    }
}
