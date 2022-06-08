using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDetails : MonoBehaviour
{

    [SerializeField] List<SceneDetails> connectedScenes;

    public bool IsLoaded { get; private set; }

    List<SavableEntity> savableEntities;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            Debug.Log($"Entroto in {gameObject.name}");

            LoadScene();
            GameController.Instance.SetCurrentScene(this);

            //Carica tutte le scene connesse
            foreach(var scene in connectedScenes)
            {
                scene.LoadScene();
            }


            //Distruggi le scene non più connesse
            var prevScene = GameController.Instance.PrevScene;
            if (prevScene != null)
            {
                var previoslyLoadedScenes = prevScene.connectedScenes;

                foreach( var scene in previoslyLoadedScenes)
                {
                    if (!connectedScenes.Contains(scene) && scene != this)
                        scene.UnloadScene();
                }

                if (!connectedScenes.Contains(prevScene))
                    prevScene.UnloadScene();

            }

        }
    }

    public void LoadScene()
    {
        if (!IsLoaded)
        {
            var operation = SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            IsLoaded = true;

            operation.completed += (AsyncOperation op) =>
            {
                savableEntities = GetSavableEntitiesInScene();
                SavingSystem.i.RestoreEntityStates(savableEntities);
            };
            
        }
    }

    public void UnloadScene()
    {
        if (IsLoaded)
        {
            SavingSystem.i.CaptureEntityStates(savableEntities);

            SceneManager.UnloadSceneAsync(gameObject.name);
            IsLoaded = false;
        }
    }

    List<SavableEntity> GetSavableEntitiesInScene()
    {
        var currScene = SceneManager.GetSceneByName(gameObject.name);
        var savEntities = FindObjectsOfType<SavableEntity>().Where(x => x.gameObject.scene == currScene).ToList();
        return savEntities;
    }

}
