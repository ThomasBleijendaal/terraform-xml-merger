# Apim Terraform

Additions to make cicd for API Management better.

## Xml Merger Terraform Provider

Merges xml policies into bigger files, so xml policies can be composed of fragments,
without relying on policy fragments.

### Environment variables

- `TF_XML_POLICIES_FOLDER`: Folder to scan for xml fragments.
- `TF_XML_BASE_POLICY`: Path to base xml file to merge fragments into.

### Variable format

`%{A-Z_}:{a-z}`: Name of variable (`EXAMPLE_VARIABLE`) followed by type (`bool|int|string`): (`%EXAMPLE_VARIABLE:bool%`).

Variables will be parsed and added to terraform schema.

## Swagger Merger

Merges multiple swagger json files into single files, so API Management api version sets
can be composed of multiple backends.

### Request format

```/json?url=http://absolute-swagger.url/1&url=http://absolute-swagger.url/2```
