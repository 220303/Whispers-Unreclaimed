Write-Host "欢迎使用《未追回的呓语》构建脚本！"
Write-Host ""
Write-Host "本脚本用于构建《未追回的呓语》发布压缩文件。"
Write-Host "本脚本需被置于项目文件夹内。"
Write-Host "本脚本针对Dependent、Independent-win-x86-32、Independent-win-x86-64 三个目标架构进行构建。"
Write-Host ""
Write-Host "本脚本使用绝对路径！若相关文件夹发生改变，请注意修改！" -ForegroundColor Red

$de = "T:\ELCO\Computer Science\Software Engineering\烟尘记\烟尘记 [打包发布]\Release\Dependent"
$in32 = "T:\ELCO\Computer Science\Software Engineering\烟尘记\烟尘记 [打包发布]\Release\Independent\Windows x86-32"
$in64 = "T:\ELCO\Computer Science\Software Engineering\烟尘记\烟尘记 [打包发布]\Release\Independent\Windows x86-64" 

$de7z = "T:\ELCO\Computer Science\Software Engineering\烟尘记\烟尘记 [打包发布]\Release\Dependent-win.7z"
$in327z = "T:\ELCO\Computer Science\Software Engineering\烟尘记\烟尘记 [打包发布]\Release\Independent-win-x86-32.7z"
$in647z = "T:\ELCO\Computer Science\Software Engineering\烟尘记\烟尘记 [打包发布]\Release\Independent-win-x86-64.7z"

$deFS = "T:\ELCO\Computer Science\Software Engineering\烟尘记\烟尘记 [打包发布]\Release\Dependent\Filesystem"
$in32FS = "T:\ELCO\Computer Science\Software Engineering\烟尘记\烟尘记 [打包发布]\Release\Independent\Windows x86-32\Filesystem"
$in64FS = "T:\ELCO\Computer Science\Software Engineering\烟尘记\烟尘记 [打包发布]\Release\Independent\Windows x86-64\Filesystem" 

$FS = "T:\ELCO\Computer Science\Software Engineering\烟尘记\烟尘记 [打包发布]\Release\Filesystem"
$lin = "T:\ELCO\Computer Science\Software Engineering\烟尘记\烟尘记 [打包发布]\Release\未追回的呓语"
$SevenZip = "D:\7-zip\7-zip\7z.exe"


Write-Host ""
Write-Host "————————————————————————————————————————————————————————————————————————————————————"
Write-Host ""

Write-Host "开始清理旧版本文件..." -ForegroundColor Blue
Write-Host "......" -ForegroundColor Blue
Remove-Item -LiteralPath $de7z   -Force 
Remove-Item -LiteralPath $in327z -Force 
Remove-Item -LiteralPath $in647z -Force 

# 用 Get-ChildItem -LiteralPath 列出目录下的项，再直接管道到 Remove-Item
Get-ChildItem -LiteralPath $de -Force | Remove-Item -Recurse -Force
Get-ChildItem -LiteralPath $in32 -Force | Remove-Item -Recurse -Force
Get-ChildItem -LiteralPath $in64 -Force | Remove-Item -Recurse -Force

Write-Host "旧版本文件清理完毕！" -ForegroundColor Green

Write-Host ""
Write-Host "————————————————————————————————————————————————————————————————————————————————————"
Write-Host ""

Write-Host "现在开始构建Dependent平台:"

Write-Host ""

Write-Host "发布..." -ForegroundColor Blue
dotnet publish -c Release -p:PublishReadyToRun=true -o "$de"
Write-Host "发布完毕！" -ForegroundColor Green

Write-Host ""

Write-Host "开始替换Filesystem文件夹..." -ForegroundColor Blue
Remove-Item -LiteralPath $deFS -Recurse -Force
Copy-Item -LiteralPath $FS -Destination $de -Recurse
Write-Host "Filesystem文件夹替换完毕！" -ForegroundColor Green

Write-Host ""

Write-Host "开始建立临时‘未追回的呓语’文件夹..." -ForegroundColor Blue
New-Item -Path "$lin" -ItemType Directory

Write-Host ""

Write-Host "开始复制文件..." -ForegroundColor Blue
Get-ChildItem -LiteralPath $de | ForEach-Object {
    Copy-Item -LiteralPath $_.FullName -Destination $lin -Recurse
}


Write-Host ""

Write-Host "开始改名..." -ForegroundColor Blue
Rename-Item -LiteralPath ${lin}\烟尘记.exe -NewName "未追回的呓语.exe"

Write-Host ""

Write-Host "压缩..." -ForegroundColor Blue
& "$SevenZip" a -mx=7 "$de7z" "${lin}*"
Write-Host "压缩完毕！" -ForegroundColor Green


Remove-Item -LiteralPath $lin -Recurse -Force
Write-Host "临时‘未追回的呓语’文件夹清除完毕！" -ForegroundColor Green

Write-Host "Dependent平台构建完毕！" -ForegroundColor Green


Write-Host ""
Write-Host ""
Write-Host "————————————————————————————————————————————————————————————————————————————————————"
Write-Host ""
Write-Host ""


Write-Host "现在开始构建Independent-win-x86-32平台:"

Write-Host ""

Write-Host "发布..." -ForegroundColor Blue
dotnet publish -c Release -r win-x86 --self-contained true -p:PublishReadyToRun=true -o "$in32"
Write-Host "发布完毕！" -ForegroundColor Green

Write-Host ""

Write-Host "开始替换Filesystem文件夹..." -ForegroundColor Blue
Remove-Item -LiteralPath $in32FS -Recurse -Force
Copy-Item -LiteralPath $FS -Destination $in32FS -Recurse
Write-Host "Filesystem文件夹替换完毕！" -ForegroundColor Green

Write-Host ""

Write-Host "开始建立临时‘未追回的呓语’文件夹..." -ForegroundColor Blue
New-Item -Path "$lin" -ItemType Directory

Write-Host ""

Write-Host "开始复制文件..." -ForegroundColor Blue
Get-ChildItem -LiteralPath $in32 | ForEach-Object {
    Copy-Item -LiteralPath $_.FullName -Destination $lin -Recurse
}

Write-Host ""

Write-Host "开始改名..." -ForegroundColor Blue
Rename-Item -LiteralPath ${lin}\烟尘记.exe -NewName "未追回的呓语.exe"

Write-Host ""

Write-Host "压缩..." -ForegroundColor Blue
& "$SevenZip" a -mx=7 "$in327z" "${lin}*"
Write-Host "压缩完毕！" -ForegroundColor Green

Remove-Item -LiteralPath $lin -Recurse -Force
Write-Host "临时‘未追回的呓语’文件夹清除完毕！" -ForegroundColor Green

Write-Host "Independent-win-x86-32平台构建完毕！" -ForegroundColor Green


Write-Host ""
Write-Host ""
Write-Host "————————————————————————————————————————————————————————————————————————————————————"
Write-Host ""
Write-Host ""


Write-Host "现在开始构建Independent-win-x86-64平台:"

Write-Host ""

Write-Host "发布..." -ForegroundColor Blue
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishReadyToRun=true -o "$in64"
Write-Host "发布完毕！" -ForegroundColor Green

Write-Host ""

Write-Host "开始替换Filesystem文件夹..." -ForegroundColor Blue
Remove-Item -LiteralPath $in64FS -Recurse -Force
Copy-Item -LiteralPath $FS -Destination $in64FS -Recurse
Write-Host "Filesystem文件夹替换完毕！" -ForegroundColor Green

Write-Host ""

Write-Host "开始建立临时‘未追回的呓语’文件夹..." -ForegroundColor Blue
New-Item -Path "$lin" -ItemType Directory

Write-Host ""

Write-Host "开始复制文件..." -ForegroundColor Blue
Get-ChildItem -LiteralPath $in64 | ForEach-Object {
    Copy-Item -LiteralPath $_.FullName -Destination $lin -Recurse
}

Write-Host ""

Write-Host "开始改名..." -ForegroundColor Blue
Rename-Item -LiteralPath ${lin}\烟尘记.exe -NewName "未追回的呓语.exe"

Write-Host ""

Write-Host "压缩..." -ForegroundColor Blue
& "$SevenZip" a -mx=7 "$in647z" "${lin}*"
Write-Host "压缩完毕！" -ForegroundColor Green

Remove-Item -LiteralPath $lin -Recurse -Force
Write-Host "临时‘未追回的呓语’文件夹清除完毕！" -ForegroundColor Green

Write-Host "Independent-win-x86-64平台构建完毕！" -ForegroundColor Green


Write-Host ""
Write-Host ""
Write-Host "————————————————————————————————————————————————————————————————————————————————————"
Write-Host ""
Write-Host ""

Write-Host "工作完毕！感谢您的使用！"
