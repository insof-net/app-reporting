using System.Collections.Generic;
using System.Linq;
using Insof.AppReporting.Client.Models;

namespace Insof.AppReporting.Client.Helpers;

internal static class AttributeHelper
{
    internal static List<AppEventAttribute> MapAttributes(this object attributes)
    {
        var type = attributes.GetType();

        var properties = type.GetProperties()
            .Select(p => new AppEventAttribute
            {
                Name = p.Name,
                Value = type.GetProperty(p.Name)?.GetValue(attributes)?.ToString() ?? string.Empty
            })
            .ToList();

        return properties;
    }
}