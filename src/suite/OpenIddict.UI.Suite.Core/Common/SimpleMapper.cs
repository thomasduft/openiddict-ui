using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace tomware.OpenIddict.UI.Suite.Core
{
  public class SimpleMapper
  {
    private static readonly ConcurrentDictionary<string, PropertyMap[]> maps
      = new(StringComparer.Ordinal);

    public static TTarget From<TSource, TTarget>(TSource source)
    {
      var target = (TTarget)Activator.CreateInstance(typeof(TTarget));
      var mapper = Create<TSource, TTarget>();
      mapper.Copy(source, target);

      return target;
    }

    public static void Map<TSource, TTarget>(TSource source, TTarget target)
    {
      var mapper = Create<TSource, TTarget>();
      mapper.Copy(source, target);
    }

    private static SimpleMapper Create<TSource, TTarget>()
    {
      var mapper = new SimpleMapper();
      MapTypes(typeof(TSource), typeof(TTarget));
      return mapper;
    }

    private static void MapTypes(Type source, Type target)
    {
      var key = GetMapKey(source, target);
      if (maps.ContainsKey(key)) return;

      var props = GetMatchingProperties(source, target);
      maps.TryAdd(key, props.ToArray());
    }

#pragma warning disable CA1822
    private void Copy(object source, object target)
    {
      var sourceType = source.GetType();
      var targetType = target.GetType();

      var key = GetMapKey(sourceType, targetType);
      if (!maps.ContainsKey(key))
      {
        MapTypes(sourceType, targetType);
      }

      var propMap = maps[key];

      for (var i = 0; i < propMap.Length; i++)
      {
        var prop = propMap[i];
        var sourceValue = prop.SourceProperty.GetValue(source, index: null);
        prop.TargetProperty.SetValue(target, sourceValue, index: null);
      }
    }
#pragma warning restore CA1822

    private static IList<PropertyMap> GetMatchingProperties(Type source, Type target)
    {
      var sourceProperties = source.GetProperties();
      var targetProperties = target.GetProperties();

      return (from s in sourceProperties
              from t in targetProperties
              where string.Equals(s.Name, t.Name, StringComparison.Ordinal) &&
                    s.CanRead &&
                    t.CanWrite &&
                    s.PropertyType == t.PropertyType
              select new PropertyMap
              {
                SourceProperty = s,
                TargetProperty = t,
              }).ToList();
    }

    private static string GetMapKey(Type source, Type target)
    {
      return $"{source.GetHashCode()}_{target.GetHashCode()}";
    }

    private class PropertyMap
    {
      public PropertyInfo SourceProperty { get; set; }
      public PropertyInfo TargetProperty { get; set; }
    }
  }
}
