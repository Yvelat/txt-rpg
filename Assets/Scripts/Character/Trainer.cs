using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Trainer/Crea nuovo Trainer")]
public class Trainer : ScriptableObject
{
    [SerializeField] string trainerName;
    [SerializeField] List<Monster> party;

    [Header("DropTable")]
    [SerializeField] Vector2Int coinsRange;
    [SerializeField] Vector2Int gemsRange;

    [HideInInspector]
    public List<Monster> Party;

    public void Init()
    {
        Party = party;

        foreach (var m in Party)
        {
            m.Init();
        }
    }

    public Monster GetHealthyMonster()
    {
        return Party.Where(x => x.HP > 0).FirstOrDefault();
    }

    public int CoinsDrop()
    {
        return coinsRange.y == 0 ? coinsRange.x : Random.Range(coinsRange.x, coinsRange.y + 1);
    }

    public int GemDrop()
    {
        return gemsRange.y == 0 ? gemsRange.x : Random.Range(gemsRange.x, gemsRange.y + 1);
    }

    public int GetPartyCount()
    {
        return party.Count;
    }

    public string Name => trainerName;

}
