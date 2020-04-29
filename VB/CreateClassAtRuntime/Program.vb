Imports System
Imports System.Collections.Generic
Imports System.Text
Imports DevExpress.Xpo
Imports DevExpress.Xpo.Metadata
Imports DevExpress.Xpo.DB

Namespace CreateClassAtRuntime
    Friend Class Program
        Shared Sub Main(ByVal args() As String)
            Dim provider As IDataStore = New DevExpress.Xpo.DB.InMemoryDataStore()
            'string connectionString = MSSqlConnectionProvider.GetConnectionString("localhost", "E1139");
            'IDataStore provider = XpoDefault.GetConnectionProvider(connectionString, AutoCreateOption.DatabaseAndSchema);

            Dim dictionary As XPDictionary = New ReflectionDictionary()
            Dim myBaseClass As XPClassInfo = dictionary.GetClassInfo(GetType(MyBaseObject))
            Dim myClassA As XPClassInfo = dictionary.CreateClass(myBaseClass, "MyObjectA")
            myClassA.CreateMember("ID", GetType(Integer), New KeyAttribute(True))
            myClassA.CreateMember("Name", GetType(String))

            XpoDefault.Session = Nothing
            XpoDefault.DataLayer = New SimpleDataLayer(dictionary, provider)
            'XpoDefault.DataLayer = new ThreadSafeDataLayer(dictionary, provider);

            Using session As New Session()
                session.UpdateSchema(myClassA)
            End Using
            Using session As New Session()
                Console.WriteLine("Create a new object:")
                Dim obj As XPBaseObject = CType(myClassA.CreateNewObject(session), XPBaseObject)
                obj.SetMemberValue("Name", String.Format("sample {0}", Date.UtcNow.Ticks))
                obj.Save()
                Console.WriteLine("ID:" & ControlChars.Tab & "{0}, Name:" & ControlChars.Tab & "{1}", obj.GetMemberValue("ID"), obj.GetMemberValue("Name"))
            End Using
            Console.WriteLine("----------------------------")
            Using session As New Session()
                Dim collection As New XPCollection(session, myClassA)
                Console.WriteLine("Objects loaded. Total count: {0}", collection.Count)
                For Each obj As XPBaseObject In collection
                    Console.WriteLine("ID:" & ControlChars.Tab & "{0}, Name:" & ControlChars.Tab & "{1}", obj.GetMemberValue("ID"), obj.GetMemberValue("Name"))
                Next obj

            End Using
            Console.WriteLine("----------------------------")
            Console.WriteLine("Press Enter to Exit")
            Console.ReadLine()
        End Sub
    End Class

    <NonPersistent> _
    Public Class MyBaseObject
        Inherits XPLiteObject

        Public Sub New(ByVal session As Session)
            MyBase.New(session)
        End Sub
        Public Sub New(ByVal session As Session, ByVal classInfo As XPClassInfo)
            MyBase.New(session, classInfo)
        End Sub
    End Class
End Namespace
