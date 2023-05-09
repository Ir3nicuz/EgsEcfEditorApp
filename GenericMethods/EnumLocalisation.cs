using System;
using System.Diagnostics;

namespace GenericMethods
{
    public static class EnumLocalisation
    {
        public static string GetLocalizedEnum<E>(E enumMember, ) where E : Enum
        {
            string enumPath = string.Format("{0}.{1}", enumMember.GetType(), enumMember);
            string localizedEnum = EnumRecources.ResourceManager.GetString(enumPath);
            bool enumValid = !string.IsNullOrEmpty(localizedEnum);
            if (Debugger.IsAttached) { Debug.Assert(enumValid, string.Format("Enum {0} has no localisation!", enumPath)); }
            return enumValid ? localizedEnum : enumPath;
        }
    }
}
