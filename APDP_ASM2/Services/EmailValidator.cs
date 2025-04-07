using APDP_ASM2.Interface;
using System.Text.RegularExpressions;

namespace APDP_ASM2.Services
{
    namespace APDP_ASM2.Validators
    {
        public class EmailValidator : IEmailValidator
        {
            private const string EMAIL_PATTERN = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";

            public bool IsValid(string email)
            {
                if (string.IsNullOrWhiteSpace(email))
                    return false;

                try
                {
                    return Regex.IsMatch(email, EMAIL_PATTERN);
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
