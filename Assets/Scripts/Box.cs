using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Box : MonoBehaviour
{
    #region IsMine
        private bool _isMine;
        public bool IsMine{
            get { return _isMine; }
            private set { _isMine = value; }
        }
    #endregion

    #region IsMineClicked
        private bool _isMineClicked;
        public bool IsMineClicked{
            get { return _isMineClicked; }
            private set { _isMineClicked = value; }
        }
    #endregion
    
    #region IsCovered
        private bool _isCovered;

        public bool IsCovered{
            get { return _isCovered; }
            private set { _isCovered = value; }
        }
    #endregion
    
    #region IsFlagged
        private bool _isFlagged;

        public bool IsFlagged{
            get { return _isFlagged; }
            private set { _isFlagged = value; }
        }
    #endregion
    
    #region AdjacentMines
        private int _adjacentMines;
        public int AdjacentMines{
            get { return _adjacentMines; }
            set { _adjacentMines = value; }
        }
    #endregion
    
    #region X
        private int _x;

        public int X{
            get { return _x; }
            private set { _x = value; }
        }
    #endregion

    #region Y
        private int _y;
        
        public int Y{
            get { return _y; }
            private set { _y = value; }
        }
    #endregion

    #region Neighbours
        private List<Box> _neighbours;
        public List<Box> Neighbours{
            get { return _neighbours; }
            internal set { _neighbours = value; }
        }
    #endregion

    private SpriteRenderer spriteRenderer;

    void Awake(){
        //init
        //TODO set global number of mines
        IsMine = Random.value < 0.1;
        IsCovered = true;
        IsFlagged = false;
        IsMineClicked = false;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        Neighbours = new List<Box>();

        // set position
        X = (int)transform.position.x;
        Y = (int)transform.position.y;
        //Register in Grid
        GameManager.Instance.Grid.SetBoxAt(this);
    }

    /// <summary>
    /// Opens the neighbours of the current box and updates the grid and game state
    /// </summary>
    private void OpenNeighbours()
    {
        foreach(Box elem in Neighbours){
            if(elem.IsCovered && !elem.IsFlagged){
                elem.OpenBox();
            }
        }
    }

    /// <summary>
    /// Opens the current Box and updates the grid and game state
    /// </summary>
    private void OpenBox()
    {
        //if a mine is opened we lose
        if (IsMine) {
            LoseGame(this);
        }
        else {
            UncoverBox();
            if (AdjacentMines == 0){
                // uncover area without mines
                GameManager.Instance.FFUncover(X, Y,
                        new bool[GameManager.Instance.Grid.Width,
                                GameManager.Instance.Grid.Height]);
            }
            if(GameManager.Instance.isGameCompleted()){
                WinGame();
            }
        }
    }

    /// <summary>
    /// Checks if there are at least the same number of flagged neighbors and adjacent mines
    /// </summary>
    /// <returns></returns>
    private bool enoughFlaggedNeighbours()
    {
        int flaggedNeighbours = 0;
        foreach(Box box in Neighbours){
            if(checkFlaggedNeighbour(box)){
                flaggedNeighbours++;
            }
        }

        return flaggedNeighbours >= AdjacentMines;
    }

    /// <summary>
    /// Check if the current neighbour is flagged
    /// </summary>
    /// <param name="box">Neighbour</param>
    /// <returns></returns>
    public bool checkFlaggedNeighbour(Box box){
        if(GameManager.Instance.areValidCoordinates(box.X, box.Y)){
            return box.IsCovered && box.IsFlagged;
        }
        return false;
    }

    /// <summary>
    /// Updates the flagged status of the box and updates its texture
    /// </summary>
    private void addFlag()
    {
        IsFlagged = !IsFlagged;
        updateTexture();
    }

    /// <summary>
    /// Sets the <see cref="Box.IsMineClicked"/> to true and starts the game ending operations
    /// </summary>
    /// <param name="elem"></param>
    private void LoseGame(Box elem)
    {
        elem.IsMineClicked = true;
        GameManager.Instance.endGame();
    }

    /// <summary>
    /// Sets the status of the game to won and starts the end game operations
    /// </summary>
    private void WinGame(){
        GameManager.isGameWon = true;
        SceneManager.LoadScene("RestartScene");
    }

    /// <summary>
    /// Updates the texture based on its current state
    /// </summary>
    public void updateTexture(){
        if(IsCovered){
            spriteRenderer.sprite = IsFlagged ? 
                    GameManager.Instance.FlagSprite : 
                    GameManager.Instance.CoveredSprite;
        }else{
            if(IsMine){
                spriteRenderer.sprite = IsMineClicked ? 
                                    GameManager.Instance.ClickedMineSprite : 
                                    GameManager.Instance.MineSprite;
            }
            else{
                spriteRenderer.sprite = GameManager.Instance.getEmptySpriteAt(AdjacentMines);
            }
        }    
    }

    /// <summary>
    /// Sets the <see cref="Box.IsCovered"/> to false and updates its texture
    /// </summary>
    /// <param name="elem"></param>
    public void UncoverBox(){
        IsCovered = false;
        this.updateTexture();
    }

    /// <summary>
    /// Performs the action selected by the <see cref="GameManager.Instance.ButtonManager"/>
    /// </summary>
    public void performAction(){
        if(GameManager.Instance.ButtonManager.IsbombButtonSelected){
            if(IsCovered){
                if(!IsFlagged){
                    OpenBox();
                }
            }else if(enoughFlaggedNeighbours()){
                OpenNeighbours();
            }
        }else if(GameManager.Instance.ButtonManager.IsflagButtonSelected){
            if(IsCovered){
                addFlag();
            }else if(enoughFlaggedNeighbours()){
                OpenNeighbours();
            }
        }
    }    
}
