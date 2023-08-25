using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class GameLevelData
{
    public PlayerData PlayerConfigurationData { get; }

    public EnemyData EnemyConfigurationData { get; }

    public int LevelNumber { get; }

    public string LevelMusicName { get; }

    private GameLevelData(Builder builder)
    {
        LevelNumber = builder.LevelNumber;
        PlayerConfigurationData = builder.PlayerConfigurationData;
        EnemyConfigurationData = builder.EnemyConfigurationData;
        LevelMusicName = builder.LevelMusicName;
    }

    public class Builder
    {
        public PlayerData PlayerConfigurationData { get; private set; }

        public EnemyData EnemyConfigurationData { get; private set; }

        public int LevelNumber { get; private set; }

        public string LevelMusicName { get; private set; }

        public Builder SetLevelNumber(int levelNumber)
        {
            LevelNumber = levelNumber;
            return this;
        }

        public Builder SetLevelMusicName(string levelMusicName)
        {
            LevelMusicName = levelMusicName;

            return this;
        }

        public Builder SetPlayerConfigurationData(PlayerData playerConfigurationData)
        {
            PlayerConfigurationData = playerConfigurationData;
            return this;
        }

        public Builder SetEnemyConfigurationData(EnemyData enemyConfigurationData)
        {
            EnemyConfigurationData = enemyConfigurationData;
            return this;
        }

        public GameLevelData Build()
        {
            return new GameLevelData(this);
        }
    }
}