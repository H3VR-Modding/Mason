# Continuous Integration

Continuous integration is a feature of many source control websites that, as the name implies, continuously integrates source code changes into the product. For the purposes of Mason, continuous integration is just automatic packaging.

## GitHub

Continuous integration with GitHub is simple. Simply copy [this file](https://github.com/H3VR-Modding/Mason-Quickstart/blob/main/.github/workflows/package.yml) into your project, using the same path. GitHub will run this script every time a push or pull request is made to the `main` or `master` branches that affect the contents of a Thunderstore package, and it will yield an artifact containing the Thunderstore package. Additionally, you may add a branch protection rule to the main branch and require that the script succeeds before a pull request can be merged.
