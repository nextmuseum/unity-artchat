using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class DataManager
{
    public static void SaveUser(User userData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/user.data";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, userData);
        stream.Close();
    }

    public static User LoadUser()
    {
        string path = Application.persistentDataPath + "/user.data";
        // Debug.Log(path);
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            User userData = formatter.Deserialize(stream) as User;
            stream.Close();

            return userData;
        }
        else return null;
    }

    public static void DeleteUser()
    {
        string path = Application.persistentDataPath + "/user.data";
        if (File.Exists(path)) File.Delete(path);
    }
}
