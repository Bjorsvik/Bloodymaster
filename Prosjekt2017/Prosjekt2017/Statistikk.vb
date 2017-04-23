Public Class Statistikk
    Private db As New Database()

    'Tom konstruktør
    Sub New()


    End Sub

    Public Function bgRegistrertMaaned()
        Return db.Query("SELECT * From (
                         SELECT COUNT(Person.personID) As Antallregistrerte,
                         MONTHNAME(registrertdato) As Month,
                         YEAR(registrertdato) As Year
                         FROM Person
                         Join Blodgiver ON Person.personID = Blodgiver.personId
                         GROUP BY Month
                         ) As Innertable
                         WHERE Month = MONTHNAME(CURDATE()) AND Year = YEAR(CURDATE())")
    End Function

    Public Function bgRegistrertAar()
        Return db.Query("SELECT * From (
                         SELECT COUNT(Person.personID) As Antallregistrerte,
                         YEAR(registrertdato) As Year
                         FROM Person
                         JOIN Blodgiver ON Person.personID = Blodgiver.personId
                         GROUP BY Year
                         ) As Innertable
                         WHERE YEAR = YEAR(CURDATE())")
    End Function
End Class
