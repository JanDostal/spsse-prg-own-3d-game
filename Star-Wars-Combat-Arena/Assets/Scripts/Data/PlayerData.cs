using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerData
{
    public int LightsaberDamage { get; }

    public int StaminaRegenSpeed { get; }

    public int StaminaPenaltyForAttack { get; }

    public int StaminaMaxLimit { get; }

    public int HealthMaxLimit { get; }

    public Color32 LightsaberColor { get; }

    private PlayerData(Builder builder)
    {
        LightsaberDamage = builder.LightsaberDamage;
        StaminaRegenSpeed = builder.StaminaRegenSpeed;
        StaminaPenaltyForAttack = builder.StaminaPenaltyForAttack;
        StaminaMaxLimit = builder.StaminaMaxLimit;
        HealthMaxLimit = builder.HealthMaxLimit;
        LightsaberColor = builder.LightsaberColor;

    }

    public class Builder
    {
        public int LightsaberDamage { get; private set; }

        public int StaminaRegenSpeed { get; private set; }

        public int StaminaPenaltyForAttack { get; private set; }

        public int StaminaMaxLimit { get; private set; }

        public int HealthMaxLimit { get; private set; }

        public Color32 LightsaberColor { get; private set; }

        public Builder SetLightsaberDamage(int lightsaberDamage)
        {
            LightsaberDamage = lightsaberDamage;
            return this;
        }

        public Builder SetStaminaRegenSpeed(int staminaRegenSpeed)
        {
            StaminaRegenSpeed = staminaRegenSpeed;
            return this;
        }

        public Builder SetStaminaPenaltyForAttack(int staminaPenaltyForAttack)
        {
            StaminaPenaltyForAttack = staminaPenaltyForAttack;
            return this;
        }

        public Builder SetStaminaMaxLimit(int staminaMaxLimit)
        {
            StaminaMaxLimit = staminaMaxLimit;
            return this;
        }

        public Builder SetHealthMaxLimit(int healthMaxLimit)
        {
            HealthMaxLimit = healthMaxLimit;
            return this;
        }

        public Builder SetLightsaberColor(Color32 lightsaberColor)
        {
            LightsaberColor = lightsaberColor;
            return this;
        }

        public PlayerData Build()
        {
            return new PlayerData(this);
        }
    }
}
