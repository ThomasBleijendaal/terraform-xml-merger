# Apim Terraform

Additions to make cicd with API Management better.

## Environment variables

- `TF_XML_POLICIES_FOLDER`: Folder to scan for xml fragments.
- `TF_XML_BASE_POLICY`: Path to base xml file to merge fragments into.

## Variable format

```%{A-Z_}:{a-z}```: Name of variable (EXAMPLE_VARIABLE) followed by type (bool|int|string): (`%EXAMPLE_VARIABLE:bool%`).

Variable will be parsed and added to terraform schema.
