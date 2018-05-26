FROM microsoft/dotnet:2.1.300-rc1-sdk-bionic
ADD gitlab-pages/linux-x64 /gitlab-pages
RUN mkdir /work
ENTRYPOINT ["/gitlab-pages/GitLabPages.Web"]
WORKDIR /work