using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FieldHandler : MonoBehaviour
{
    public Text myName;
    // Start is called before the first frame update
    void Start()
    {
        myName.text = PlayerNameController.playerName;
    }
}
