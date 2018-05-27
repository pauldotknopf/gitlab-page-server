# gitlab-page-server

A GitLab page server. Similar to the built-in page server, but with some additional features.

[![Build Status](https://travis-ci.com/pauldotknopf/gitlab-page-server.svg?branch=develop)](https://travis-ci.com/pauldotknopf/gitlab-page-server)

## Details



## Installation

1. Run the server. Example `docker-compose.yml` file [here](build/docker/example/docker-compose.yml). Configurable options [here](#more-options).
   * At a bare minimum, you should have the following configured for things to work properly.
   ```
   {
     "Pages": {
       "ServerUrl": "http://192.168.0.6",
       "AuthToken": "AdNAuSLZxGvU1cHycNxU"
     }
   }
   ```
   This configuration should go in a `config.json` file in the working directory of the running server.
2. On GitLab under `Project > Settings > Integrations`, add a web hook that points to `https://your-page-server-instance.com/hook` and tick the following:
   * [x] Job events
   * [x] Pipeline events

That's it. Visit `your-domain.com/group/project` to serve your static pages.

## More options

### Logging

**Defaults**:

```
{
  "Logging": {
    "MinimumLevel": "Information"
  }
}
```

### Pages

**Defaults**:

```
{
  "Pages": {
    "ServerUrl": null,
    "AuthToken": null,
    "HookToken": null,
    "AdditionalParentGroups": 0,
    "ArtifactsCacheDirectory": "artifacts",
    "JobArtifactsBasePath": "public",
    "RepositoryBranch": "master",
    "BuildJobName": "pages",
    "CacheProjectType": "Sliding",
    "CacheProjectSeconds": 60,
    "CachePipelineType": "Sliding",
    "CachePipelineSeconds": 60,
    "CacheJobType": "Sliding",
    "CacheJobSeconds": 60
  }
}
```

**Details**:

* `"ServerUrl"`: You can point this to ```gitlab.com``` or your own hosted GitLab instance.
* `"AuthToken"`: Generate this from your account settings.
* `"HookToken"`: The secret token, configured in GitLab, for the web hook. This ensures that only GitLab can post to your hook.
* `"AdditionalParentGroups"`: By default, all requests will attempt to match one parent group. Add more using this property. This allows you to have deep pages (`domain.com/parent-group/sub-group/another-sub-group/project`).
* `"ArtifactsCacheDirectory"`: The location where the artifacts will be cached on disk. This path is relative to the working directory.
* `"JobArtifactsBasePath"`: The directory within the download artifacts that content will be served from.
* `"RepositoryBranch"`: The branch that the url `group/project` will serve from.
* `"BuildJobName"`: The name of the job that artifacts will be downloaded from, within a pipeline.
* `"Cache*"`: Various knobs for tuning caching. You likely won't need to change this. The web hooks will invalidate the cache when new artifacts are available.
