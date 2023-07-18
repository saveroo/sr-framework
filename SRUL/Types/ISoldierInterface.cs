using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DevExpress.Utils.Extensions;
using Newtonsoft.Json;

namespace SRUL;

public interface IRepository<T> where T : class {
    void Add(T entity);
    void Remove(T entity);
    IEnumerable<T?> GetAll();
    T? GetByFieldName(string fieldName, object value);
    bool UpdateByFieldName(string fieldName, object value);
    // Create Serializer
    bool Serialize(string fileName);
    // Create Deserializer
    bool Deserialize(string fileName);

    void AddIfNotExists(string fieldName, string fieldValue, T entity);
}
// create generic repository class
public abstract class Repository<T> : IRepository<T> where T : class {
    private IList<T?> _entities;
    public Repository()
    {
        _entities = new List<T?>();
    }

    public void AddIfNotExists(string fieldName, string fieldValue, T entity)
    {
        if(!_entities.Any(e => e.GetType().GetProperty(fieldName).GetValue(e).Equals(fieldValue)))
            _entities.Add(entity);
            
        // foreach (var e in _entities)
        // {
        //     if(!e.GetType().GetProperty(fieldName).GetValue(e).Equals(fieldValue)) 
        // }
    }

    public void Add(T entity) {
        _entities.Add(entity);
    }
    public void Remove(T entity) {
        _entities.Remove(entity);
    }

    public IEnumerable<T?> GetAll()
    {
        return _entities.ToList();
    }

    public T? GetByFieldName(string fieldName, object value)
    {
        foreach (var entity in _entities)
        {
            var property = entity.GetType().GetProperty(fieldName);
            if (property != null && property.GetValue(entity).Equals(value))
            {
                return entity;
            }
        }
        return null;
    }

    public bool UpdateByFieldName(string fieldName, object value)
    {
        foreach (var entity in _entities)
        {
            var property = entity.GetType().GetProperty(fieldName);
            if (property != null && property.CanWrite)
            {
                entity.GetType().GetProperty(fieldName)?.SetValue(entity, value);
                return true;
            }
        }
        return false;
    }

    bool IRepository<T>.Serialize(string fileName)
    {
        using(JsonTextWriter writer = new JsonTextWriter(new StreamWriter(fileName)))
        {
            try
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, _entities);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }

    bool IRepository<T>.Deserialize(string fileName)
    {
        using (JsonTextReader reader = new JsonTextReader(new StreamReader(fileName)))
        {
            try
            {
                JsonSerializer serializer = new JsonSerializer();
                _entities = (IList<T?>)serializer.Deserialize(reader, typeof(T));
                return true;
            }
            catch (Exception e)
            {
                return false;
                throw;
            }
        }
    }
    
}

