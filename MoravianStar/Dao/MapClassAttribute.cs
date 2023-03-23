using System;

namespace MoravianStar.Dao
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MapClassAttribute : Attribute
    {
        public MapClassAttribute(Type entityType)
        {
            EntityType = entityType;
        }

        public Type EntityType { get; private set; }
    }
}