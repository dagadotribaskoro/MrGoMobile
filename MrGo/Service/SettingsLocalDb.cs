using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Provider;
using Android.Database.Sqlite;

namespace MrGo.Service
{
    public enum SettingName
    {
        CurrentLogin,
        Email
    }
    public class SettingsStringAutoComplete
    {
        public static String[] GetAutoComplete(SettingName name, Context ctx)
        {
            SettingsServiceLocalDB svc = new SettingsServiceLocalDB(ctx);
            Settings setting = svc.GetByName(name);
            if (setting == null)
            {
                setting = new Settings();
                setting.Name = name;
                setting.Val_1 = ",";
                svc.Insert(setting);
            }
            string[] result = setting.Val_1.Split(',');
            return result;
        }
        public static void UpdateAutocomplete(SettingName name, string newVal, Context ctx)
        {
            SettingsServiceLocalDB svc = new SettingsServiceLocalDB(ctx);
            Settings setting = svc.GetByName(name);
            if (setting == null)
            {
                setting = new Settings();
                setting.Name = name;
                setting.Val_1 = ",";
                svc.Insert(setting);
            }
            string[] result = setting.Val_1.Split(',');
            bool exist = false;
            foreach (string rs in result)
            {
                if (rs == newVal)
                {
                    exist = true;
                    break;
                }
            }
            if (!exist)
            {
                setting.Val_1 = setting.Val_1 + "," + newVal;
                svc.Update(setting);
            }
        }
    }
    public class Settings
    {
        public long Id { get; set; }
        public SettingName Name { get; set; }
        public string Val_1 { get; set; }
        public string Val_2 { get; set; }
        public string Val_3 { get; set; }
        public string Val_4 { get; set; }
        public string Val_5 { get; set; }
    }
    public class SettingsServiceLocalDB
    {
        // Gets the data repository in write mode
        FeedReaderDbHelper helper;
        SQLiteDatabase db;
        public SettingsServiceLocalDB(Context ctx)
        {
            helper = new FeedReaderDbHelper(ctx);
            db = helper.WritableDatabase;
        }
        public long Insert(Settings setting)
        {
            // Create a new map of values, where column names are the keys
            ContentValues values = new ContentValues();
            values.Put(FeedEntry.COLUMN_NAME_NAME, setting.Name.ToString());
            values.Put(FeedEntry.COLUMN_NAME_VAL_1, setting.Val_1);
            values.Put(FeedEntry.COLUMN_NAME_VAL_2, setting.Val_2);
            values.Put(FeedEntry.COLUMN_NAME_VAL_3, setting.Val_3);
            values.Put(FeedEntry.COLUMN_NAME_VAL_4, setting.Val_4);
            values.Put(FeedEntry.COLUMN_NAME_VAL_5, setting.Val_5);
            setting.Id = db.Insert(FeedEntry.TABLE_NAME, null, values);
            // Insert the new row, returning the primary key value of the new row
            return setting.Id;
        }

        public Settings GetByName(SettingName settingname)
        {
            String[] projection = {
                FeedEntry.Id,
                FeedEntry.COLUMN_NAME_NAME,
                FeedEntry.COLUMN_NAME_VAL_1,
                FeedEntry.COLUMN_NAME_VAL_2,
                FeedEntry.COLUMN_NAME_VAL_3,
                FeedEntry.COLUMN_NAME_VAL_4,
                FeedEntry.COLUMN_NAME_VAL_5,
                };
            String selection = FeedEntry.COLUMN_NAME_NAME + " = ?";
            String[] selectionArgs = { settingname.ToString() };
            String sortOrder = FeedEntry.COLUMN_NAME_NAME + " DESC";
            SQLiteCursor cursor = (SQLiteCursor)db.Query(
                FeedEntry.TABLE_NAME,                     // The table to query
                projection,                               // The columns to return
                selection,                                // The columns for the WHERE clause
                selectionArgs,                            // The values for the WHERE clause
                null,                                     // don't group the rows
                null,                                     // don't filter by row groups
                sortOrder                                 // The sort order
                );
            cursor.MoveToFirst();
            Settings result = null;

            if (cursor.Count > 0)
            {
                result = new Settings();
                result.Id = cursor.GetLong(cursor.GetColumnIndexOrThrow(FeedEntry.Id));
                result.Name = (SettingName)Enum.Parse(typeof(SettingName), cursor.GetString(cursor.GetColumnIndexOrThrow(FeedEntry.COLUMN_NAME_NAME)));
                result.Val_1 = cursor.GetString(cursor.GetColumnIndexOrThrow(FeedEntry.COLUMN_NAME_VAL_1));
                result.Val_2 = cursor.GetString(cursor.GetColumnIndexOrThrow(FeedEntry.COLUMN_NAME_VAL_2));
                result.Val_3 = cursor.GetString(cursor.GetColumnIndexOrThrow(FeedEntry.COLUMN_NAME_VAL_3));
                result.Val_4 = cursor.GetString(cursor.GetColumnIndexOrThrow(FeedEntry.COLUMN_NAME_VAL_4));
                result.Val_5 = cursor.GetString(cursor.GetColumnIndexOrThrow(FeedEntry.COLUMN_NAME_VAL_5));
                cursor.Close();
            }
            return result;
        }
        public void Update(Settings setting)
        {
            // New value for one column
            ContentValues values = new ContentValues();
            values.Put(FeedEntry.COLUMN_NAME_NAME, setting.Name.ToString());
            values.Put(FeedEntry.COLUMN_NAME_VAL_1, setting.Val_1);
            values.Put(FeedEntry.COLUMN_NAME_VAL_2, setting.Val_2);
            values.Put(FeedEntry.COLUMN_NAME_VAL_3, setting.Val_3);
            values.Put(FeedEntry.COLUMN_NAME_VAL_4, setting.Val_4);
            values.Put(FeedEntry.COLUMN_NAME_VAL_5, setting.Val_5);

            // Which row to update, based on the title
            String selection = FeedEntry.Id + " = ?";
            String[] selectionArgs = { setting.Id.ToString() };

            int count = db.Update(
                FeedEntry.TABLE_NAME,
                values,
                selection,
                selectionArgs);
        }
        public void Delete(Settings setting)
        {
            // Define 'where' part of query.
            String selection = FeedEntry.Id + " = ?";
            // Specify arguments in placeholder order.
            String[] selectionArgs = { setting.Id.ToString() };
            // Issue SQL statement.
            db.Delete(FeedEntry.TABLE_NAME, selection, selectionArgs);
        }

        public class FeedReaderContract
        {
            // To prevent someone from accidentally instantiating the contract class,
            // make the constructor private.
            private FeedReaderContract() { }

            /* Inner class that defines the table contents */

        }
        public class FeedEntry : Object
        {
            [Register("_COUNT")]
            public static string Count = "_count";
            [Register("_ID")]
            public static string Id = "_id";
            public static String TABLE_NAME = "settings";
            public static String COLUMN_NAME_NAME = "setting_name";
            public static String COLUMN_NAME_VAL_1 = "val_1";
            public static String COLUMN_NAME_VAL_2 = "val_2";
            public static String COLUMN_NAME_VAL_3 = "val_3";
            public static String COLUMN_NAME_VAL_4 = "val_4";
            public static String COLUMN_NAME_VAL_5 = "val_5";
        }
        public class FeedReaderDbHelper : SQLiteOpenHelper
        {
            private static String TEXT_TYPE = " TEXT";
            private static String COMMA_SEP = ",";
            private static String SQL_CREATE_ENTRIES =
                "CREATE TABLE " + FeedEntry.TABLE_NAME + " (" +
                FeedEntry.Id + " INTEGER PRIMARY KEY," +
                FeedEntry.COLUMN_NAME_NAME + TEXT_TYPE + COMMA_SEP +
                FeedEntry.COLUMN_NAME_VAL_1 + TEXT_TYPE + COMMA_SEP +
                        FeedEntry.COLUMN_NAME_VAL_2 + TEXT_TYPE + COMMA_SEP +
                        FeedEntry.COLUMN_NAME_VAL_3 + TEXT_TYPE + COMMA_SEP +
                        FeedEntry.COLUMN_NAME_VAL_4 + TEXT_TYPE + COMMA_SEP +
                        FeedEntry.COLUMN_NAME_VAL_5 + TEXT_TYPE + " )";

            private static String SQL_DELETE_ENTRIES =
                "DROP TABLE IF EXISTS " + FeedEntry.TABLE_NAME;

            public FeedReaderDbHelper(Context context) : base(context, DATABASE_NAME, null, DATABASE_VERSION)
            {
            }
            // If you change the database schema, you must increment the database version.
            public static int DATABASE_VERSION = 1;
            public static String DATABASE_NAME = "FeedReader.db";

            public override void OnCreate(SQLiteDatabase db)
            {
                db.ExecSQL(SQL_CREATE_ENTRIES);
            }
            public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
            {
                db.ExecSQL(SQL_DELETE_ENTRIES);
                OnCreate(db);
            }
            public override void OnDowngrade(SQLiteDatabase db, int oldVersion, int newVersion)
            {
                OnUpgrade(db, oldVersion, newVersion);

            }

        }
    }
}