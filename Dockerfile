FROM public.ecr.aws/amazonlinux/amazonlinux:2 AS base
WORKDIR /source
# Install .NET 7 and other dependencies for compiling naively
RUN rpm -Uvh https://packages.microsoft.com/config/centos/7/packages-microsoft-prod.rpm
RUN yum update -y && yum install -y dotnet-sdk-7.0 clang krb5-devel openssl-devel zip

COPY . .
RUN dotnet publish -r linux-x64 -c Release --self-contained
RUN strip /source/src/SgBlogApi.CreatePost/bin/Release/net7.0/linux-x64/native/bootstrap
RUN mkdir SgBlogApi.CreatePost
RUN cp /source/src/SgBlogApi.CreatePost/bin/Release/net7.0/linux-x64/native/bootstrap SgBlogApi.CreatePost/bootstrap