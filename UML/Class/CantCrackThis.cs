using System;
using System.Security.Cryptography;
using System.Text;
using UML.Services;

namespace UML.Security
{
    public class CantCrackThis
    {
        private static readonly string DEFAULT_MASTER_KEY = "LpK2hQv8RtMn4xZj7FgD6bNs9WcE3yXa5VuP1TqJ0YrGiHfA2zBdCmS7LxOkU4";

        private readonly string _masterKey;
        private readonly int _keyRotationInterval;

        public CantCrackThis(string masterKey = null, int keyRotationInterval = 86400)
        {
            _masterKey = masterKey ?? DEFAULT_MASTER_KEY;
            _keyRotationInterval = keyRotationInterval;
        }

        private string GetCurrentDerivedKey()
        {
            long timeSlot = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / _keyRotationInterval;
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_masterKey)))
            {
                var timeSlotBytes = BitConverter.GetBytes(timeSlot);
                var derivedKey = hmac.ComputeHash(timeSlotBytes);
                return Convert.ToBase64String(derivedKey);
            }
        }

        public string GenerateAuthToken()
        {
            var currentKey = GetCurrentDerivedKey();
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            var nonce = Guid.NewGuid().ToString("N");
            var payload = $"{timestamp}:{nonce}";
            using (var hmac = new HMACSHA256(Convert.FromBase64String(currentKey)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
                var signature = Convert.ToBase64String(hash);
                return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{payload}:{signature}"));
            }
        }

        public bool ValidateAuthToken(string token, int timeWindowSeconds = 120)
        {
            try
            {
                var tokenData = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                var parts = tokenData.Split(':');

                if (parts.Length != 3)
                {
                    return false;
                }

                var timestamp = parts[0];
                var nonce = parts[1];
                var receivedSignature = parts[2];

                var tokenTime = long.Parse(timestamp);
                var currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                Logger.Log($"Time check - Signature time: {tokenTime}, Difference: {currentTime - tokenTime}s");

                long timeSlot = tokenTime / _keyRotationInterval;

                using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_masterKey)))
                {
                    var timeSlotBytes = BitConverter.GetBytes(timeSlot);
                    if (!BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(timeSlotBytes);
                    }

                    var derivedKey = hmac.ComputeHash(timeSlotBytes);
                    var keyForValidation = Convert.ToBase64String(derivedKey);

                    var payload = $"{timestamp}:{nonce}";

                    using (var validationHmac = new HMACSHA256(Convert.FromBase64String(keyForValidation)))
                    {
                        var hash = validationHmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
                        var calculatedSignature = Convert.ToBase64String(hash);

                        bool isValid = calculatedSignature == receivedSignature;
                        Logger.Log(isValid ? "Signature valid." : "Signature invalid.");
                        return isValid;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Validation error: {ex.Message}");
                return false;
            }
        }
    }
}