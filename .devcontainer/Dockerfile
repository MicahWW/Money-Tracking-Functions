FROM ubuntu:22.04

# create a user
RUN useradd -ms /bin/bash dev
RUN echo dev:addToDocker | chpasswd

RUN apt update

# install git tools
RUN apt install git gh -y

# install az CLI
RUN apt install curl -y
RUN curl -sL https://aka.ms/InstallAzureCLIDeb | bash

# Used for Azure Functions development
    # DOTNET install
    RUN apt install -y dotnet-sdk-8.0

    # func CLI installation
    # ref: https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local
    RUN apt install gpg lsb-release -y
    RUN curl https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > microsoft.gpg
    RUN mv microsoft.gpg /etc/apt/trusted.gpg.d/microsoft.gpg
    RUN sh -c 'echo "deb [arch=amd64] https://packages.microsoft.com/repos/microsoft-ubuntu-$(lsb_release -cs)-prod $(lsb_release -cs) main" > /etc/apt/sources.list.d/dotnetdev.list'
    RUN apt update
    RUN apt install azure-functions-core-tools-4

# Install MySql
    RUN apt install mysql-server -y
    # this just gets rid of a warning that doesn't really mean anything
    RUN usermod -d /var/lib/mysql mysql
    # this gives access to all users that are part of the mysql group
    RUN chmod 770 /var/run/mysqld
    # The next 3 lines setup the users of group mysql to start and stop the mysql service
    RUN apt install sudo -y
    RUN echo "# Allow members of mysql group to start and stop mysql service\n%mysql ALL=/usr/sbin/service mysql start\n%mysql ALL=/usr/sbin/service mysql stop\n%mysql ALL=/usr/sbin/service mysql restart" > /etc/sudoers.d/mysqlService
    RUN adduser dev mysql

USER dev