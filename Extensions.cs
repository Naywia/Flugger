using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Flugger
{
    public static class Extensions
    {
        // PropertyInfoExtensions
        public static string GetDanishName(this PropertyInfo property)
        {
            var displayAttr = property.GetCustomAttribute<DisplayAttribute>();
            return displayAttr?.Name ?? property.Name; // Fall back to original if no attribute
        }

    }
}
