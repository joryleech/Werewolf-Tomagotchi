
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class SaveManager
{
    public string fileName = "/save1.sav";
    protected Dictionary<string, dynamic> saveObject;


    public SaveManager()
    {
        saveObject = new Dictionary<string, dynamic>();
    }

    public bool exists()
    {
        return File.Exists(Application.dataPath + this.fileName);
    }


    public bool save()
    {
        try
        {
            string json = JsonConvert.SerializeObject(saveObject);
            writeFile(this.fileName, json);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        return false;
    }

    public void load()
    {
        try
        {
            string fileContents = readFile( this.fileName);
            this.saveObject = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(fileContents, new JsonConverter[] { new MyConverter() });
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }
    }

    public void setKey(string key, dynamic value)
    {
        string[] keys = key.Split('.');
        int keysLength = keys.Length;
        Dictionary<string, dynamic> currentDictionary = saveObject;
        for (int i = 0; i < keysLength - 1; i++)
        {
            if (currentDictionary.ContainsKey(keys[i]))
            {
                currentDictionary = currentDictionary[keys[i]];
            }
            else
            {
                Dictionary<string, dynamic> newDictionary = new Dictionary<string, dynamic>();
                currentDictionary[keys[i]] = newDictionary;
                currentDictionary = newDictionary;
            }
        }
        currentDictionary[keys[keysLength - 1]] = value;
    }

    public dynamic getKey(string key)
    {
        string[] keys = key.Split('.');
        int keysLength = keys.Length;
        Dictionary<string, dynamic> currentDictionary = saveObject;
        for (int i = 0; i < keysLength - 1; i++)
        {
            if (currentDictionary.ContainsKey(keys[i]))
            {
          
                currentDictionary = (Dictionary<string,dynamic>)currentDictionary[keys[i]];
            }
            else
            {
                return null;
            }
        }

        return currentDictionary[keys[keysLength - 1]];
    }

    protected static string readFile(string filename)
    {
        string content = String.Empty;
        using (var sr = new StreamReader(Application.dataPath + filename))
        {
            content = sr.ReadToEnd();
        }
        return content;
    }

    protected static void writeFile(string filename, string content)
    {
        FileStream fileStream = new FileStream(Application.dataPath + filename, FileMode.Create);
        Debug.Log(Application.dataPath + filename);
        using (StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(content);
        }
    }

    public event Action<SaveManager> afterSaveActions;
    protected void onSave()
    {
        if (afterSaveActions != null)
        {
            afterSaveActions(this);
        }
    }

    public event Action<SaveManager> afterLoadActions;
    protected void onLoad()
    {
        if (afterLoadActions != null)
        {
            afterLoadActions(this);
        }
    }
}


class MyConverter : CustomCreationConverter<IDictionary<string, object>>
{
    public override IDictionary<string, object> Create(Type objectType)
    {
        return new Dictionary<string, object>();
    }

    public override bool CanConvert(Type objectType)
    {
        // in addition to handling IDictionary<string, object>
        // we want to handle the deserialization of dict value
        // which is of type object
        return objectType == typeof(object) || base.CanConvert(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.StartObject
            || reader.TokenType == JsonToken.Null)
            return base.ReadJson(reader, objectType, existingValue, serializer);

        // if the next token is not an object
        // then fall back on standard deserializer (strings, numbers etc.)
        return serializer.Deserialize(reader);
    }
}