# test-action workflow

[![Test GitHub action](https://github.com/edumserrano/markdown-link-check-log-parser/actions/workflows/test-action.yml/badge.svg)](https://github.com/edumserrano/markdown-link-check-log-parser/actions/workflows/test-action.yml)

[This workflow](/.github/workflows/test-action.yml):

- Is triggered from a workflow that runs the `Markdown Link Check` which **doesn't have errors** in the log.
- Is triggered from a workflow that runs the `Markdown Link Check` which **has errors** in the log.
- Contains a step that executes the GitHub action provided by this repo.
- Checks that the output produced by the action is as expected.
- Checks that action should fail the workflow if the action fails.
- If the workflow that triggered it was from a Pull Request, then upon completion it will update the PR with a status check.

Since this workflow executes the [Docker container action](https://docs.github.com/en/actions/creating-actions/creating-a-docker-container-action) it will build and execute the docker container so if there are any issues with the action's [Dockerfile](/Dockerfile) this workflow will detect it.

> [!NOTE]
>
> For this workflow to be able to test the action when code is pushed I created an alternate `action.yml` at `/action-local` that will build the Docker image from the repo instead of using the Docker image published in the GitHub packages (which is what the `action.yml` at the root of the repo does).
>
> The downside of this approach is that I need to keep both `action.yml` files, the one at the root of the repo and the one at `/action-local`, in sync.
>
> Alternativel, I could try to setup the workflows so that this test workflow only runs after the Docker image has been published. However this approach also has problems to solve such as making sure that the checks work as expected in a pull request scenario. The current approach eliminates all problems of this type with the only downside of keeping the `action.yml` files in sync.
