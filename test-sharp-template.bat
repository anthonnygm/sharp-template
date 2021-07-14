@echo off

rem criar pasta de deploy-test
if exist deploy-test rd /q /s deploy-test
mkdir deploy-test

rem compilar projeto
pushd Appel.SharpTemplate
call dotnet restore
call dotnet build
call dotnet publish -c Release
xcopy ".\bin\Release\net5.0\publish\*" "..\deploy-test\*" /y /e
popd

rem copiar arquivo appsettings.json
copy .\Appel.SharpTemplate\appsettings.json deploy-test\appsettings.json /y

rem copiar templates de email
xcopy ".\Appel.SharpTemplate\EmailTemplates\*" deploy-test\EmailTemplates\ /y /e

rem iniciar web api
pushd deploy-test\
start call dotnet .\Appel.SharpTemplate.dll
popd
