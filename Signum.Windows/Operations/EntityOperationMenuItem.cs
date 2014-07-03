﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Entities.DynamicQuery;
using Signum.Entities;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.IO;
using Microsoft.Win32;
using Signum.Services;
using Signum.Utilities;
using Signum.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Signum.Entities.Basics;
using System.Windows.Automation;

namespace Signum.Windows.Operations
{
    public static class EntityOperationMenuItemConsturctor
    {
        public static MenuItem Construct(ContextualOperationContext coc)
        {
            MenuItem miResult = new MenuItem()
            {
                Header = coc.OperationSettings.Try(f => f.Text) ?? coc.OperationInfo.OperationSymbol.NiceToString(),
                Icon = coc.OperationSettings.Try(f => f.Icon.ToSmallImage()),
            };

            if (coc.OperationSettings != null && coc.OperationSettings.Order != 0)
                Common.SetOrder(miResult, coc.OperationSettings.Order);

            if (coc.CanExecute != null)
            {
                miResult.ToolTip = coc.CanExecute;
                miResult.IsEnabled = false;
                ToolTipService.SetShowOnDisabled(miResult, true);
                AutomationProperties.SetHelpText(miResult, coc.CanExecute);
            }

            miResult.Click += (object sender, RoutedEventArgs e) =>
            {
                coc.SearchControl.SetDirtySelectedItems();

                if (coc.OperationSettings != null && coc.OperationSettings.Click != null)
                    coc.OperationSettings.Click(coc);
                else
                {
                    var lite = coc.SearchControl.SelectedItems.Single();

                    switch (coc.OperationInfo.OperationType)
                    {
                        case OperationType.Execute:
                            Server.Return((IOperationServer os) => os.ExecuteOperationLite(lite, coc.OperationInfo.OperationSymbol)); 
                            break;
                        case OperationType.Delete:
                            Server.Execute((IOperationServer os) => os.Delete(lite, coc.OperationInfo.OperationSymbol));
                            break;
                        case OperationType.ConstructorFrom:
                            {
                                var result = Server.Return((IOperationServer os) => os.ConstructFromLite(lite, coc.OperationInfo.OperationSymbol));
                                if (Navigator.IsNavigable(result, true))
                                    Navigator.Navigate(result);
                                break;
                            }
                        case OperationType.Constructor:
                        case OperationType.ConstructorFromMany:
                            throw new InvalidOperationException("Unexpected operation type");
                    }
                }
            };

            return miResult;
        }
    }
}
