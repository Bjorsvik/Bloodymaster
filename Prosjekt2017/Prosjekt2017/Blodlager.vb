Public Class Blodlager
    Dim db As New Database()

    'Tom konstruktør
    Public Sub New()
    End Sub

    'Legge inn blodposer ved blodtapping
    Public Sub leggInnBlodposer(blodtype As String, blodposer As Integer, resID As Integer)
        db.Query("INSERT INTO Blodtype(blodtype, blodposer, resID) Values ('" & blodtype & "', '" & blodposer & "', '" & resID & "')")
    End Sub

    'Henter ut alle tilgjengelige blodplasmaposer
    Public Function getAlleTilgjengeligeBlodPlasma() As DataTable
        Return db.Query("SELECT blodtype, SUM(plasma_poser) As Plasmaposer From Blodtype JOIN Blodplasma ON Blodtype.blodID = Blodplasma.blodID
                         Group By blodtype")
    End Function

    'Henter ut alle blodcelleposer som ikke har gått ut på dato
    Public Function getAlleTilgjengeligeBlodceller() As DataTable
        Return db.Query("SELECT * From (
    SELECT
    blodtype,
    SUM(celler_poser) As Cellerposer,
    Blodceller.dato As dato,
    DATEDIFF(Blodceller.dato, CURDATE()) As diffCeller
    
     FROM Blodtype
                         Join Blodceller ON Blodtype.blodID = Blodceller.blodID  
    Group By blodtype                 
    ) As innertable

    Where diffCeller < 36")
    End Function


    'Henter ut alle blodplateposer som ikke har gått ut på dato og grupperer det
    Public Function getAlleTilgjengeligeBlodplater() As DataTable
        Return db.Query("SELECT * From (
    Select

    blodtype,
    SUM(plater_poser) As Platerposer,
    Blodplater.dato As dato,
    DATEDIFF(Blodplater.dato, CURDATE()) As diffPlater
    
     FROM Blodtype
                         Join Blodplater ON Blodtype.blodID = Blodplater.blodID  

     Group By blodtype
    ) As innertable
    Where diffPlater < 8")
    End Function

    'Henter ut alle tilgjengelige blodceller
    Public Function getBlodcellerGrid(blodtype As String) As DataTable
        Return db.Query("SELECT * From (
    SELECT
    blodtype,
    celler_poser As Cellerposer,
    Blodceller.dato As dato,
    DATEDIFF(CURDATE(), Blodceller.dato) As diffCeller,
    Blodtype.blodID
    
     FROM Blodtype
                         Join Blodceller ON Blodtype.blodID = Blodceller.blodID  
    Group By dato, blodtype
    ) As innertable

    Where diffCeller < 36 AND blodtype = '" & blodtype & "'")
    End Function

    'Henter ut alle tilgjengelige blodplater
    Public Function getPlaterGrid(blodtype As String) As DataTable
        Return db.Query("SELECT * From (
    SELECT
    blodtype,
    plater_poser As Platerposer,
    Blodplater.dato As dato,
    DATEDIFF(CURDATE(), Blodplater.dato) As diffplater,
    Blodtype.blodID
    
     FROM Blodtype
                         Join Blodplater ON Blodtype.blodID = Blodplater.blodID  
    Group By dato, blodtype
    ) As innertable

    Where diffPlater < 8 AND blodtype = '" & blodtype & "'")
    End Function
    'Henter ut alle plasma
    Public Function getPlasmaGrid(blodtype As String) As DataTable
        Return db.Query("SELECT blodtype, plasma_poser As Plasmaposer, Blodplasma.blodID
                        FROM Blodtype
                        Join Blodplasma ON Blodtype.blodID = Blodplasma.blodID
                        Where blodtype = '" & blodtype & "' 
                        ")
    End Function

    Public Sub fyllDatagrid(ByRef blodGrid As Object, ByVal blodTabell As DataTable)
        Dim blodArray As New ArrayList()
        Dim dato As Date
        Dim blodtype As String
        Dim poser As String
        Dim diffCeller As String
        Dim bID As String
        blodGrid.Rows.Clear()

        For Each row In blodTabell.Rows()
            blodtype = row(0).ToString
            poser = row(1).ToString
            dato = row(2).ToString
            diffCeller = row(3).ToString
            bID = row(4).ToString

            blodGrid.Rows.Add(dato.ToString("yyyy-MM-dd"), blodtype, poser, diffCeller, bID)
        Next
    End Sub

    Public Sub fyllPlasmagrid(ByRef blodGrid As Object, ByVal blodTabell As DataTable)
        Dim blodArray As New ArrayList()
        Dim plasmaposer As String
        Dim blodtype As String
        Dim bID As String
        blodGrid.Rows.Clear()

        For Each row In blodTabell.Rows()
            blodtype = row(0).ToString
            plasmaposer = row(1).ToString
            bID = row(2).ToString

            blodGrid.Rows.Add("-", blodtype, plasmaposer, "Ingen utløpsdato", bID)
        Next
    End Sub

    Public Sub fyllBlodproduktgrid(ByRef blodGrid As Object, ByVal blodTabell As DataTable)
        Dim blodArray As New ArrayList()
        Dim poser As String
        Dim blodtype As String
        blodGrid.Rows.Clear()

        For Each row In blodTabell.Rows()
            blodtype = row(0).ToString
            poser = row(1).ToString

            blodGrid.Rows.Add(blodtype, poser)
        Next
    End Sub

    Public Function skrivUtBlodplasma(ByVal blodID As Integer)
        Return db.Query("UPDATE Blodplasma
                         SET plasma_poser = plasma_poser - 1 
                         WHERE Blodplasma.blodID = " & blodID & " AND plasma_poser > 0")
    End Function

    Public Function skrivUtBlodceller(ByVal blodID As Integer)
        Return db.Query("UPDATE Blodceller
                         SET celler_poser = celler_poser - 1
                         WHERE Blodceller.blodID = " & blodID & " AND celler_poser > 0")
    End Function

    Public Function skrivUtBlodplater(ByVal blodID As Integer)
        Return db.Query("UPDATE Blodplater
                         SET plater_poser = plater_poser - 1 
                         WHERE Blodplater.blodID = " & blodID & " AND plater_poser > 0")
    End Function


    'Henter ut siste blodID ved bruk av resID
    Public Function getLastBlodIDByResID(ByVal reservasjonsID As String) As DataTable
        Return db.Query("SELECT MAX(blodID) AS blodID FROM Blodtype
                         Join Reservasjon ON Blodtype.resID = Reservasjon.resID
                         Where Reservasjon.resID = '" & reservasjonsID & "'")
    End Function
    'Legger inn blodplasmaposer inn i databasen
    Public Sub leggInnBlodplasma(ByVal lagerID As Integer, ByVal blodID As Integer, ByVal plasmaposer As Integer)
        db.Query("INSERT INTO Blodplasma(lagerID, blodID, plasma_poser) VALUES ('" & lagerID & "', '" & blodID & "', '" & plasmaposer & "' )")
    End Sub
    'Legger inn blodplasmaposer inn i databasen
    Public Sub leggInnBlodcelle(ByVal lagerID As Integer, ByVal blodID As Integer, ByVal celleposer As Integer, ByVal dato As String)
        db.Query("INSERT INTO Blodceller(lagerID, blodID, celler_poser, dato) VALUES ('" & lagerID & "', '" & blodID & "', '" & celleposer & "', '" & dato & "' )")
    End Sub
    'Legger inn blodplateposer inn i databasen
    Public Sub leggInnBlodplate(ByVal lagerID As Integer, ByVal blodID As Integer, ByVal plateposer As Integer, ByVal dato As String)
        db.Query("INSERT INTO Blodplater(lagerID, blodID, plater_poser, dato) VALUES ('" & lagerID & "', '" & blodID & "', '" & plateposer & "', '" & dato & "' )")
    End Sub

End Class