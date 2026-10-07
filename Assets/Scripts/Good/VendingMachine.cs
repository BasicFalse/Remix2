using Photon.Pun;
using UnityEngine;

public class VendingMachine : RemixScript
{
    public enum SodaType
    {
        Nothing = 0,
        Broke = 1,
        Grere = 2,
        Skap = 3,
        Blub = 4,
        Coal = 5,
        Gold = 6,
        Freze = 7
    }

    public SodaType currentSoda;
    public Transform sodaSpawn;
    private string sodaSpawnString;

    // Update is called once per frame
    void Update()
    {
        switch (currentSoda)
        {
            case SodaType.Nothing:
                sodaSpawnString = "SodaCans";
                break;
            case SodaType.Broke:
                sodaSpawnString = "SodaCansBroke";
                break;
            case SodaType.Grere:
                sodaSpawnString = "SodaCans\\Grere";
                break;
            case SodaType.Skap:
                sodaSpawnString = "SodaCans\\Skap";
                break;
            case SodaType.Blub:
                sodaSpawnString = "SodaCans\\Blub";
                break;
            case SodaType.Coal:
                sodaSpawnString = "SodaCans\\Coal";
                break;
            case SodaType.Gold:
                sodaSpawnString = "SodaCans\\Gold";
                break;
            case SodaType.Freze:
                sodaSpawnString = "SodaCans\\Freze";
                break;

        }
        if(sodaSpawnString != "SodaCans" && sodaSpawnString != "SodaCansBroke")
        {
                Quaternion rotation = Quaternion.Euler(sodaSpawn.rotation.eulerAngles);
            if(PhotonNetwork.InLobby || PhotonNetwork.InRoom)
            {
                PhotonNetwork.InstantiateRoomObject(sodaSpawnString, sodaSpawn.position, rotation);
            }
            else
            {
                GameObject soda = (GameObject)Instantiate(Resources.Load(sodaSpawnString));
                Instantiate(soda, sodaSpawn.position, sodaSpawn.rotation);
            }
                
            currentSoda = SodaType.Nothing;
        }
    }
}
