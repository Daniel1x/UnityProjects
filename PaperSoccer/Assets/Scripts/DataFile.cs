using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataFile", menuName = "ScriptableObjects/DataFile", order = 1)]
public class DataFile : ScriptableObject {

    public int PlayerOneWins = 0;
    public int PlayerTwoWins = 0;
}
