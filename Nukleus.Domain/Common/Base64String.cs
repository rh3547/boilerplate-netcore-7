namespace Nukleus.Domain.Common
{
    public class Base64String
    {
        public string Value { get; }

        public Base64String(string base64EncodedData)
        {
            if (!IsValidBase64(base64EncodedData))
            {
                throw new ArgumentException("The input string is not a valid Base64 encoded string.", nameof(base64EncodedData));
            }

            Value = base64EncodedData;
        }

        public static bool IsValidBase64(string base64EncodedData)
        {
            if (string.IsNullOrEmpty(base64EncodedData) || base64EncodedData.Length % 4 != 0
                || base64EncodedData.Contains(" ") || base64EncodedData.Contains("\t") || base64EncodedData.Contains("\r") || base64EncodedData.Contains("\n"))
            {
                return false;
            }

            try
            {
                Convert.FromBase64String(base64EncodedData);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        // Implicit conversion operators
        public static implicit operator Base64String(string base64EncodedData) => new Base64String(base64EncodedData);
        public static implicit operator string(Base64String base64String) => base64String.Value;

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static Base64String Base64EncodeToBase64String(string plainText)
        {
            return Base64Encode(plainText);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string Base64Decode(Base64String base64EncodedData)
        {
            return Base64Decode(base64EncodedData.Value);
        }
    }
}