﻿using System;
using System.IO;
using Arrowgene.Ddon.Database.Model;
using Arrowgene.Ddon.Database.Sql;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.Database
{
    public static class DdonDatabaseBuilder
    {
        private static readonly ILogger Logger = LogProvider.Logger<Logger>(typeof(DdonDatabaseBuilder));

        public static IDatabase Build(DatabaseSetting settings)
        {
            IDatabase database = null;
            switch (settings.Type)
            {
                case DatabaseType.SQLite:
                    string sqLitePath = Path.Combine(settings.SqLiteFolder, $"db.v{DdonSqLiteDb.Version}.sqlite");
                    database = BuildSqLite(settings.SqLiteFolder, sqLitePath, settings.WipeOnStartup);
                    break;
            }

            if (database == null)
            {
                Logger.Error("Database could not be created, exiting...");
                Environment.Exit(1);
            }

            return database;
        }

        public static DdonSqLiteDb BuildSqLite(string sqLiteFolder, string sqLitePath, bool deleteIfExists)
        {
            if (deleteIfExists)
            {
                try
                {
                    File.Delete(sqLitePath);
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            DdonSqLiteDb db = new DdonSqLiteDb(sqLitePath);
            if (db.CreateDatabase())
            {
                ScriptRunner scriptRunner = new ScriptRunner(db);
                scriptRunner.Run(Path.Combine(sqLiteFolder, "Script/schema_sqlite.sql"));
            }

            return db;
        }
    }
}
