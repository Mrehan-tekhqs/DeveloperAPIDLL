using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.VisualBasic;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json;

namespace EEPM
{
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class CheckedTracker : ICheckedTracker
    {
        private readonly FileInfo appDataFileInfo;
        private readonly IClock clock;

        public CheckedTracker(string appDataFilePath = null, IClock clock = null/* TODO Change to default(_) if this is not a reference type */)
        {
            appDataFilePath = appDataFilePath == null ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"eBizCharge\\user.dat") : appDataFilePath;
            this.appDataFileInfo = new FileInfo(appDataFilePath);
            this.clock = clock == null ? new Clock() : clock;
        }

        public bool LastCheckedWithin(TimeSpan timeSpan)
        {
            if (!File.Exists(appDataFileInfo.FullName))
                return false;
            return UserData.SourceKeyLastChecked == null/* TODO Change to default(_) if this is not a reference type */? false : clock.Now - DateTime.FromBinary(UserData.SourceKeyLastChecked) < timeSpan;
        }

        public void MarkChecked()
        {
            UserData.SourceKeyLastChecked = clock.Now.ToBinary();
            using (MemoryStream ms = new MemoryStream())
            {
                using (BsonWriter writer = new BsonWriter(ms))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(writer, UserData);
                }
                if (!Directory.Exists(appDataFileInfo.Directory.FullName))
                    appDataFileInfo.Directory.Create();
                using (var sw = appDataFileInfo.CreateText())
                {
                    sw.Write(Convert.ToBase64String(ms.ToArray()));
                }
            }
        }

        private UserData userDataBackingField;
        private UserData UserData
        {
            get
            {
                if (userDataBackingField != null)
                    return userDataBackingField;
                if (!File.Exists(appDataFileInfo.FullName))
                {
                    userDataBackingField = new UserData();
                    return userDataBackingField;
                }
                try
                {
                    using (var sr = appDataFileInfo.OpenText())
                    {
                        byte[] data = Convert.FromBase64String(sr.ReadToEnd());
                        MemoryStream memoryStream = new MemoryStream(data);
                        using (BsonReader reader = new BsonReader(memoryStream))
                        {
                            JsonSerializer jsonSerializer = new JsonSerializer();
                            jsonSerializer.DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate;
                            userDataBackingField = jsonSerializer.Deserialize<UserData>(reader);
                            return userDataBackingField;
                        }
                    }
                }
                catch (JsonReaderException e)
                {
                    appDataFileInfo.Delete();
                    userDataBackingField = new UserData();
                    return userDataBackingField;
                }
            }
        }
    }
}
