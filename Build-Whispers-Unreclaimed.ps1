Write-Host ”欢迎使用《未追回的呓语》构建脚本！“
Write-Host ”“
Write-Host ”本脚本用于构建《未追回的呓语》发布压缩文件。“
Write-Host ”本脚本需被置于项目文件夹内。“
Write-Host ”本脚本针对Dependent、Independent-win-x86-32、Independent-win-x86-64 三个目标架构进行构建。“
Write-Host ”“
Write-Host ”本脚本使用绝对路径！若相关文件夹发生改变，请注意修改！“ -ForegroundColor Red

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

Write-Host ”“
Write-Host “————————————————————————————————————————————————————————————————————————————————————“
Write-Host ”“

Write-Host ”开始清理旧版本文件...“ -ForegroundColor Blue

Remove-Item "$de" -Force
Remove-Item "$in32" -Force
Remove-Item "$in64" -Force

Remove-Item "${de}\*" -Recurse -Force
Remove-Item "${in32}\*" -Recurse -Force
Remove-Item "${in64}\*" -Recurse -Force

Write-Host ”旧版本文件清理完毕！“ -ForegroundColor Green

Write-Host ”“
Write-Host “————————————————————————————————————————————————————————————————————————————————————“
Write-Host ”“

$SevenZip = "D:\7-zip\7-zip\7z.exe"

Write-Host ”现在开始构建Dependent平台:“
Write-Host ”发布...“ -ForegroundColor Blue
dotnet publish -c Release -p:PublishReadyToRun=true -o "$de"
Write-Host ”发布完毕！“ -ForegroundColor Green
Write-Host ""
Write-Host ”压缩...“ -ForegroundColor Blue
& "$SevenZip" a -mx=7 "$de7z" "${de}\*"
Write-Host ”压缩完毕！“ -ForegroundColor Green
Write-Host ”Dependent平台构建完毕！“ -ForegroundColor Green

Write-Host ""
Write-Host ""

Write-Host ”现在开始构建Independent-win-x86-32平台:“
Write-Host ”发布...“ -ForegroundColor Blue
dotnet publish -c Release -r win-x86 --self-contained true -p:PublishReadyToRun=true -o "$in32"
Write-Host ”发布完毕！“ -ForegroundColor Green
Write-Host ""
Write-Host ”压缩...“ -ForegroundColor Blue
& "$SevenZip" a -mx=7 "$in327z" "${in32}\*"
Write-Host ”压缩完毕！“ -ForegroundColor Green
Write-Host ”Independent-win-x86-32平台构建完毕！“ -ForegroundColor Green

Write-Host ""
Write-Host ""

Write-Host ”现在开始构建Independent-win-x86-64平台:“
Write-Host ”发布...“ -ForegroundColor Blue
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishReadyToRun=true -o "$in64"
Write-Host ”发布完毕！“ -ForegroundColor Green
Write-Host ""
Write-Host ”压缩...“ -ForegroundColor Blue
& "$SevenZip" a -mx=7 "$in647z" "${in64}\*"
Write-Host ”压缩完毕！“ -ForegroundColor Green
Write-Host ”Independent-win-x86-64平台构建完毕！“ -ForegroundColor Green

Write-Host ”“
Write-Host “————————————————————————————————————————————————————————————————————————————————————“
Write-Host ”“

Write-Host ”开始替换Filesystem文件夹...“

Remove-Item "${deFS}\*" -Recurse -Force
Remove-Item "${in32FS}\*" -Recurse -Force
Remove-Item "${in64FS}\*" -Recurse -Force

Copy-Item -Path "$FS" -Destination "$deFS" -Recurse
Copy-Item -Path "$FS" -Destination "$in32FS" -Recurse
Copy-Item -Path "$FS" -Destination "$in64FS" -Recurse

Write-Host ”Filesystem文件夹替换完毕！“ -ForegroundColor Green

Write-Host ”“
Write-Host “————————————————————————————————————————————————————————————————————————————————————“
Write-Host ”“

Write-Host ”工作完毕！感谢您的使用！"