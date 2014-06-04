using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {
    string registeredGameName = "SERVER_myuniquename";
    float refreshRequestLenght = 3.0f;
    HostData[] hostData;

    private void StartServer()
    {
        //8 max number connection
        Network.InitializeServer(8, 25002, false);
        MasterServer.RegisterHost(registeredGameName, "Main Server Surivival", 
            "Servidor oficial dedicado para survival v0.1");

    }

    void OnServerInitialized(){
        Debug.Log("Servidor iniciado");
    }

    void OnMasterServerEvent(MasterServerEvent masterServerEvent)
    {
        if (masterServerEvent == MasterServerEvent.RegistrationSucceeded)
        {
            Debug.Log("Registro exitoso");
        }
    }

    public IEnumerator RefreshHostList() {
        Debug.Log("Refrescando...");
        MasterServer.RequestHostList(registeredGameName);
        float timeStarted = Time.time;
        float timeEnd = Time.time + refreshRequestLenght;

        while (Time.time < timeEnd)
        {
            hostData = MasterServer.PollHostList();
            yield return new WaitForEndOfFrame();
        }

        if (hostData == null || hostData.Length == 0)
        {
            Debug.Log("No se encontraron servidores.");
        }
        else
        {
            Debug.Log(hostData.Length + " servidores encontrados.");
        }
    }

    public void OnGUI(){
        if (Network.isClient || Network.isServer)
            return;

        if(GUI.Button(new Rect(25f, 25f, 150f, 20f), "Iniciar Servidor")){
            StartServer();
        }

        if(GUI.Button(new Rect(25f, 65f, 150f, 20f), "Refrescar lista de servidores")){
            StartCoroutine("RefreshHostList");
        }

        if (hostData != null)
        {
            for(int i=0; i< hostData.Length; i++)
            {
                if(GUI.Button(new Rect(Screen.width/2, 65f + (30f * i), 300f, 30f), hostData[i].gameName)){
                    Network.Connect(hostData[i]);
                }
            }
        }
    }
}
