FROM microsoft/aspnetcore:1.0.1

COPY ./out/ /app

WORKDIR /app

EXPOSE 80

ENTRYPOINT ["dotnet", "ChatBotWithWS.dll"]
