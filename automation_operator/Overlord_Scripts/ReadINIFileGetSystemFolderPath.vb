Imports System
Imports System.IO
Imports System.Text
Imports System.Windows.Forms
Imports Microsoft.VisualBasic

Namespace Script
Public Class ScriptClass

Private Declare Function GetPrivateProfileString Lib "kernel32" Alias "GetPrivateProfileStringA" (ByVal lpAppName As String, _
        ByVal lpKeyName As String, _
        ByVal lpDefault As String, _
        ByVal lpReturnedString As StringBuilder, _
        ByVal nSize As Integer, _
        ByVal lpFileName As String) As integer

			
Public Sub ExecuteCode()
	Dim sb As New StringBuilder(500)
	GetPrivateProfileString("MAIN", "SystemFolder", "", sb, sb.capacity, "C:\SystemConfig\RobotSystemConfig.ini")
	[system_folder] = sb
End Sub
	


End Class
End Namespace