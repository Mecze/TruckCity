using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class sProfileManager : Singleton<sProfileManager> {
    /*
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





    void Start()
    {//Esto solo sera ejecutado cuando se crea este objeto.
     //Gracias a que hereda de Singleton<> y "Is Peristent" es true

        //DEVELOPER!
        //sSaveLoad.DeleteSavedGame(); //
        //---DEVELOPER

        //Inicializa el Perfil   (Se hace en el GET)
        Profile pf = sProfileManager.ProfileSingleton;

        //Comprueba si hay una partida guardada... estabamos jugando
        //pf.currentlyPlayingLevel = sSaveLoad.CheckIfSavedGame();

        //Si estabamos jugando:
        if (pf.currentlyPlayingLevel)
        {
            StartCoroutine(ChangeScene(1)); //Vamos al juego
        }else
        {
            StartCoroutine(ChangeScene(2)); //Vamos al menu
        }
        
        


    }

    IEnumerator ChangeScene(int i)
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(i);
    }
    */

}