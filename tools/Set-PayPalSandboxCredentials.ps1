$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName System.Drawing

$form = New-Object System.Windows.Forms.Form
$form.Text = 'PayPal Sandbox Credentials'
$form.Size = New-Object System.Drawing.Size(510,240)
$form.StartPosition = 'CenterScreen'
$form.FormBorderStyle = 'FixedDialog'
$form.MaximizeBox = $false
$form.MinimizeBox = $false

$idLabel = New-Object System.Windows.Forms.Label
$idLabel.Text = 'Sandbox Client ID'
$idLabel.Location = New-Object System.Drawing.Point(20,20)
$idLabel.AutoSize = $true
$form.Controls.Add($idLabel)

$idBox = New-Object System.Windows.Forms.TextBox
$idBox.Location = New-Object System.Drawing.Point(20,45)
$idBox.Width = 450
$form.Controls.Add($idBox)

$secretLabel = New-Object System.Windows.Forms.Label
$secretLabel.Text = 'Sandbox Client Secret'
$secretLabel.Location = New-Object System.Drawing.Point(20,85)
$secretLabel.AutoSize = $true
$form.Controls.Add($secretLabel)

$secretBox = New-Object System.Windows.Forms.TextBox
$secretBox.Location = New-Object System.Drawing.Point(20,110)
$secretBox.Width = 450
$secretBox.UseSystemPasswordChar = $true
$form.Controls.Add($secretBox)

$save = New-Object System.Windows.Forms.Button
$save.Text = 'Save securely'
$save.Location = New-Object System.Drawing.Point(345,150)
$save.Width = 125
$form.Controls.Add($save)

$save.Add_Click({
    if ([string]::IsNullOrWhiteSpace($idBox.Text) -or
        [string]::IsNullOrWhiteSpace($secretBox.Text)) {
        [System.Windows.Forms.MessageBox]::Show(
            'Both fields are required.',
            'Validation'
        ) | Out-Null
        return
    }

    $dir = Join-Path $env:LOCALAPPDATA 'PayGuard\secrets'
    [System.IO.Directory]::CreateDirectory($dir) | Out-Null

    $idEncrypted = ConvertTo-SecureString -String $idBox.Text -AsPlainText -Force |
        ConvertFrom-SecureString
    $secretEncrypted = ConvertTo-SecureString -String $secretBox.Text -AsPlainText -Force |
        ConvertFrom-SecureString

    [System.IO.File]::WriteAllLines(
        (Join-Path $dir 'paypal-sandbox.dpapi'),
        @($idEncrypted, $secretEncrypted)
    )

    $secretBox.Clear()
    $idBox.Clear()
    $form.DialogResult = [System.Windows.Forms.DialogResult]::OK
    $form.Close()
})

$form.AcceptButton = $save
$result = $form.ShowDialog()

if ($result -eq [System.Windows.Forms.DialogResult]::OK) {
    'RESULT: SANDBOX CREDENTIALS SAVED LOCALLY'
} else {
    'RESULT: CANCELLED; NO CREDENTIALS SAVED'
}