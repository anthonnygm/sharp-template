@echo off

rem criar pasta de deploy-test
if exist deploy-test rd /q /s deploy-test
mkdir deploy-test

rem compilar projeto
pushd src
pushd Appel.SharpTemplate.API
call dotnet restore
call dotnet build
call dotnet publish -c Release
xcopy ".\bin\Release\net6.0\publish\*" "..\..\deploy-test\*" /y /e
popd
popd

rem copiar templates de email
xcopy ".\src\Appel.SharpTemplate.Infrastructure\Application\EmailTemplates\*" deploy-test\EmailTemplates\ /y /e

rem iniciar web api
pushd deploy-test\
set ASPNETCORE_ENVIRONMENT=Development
start call dotnet .\Appel.SharpTemplate.API.dll
popd
