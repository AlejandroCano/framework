﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Entities;
using Signum.Engine.Maps;
using Signum.Engine;
using System.IO;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Signum.Entities.Basics;
using Signum.Engine.DynamicQuery;
using Signum.Utilities.ExpressionTrees;
using Signum.Utilities;
using Signum.Engine.Operations;
using Signum.Engine.Basics;
using Microsoft.Extensions.Configuration;

namespace Signum.Test.Environment
{
    public static class MusicStarter
    {
        static bool startedAndLoaded = false;
        public static void StartAndLoad()
        {
            if (startedAndLoaded)
                return;

            lock (typeof(MusicStarter))
            {
                if (startedAndLoaded)
                    return;

                var conf = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
                var connectionString = conf.GetConnectionString("SignumTest");

                Start(UserConnections.Replace(connectionString));

                Administrator.TotalGeneration();

                Schema.Current.Initialize();

                MusicLoader.Load();

                startedAndLoaded = true;
            }
        }

        public static void Start(string connectionString)
        {
            SchemaBuilder sb = new SchemaBuilder(true);
            DynamicQueryManager dqm = new DynamicQueryManager();

            //Connector.Default = new SqlCeConnector(@"Data Source=C:\BaseDatos.sdf", sb.Schema, dqm);

            var sqlVersion = SqlServerVersionDetector.Detect(connectionString);

            Connector.Default = new SqlConnector(connectionString, sb.Schema, dqm, sqlVersion ?? SqlServerVersion.SqlServer2017);
            
            sb.Schema.Version = typeof(MusicStarter).Assembly.GetName().Version;

            sb.Schema.Settings.FieldAttributes((OperationLogEntity ol) => ol.User).Add(new ImplementedByAttribute());
            sb.Schema.Settings.FieldAttributes((ExceptionEntity e) => e.User).Add(new ImplementedByAttribute());

            if(Connector.Current.SupportsTemporalTables)
            {
                sb.Schema.Settings.TypeAttributes<FolderEntity>().Add(new SystemVersionedAttribute());
            }

            if (!Schema.Current.Settings.TypeValues.ContainsKey(typeof(TimeSpan)))
            {
                sb.Settings.FieldAttributes((AlbumEntity a) => a.Songs[0].Duration).Add(new Signum.Entities.IgnoreAttribute());
                sb.Settings.FieldAttributes((AlbumEntity a) => a.BonusTrack.Duration).Add(new Signum.Entities.IgnoreAttribute());
            }

            Validator.PropertyValidator((OperationLogEntity e) => e.User).Validators.Clear();
            
            TypeLogic.Start(sb, dqm);

            OperationLogic.Start(sb, dqm);
            ExceptionLogic.Start(sb, dqm);

            MusicLogic.Start(sb, dqm);

            sb.Schema.OnSchemaCompleted();
        }
    }
}
