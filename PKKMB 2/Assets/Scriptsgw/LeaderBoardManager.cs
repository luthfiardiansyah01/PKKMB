using UnityEngine;

public class LeaderBoardManager : MonoBehaviour
{
    PlayFabLeaderboard playFabLeaderboard;
    public GameObject lbListPrefab; // Prefab list leaderboard
    public Transform content;       // Parent content dari ScrollView

    void Start()
    {
        for (int i = 0; i < 7; i++)
        {
            Instantiate(lbListPrefab, content);
        }
    }
}

// using System.Collections.Generic;
// using UnityEngine;
// using PlayFab.ClientModels;

// public class LeaderBoardManager : MonoBehaviour
// {
//     public GameObject lbListPrefab;
//     public Transform content;
    
    

//     public void UpdateLeaderboard(List<PlayerLeaderboardEntry> leaderboardData)
//     {
//         // Hapus semua anak dari content dulu (jika ada)
//         foreach (Transform child in content)
//         {
//             Destroy(child.gameObject);
//         }

//         // Loop data dari PlayFab
//         foreach (var entry in leaderboardData)
//         {
//             Debug.Log("muncul");
//             GameObject item = Instantiate(lbListPrefab, content);
//             LBListItem listItem = item.GetComponent<LBListItem>();

//             string name = string.IsNullOrEmpty(entry.DisplayName) ? entry.PlayFabId : entry.DisplayName;
//             listItem.SetData(entry.Position + 1, name, entry.StatValue);
//         }
//     }
// }
