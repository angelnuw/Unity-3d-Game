using UnityEngine;

#if USE_SQLITE
using System;
using System.IO;
using SQLite4Unity3d; // or: using SQLite;
#endif

public class DataService : MonoBehaviour
{
    public static DataService Instance { get; private set; }

#if USE_SQLITE
    private SQLiteConnection _conn;
#endif

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

#if USE_SQLITE
        var dbPath = Path.Combine(Application.persistentDataPath, "game.db");
        _conn = new SQLiteConnection(dbPath);
        _conn.CreateTable<PlayerProfile>();
#endif
    }

    // --- minimal profile API ---
    public void SavePlayerName(string name)
    {
#if USE_SQLITE
        var row = _conn.Table<PlayerProfile>().FirstOrDefault();
        if (row == null)
            _conn.Insert(new PlayerProfile { Name = name, CreatedAt = DateTime.UtcNow });
        else
        {
            row.Name = name;
            _conn.Update(row);
        }
#else
        PlayerPrefs.SetString("player_name", name);
        PlayerPrefs.Save();
#endif
    }

    public string LoadPlayerName()
    {
#if USE_SQLITE
        var row = _conn.Table<PlayerProfile>().FirstOrDefault();
        return row != null ? row.Name : "";
#else
        return PlayerPrefs.GetString("player_name", "");
#endif
    }
}

#if USE_SQLITE
public class PlayerProfile
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
}
#endif
