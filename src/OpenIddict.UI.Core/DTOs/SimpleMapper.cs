using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace tomware.OpenIddict.UI.Core
{
  public class SimpleMapper
  {
    // see: https://www.twilio.com/blog/building-blazing-fast-object-mapper-c-sharp-net-core

    private static ConcurrentDictionary<string, PropertyMap[]> maps
      = new ConcurrentDictionary<string, PropertyMap[]>();

    private static SimpleMapper Create<TSource, TTarget>()
    {
      var mapper = new SimpleMapper();
      mapper.MapTypes(typeof(TSource), typeof(TTarget));
      return mapper;
    }

    public static TTarget From<TSource, TTarget>(TSource source)
    {
      var target = (TTarget)Activator.CreateInstance(typeof(TTarget));
      Create<TSource, TTarget>().Copy(source, target);

      return target;
    }

    public static void Map<TSource, TTarget>(TSource source, TTarget target)
    {
      Create<TSource, TTarget>().Copy(source, target);
    }

    private void MapTypes(Type source, Type target)
    {
      var key = GetMapKey(source, target);
      if (maps.ContainsKey(key)) return;

      var props = GetMatchingProperties(source, target);
      maps.TryAdd(key, props.ToArray());
    }

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
        var sourceValue = prop.SourceProperty.GetValue(source, null);
        prop.TargetProperty.SetValue(target, sourceValue, null);
      }
    }

    private IList<PropertyMap> GetMatchingProperties(Type source, Type target)
    {
      var sourceProperties = source.GetProperties();
      var targetProperties = target.GetProperties();

      return (from s in sourceProperties
              from t in targetProperties
              where s.Name == t.Name &&
                    s.CanRead &&
                    t.CanWrite &&
                    s.PropertyType == t.PropertyType
              select new PropertyMap
              {
                SourceProperty = s,
                TargetProperty = t
              }).ToList();
    }

    private string GetMapKey(Type source, Type target)
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
