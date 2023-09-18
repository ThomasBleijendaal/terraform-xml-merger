# Apim Terraform

Additions to make cicd for API Management better.

## TODO

- [ ] named values support (separate provider that first checks if the named value exists)

## Xml Merger Terraform Provider

Merges xml policies into bigger files, so xml policies can be composed of fragments,
without relying on policy fragments.

### Environment variables

- `TF_XML_POLICIES_FOLDER`: Folder to scan for xml fragments.
- `TF_XML_BASE_POLICY`: Path to base xml file to merge fragments into.

### Variable format

`%{A-Z_}:{a-z}`: Name of variable (`EXAMPLE_VARIABLE`) followed by type (`bool|int|string`): (`%EXAMPLE_VARIABLE:bool%`).

Variables will be parsed and added to terraform schema.

### XDT details

Read [this](https://learn.microsoft.com/en-us/previous-versions/aspnet/dd465326(v=vs.110)).

## OpenApi Merger

Merges multiple OpenApi json files into single files, so API Management api version sets
can be composed of multiple backends.
