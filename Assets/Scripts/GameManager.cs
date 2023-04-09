using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}

    #region Grid
        private Grid _grid;
        public Grid Grid{
            get { return _grid; }
        }
    #endregion

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private GameObject box;

    public static bool isGameWon;

    #region EmptySprites
        [SerializeField]
        private Sprite[] _emptySprites;

        /// <summary>
        /// Gets the empty sprite based on the number of adjacent mines
        /// </summary>
        /// <param name="position">Number of adjacent mines</param>
        /// <returns>The appropiate sprite form the emptySprites array</returns>
        public Sprite getEmptySpriteAt(int position){
            return _emptySprites[position];
        }
    #endregion

    #region MineSprite
        [SerializeField]
        private Sprite _mineSprite;

        public Sprite MineSprite{get { return _mineSprite; } }
    #endregion

    #region ClickedMineSprite
        [SerializeField]
        private Sprite _clickedMineSprite;

        public Sprite ClickedMineSprite{get { return _clickedMineSprite; } }
    #endregion

    #region CoveredSprite
        [SerializeField]
        private Sprite _coveredSprite;

        public Sprite CoveredSprite{get { return _coveredSprite; } }
    #endregion
    
    #region FlagSprite
        [SerializeField]
        private Sprite _flagSprite;

        public Sprite FlagSprite{get { return _flagSprite; } }
    #endregion

    #region ButtonManager
        private ButtonManager _buttonManager;
        public ButtonManager ButtonManager{
            get { return _buttonManager; }
        }
    #endregion
    
    void Awake()
    {
        if (Instance != null) {
            Debug.LogError("There is more than one GameManager instance!");
            return;
        }

        Instance = this;

        _buttonManager = gameObject.GetComponent<ButtonManager>();
        isGameWon = false;
    }
    
    void Start(){
        StartGame();
        SetupCamera();
    }

    /// <summary>
    /// Initialization of the camera position and size depending on the size of the grid
    /// </summary>
    public void SetupCamera(){
        Vector2 bottomLeftPos = _grid.GetBoxAt(0, 0).transform.position;
        Vector2 bottomRightPos = _grid.GetBoxAt(_grid.Width - 1, 0).transform.position;
        Vector2 upperRightPos = _grid.GetBoxAt(_grid.Width - 1, _grid.Height -1).transform.position;
        Vector3 middlePos = new Vector3((bottomRightPos.x - bottomLeftPos.x) /2, (upperRightPos.y - bottomRightPos.y) /2, -10);

        cam.transform.position = middlePos;

        float newSize = middlePos.x * 2;
        //offset fix for grids with odd number of elements
        if(_grid.Width * _grid.Height %2 != 0){
            newSize += 1;
        }

        cam.orthographicSize = newSize;
    }

    /// <summary>
    /// Initialization of the game by spawning every <see cref="Box"/> and then initializating some of their attributes
    /// </summary>
    private void StartGame()
    {
        _grid = new Grid();

        //generate map
        for (int x = 0; x < _grid.Width; x++)
        {
            for (int y = 0; y < _grid.Height; y++)
            {
                SpawnElementAt(x, y);
            }
        }
        updateElements();
    }

    /// <summary>
    /// Spawns a <see cref="Box"/> at the given coordinates in a 2d plane
    /// </summary>
    /// <param name="x">x coordinate</param>
    /// <param name="y">y coordinate</param>
    private void SpawnElementAt(int x, int y)
    {
        Vector3 position = new Vector3(x, y, 0);
        Instantiate(box, position, Quaternion.identity);
    }

    private void updateElements()
    {
        foreach(Box box in _grid.Boxes){
            //might be good to implement it just when called
            box.Neighbours = _grid.GetNeighboursOf(box);

            int adjacentMines = _grid.GetAdjacentMinesOf(box);
            box.AdjacentMines = adjacentMines;
            box.updateTexture();
        }
    }

    /// <summary>
    /// Game ends
    /// </summary>
    public void endGame()
    {
        //We uncover the remaining boxes 
        foreach (Box elem in _grid.Boxes){
            if(elem.IsCovered){
                elem.UncoverBox();
            }
        }
        // game finished
        SceneManager.LoadScene("RestartScene");
    }    

    // Flood Fill empty elements
    public void FFUncover(int x, int y, bool[,] visited) {
        if(areValidCoordinates(x, y)){
            // visited already?
            if (visited[x, y])
                return;

            // set visited flag
            visited[x, y] = true;

            Box box = _grid.GetBoxAt(x, y);
            box.UncoverBox();

            if(box.AdjacentMines > 0)
                return;

            // recursion
            foreach(Box neighbour in box.Neighbours){
                FFUncover(neighbour.X, neighbour.Y, visited);
            }
        }
    }

    /// <summary>
    /// Checks if the coordinates are inside the grid
    /// </summary>
    /// <param name="x">x coord</param>
    /// <param name="y">y coord</param>
    /// <returns>whether the coordinates are inside the grid</returns>
    public bool areValidCoordinates(int x, int y){
        return (x >= 0 && y >= 0) && (x < _grid.Width && y < _grid.Height);
    }

    /// <summary>
    /// Checks if the game has ended
    /// </summary>
    /// <returns>whether the game has ended</returns>
    public bool isGameCompleted(){
        foreach(Box element in _grid.Boxes){
            if(element.IsCovered && !element.IsMine){
                return false;
            }
        }
        return true;
    }


    /// <summary>
    /// Checks if the position clicked by the user is a box and then performs the required action on it
    /// </summary>
    /// <param name="touchPosition">position where the user has clicked</param>
    internal void performActionInBoxClicked(Vector3 touchPosition)
    {
        foreach(Box box in Grid.Boxes){

            if(isBetween(touchPosition.x, box.X - 0.5f, box.X + 0.5f) &&
                isBetween(touchPosition.y, box.Y - 0.5f, box.Y + 0.5f)){
                //print("Box clicked: [" + box.X + ", " + box.Y + "]");
                //perform action on box clicked
                box.performAction();
            }
        }
    }

    /// <summary>
    /// Checks if the given coordinate is between the values passed
    /// </summary>
    /// <param name="coord">coord to check</param>
    /// <param name="minValue">minimum value</param>
    /// <param name="maxValue">maximum value</param>
    /// <returns>Whether the given coordinate is between the values passed</returns>
    private bool isBetween(float coord, float minValue, float maxValue){
        return coord >= minValue && coord <= maxValue;
    }
}
