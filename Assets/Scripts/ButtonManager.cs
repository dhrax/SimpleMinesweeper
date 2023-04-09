using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{

    [SerializeField]
    private Button bombButton;
    [SerializeField]
    private Button flagButton;

    [SerializeField]
    private Sprite bombButtonSelected;

    [SerializeField]
    private Sprite bombButtonNotSelected;

    [SerializeField]
    private Sprite flagButtonSelected;

    [SerializeField]
    private Sprite flagButtonNotSelected;

    private bool isbombButtonSelected, isflagButtonSelected;
    public bool IsbombButtonSelected{
        get{ return isbombButtonSelected;}
        set{isbombButtonSelected = value;}
    }

    public bool IsflagButtonSelected{
        get{ return isflagButtonSelected;}
        set{isflagButtonSelected = value;}
    }
    
    void Start()
    {
        updateState();
    }

    /// <summary>
    /// Bomb button clicked so it state is updated
    /// </summary>
    public void BombButtonClicked(){
        if(!isbombButtonSelected){
            updateState();
        }
    }

    /// <summary>
    /// Flag button clicked so it state is updated
    /// </summary>
    public void FlagButtonClicked(){
        if(!isflagButtonSelected){
            updateState(true);
        }
    }

    /// <summary>
    /// Sets the button states and updates their textures accordingly
    /// </summary>
    /// <param name="flagClicked"></param>
    private void updateState(bool flagClicked = false)
    {
        isbombButtonSelected = !flagClicked;
        isflagButtonSelected = flagClicked;

        bombButton.image.sprite = isbombButtonSelected ? 
                                    bombButtonSelected : 
                                    bombButtonNotSelected;

        flagButton.image.sprite = isflagButtonSelected ? 
                                    flagButtonSelected : 
                                    flagButtonNotSelected;
    }
}
