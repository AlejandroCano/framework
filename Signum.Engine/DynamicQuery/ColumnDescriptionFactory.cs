﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Entities;
using Signum.Entities.DynamicQuery;
using Signum.Entities.Reflection;
using Signum.Utilities;
using Signum.Utilities.ExpressionTrees;
using System.Reflection;
using System.Linq.Expressions;
using Signum.Engine.Maps;

namespace Signum.Engine.DynamicQuery
{
    public class ColumnDescriptionFactory
    {
        readonly internal Meta Meta;
        public Func<string> OverrideDisplayName { get; set; }
        public Func<string> OverrideIsAllowed { get; set; }

        public string Name { get; internal set; }
        public Type Type { get; internal set; }

        public string Format { get; set; }
        public string Unit { get; set; }
        Implementations? implementations;
        public Implementations? Implementations
        {
            get { return implementations; }

            set
            {
                if (value != null && !value.Value.IsByAll)
                {
                    var ct = Type.CleanType();
                    string errors = value.Value.Types.Where(t => !ct.IsAssignableFrom(t)).ToString(a => a.Name, ", ");

                    if (errors.Any())
                        throw new InvalidOperationException("Column {0} Implenentations should be assignable to {1}: {2}".Formato(Name, ct.Name, errors));
                }

                implementations = value;
            }
        }

        PropertyRoute[] propertyRoutes;
        public PropertyRoute[] PropertyRoutes
        {
            get { return propertyRoutes; }
            set
            {
                propertyRoutes = value;
                if (propertyRoutes != null && propertyRoutes.Any() /*Out of IB casting*/)
                {
                    var cleanType = Type.CleanType();

                    Format = GetFormat(propertyRoutes);
                    Unit = GetUnit(propertyRoutes);
                }
            }
        }



        internal static string GetUnit(PropertyRoute[] routes)
        {
            switch (routes[0].PropertyRouteType)
            {
                case PropertyRouteType.LiteEntity:
                case PropertyRouteType.Root:
                    return null;
                case PropertyRouteType.FieldOrProperty:
                    return routes.Select(pr => pr.SimplifyNoRoot().PropertyInfo.SingleAttribute<UnitAttribute>().Try(u => u.UnitName)).Distinct().Only();
                case PropertyRouteType.MListItems:
                    return null;
            }

            throw new InvalidOperationException();
        }

        internal static string GetFormat(PropertyRoute[] routes)
        {
            switch (routes[0].PropertyRouteType)
            {
                case PropertyRouteType.LiteEntity:
                case PropertyRouteType.Root:
                    return null;
                case PropertyRouteType.FieldOrProperty:
                    return routes.Select(pr => Reflector.FormatString(pr)).Distinct().Only();
                case PropertyRouteType.MListItems:
                    return Reflector.FormatString(routes[0].Type);
            }

            throw new InvalidOperationException();
        }

        public ColumnDescriptionFactory(int index, MemberInfo mi, Meta meta)
        {
            Name = mi.Name;

            Type = mi.ReturningType();
            Meta = meta;

            //if (Type.IsIIdentifiable())
            //    throw new InvalidOperationException("The Type of column {0} is a subtype of IIdentifiable, use a Lite instead".Formato(mi.MemberName()));

            if (IsEntity && !Type.CleanType().IsIIdentifiable())
                throw new InvalidOperationException("Entity must be a Lite or an IIdentifiable");

            if (meta != null)
            {
                Implementations = meta.Implementations;
                if (meta is CleanMeta)
                    PropertyRoutes = ((CleanMeta)meta).PropertyRoutes;
            }

        }

        public string DisplayName()
        {
            if (OverrideDisplayName != null)
                return OverrideDisplayName();

            if (IsEntity)
                return this.Type.CleanType().NiceName();

            if (propertyRoutes != null && 
                propertyRoutes[0].PropertyRouteType == PropertyRouteType.FieldOrProperty &&
                propertyRoutes[0].PropertyInfo.Name == Name)
            {
                var result = propertyRoutes.Select(pr=>pr.PropertyInfo.NiceName()).Only();
                if (result != null)
                    return result;
            }

            return Name.NiceName();
        }

        public void SetPropertyRoutes<T>(params Expression<Func<T, object>>[] propertyRoutes)
            where T : IdentifiableEntity
        {
            PropertyRoutes = propertyRoutes.Select(exp => PropertyRoute.Construct(exp)).ToArray();
        }

        public bool IsEntity
        {
            get { return this.Name == ColumnDescription.Entity; }
        }

        public string IsAllowed()
        {
            if (OverrideIsAllowed != null)
                return OverrideIsAllowed();

            if (Meta != null)
                return Meta.IsAllowed();

            return null;
        }

        public ColumnDescription BuildColumnDescription()
        {
            return new ColumnDescription(Name, Reflector.IsIIdentifiable(Type) ? Lite.Generate(Type) : Type)
            {
                PropertyRoutes = propertyRoutes,
                Implementations = Implementations,

                DisplayName = DisplayName(),
                Format = Format,
                Unit = Unit,
            };
        }


    }
}
