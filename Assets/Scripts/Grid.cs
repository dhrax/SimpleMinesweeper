using System.Collections.Generic;
public class Grid{

    #region width
        private int _width = 50;
        public int Width{
            get { return _width; }
            set { _width = value; }
        }
    #endregion

    #region height
        private int _height = 50;
        public int Height{
            get { return _height; }
            set { _height = value; }
        }
    #endregion

    #region boxes
        private Box[,] _boxes;
        public Box[,] Boxes{
            get { return _boxes; }
        }
    #endregion
    
    public Grid(){
        _boxes = new Box[_width, _height];
    }

    /// <summary>
    /// Return the neighbours (in 8 directions) of a certain <see cref="Box"/>
    /// </summary>
    /// <param name="gridPositionX"></param>
    /// <param name="gridPositionY"></param>
    /// <returns></returns>
    public List<Box> GetNeighboursOf(Box box)
    {
        List<Box> neighbours = new List<Box>();
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x == 0 && y == 0) {
                    continue; // You are not neighbour to yourself
                }
                if (GameManager.Instance.areValidCoordinates(box.X + x, box.Y + y)) {
                    neighbours.Add(GetBoxAt(box.X + x, box.Y + y));
                }
            }
        }

        return neighbours;
    }

    /// <summary>
    /// Gets the <see cref="Box"/> at a given position
    /// </summary>
    /// <param name="x">column</param>
    /// <param name="y">file</param>
    /// <returns><see cref="Box"/> in this position</returns>
    public Box GetBoxAt(int x, int y)
    {
        return _boxes[x, y];
    }

    /// <summary>
    /// Sets the <see cref="Box"/> at a given position
    /// </summary>
    /// <param name="x">column</param>
    /// <param name="y">file</param>
    /// <returns><see cref="Box"/> to be added</returns>
    public void SetBoxAt(Box box){
        _boxes[box.X, box.Y] = box;
    }

    // 
    /// <summary>
    /// Counts the adjacent number of mines of a <see cref="Box"/>
    /// </summary>
    /// <param name="box">box</param>
    /// <returns>Number of neighbour mines</returns>
    public int GetAdjacentMinesOf(Box box) {
        int count = 0;

        foreach(Box neighbour in box.Neighbours){
            if (neighbour.IsMine){
                ++count;
            } 
        }
        return count;
    }
}