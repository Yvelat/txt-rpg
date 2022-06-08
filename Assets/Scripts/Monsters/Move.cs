using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    public MoveBase Base { get; set; }
    public int StaminaCost { get; set; }

    public Move(MoveBase pBase)
    {
        Base = pBase;
        StaminaCost = pBase.StaminaCost;
    }

    
    public Move(MoveSaveData saveData)
    {
        Base = MoveDB.GetObjectByName(saveData.name);
        StaminaCost = saveData.staminaCost;
    }

    
    public MoveSaveData GetSaveData()
    {
        var saveData = new MoveSaveData()
        {
            name = Base.name,
            staminaCost = StaminaCost
        };
        return saveData;
    }
}


[System.Serializable]
public class MoveSaveData
{
    public string name;
    public int staminaCost;
}