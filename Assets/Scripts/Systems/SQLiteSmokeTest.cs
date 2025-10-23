using UnityEngine;

#if USE_SQLITE
using System;
using System.IO;
// Use the namespace your SQLite.cs declares at the top:
using SQLite4Unity3d;  // or: using SQLite;
#endif

public class SQLiteSmokeTest : MonoBehaviour
{
    void Start()
    {
        Debug.Log("[SmokeTest] Start reached");   // always prints so you know the component ran

#if USE_SQLITE
        try
        {
            var path = Path.Combine(Application.persistentDataPath, "smoke.db");
            var conn = new SQLiteConnection(path);
            conn.CreateTable<TestRow>();
            conn.Insert(new TestRow { Msg = "hello", Created = DateTime.UtcNow });

            var count = conn.Table<TestRow>().Count();
            Debug.Log("[SQLite] OK rows=" + count + " @ " + path);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("[SQLite] " + ex.GetType().Name + ": " + ex.Message + "\n" + ex.StackTrace);
        }
#else
        Debug.LogWarning("[SQLite] USE_SQLITE not defined. Add it in Player Settings → Other Settings → Scripting Define Symbols.");
#endif
    }

#if USE_SQLITE
    class TestRow
    {
        [PrimaryKey, AutoIncrement] public int Id { get; set; }
        public string Msg { get; set; }
        public DateTime Created { get; set; }
    }
#endif
}
