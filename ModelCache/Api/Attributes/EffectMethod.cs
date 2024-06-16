// <copyright file="EffectMethod.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ModelCache.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CreateEffect : Attribute
    {
        public CreateEffect(Type associatedType)
        {
            AssociatedType = associatedType;
        }

        public Type AssociatedType { get; }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ReadEffect : Attribute
    {
        public ReadEffect(Type associatedType)
        {
            AssociatedType = associatedType;
        }

        public Type AssociatedType { get; }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ReadManyEffect : Attribute
    {
        public ReadManyEffect(Type associatedType)
        {
            AssociatedType = associatedType;
        }

        public Type AssociatedType { get; }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ReadManyFromCacheEffect : Attribute
    {
        public ReadManyFromCacheEffect(Type associatedType)
        {
            AssociatedType = associatedType;
        }

        public Type AssociatedType { get; }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class UpdateEffect : Attribute
    {
        public UpdateEffect(Type associatedType)
        {
            AssociatedType = associatedType;
        }

        public Type AssociatedType { get; }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class DeleteEffect : Attribute
    {
        public DeleteEffect(Type associatedType)
        {
            AssociatedType = associatedType;
        }

        public Type AssociatedType { get; }
    }
}