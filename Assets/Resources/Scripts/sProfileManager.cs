using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class sProfileManager : Singleton<sProfileManager> {
    


    static Profile _singleton = null;
    public static Profile ProfileSingleton
    {
        get
        {
            if (_singleton == null)
            {
                Profile pf = sSaveLoad.LoadProfile();
                if (pf == null || pf.version == "" || pf.version != sProfileManager.instance.defaultProfile.version)
                {
                    Debug.Log("NEW PROFILE!");
                    _singleton = sProfileManager.instance.defaultProfile;
                    sSaveLoad.savedProfile = _singleton;
                }
                else
                {
                    _singleton = pf;
                    sSaveLoad.savedProfile = _singleton;
                    Debug.Log("Profile Loaded");
                }
            }
            return _singleton;


        }
        set
        {
            _singleton = value;
        }
    }

    public Profile defaultProfile;

    public List<LevelConditions> levelconditions;




    void Start()
    {//Esto solo sera ejecutado cuando se crea este objeto.
     //Gracias a que hereda de Singleton<> y "Is Peristent" es true

        //DEVELOPER!
        //sSaveLoad.DeleteSavedGame(); //
        //---DEVELOPER


        //Inicializa el Perfil   (Se hace en el GET)
#pragma warning disable 0219
        Profile pf = sProfileManager.ProfileSingleton;
#pragma warning restore 0219
        //pf no vale para nada realmente! (Por ahora)

        //Comprueba si hay una partida guardada... estabamos jugando
        //pf.currentlyPlayingLevel = sSaveLoad.CheckIfSavedGame();

        //Si estabamos jugando:

        StartCoroutine(ChangeScene(1)); //Vamos al menu
        
        
        


    }

    IEnumerator ChangeScene(int i)
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(i);
    }
    

}