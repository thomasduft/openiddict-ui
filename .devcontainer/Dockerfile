# [Choice] .NET version: 5.0, 3.1, 2.1
ARG VARIANT=6.0
FROM mcr.microsoft.com/vscode/devcontainers/dotnet:dev-${VARIANT}

# [Option] Install Node.js
ARG INSTALL_NODE="true"
ARG NODE_VERSION="lts/*"
RUN if [ "${INSTALL_NODE}" = "true" ]; then su vscode -c "umask 0002 && . /usr/local/share/nvm/nvm.sh && nvm install ${NODE_VERSION} 2>&1"; fi
