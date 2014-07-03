﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Utilities;
using System.Windows;
using Signum.Entities;
using Signum.Entities.Basics;
using System.Reflection;
using Signum.Entities.Reflection;
using Signum.Entities.DynamicQuery;
using Signum.Windows.Operations;

namespace Signum.Windows
{
    public static class Constructor
    {
        public static ConstructorManager Manager; 

        public static void Start(ConstructorManager manager)
        {
            Manager = manager;
        }

        public static T Construct<T>(this FrameworkElement element, List<object> args = null)
         where T : ModifiableEntity
        {
            return (T)Manager.SurroundConstruct(new ConstructorContext(typeof(T), element, args), Manager.ConstructCore);
        }

        public static ModifiableEntity Construct(this FrameworkElement element, Type type, List<object> args = null)
        {
            return Manager.SurroundConstruct(new ConstructorContext(type, element, args), Manager.ConstructCore);
        }

        public static T SurroundConstruct<T>(this FrameworkElement element, Func<ConstructorContext, T> constructor)
          where T : ModifiableEntity
        {
            return (T)Manager.SurroundConstruct(new ConstructorContext(typeof(T), element, null), constructor);
        }

        public static T SurroundConstruct<T>(this FrameworkElement element, List<object> args,  Func<ConstructorContext, T> constructor)
            where T : ModifiableEntity
        {
            return (T)Manager.SurroundConstruct(new ConstructorContext(typeof(T), element, args), constructor);
        }

        public static ModifiableEntity SurroundConstruct(this FrameworkElement element, Type type, List<object> args, Func<ConstructorContext, ModifiableEntity> constructor)
        {
            return Manager.SurroundConstruct(new ConstructorContext(type, element, args), constructor);
        }

        public static void Register<T>(Func<ConstructorContext, T> constructor)
            where T : ModifiableEntity
        {
            Manager.Constructors.Add(typeof(T), constructor);
        }
    }

    public class ConstructorContext
    {
        public ConstructorContext(Type type, FrameworkElement element, List<object> args)
        {
            if (type == null)
                throw new ArgumentNullException("type"); 

            this.Type = type;
            this.Element = element;
            this.Args = args ?? new List<object>();
        }

        public Type Type { get; private set; }
        public FrameworkElement Element { get; private set; }
        public List<object> Args { get; private set; }
        public bool CancelConstruction { get; set; }
    }

    public class ConstructorManager
    {
        public event Func<ConstructorContext, IDisposable> PreConstructors;

        public Dictionary<Type, Func<ConstructorContext, ModifiableEntity>> Constructors = new Dictionary<Type, Func<ConstructorContext, ModifiableEntity>>();

        public event Action<ConstructorContext, ModifiableEntity> PostConstructors;

        public ConstructorManager()
        {
            PostConstructors += PostConstructors_AddFilterProperties;
        }

        public virtual ModifiableEntity ConstructCore(ConstructorContext ctx)
        {
            Func<ConstructorContext, ModifiableEntity> c = Constructors.TryGetC(ctx.Type);
            if (c != null)
            {
                ModifiableEntity result = c(ctx);
                return result;
            }

            if (ctx.Type.IsIdentifiableEntity() && OperationClient.Manager.HasConstructOperations(ctx.Type))
                return OperationClient.Manager.Construct(ctx);

            return (ModifiableEntity)Activator.CreateInstance(ctx.Type);
        }

        public virtual ModifiableEntity SurroundConstruct(ConstructorContext ctx, Func<ConstructorContext, ModifiableEntity> constructor)
        {
            IDisposable disposable = null;
            try
            {

                if (PreConstructors != null)
                    foreach (Func<ConstructorContext, IDisposable> pre in PreConstructors.GetInvocationList())
                    {
                        disposable = Disposable.Combine(disposable, pre(ctx));

                        if (ctx.CancelConstruction)
                            return null;
                    }

                var entity = constructor(ctx);

                if (entity == null || ctx.CancelConstruction)
                    return null;

                if (PostConstructors != null)
                    foreach (Action<ConstructorContext, ModifiableEntity> post in PostConstructors.GetInvocationList())
                    {
                        post(ctx, entity);

                        if (ctx.CancelConstruction)
                            return null;
                    }

                return entity;
            }
            finally
            {
                if (disposable != null)
                    disposable.Dispose();
            }
        }


        public static void PostConstructors_AddFilterProperties(ConstructorContext ctx, ModifiableEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("result");

            if (ctx.Element is SearchControl)
            {
                var filters = ((SearchControl)ctx.Element).FilterOptions.Where(fo => fo.Operation == FilterOperation.EqualTo && fo.Token is ColumnToken);

                var pairs = from pi in ctx.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            join fo in filters on pi.Name equals fo.Token.Key
                            where Server.CanConvert(fo.Value, pi.PropertyType) && fo.Value != null
                            select new { pi, fo };

                foreach (var p in pairs)
                {
                    p.pi.SetValue(entity, Server.Convert(p.fo.Value, p.pi.PropertyType), null);
                }
            }
        }

    }
}
