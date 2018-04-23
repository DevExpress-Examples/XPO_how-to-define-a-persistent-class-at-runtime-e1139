using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.DB;

namespace CreateClassAtRuntime {
    class Program {
        static void Main(string[] args) {
            IDataStore provider = new DevExpress.Xpo.DB.InMemoryDataStore();
            //string connectionString = MSSqlConnectionProvider.GetConnectionString("localhost", "E1139");
            //IDataStore provider = XpoDefault.GetConnectionProvider(connectionString, AutoCreateOption.DatabaseAndSchema);

            XPDictionary dictionary = new ReflectionDictionary();
            XPClassInfo myBaseClass = dictionary.GetClassInfo(typeof(MyBaseObject));
            XPClassInfo myClassA = dictionary.CreateClass(myBaseClass, "MyObjectA");
            myClassA.CreateMember("ID", typeof(int), new KeyAttribute(true));
            myClassA.CreateMember("Name", typeof(string));

            XpoDefault.Session = null;
            XpoDefault.DataLayer = new SimpleDataLayer(dictionary, provider);
            //XpoDefault.DataLayer = new ThreadSafeDataLayer(dictionary, provider);

            using (Session session = new Session()) {
                session.UpdateSchema(myClassA);
            }
            using (Session session = new Session()) {
                Console.WriteLine("Create a new object:");
                XPBaseObject obj = (XPBaseObject)myClassA.CreateNewObject(session);
                obj.SetMemberValue("Name", String.Format("sample {0}", DateTime.UtcNow.Ticks));
                obj.Save();
                Console.WriteLine("ID:\t{0}, Name:\t{1}", obj.GetMemberValue("ID"), obj.GetMemberValue("Name"));
            }
            Console.WriteLine("----------------------------");
            using (Session session = new Session()) {
                XPCollection collection = new XPCollection(session, myClassA);
                Console.WriteLine("Objects loaded. Total count: {0}", collection.Count);
                foreach (XPBaseObject obj in collection) {
                    Console.WriteLine("ID:\t{0}, Name:\t{1}", obj.GetMemberValue("ID"), obj.GetMemberValue("Name"));
                }

            }
            Console.WriteLine("----------------------------");
            Console.WriteLine("Press Enter to Exit");
            Console.ReadLine();
        }
    }

    [NonPersistent]
    public class MyBaseObject : XPLiteObject {
        public MyBaseObject(Session session)
            : base(session) { }
        public MyBaseObject(Session session, XPClassInfo classInfo)
            : base(session, classInfo) { }
    }
}
