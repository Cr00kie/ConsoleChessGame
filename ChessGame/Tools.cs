using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame
{
    public class Tools
    {
        public static int[,] CreateRectangularArray(List<int[]> list)
        {
            //Hecho por Jon Skeet en stackoverflow.com
            // TODO: Validation and special-casing for list.Count == 0
            if (list.Count == 0) { return new int[,] { { -1, 0 } }; }
            int minorLength = list[0].Length;
            int[,] retArray = new int[list.Count, minorLength];
            for (int i = 0; i < list.Count; i++)
            {
                var array = list[i];
                if (array.Length != minorLength)
                {
                    throw new ArgumentException
                        ("All arrays must be the same length");
                }
                for (int j = 0; j < minorLength; j++)
                {
                    retArray[i, j] = array[j];
                }
            }
            return retArray;
        }
    }
}
