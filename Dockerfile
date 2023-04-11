FROM public.ecr.aws/amazonlinux/amazonlinux:2 AS base
WORKDIR /source

# Install dotnet 7 and other dependencies for compiling naively
RUN rpm -Uvh https://packages.microsoft.com/config/centos/7/packages-microsoft-prod.rpm
RUN yum update -y && yum install -y dotnet-sdk-7.0 clang krb5-devel openssl-devel zip

# Only copy the dotnet source code
COPY ./src/ ./src
COPY ./sg-blog-api.sln sg-blog-api.sln

ENV DOTNET_NOLOGO=true
ENV DOTNET_CLI_TELEMETRY_OPTOUT=true

RUN dotnet publish -r linux-x64 -c Release --self-contained