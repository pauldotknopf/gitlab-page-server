# gitlab-page-server

**WIP**: *You literally can't use it yet.*

Serving static content from GitLab.

This will mirror the features of the built in server GitLab uses for static pages, but with some additional things/features.

## Road map

It will listen on one domain.

* `https://domain.com/group1/project1`
* `https://domain.com/group2/group3/project2`

You will also be able to host a custom project (via config) at the root (`https://domain.com`). This will be ideal for creating a project that will serve as an "index".

I am also adding custom endpoints for branches and merge requests.

* `https://domain.com/group1/project1/mr/1`
* `https://domain.com/group2/group3/project2/branch/develop`
