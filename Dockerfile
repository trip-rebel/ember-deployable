FROM microsoft/dotnet

COPY TREmberDeployable/project.json /app/

WORKDIR /app

RUN ["dotnet", "restore"]

COPY TREmberDeployable /app

EXPOSE 5000

ENTRYPOINT ["dotnet", "run"]
