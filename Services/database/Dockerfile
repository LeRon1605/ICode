FROM mcr.microsoft.com/mssql/server:2019-CU13-ubuntu-20.04
WORKDIR /usr/src/app
COPY . .

ENV SA_PASSWORD=leron@1605
ENV ACCEPT_EULA=Y
ENV MSSQL_PID=Express

CMD /bin/bash ./entrypoint.sh