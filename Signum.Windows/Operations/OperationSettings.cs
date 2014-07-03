using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Entities;
using System.Windows;
using System.Windows.Media;
using Signum.Entities.Basics;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using Signum.Utilities;

namespace Signum.Windows.Operations
{
    public abstract class OperationSettings
    {
        public OperationSymbol OperationSymbol { get; set; }
        public string Text { get; set; }
        public ImageSource Icon { get; set; }
        public Color? Color { get; set; }

        public OperationSettings(IOperationSymbolContainer symbol)
        {
            this.OperationSymbol = symbol.Operation;
        }

        public OperationSettings(OperationSymbol operationSymbol)
        {
            this.OperationSymbol = operationSymbol;
        }

    }

    public class EntityOperationGroup
    {
        public static readonly EntityOperationGroup None = new EntityOperationGroup();

        public static EntityOperationGroup Create = new EntityOperationGroup
        {
            Description = () => OperationMessage.Create.NiceToString(),
            SimplifyName = cs => Regex.Replace(cs, OperationMessage.CreateFromRegex.NiceToString(), m => m.Groups[1].Value.FirstUpper(), RegexOptions.IgnoreCase),
            Background = Brushes.Green,
            AutomationName = "Create"
        }; 

        public Func<string> Description;
        public Func<string, string> SimplifyName;
        public Brush Background;
        public string AutomationName;
        public double Order = 100;
    }

    public class EntityOperationSettings : OperationSettings
    {
        private Entities.OperationSymbol item;

        public Func<EntityOperationContext, IdentifiableEntity> Click { get; set; }
        public Func<EntityOperationContext, bool> IsVisible { get; set; }

        public ContextualOperationSettings ContextualFromMany { get; private set; }
        public ContextualOperationSettings Contextual { get; private set; }

        public bool AvoidMoveToSearchControl { get; set; }
        public double Order { get; set; }

        public EntityOperationGroup Group { get; set; }

        public EntityOperationSettings(IOperationSymbolContainer symbolContainer)
            : base(symbolContainer)
        {
            Contextual = new ContextualOperationSettings(symbolContainer);
            ContextualFromMany = new ContextualOperationSettings(symbolContainer); 
        }

        public EntityOperationSettings(OperationSymbol operationSymbol)
            : base(operationSymbol)
        {
            Contextual = new ContextualOperationSettings(operationSymbol);
            ContextualFromMany = new ContextualOperationSettings(operationSymbol);
        }
    }

    public class EntityOperationContext
    {
        public FrameworkElement EntityControl { get; set; }
        public Control SenderButton { get; set; }
        public OperationInfo OperationInfo { get; set; }
        public ViewMode ViewButtons { get; set; }
        public bool ShowOperations { get; set; }
        public string CanExecute { get; set; }

        public IdentifiableEntity Entity { get; set; }
        public EntityOperationSettings OperationSettings { get; set; }
    }

    public class ConstructorSettings : OperationSettings
    {
        public Func<OperationInfo, ConstructorContext, IdentifiableEntity> Constructor { get; set; }
        public Func<OperationInfo, bool> IsVisible { get; set; }

        public ConstructorSettings(IOperationSymbolContainer symbolContainer)
            : base(symbolContainer)
        {
        }
    }

    public class ContextualOperationSettings : OperationSettings
    {
        public Action<ContextualOperationContext> Click { get; set; }
        public Func<ContextualOperationContext, bool> IsVisible { get; set; }
        public double Order { get; set; }

        public ContextualOperationSettings(IOperationSymbolContainer symbolContainer)
            : base(symbolContainer)
        {
        }

        public ContextualOperationSettings(Entities.OperationSymbol operationSymbol)
            :base(operationSymbol)
        {   
        }
    }

    public class ContextualOperationContext 
    {
        public Lite<IdentifiableEntity>[] Entities { get; set; }
        public SearchControl SearchControl { get; set; }
        public OperationInfo OperationInfo { get; set; }
        public string CanExecute { get; set; }
        public ContextualOperationSettings OperationSettings { get; set; }
    }
}
