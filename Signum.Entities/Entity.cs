using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Utilities;
using System.Runtime.Serialization;
using System.Reflection;
using System.Collections.Specialized;
using Signum.Utilities.DataStructures;
using Signum.Entities.Reflection;
using System.ComponentModel;
using System.Linq.Expressions;
using Signum.Utilities.Reflection;
using Signum.Utilities.ExpressionTrees;
using Signum.Services;
using System.Runtime.CompilerServices;
using System.Data;
using System.Globalization;

namespace Signum.Entities
{
    [Serializable, DescriptionOptions(DescriptionOptions.All)]
    public abstract class Entity : ModifiableEntity, IEntity
    {
        [Ignore]
        internal PrimaryKey? id = null;
        [Ignore]
        protected internal string toStr; //for queries and lites on entities with non-expression ToString 

        [HiddenProperty, Description("Id")]
        public PrimaryKey Id
        {
            get
            {
                if (id == null)
                    throw new InvalidOperationException("{0} is new and has no Id".Formato(this.GetType().Name));
                return id.Value;
            }
            internal set { id = value; }
        }

        [HiddenProperty]
        public PrimaryKey? IdOrNull
        {
            get { return id; }
        }

        [Ignore]
        bool isNew = true;
        [HiddenProperty]
        public bool IsNew
        {
            get { return isNew; }
            internal set { isNew = value; }
        }

        [Ignore]
        internal long ticks = 0;
        [HiddenProperty]
        public long Ticks
        {
            get { return ticks; }
            set { ticks = value; }
        }

        protected bool SetIfNew<T>(ref T field, T value, [CallerMemberNameAttribute]string automaticPropertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            if (!IsNew)
            {
                throw new InvalidOperationException("Attempt to modify '{0}' when the entity is not new".Formato(automaticPropertyName));
            }

            return base.Set<T>(ref field, value, automaticPropertyName);
        }

        public override string ToString()
        {
            return BaseToString();
        }

        public string BaseToString()
        {
            return "{0} ({1})".Formato(GetType().NiceName(), id.HasValue ? id.ToString() : LiteMessage.New_G.NiceToString().ForGenderAndNumber(this.GetType().GetGender()));
        }

        public override bool Equals(object obj)
        {
            if(obj == this)
                return true;

            if(obj == null)
                return false;

            Entity ident = obj as Entity;
            if (ident != null && ident.GetType() == this.GetType() && this.id != null && this.id == ident.id)
                return true;

            return false;
        }

        public virtual string IdentifiableIntegrityCheck()
        {
            using (Mixins.OfType<CorruptMixin>().Any(c => c.Corrupt) ? Corruption.AllowScope() : null)
            {
                return IdentifiableIntegrityCheckBase();
            }
        }

        internal virtual string IdentifiableIntegrityCheckBase()
        {
            using (HeavyProfiler.LogNoStackTrace("IdentifiableIntegrityCheck", () => GetType().Name))
                return GraphExplorer.IdentifiableIntegrityCheck(GraphExplorer.FromRootIdentifiable(this));
        }

        public override int GetHashCode()
        {
            return id == null ?
                base.GetHashCode() :
                StringHashEncoder.GetHashCode32(GetType().FullName) ^ id.Value.GetHashCode();
        }

        public Entity()
        {
            mixin = MixinDeclarations.CreateMixins(this);
        }

        [Ignore]
        readonly MixinEntity mixin;
        public M Mixin<M>() where M : MixinEntity
        {
            var current = mixin;
            while (current != null)
            {
                if (current is M)
                    return (M)current;
                current = current.Next;
            }

            throw new InvalidOperationException("Mixin {0} not declared for {1} in MixinDeclarations"
                .Formato(typeof(M).TypeName(), GetType().TypeName()));
        }

        public M TryMixin<M>() where M : MixinEntity
        {
            var current = mixin;
            while (current != null)
            {
                if (current is M)
                    return (M)current;
                current = current.Next;
            }

            return null;
        }

        public MixinEntity GetMixin(Type mixinType)
        {
            var current = mixin;
            while (current != null)
            {
                if (current.GetType() == mixinType)
                    return current;
                current = current.Next;
            }

            throw new InvalidOperationException("Mixin {0} not declared for {1} in MixinDeclarations"
                .Formato(mixinType.TypeName(), GetType().TypeName()));
        }

        [HiddenProperty]
        public MixinEntity this[string mixinName]
        {
            get
            {
                var current = mixin;
                while (current != null)
                {
                    if (current.GetType().Name == mixinName)
                        return current;
                    current = current.Next;
                }

                throw new InvalidOperationException("Mixin {0} not declared for {1} in MixinDeclarations"
                    .Formato(mixinName, GetType().TypeName()));
            }
        }

        [HiddenProperty]
        public IEnumerable<MixinEntity> Mixins
        {
            get
            {
                var current = mixin;
                while (current != null)
                {
                    yield return current;
                    current = current.Next;
                }
            }
        }

       
    }

    public interface IEntity : INotifyPropertyChanged, IDataErrorInfo, IRootEntity
    {
        PrimaryKey Id { get; }

        [HiddenProperty]
        PrimaryKey? IdOrNull { get; }

        [HiddenProperty]
        bool IsNew { get; }

        [HiddenProperty]
        string ToStringProperty { get; }
    }

    public interface IRootEntity
    {

    }

    public static class UnsafeEntityExtensions
    {
        public static T SetNotModified<T>(this T ident)
            where T : Modifiable
        {
            if (ident is Entity)
                ((Entity)(Modifiable)ident).IsNew = false;
            ident.Modified = ModifiedState.Clean;
            return ident;
        }

        public static T SetModified<T>(this T ident)
            where T : Modifiable
        {
            ident.Modified = ModifiedState.SelfModified;
            return ident;
        }
    }

}
