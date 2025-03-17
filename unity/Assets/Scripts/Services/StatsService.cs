using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
public class StatsService : BaseService
{
    // Get player stats.
    public IEnumerator GetPlayerStats(int campaignID, string firebaseToken, System.Action<PlayerStats> onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/stats/{campaignID}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", "Bearer " + firebaseToken);
        yield return SendRequest(request,
            response =>
            {
                PlayerStats stats = JsonUtility.FromJson<PlayerStats>(response);
                onSuccess?.Invoke(stats);
            },
            error => { onError?.Invoke(error); }
        );
    }

    // Update player stats.
    public IEnumerator UpdatePlayerStats(int campaignID, float attack, int hp, int mana, string firebaseToken, System.Action onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/stats/update/{campaignID}";
        string jsonData = $"{{\"attack\":{attack},\"hp\":{hp},\"mana\":{mana}}}";
        UnityWebRequest request = UnityWebRequest.Put(url, jsonData);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + firebaseToken);
        yield return SendRequest(request,
            response => { onSuccess?.Invoke(); },
            error => { onError?.Invoke(error); }
        );
    }

    // Get list of spells.
    public IEnumerator GetSpells(string firebaseToken, System.Action<List<Spell>> onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/stats/spells";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", "Bearer " + firebaseToken);
        yield return SendRequest(request,
            response =>
            {
                SpellListWrapper wrapper = JsonUtility.FromJson<SpellListWrapper>("{\"spells\":" + response + "}");
                onSuccess?.Invoke(wrapper.spells);
            },
            error => { onError?.Invoke(error); }
        );
    }

    // Assign a spell to a player.
    public IEnumerator AssignSpell(int campaignID, int spellID, string firebaseToken, System.Action onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/stats/assign_spell";
        string jsonData = $"{{\"campaignID\":{campaignID},\"spellID\":{spellID}}}";
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + firebaseToken);
        yield return SendRequest(request,
            response => { onSuccess?.Invoke(); },
            error => { onError?.Invoke(error); }
        );
    }

    // Get the spells assigned to a player.
    public IEnumerator GetPlayerSpells(int campaignID, string firebaseToken, System.Action<List<PlayerSpell>> onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/stats/player_spells/{campaignID}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", "Bearer " + firebaseToken);
        yield return SendRequest(request,
            response =>
            {
                PlayerSpellListWrapper wrapper = JsonUtility.FromJson<PlayerSpellListWrapper>("{\"playerSpells\":" + response + "}");
                onSuccess?.Invoke(wrapper.playerSpells);
            },
            error => { onError?.Invoke(error); }
        );
    }

    // Create new player stats.
    public IEnumerator CreatePlayerStats(int campaignID, float attack, int hp, int mana, string affinity, string firebaseToken, System.Action onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/stats/create";
        string jsonData = $"{{\"campaignID\":{campaignID},\"attack\":{attack},\"hp\":{hp},\"mana\":{mana},\"affinity\":\"{affinity}\"}}";
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + firebaseToken);
        yield return SendRequest(request,
            response => { onSuccess?.Invoke(); },
            error => { onError?.Invoke(error); }
        );
    }

    // Delete an assigned player spell.
    public IEnumerator DeletePlayerSpell(int playerSpellID, string firebaseToken, System.Action onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/stats/player_spells/delete/{playerSpellID}";
        UnityWebRequest request = UnityWebRequest.Delete(url);
        request.SetRequestHeader("Authorization", "Bearer " + firebaseToken);
        yield return SendRequest(request,
            response => { onSuccess?.Invoke(); },
            error => { onError?.Invoke(error); }
        );
    }

    // Create a new spell.
    public IEnumerator CreateSpell(string spellName, string description, string spellElement, string firebaseToken, System.Action onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/stats/spells/create";
        string jsonData = $"{{\"spellName\":\"{spellName}\",\"description\":\"{description}\",\"spellElement\":\"{spellElement}\"}}";
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + firebaseToken);
        yield return SendRequest(request,
            response => { onSuccess?.Invoke(); },
            error => { onError?.Invoke(error); }
        );
    }

    // Replenish mana for a player.
    public IEnumerator ReplenishMana(int campaignID, int manaAmount, string firebaseToken, System.Action<int> onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/stats/replenish_mana/{campaignID}";
        string jsonData = $"{{\"manaAmount\":{manaAmount}}}";
        UnityWebRequest request = UnityWebRequest.Put(url, jsonData);
        request.method = "PATCH";
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + firebaseToken);
        yield return SendRequest(request,
            response =>
            {
                ManaResponse manaResponse = JsonUtility.FromJson<ManaResponse>(response);
                onSuccess?.Invoke(manaResponse.newMana);
            },
            error => { onError?.Invoke(error); }
        );
    }

    // Local classes for JSON parsing.
    [System.Serializable]
    public class PlayerStats
    {
        public int playerID;
        public float attack;
        public int hp;
        public int mana;
        public string affinity;
        // Note: UnlockedList and Slots are not provided by these endpoints.
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