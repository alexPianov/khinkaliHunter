using CodeStage.AntiCheat.ObscuredTypes;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;

namespace Playstel
{
    [CreateAssetMenu(menuName = "User/Data")]
    public class UserData : ScriptableObject
    {
        private UserPayload _userPayload;
        
        public Dictionary<ObscuredString, ObscuredString> userDataSafe;
        public enum UserDataType
        {
            Newly, UnitSkin, Items, Bag, Lives, Currency, BattlePass, Cutscenes
        }

        public ObscuredString GetUserData(UserDataType type)
        {
            userDataSafe.TryGetValue(type.ToString(), out ObscuredString value);
            
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
            {
                Debug.Log(type + " value is null or empty");
                return null;
            }

            return value;
        }

        public void UpdateUserDataRecord(Dictionary<string, UserDataRecord> userData)
        {
            userDataSafe = DataHandler.ConvertToSafeData(userData);
        }
    }
}
