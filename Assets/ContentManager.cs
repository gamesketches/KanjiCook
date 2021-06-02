using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ContentManager : MonoBehaviour
{
	public TextAsset kanjiFile;
	KanjiInfoFile myKanji;
    // Start is called before the first frame update
    void Start()
    {
        myKanji = JsonUtility.FromJson<KanjiInfoFile>(kanjiFile.text);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[Serializable]
public class KanjiInfoFile {
	public KanjiInfo[] kanjiInfos;
}

[Serializable]
public class KanjiInfo {
	public string[] meanings;
	public string kanji;
	public string[] radicals;
}
