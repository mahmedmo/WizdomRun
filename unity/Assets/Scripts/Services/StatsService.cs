using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class StatsService : BaseService
{
    public IEnumerator GetPlayerStats(int campaignID, string firebaseToken, System.Action<PlayerStats> onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/stats/{campaignID}";
        while (true)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            request.SetRequestHeader("Authorization", "Bearer " + firebaseToken);
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                PlayerStats stats = JsonUtility.FromJson<PlayerStats>(request.downloadHandler.text);
                onSuccess?.Invoke(stats);
                break;
            }
            else
            {
                onError?.Invoke(request.error);
                yield return new WaitForSeconds(1f);
            }
        }
    }

    public IEnumerator UpdatePlayerStats(int campaignID, int gold, string affinity, string firebaseToken, System.Action onSuccess, System.Action<string> onError)
    {
        // Parses affinity for a true null value if applicable
        string affinityParse = affinity == null ? "null" : $"\"{affinity}\"";

        string url = $"{baseUrl}/stats/update/{campaignID}";
        string jsonData = $"{{\"mana\":{gold},\"affinity\":{affinityParse}}}";
        while (true)
        {
            UnityWebRequest request = UnityWebRequest.Put(url, jsonData);
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + firebaseToken);
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke();
                break;
            }
            else
            {
                onError?.Invoke(request.error);
                yield return new WaitForSeconds(1f);
            }
        }
    }

    public IEnumerator GetSpells(string firebaseToken, System.Action<List<Spell>> onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/stats/spells";
        while (true)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            request.SetRequestHeader("Authorization", "Bearer " + firebaseToken);
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                // Wrap the response JSON as expected.
                SpellListWrapper wrapper = JsonUtility.FromJson<SpellListWrapper>("{\"spells\":" + request.downloadHandler.text + "}");
                onSuccess?.Invoke(wrapper.spells);
                break;
            }
            else
            {
                onError?.Invoke(request.error);
                yield return new WaitForSeconds(1f);
            }
        }
    }

    // Assign spells to a player.
    // Expects exactly 4 spell IDs
    public IEnumerator AssignSpells(int campaignID, List<int> spellIDs, string firebaseToken, System.Action onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/stats/assign_spells";
        string spellIDsJson = string.Join(",", spellIDs);
        string jsonData = $"{{\"campaignID\":{campaignID},\"spellIDs\":[{spellIDsJson}]}}";

        while (true)
        {
            UnityWebRequest request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + firebaseToken);

            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke();
                break;
            }
            else
            {
                onError?.Invoke(request.error);
                yield return new WaitForSeconds(1f);
            }
        }
    }

    // Get the spell IDs assigned to a player.
    public IEnumerator GetPlayerSpells(int campaignID, string firebaseToken, System.Action<List<int>> onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/stats/player_spells/{campaignID}";
        while (true)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            request.SetRequestHeader("Authorization", "Bearer " + firebaseToken);
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                PlayerSpellListWrapper wrapper = JsonUtility.FromJson<PlayerSpellListWrapper>("{\"playerSpells\":" + request.downloadHandler.text + "}");
                List<int> spellIDs = new List<int>();
                foreach (var ps in wrapper.playerSpells)
                {
                    spellIDs.Add(ps.spellID);
                }
                onSuccess?.Invoke(spellIDs);
                break;
            }
            else
            {
                onError?.Invoke(request.error);
                yield return new WaitForSeconds(1f);
            }
        }
    }

    // Create new player stats.
    public IEnumerator CreatePlayerStats(int campaignID, float attack, int hp, int gold, string affinity, string firebaseToken, System.Action onSuccess, System.Action<string> onError)
    {
        // Parses affinity for a true null value if applicable
        string affinityParse = affinity == null ? "null" : $"\"{affinity}\"";

        string url = $"{baseUrl}/stats/create";
        string jsonData = $"{{\"campaignID\":{campaignID},\"attack\":{attack},\"hp\":{hp},\"mana\":{gold},\"affinity\":{affinityParse}}}";
        while (true)
        {
            UnityWebRequest request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + firebaseToken);

            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke();
                break;
            }
            else
            {
                onError?.Invoke(request.error);
                yield return new WaitForSeconds(1f);
            }
        }
    }

    // Response Objects

    [System.Serializable]
    public class PlayerStats
    {
        public int playerID;
        public int mana;
        public string affinity;
    }

    [System.Serializable]
    public class Spell
    {
        public int spellID;
        public string name;
        public string element;
    }

    [System.Serializable]
    public class SpellListWrapper
    {
        public List<Spell> spells;
    }

    [System.Serializable]
    public class PlayerSpell
    {
        public int playerspellID;
        public int spellID;
        public string spellName;
        public string spellElement;
    }

    [System.Serializable]
    public class PlayerSpellListWrapper
    {
        public List<PlayerSpell> playerSpells;
    }

    [System.Serializable]
    public class ManaResponse
    {
        public int newMana;
    }
}