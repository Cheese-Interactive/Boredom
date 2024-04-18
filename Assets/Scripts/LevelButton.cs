using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : Button {

    [Header("References")]
    [SerializeField] private TMP_Text levelNumText;
    private Level level;

    public void Initialize(int levelNum, Level level) {

        levelNumText.text = levelNum + "";
        this.level = level;

    }

    public Level GetLevel() => level;

}
